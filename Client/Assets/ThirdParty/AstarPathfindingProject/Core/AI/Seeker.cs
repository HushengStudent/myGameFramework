using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

/** Handles path calls for a single unit.
 * \ingroup relevant
 * This is a component which is meant to be attached to a single unit (AI, Robot, Player, whatever) to handle its pathfinding calls.
 * It also handles post-processing of paths using modifiers.
 *
 * \shadowimage{seeker_inspector.png}
 *
 * \see \ref calling-pathfinding
 * \see \ref modifiers
 */
[AddComponentMenu("Pathfinding/Seeker")]
[HelpURL("http://arongranberg.com/astar/docs/class_seeker.php")]
public class Seeker : VersionedMonoBehaviour {
	/** Enables drawing of the last calculated path using Gizmos.
	 * The path will show up in green.
	 *
	 * \see OnDrawGizmos
	 */
	public bool drawGizmos = true;

	/** Enables drawing of the non-postprocessed path using Gizmos.
	 * The path will show up in orange.
	 *
	 * Requires that #drawGizmos is true.
	 *
	 * This will show the path before any post processing such as smoothing is applied.
	 *
	 * \see drawGizmos
	 * \see OnDrawGizmos
	 */
	public bool detailedGizmos;

	/** Path modifier which tweaks the start and end points of a path */
	public StartEndModifier startEndModifier = new StartEndModifier();

	/** The tags which the Seeker can traverse.
	 *
	 * \note This field is a bitmask.
	 * \see https://en.wikipedia.org/wiki/Mask_(computing)
	 */
	[HideInInspector]
	public int traversableTags = -1;

	/** Required for serialization backwards compatibility.
	 * \since 3.6.8
	 */
	[UnityEngine.Serialization.FormerlySerializedAs("traversableTags")]
	[SerializeField]
	[HideInInspector]
	protected TagMask traversableTagsCompatibility = new TagMask(-1, -1);

	/** Penalties for each tag.
	 * Tag 0 which is the default tag, will have added a penalty of tagPenalties[0].
	 * These should only be positive values since the A* algorithm cannot handle negative penalties.
	 *
	 * \note This array should always have a length of 32 otherwise the system will ignore it.
	 *
	 * \see Pathfinding.Path.tagPenalties
	 */
	[HideInInspector]
	public int[] tagPenalties = new int[32];

	/** Callback for when a path is completed.
	 * Movement scripts should register to this delegate.\n
	 * A temporary callback can also be set when calling StartPath, but that delegate will only be called for that path
	 */
	public OnPathDelegate pathCallback;

	/** Called before pathfinding is started */
	public OnPathDelegate preProcessPath;

	/** Called after a path has been calculated, right before modifiers are executed.
	 * Can be anything which only modifies the positions (Vector3[]).
	 */
	public OnPathDelegate postProcessPath;

	/** Used for drawing gizmos */
	[System.NonSerialized]
	List<Vector3> lastCompletedVectorPath;

	/** Used for drawing gizmos */
	[System.NonSerialized]
	List<GraphNode> lastCompletedNodePath;

	/** The current path */
	[System.NonSerialized]
	protected Path path;

	/** Previous path. Used to draw gizmos */
	[System.NonSerialized]
	private Path prevPath;

	/** Cached delegate to avoid allocating one every time a path is started */
	private readonly OnPathDelegate onPathDelegate;
	/** Cached delegate to avoid allocating one every time a path is started */
	private readonly OnPathDelegate onPartialPathDelegate;

	/** Temporary callback only called for the current path. This value is set by the StartPath functions */
	private OnPathDelegate tmpPathCallback;

	/** The path ID of the last path queried */
	protected uint lastPathID;

	/** Internal list of all modifiers */
	readonly List<IPathModifier> modifiers = new List<IPathModifier>();

	public enum ModifierPass {
		PreProcess,
		// An obsolete item occupied index 1 previously
		PostProcess = 2,
	}

	public Seeker () {
		onPathDelegate = OnPathComplete;
		onPartialPathDelegate = OnPartialPathComplete;
	}

	/** Initializes a few variables */
	protected override void Awake () {
		base.Awake();
		startEndModifier.Awake(this);
	}

	/** Path that is currently being calculated or was last calculated.
	 * You should rarely have to use this. Instead get the path when the path callback is called.
	 *
	 * \see pathCallback
	 */
	public Path GetCurrentPath () {
		return path;
	}

