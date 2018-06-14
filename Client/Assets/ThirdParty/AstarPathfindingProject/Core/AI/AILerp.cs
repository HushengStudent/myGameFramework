using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;

/** Linearly interpolating movement script.
 * This movement script will follow the path exactly, it uses linear interpolation to move between the waypoints in the path.
 * This is desirable for some types of games.
 * It also works in 2D.
 *
 * Recommended setup:
 *
 * This depends on what type of movement you are aiming for.
 * If you are aiming for movement where the unit follows the path exactly (you are likely using a grid or point graph)
 * the default settings on this component should work quite well, however I recommend that you adjust the StartEndModifier
 * on the Seeker component: set the 'Exact Start Point' field to 'NodeConnection' and the 'Exact End Point' field to 'SnapToNode'.
 *
 * If you on the other hand want smoother movement I recommend adding the Simple Smooth Modifier to the GameObject as well.
 * You may also want to tweak the #rotationSpeed.
 *
 * \ingroup movementscripts
 */
[RequireComponent(typeof(Seeker))]
[AddComponentMenu("Pathfinding/AI/AILerp (2D,3D)")]
[HelpURL("http://arongranberg.com/astar/docs/class_a_i_lerp.php")]
public class AILerp : VersionedMonoBehaviour {
	/** Determines how often it will search for new paths.
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
	public float repathRate = 0.5F;

	/** Target to move towards.
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
	public Transform target;

	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
	public bool canSearch = true;

	/** Enables or disables movement.
	 * \see #canSearch */
	public bool canMove = true;

	/** Speed in world units */
	public float speed = 3;

	/** If true, the AI will rotate to face the movement direction */
	public bool enableRotation = true;

	/** If true, rotation will only be done along the Z axis so that the Y axis is the forward direction of the character.
	 * This is useful for 2D games in which one often want to have the Y axis as the forward direction to get sprites and 2D colliders to work properly.
	 * \shadowimage{aibase_forward_axis.png}
	 */
	public bool rotationIn2D = false;

	/** How quickly to rotate */
	public float rotationSpeed = 10;

	/** If true, some interpolation will be done when a new path has been calculated.
	 * This is used to avoid short distance teleportation.
	 */
	public bool interpolatePathSwitches = true;

	/** How quickly to interpolate to the new path */
	public float switchPathInterpolationSpeed = 5;

	/** Cached Seeker component */
	protected Seeker seeker;

	/** Cached Transform component */
	protected Transform tr;

	/** Time when the last path request was sent */
	protected float lastRepath = -9999;

	/** Current path which is followed */
	protected ABPath path;

	/** True if the end-of-path is reached.
	 * \see TargetReached */
	public bool targetReached { get; private set; }

	/** Only when the previous path has been returned should be search for a new path */
	protected bool canSearchAgain = true;

	/** When a new path was returned, the AI was moving along this ray.
	 * Used to smoothly interpolate between the previous movement and the movement along the new path.
	 * The speed is equal to movement direction.
	 */
	protected Vector3 previousMovementOrigin;
	protected Vector3 previousMovementDirection;
	protected float previousMovementStartTime = -9999;

	protected PathInterpolator interpolator = new PathInterpolator();

	/** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
	private bool startHasRun = false;

	/** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	 * */
	protected override void Awake () {
		base.Awake();
		//This is a simple optimization, cache the transform component lookup
		tr = transform;

		seeker = GetComponent<Seeker>();

		// Tell the StartEndModifier to ask for our exact position when post processing the path This
		// is important if we are using prediction and requesting a path from some point slightly ahead
		// of us since then the start point in the path request may be far from our position when the
		// path has been calculated. This is also good because if a long path is requested, it may take
		// a few frames for it to be calculated so we could have moved some distance during that time
		seeker.startEndModifier.adjustStartPoint = () => tr.position;
	}

	/** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see #Init
	 * \see #RepeatTrySearchPath
	 */
	protected virtual void Start () {
		startHasRun = true;
		Init();
	}

	/** Called when the component is enabled */
	protected virtual void OnEnable () {
		// Make sure we receive callbacks when paths complete
		seeker.pathCallback += OnPathComplete;
		Init();
	}

	void Init () {
		if (startHasRun) {
			lastRepath = float.NegativeInfinity;
			StartCoroutine(RepeatTrySearchPath());
		}
	}

	public void OnDisable () {
		// Abort any calculations in progress
		if (seeker != null) seeker.CancelCurrentPathRequest();
		canSearchAgain = true;

		// Release the current path so that it can be pooled
		if (path != null) path.Release(this);
		path = null;

		// Make sure we no longer receive callbacks when paths complete
		seeker.pathCallback -= OnPathComplete;
	}

	/** Tries to search for a path every #repathRate seconds.
	 * \see TrySearchPath
	 */
	protected IEnumerator RepeatTrySearchPath () {
		while (true) {
			float v = TrySearchPath();
			yield return new WaitForSeconds(v);
		}
	}