	/** Stop calculating the current path request.
	 * If this Seeker is currently calculating a path it will be canceled.
	 * The callback (usually to a method named OnPathComplete) will soon be called
	 * with a path that has the 'error' field set to true.
	 *
	 * This does not stop the character from moving, it just aborts
	 * the path calculation.
	 *
	 * \param pool If true then the path will be pooled when the pathfinding system is done with it.
	 */
	public void CancelCurrentPathRequest (bool pool = true) {
		if (!IsDone()) {
			path.Error();
			if (pool) {
				// Make sure the path has had its reference count incremented and decremented once.
				// If this is not done the system will think no pooling is used at all and will not pool the path.
				// The particular object that is used as the parameter (in this case 'path') doesn't matter at all
				// it just has to be *some* object.
				path.Claim(path);
				path.Release(path);
			}
		}
	}

	/** Cleans up some variables.
	 * Releases any eventually claimed paths.
	 * Calls OnDestroy on the #startEndModifier.
	 *
	 * \see ReleaseClaimedPath
	 * \see startEndModifier
	 */
	public void OnDestroy () {
		ReleaseClaimedPath();
		startEndModifier.OnDestroy(this);
	}

	/** Releases the path used for gizmos (if any).
	 * The seeker keeps the latest path claimed so it can draw gizmos.
	 * In some cases this might not be desireable and you want it released.
	 * In that case, you can call this method to release it (not that path gizmos will then not be drawn).
	 *
	 * If you didn't understand anything from the description above, you probably don't need to use this method.
	 *
	 * \see \ref pooling
	 */
	public void ReleaseClaimedPath () {
		if (prevPath != null) {
			prevPath.Release(this, true);
			prevPath = null;
		}
	}

	/** Called by modifiers to register themselves */
	public void RegisterModifier (IPathModifier modifier) {
		modifiers.Add(modifier);

		// Sort the modifiers based on their specified order
		modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
	}

	/** Called by modifiers when they are disabled or destroyed */
	public void DeregisterModifier (IPathModifier modifier) {
		modifiers.Remove(modifier);
	}

	/** Post Processes the path.
	 * This will run any modifiers attached to this GameObject on the path.
	 * This is identical to calling RunModifiers(ModifierPass.PostProcess, path)
	 * \see RunModifiers
	 * \since Added in 3.2
	 */
	public void PostProcess (Path p) {
		RunModifiers(ModifierPass.PostProcess, p);
	}

	/** Runs modifiers on path \a p */
	public void RunModifiers (ModifierPass pass, Path p) {
		// Call delegates if they exist
		if (pass == ModifierPass.PreProcess && preProcessPath != null) {
			preProcessPath(p);
		} else if (pass == ModifierPass.PostProcess && postProcessPath != null) {
			postProcessPath(p);
		}

		// Loop through all modifiers and apply post processing
		for (int i = 0; i < modifiers.Count; i++) {
			// Cast to MonoModifier, i.e modifiers attached as scripts to the game object
			var mMod = modifiers[i] as MonoModifier;

			// Ignore modifiers which are not enabled
			if (mMod != null && !mMod.enabled) continue;

			if (pass == ModifierPass.PreProcess) {
				modifiers[i].PreProcess(p);
			} else if (pass == ModifierPass.PostProcess) {
				modifiers[i].Apply(p);
			}
		}
	}

	/** Is the current path done calculating.
	 * Returns true if the current #path has been returned or if the #path is null.
	 *
	 * \note Do not confuse this with Pathfinding.Path.IsDone. They usually return the same value, but not always
	 * since the path might be completely calculated, but it has not yet been processed by the Seeker.
	 *
	 * \since Added in 3.0.8
	 * \version Behaviour changed in 3.2
	 */
	public bool IsDone () {
		return path == null || path.PipelineState >= PathState.Returned;
	}

	/** Called when a path has completed.
	 * This should have been implemented as optional parameter values, but that didn't seem to work very well with delegates (the values weren't the default ones)
	 * \see OnPathComplete(Path,bool,bool)
	 */
	void OnPathComplete (Path p) {
		OnPathComplete(p, true, true);
	}

	/** Called when a path has completed.
	 * Will post process it and return it by calling #tmpPathCallback and #pathCallback
	 */
	void OnPathComplete (Path p, bool runModifiers, bool sendCallbacks) {
		if (p != null && p != path && sendCallbacks) {
			return;
		}

		if (this == null || p == null || p != path)
			return;

		if (!path.error && runModifiers) {
			// This will send the path for post processing to modifiers attached to this Seeker
			RunModifiers(ModifierPass.PostProcess, path);
		}

		if (sendCallbacks) {
			p.Claim(this);

			lastCompletedNodePath = p.path;
			lastCompletedVectorPath = p.vectorPath;

			// This will send the path to the callback (if any) specified when calling StartPath
			if (tmpPathCallback != null) {
				tmpPathCallback(p);
			}

			// This will send the path to any script which has registered to the callback
			if (pathCallback != null) {
				pathCallback(p);
			}

			// Recycle the previous path to reduce the load on the GC
			if (prevPath != null) {
				prevPath.Release(this, true);
			}

			prevPath = p;

			// If not drawing gizmos, then storing prevPath is quite unecessary
			// So clear it and set prevPath to null
			if (!drawGizmos) ReleaseClaimedPath();
		}
	}