	/** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true and there is a target.
	 *
	 * \returns The time to wait until calling this function again (based on #repathRate)
	 */
	public float TrySearchPath () {
		if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch && target != null) {
			SearchPath();
			return repathRate;
		} else {
			return Mathf.Max(0, repathRate - (Time.time-lastRepath));
		}
	}

	/** Requests a path to the target.
	 * Some inheriting classes will prevent the path from being requested immediately when
	 * this function is called, for example when the AI is currently traversing a special path segment
	 * in which case it is usually a bad idea to search for a new path.
	 */
	public virtual void SearchPath () {
		ForceSearchPath();
	}

	/** Requests a path to the target.
	 * Bypasses 'is-it-a-good-time-to-request-a-path' checks.
	 */
	public virtual void ForceSearchPath () {
		if (target == null) throw new System.InvalidOperationException("Target is null");

		lastRepath = Time.time;
		// This is where we should search to
		var targetPosition = target.position;
		var currentPosition = GetFeetPosition();

		// If we are following a path, start searching from the node we will
		// reach next this can prevent odd turns right at the start of the path
		if (interpolator.valid) {
			var prevDist = interpolator.distance;
			// Move to the end of the current segment
			interpolator.MoveToSegment(interpolator.segmentIndex, 1);
			currentPosition = interpolator.position;
			// Move back to the original position
			interpolator.distance = prevDist;
		}

		canSearchAgain = false;

		// Alternative way of requesting the path
		//ABPath p = ABPath.Construct (currentPosition,targetPoint,null);
		//seeker.StartPath (p);

		// We should search from the current position
		seeker.StartPath(currentPosition, targetPosition);
	}

	/** The end of the path has been reached.
	 * If you want custom logic for when the AI has reached it's destination
	 * add it here.
	 * You can also create a new script which inherits from this one
	 * and override the function in that script.
	 */
	public virtual void OnTargetReached () {
	}

	/** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
	public virtual void OnPathComplete (Path _p) {
		ABPath p = _p as ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

		canSearchAgain = true;

		// Increase the reference count on the path.
		// This is used for path pooling
		p.Claim(this);

		// Path couldn't be calculated of some reason.
		// More info in p.errorLog (debug string)
		if (p.error) {
			p.Release(this);
			return;
		}

		if (interpolatePathSwitches) {
			ConfigurePathSwitchInterpolation();
		}

		// Release the previous path
		// This is used for path pooling.
		// Note that this will invalidate the interpolator
		// since the vectorPath list will be pooled.
		if (path != null) path.Release(this);

		// Replace the old path
		path = p;
		targetReached = false;

		// Just for the rest of the code to work, if there
		// is only one waypoint in the path add another one
		if (path.vectorPath != null && path.vectorPath.Count == 1) {
			path.vectorPath.Insert(0, GetFeetPosition());
		}

		// Reset some variables
		ConfigureNewPath();
	}

	protected virtual void ConfigurePathSwitchInterpolation () {
		bool reachedEndOfPreviousPath = interpolator.valid && interpolator.remainingDistance < 0.0001f;

		if (interpolator.valid && !reachedEndOfPreviousPath) {
			previousMovementOrigin = interpolator.position;
			previousMovementDirection = interpolator.tangent.normalized * interpolator.remainingDistance;
			previousMovementStartTime = Time.time;
		} else {
			previousMovementOrigin = Vector3.zero;
			previousMovementDirection = Vector3.zero;
			previousMovementStartTime = -9999;
		}
	}

	public virtual Vector3 GetFeetPosition () {
		return tr.position;
	}

	/** Finds the closest point on the current path and configures the #interpolator */
	protected virtual void ConfigureNewPath () {
		var hadValidPath = interpolator.valid;
		var prevTangent = hadValidPath ? interpolator.tangent : Vector3.zero;

		interpolator.SetPath(path.vectorPath);
		interpolator.MoveToClosestPoint(GetFeetPosition());

		if (interpolatePathSwitches && switchPathInterpolationSpeed > 0.01f && hadValidPath) {
			var correctionFactor = Mathf.Max(-Vector3.Dot(prevTangent.normalized, interpolator.tangent.normalized), 0);
			interpolator.distance -= speed*correctionFactor*(1f/switchPathInterpolationSpeed);
		}
	}

	protected virtual void Update () {
		if (canMove) {
			Vector3 direction;
			Vector3 nextPos = CalculateNextPosition(out direction);

			// Rotate unless we are really close to the target
			if (enableRotation && direction != Vector3.zero) {
				if (rotationIn2D) {
					float angle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 180;
					Vector3 euler = tr.eulerAngles;
					euler.z = Mathf.LerpAngle(euler.z, angle, Time.deltaTime * rotationSpeed);
					tr.eulerAngles = euler;
				} else {
					Quaternion rot = tr.rotation;
					Quaternion desiredRot = Quaternion.LookRotation(direction);

					tr.rotation = Quaternion.Slerp(rot, desiredRot, Time.deltaTime * rotationSpeed);
				}
			}

			tr.position = nextPos;
		}
	}

	/** Calculate the AI's next position (one frame in the future).
	 * \param direction The tangent of the segment the AI is currently traversing. Not normalized.
	 */
	protected virtual Vector3 CalculateNextPosition (out Vector3 direction) {
		if (!interpolator.valid) {
			direction = Vector3.zero;
			return tr.position;
		}

		interpolator.distance += Time.deltaTime * speed;
		if (interpolator.remainingDistance < 0.0001f && !targetReached) {
			targetReached = true;
			OnTargetReached();
		}

		direction = interpolator.tangent;
		float alpha = switchPathInterpolationSpeed * (Time.time - previousMovementStartTime);
		if (interpolatePathSwitches && alpha < 1f) {
			// Find the approximate position we would be at if we
			// would have continued to follow the previous path
			Vector3 positionAlongPreviousPath = previousMovementOrigin + Vector3.ClampMagnitude(previousMovementDirection, speed * (Time.time - previousMovementStartTime));

			// Interpolate between the position on the current path and the position
			// we would have had if we would have continued along the previous path.
			return Vector3.Lerp(positionAlongPreviousPath, interpolator.position, alpha);
		} else {
			return interpolator.position;
		}
	}
}