	/** Called for each path in a MultiTargetPath.
	 * Only post processes the path, does not return it.
	 * \astarpro */
	void OnPartialPathComplete (Path p) {
		OnPathComplete(p, true, false);
	}

	/** Called once for a MultiTargetPath. Only returns the path, does not post process.
	 * \astarpro */
	void OnMultiPathComplete (Path p) {
		OnPathComplete(p, false, true);
	}

	/** Returns a new path instance.
	 * The path will be taken from the path pool if path recycling is turned on.\n
	 * This path can be sent to #StartPath(Path,OnPathDelegate,int) with no change, but if no change is required #StartPath(Vector3,Vector3,OnPathDelegate) does just that.
	 * \code
	 * var seeker = GetComponent<Seeker>();
	 * Path p = seeker.GetNewPath (transform.position, transform.position+transform.forward*100);
	 * // Disable heuristics on just this path for example
	 * p.heuristic = Heuristic.None;
	 * seeker.StartPath (p, OnPathComplete);
	 * \endcode
	 * \deprecated Use ABPath.Construct(start, end, null) instead.
	 */
	[System.Obsolete("Use ABPath.Construct(start, end, null) instead")]
	public ABPath GetNewPath (Vector3 start, Vector3 end) {
		// Construct a path with start and end points
		return ABPath.Construct(start, end, null);
	}

	/** Call this function to start calculating a path.
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 */
	public Path StartPath (Vector3 start, Vector3 end) {
		return StartPath(start, end, null, -1);
	}

	/** Call this function to start calculating a path.
	 *
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 * \param callback	The function to call when the path has been calculated
	 *
	 * \a callback will be called when the path has completed.
	 * \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed) */
	public Path StartPath (Vector3 start, Vector3 end, OnPathDelegate callback) {
		return StartPath(start, end, callback, -1);
	}

	/** Call this function to start calculating a path.
	 *
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 * \param callback	The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See #Pathfinding.NNConstraint.graphMask.
	 *
	 * \a callback will be called when the path has completed.
	 * \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed) */
	public Path StartPath (Vector3 start, Vector3 end, OnPathDelegate callback, int graphMask) {
		return StartPath(ABPath.Construct(start, end, null), callback, graphMask);
	}

	/** Call this function to start calculating a path.
	 *
	 * \param p			The path to start calculating
	 * \param callback	The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See #Pathfinding.NNConstraint.graphMask.
	 *
	 * The \a callback will be called when the path has been calculated (which may be several frames into the future).
	 * The \a callback will not be called if a new path request is started before this path request has been calculated.
	 *
	 * \version Since 3.8.3 this method works properly if a MultiTargetPath is used.
	 * It now behaves identically to the StartMultiTargetPath(MultiTargetPath) method.
	 */
	public Path StartPath (Path p, OnPathDelegate callback = null, int graphMask = -1) {
		var mtp = p as MultiTargetPath;

		if (mtp != null) {
			// TODO: Allocation, cache
			var callbacks = new OnPathDelegate[mtp.targetPoints.Length];

			for (int i = 0; i < callbacks.Length; i++) {
				callbacks[i] = onPartialPathDelegate;
			}

			mtp.callbacks = callbacks;
			p.callback += OnMultiPathComplete;
		} else {
			p.callback += onPathDelegate;
		}

		p.enabledTags = traversableTags;
		p.tagPenalties = tagPenalties;
		p.nnConstraint.graphMask = graphMask;

		StartPathInternal(p, callback);
		return p;
	}

	/** Internal method to start a path and mark it as the currently active path */
	void StartPathInternal (Path p, OnPathDelegate callback) {
		// Cancel a previously requested path is it has not been processed yet and also make sure that it has not been recycled and used somewhere else
		if (path != null && path.PipelineState <= PathState.Processing && path.CompleteState != PathCompleteState.Error && lastPathID == path.pathID) {
			path.Error();
			path.LogError("Canceled path because a new one was requested.\n"+
				"This happens when a new path is requested from the seeker when one was already being calculated.\n" +
				"For example if a unit got a new order, you might request a new path directly instead of waiting for the now" +
				" invalid path to be calculated. Which is probably what you want.\n" +
				"If you are getting this a lot, you might want to consider how you are scheduling path requests.");
			// No callback will be sent for the canceled path
		}

		// Set p as the active path
		path = p;
		tmpPathCallback = callback;

		// Save the path id so we can make sure that if we cancel a path (see above) it should not have been recycled yet.
		lastPathID = path.pathID;

		// Pre process the path
		RunModifiers(ModifierPass.PreProcess, path);

		// Send the request to the pathfinder
		AstarPath.StartPath(path);
	}

	/** Starts a Multi Target Path from one start point to multiple end points.
	 * A Multi Target Path will search for all the end points in one search and will return all paths if \a pathsForAll is true, or only the shortest one if \a pathsForAll is false.\n
	 *
	 * \param start			The start point of the path
	 * \param endPoints		The end points of the path
	 * \param pathsForAll	Indicates whether or not a path to all end points should be searched for or only to the closest one
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask		Mask used to specify which graphs should be searched for close nodes. See Pathfinding.NNConstraint.graphMask.
	 *
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro
	 * \see Pathfinding.MultiTargetPath
	 * \see \ref MultiTargetPathExample.cs "Example of how to use multi-target-paths"
	 */
	public MultiTargetPath StartMultiTargetPath (Vector3 start, Vector3[] endPoints, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1) {
		MultiTargetPath p = MultiTargetPath.Construct(start, endPoints, null, null);

		p.pathsForAll = pathsForAll;
		StartPath(p, callback, graphMask);
		return p;
	}

	/** Starts a Multi Target Path from multiple start points to a single target point.
	 * A Multi Target Path will search from all start points to the target point in one search and will return all paths if \a pathsForAll is true, or only the shortest one if \a pathsForAll is false.\n
	 *
	 * \param startPoints	The start points of the path
	 * \param end			The end point of the path
	 * \param pathsForAll	Indicates whether or not a path from all start points should be searched for or only to the closest one
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask		Mask used to specify which graphs should be searched for close nodes. See Pathfinding.NNConstraint.graphMask.
	 *
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro
	 * \see Pathfinding.MultiTargetPath
	 * \see \ref MultiTargetPathExample.cs "Example of how to use multi-target-paths"
	 */
	public MultiTargetPath StartMultiTargetPath (Vector3[] startPoints, Vector3 end, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1) {
		MultiTargetPath p = MultiTargetPath.Construct(startPoints, end, null, null);

		p.pathsForAll = pathsForAll;
		StartPath(p, callback, graphMask);
		return p;
	}

	/** Starts a Multi Target Path.
	 * Takes a MultiTargetPath and wires everything up for it to send callbacks to the seeker for post-processing.\n
	 *
	 * \param p				The path to start calculating
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See Pathfinding.NNConstraint.graphMask.
	 *
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro
	 * \see Pathfinding.MultiTargetPath
	 * \see \ref MultiTargetPathExample.cs "Example of how to use multi-target-paths"
	 *
	 * \version Since 3.8.3 calling this method behaves identically to calling StartPath with a MultiTargetPath.
	 * \version Since 3.8.3 this method also sets enabledTags and tagPenalties on the path object.
	 *
	 * \deprecated You can use StartPath instead of this method now. It will behave identically.
	 */
	[System.Obsolete("You can use StartPath instead of this method now. It will behave identically.")]
	public MultiTargetPath StartMultiTargetPath (MultiTargetPath p, OnPathDelegate callback = null, int graphMask = -1) {
		StartPath(p, callback, graphMask);
		return p;
	}

	/** Draws gizmos for the Seeker */
	public void OnDrawGizmos () {
		if (lastCompletedNodePath == null || !drawGizmos) {
			return;
		}

		if (detailedGizmos) {
			Gizmos.color = new Color(0.7F, 0.5F, 0.1F, 0.5F);

			if (lastCompletedNodePath != null) {
				for (int i = 0; i < lastCompletedNodePath.Count-1; i++) {
					Gizmos.DrawLine((Vector3)lastCompletedNodePath[i].position, (Vector3)lastCompletedNodePath[i+1].position);
				}
			}
		}

		Gizmos.color = new Color(0, 1F, 0, 1F);

		if (lastCompletedVectorPath != null) {
			for (int i = 0; i < lastCompletedVectorPath.Count-1; i++) {
				Gizmos.DrawLine(lastCompletedVectorPath[i], lastCompletedVectorPath[i+1]);
			}
		}
	}

	protected override int OnUpgradeSerializedData (int version) {
		if (version == 0 && traversableTagsCompatibility != null && traversableTagsCompatibility.tagsChange != -1) {
			traversableTags = traversableTagsCompatibility.tagsChange;
			traversableTagsCompatibility = new TagMask(-1, -1);
		}
		return 1;
	}
}
