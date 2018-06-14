using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding {
	using Pathfinding.RVO;
	using Pathfinding.Util;

	[RequireComponent(typeof(Seeker))]
	[AddComponentMenu("Pathfinding/AI/RichAI (3D, for navmesh)")]
	/** Advanced AI for navmesh based graphs.
	 * \astarpro
	 */
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_rich_a_i.php")]
	public class RichAI : AIBase {
		/** Target position to move to */
		public Transform target;

		/** Search for new paths repeatedly.
		 *
		 * \see repathRate
		 * \see SearchPaths
		  */
		public bool repeatedlySearchPaths = false;

		/** Delay (seconds) between path searches.
		 * The agent will plan a new path to the target every N seconds.
		 *
		 * \see SearchPaths
		 */
		public float repathRate = 0.5f;

		/** Max speed of the agent.
		 * In world units per second.
		 */
		public float maxSpeed = 1;

		/** Max acceleration of the agent.
		 * In world units per second per second.
		 */
		public float acceleration = 5;

		/** Max rotation speed of the agent.
		 * In degrees per second.
		 */
		public float rotationSpeed = 360;

		/** How long before reaching the end of the path to start to slow down.
		 * A lower value will make the agent stop more abruptly.
		 *
		 * \note The agent may require more time to slow down if
		 * its maximum #acceleration is not high enough.
		 *
		 * If set to zero the agent will not even attempt to slow down.
		 * This can be useful if the target point is not a point you want the agent to stop at
		 * but it might for example be the player and you want the AI to slam into the player.
		 *
		 * \note A value of zero will behave differently from a small but non-zero value (such as 0.0001).
		 * When it is non-zero the agent will still respect its #acceleration when determining if it needs
		 * to slow down, but if it is zero it will disable that check.
		 *
		 * \htmlonly <video class="tinyshadow" controls loop><source src="images/richai_slowdown_time.mp4" type="video/mp4"></video> \endhtmlonly
		 */
		public float slowdownTime = 0.5f;

		/** Max distance to the endpoint to consider it reached.
		 * In seconds.
		 *
		 * \see #TargetReached
		 * \see #OnTargetReached
		 */
		public float endReachedDistance = 0.01f;

		/** Force to avoid walls with.
		 * The agent will try to steer away from walls slightly.
		 *
		 * \see #wallDist
		 */
		public float wallForce = 3;

		/** Walls within this range will be used for avoidance.
		 * Setting this to zero disables wall avoidance and may improve performance slightly
		 *
		 * \see #wallForce
		 */
		public float wallDist = 1;

		/** Use funnel simplification.
		 * On tiled navmesh maps, but sometimes on normal ones as well, it can be good to simplify
		 * the funnel as a post-processing step to make the paths straighter.
		 *
		 * This has a moderate performance impact during frames when a path calculation is completed.
		 */
		public bool funnelSimplification = false;
		public Animation anim;

		/** Slow down when not facing the target direction.
		 * Incurs at a small performance overhead.
		 */
		public bool slowWhenNotFacingTarget = true;

		/** Position of the agent at the end of the previous frame */
		protected Vector3 prevPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		/** Current velocity of the agent */
		protected Vector3 realVelocity;

		/** Holds the current path that this agent is following */
		protected readonly RichPath richPath = new RichPath();

		Vector3 waypoint;

		protected bool waitingForPathCalc;
		protected bool canSearchPath;
		protected bool delayUpdatePath;
		protected bool traversingSpecialPath;
		protected bool lastCorner;
		protected float distanceToWaypoint = float.PositiveInfinity;

		protected readonly List<Vector3> nextCorners = new List<Vector3>();
		protected readonly List<Vector3> wallBuffer = new List<Vector3>();

		bool startHasRun;
		protected float lastRepath = float.NegativeInfinity;

		/** Current velocity of the agent.
		 * Includes velocity due to gravity.
		 */
		public Vector3 Velocity {
			get {
				return realVelocity;
			}
		}

		public bool TraversingSpecial {
			get {
				return traversingSpecialPath;
			}
		}

		/** Waypoint that the agent is moving towards.
		 * This is either a corner in the path or the end of the path.
		 *
		 * \see #DistanceToNextWaypoint
		 */
		public Vector3 NextWaypoint {
			get {
				return waypoint;
			}
		}

		/** Distance to the next waypoint.
		 *
		 * \see #NextWaypoint
		 */
		public float DistanceToNextWaypoint {
			get {
				return distanceToWaypoint;
			}
		}

		/** True if the agent is within #endReachedDistance units from the end of the current path.
		 * Note that if the target was just changed, a new path may not be calculated immediately
		 * and thus this value will correspond to if the previous target had been reached.
		 *
		 * To be sure if a character has reached a particular target you can make sure to call #UpdatePath
		 * when you change the target position and then check for !PathPending && TargetReached.
		 * The #PathPending property indicates if the new path has been calculated yet.
		 */
		public bool TargetReached {
			get {
				return ApproachingPathEndpoint && DistanceToNextWaypoint < endReachedDistance;
			}
		}

		/** True if a path to the target is currently being calculated */
		public bool PathPending {
			get {
				return waitingForPathCalc || delayUpdatePath;
			}
		}

		/** True if approaching the last waypoint in the current part of the path.
		 * Path parts are separated by off-mesh links.
		 *
		 * \see #ApproachingPathEndpoint
		 */
		public bool ApproachingPartEndpoint {
			get {
				return lastCorner && nextCorners.Count == 1;
			}
		}

		/** True if approaching the last waypoint of all parts in the current path.
		 * Path parts are separated by off-mesh links.
		 *
		 * \see #ApproachingPartEndpoint
		 */
		public bool ApproachingPathEndpoint {
			get {
				return ApproachingPartEndpoint && richPath.IsLastPart;
			}
		}

		/** Current waypoint that the agent is moving towards.
		 * \deprecated This property has been renamed to #NextWaypoint
		 */
		[System.Obsolete("This property has been renamed to NextWaypoint")]
		public Vector3 TargetPoint { get { return NextWaypoint; } }

		/** Starts searching for paths.
		 * If you override this function you should in most cases call base.Start () at the start of it.
		 * \see #Init
		 * \see #SearchPaths
		 */
		protected virtual void Start () {
			startHasRun = true;
			Init();
		}

		/** Called when the component is enabled */
		protected virtual void OnEnable () {
			// Make sure we receive callbacks when paths are calculated
			seeker.pathCallback += OnPathComplete;
			Init();
		}

		void Init () {
			// Teleport the agent if we have moved from our previous position.
			// This check is important as the Teleport call will cancel our current path request
			// and if a script had created the RichAI component and in the same frame called
			// UpdatePath the path request would always be canceled when RichAI.Start was called.
			if (prevPosition != tr.position) Teleport(tr.position);

			if (startHasRun) {
				lastRepath = float.NegativeInfinity;
				StartCoroutine(SearchPaths());
			}
		}

		/** Instantly moves the agent to the target position.
		 * When setting transform.position directly the agent
		 * will be clamped to the part of the navmesh it can
		 * reach, so it may not end up where you wanted it to.
		 * This ensures that the agent can move to any part of the navmesh.
		 *
		 * The current path will be cleared.
		 *
		 * \see Works similarly to Unity's NavmeshAgent.Warp.
		 */
		public void Teleport (Vector3 newPosition) {
			CancelCurrentPathRequest();
			// Clamp the new position to the navmesh
			var nearest = AstarPath.active.GetNearest(newPosition);
			float elevation;
			movementPlane.ToPlane(newPosition, out elevation);
			prevPosition = tr.position = movementPlane.ToWorld(movementPlane.ToPlane(nearest.node != null ? nearest.position : newPosition), elevation);
			richPath.Clear();
			if (rvoController != null) rvoController.Move(Vector3.zero);
		}

		protected void CancelCurrentPathRequest () {
			canSearchPath = true;
			waitingForPathCalc = false;
			// Abort calculation of the current path
			if (seeker != null) seeker.CancelCurrentPathRequest();
		}

		/** Called when the component is disabled */
		protected virtual void OnDisable () {
			CancelCurrentPathRequest();
			velocity2D = Vector3.zero;
			verticalVelocity = 0f;
			lastCorner = false;
			distanceToWaypoint = float.PositiveInfinity;

			// We don't want to listen for path complete notifications any longer
			seeker.pathCallback -= OnPathComplete;
		}

		/** Force recalculation of the current path.
		 * If there is an ongoing path calculation, it will be canceled (so make sure you leave time for the paths to get calculated before calling this function again).
		 *
		 * \see #Seeker.IsDone
		 */
		public virtual void UpdatePath () {
			CancelCurrentPathRequest();

			waitingForPathCalc = true;
			lastRepath = Time.time;
			seeker.StartPath(tr.position, target.position);
		}

		/** Searches for paths periodically */
		IEnumerator SearchPaths () {
			while (true) {
				while (!repeatedlySearchPaths || waitingForPathCalc || !canSearchPath || Time.time - lastRepath < repathRate) yield return null;

				UpdatePath();
				yield return null;
			}
		}

		void OnPathComplete (Path p) {
			waitingForPathCalc = false;
			p.Claim(this);

			if (p.error) {
				p.Release(this);
				return;
			}

			if (traversingSpecialPath) {
				delayUpdatePath = true;
			} else {
				richPath.Initialize(seeker, p, true, funnelSimplification);

				// Check if we have already reached the end of the path
				// We need to do this here to make sure that the #TargetReached
				// property is up to date.
				var part = richPath.GetCurrentPart() as RichFunnel;
				if (part != null) {
					var position = movementPlane.ToPlane(UpdateTarget(part));
					if (lastCorner && nextCorners.Count == 1) {
						// Target point
						Vector2 targetPoint = waypoint = movementPlane.ToPlane(nextCorners[0]);
						distanceToWaypoint = (targetPoint - position).magnitude;
						if (distanceToWaypoint <= endReachedDistance) {
							NextPart();
						}
					}
				}
			}
			p.Release(this);
		}

		/** Declare that the AI has completely traversed the current part.
		 * This will skip to the next part, or call OnTargetReached if this was the last part
		 */
		protected void NextPart () {
			if (!richPath.CompletedAllParts) {
				if (!richPath.IsLastPart) lastCorner = false;
				richPath.NextPart();
				if (richPath.CompletedAllParts) {
					//End
					OnTargetReached();
				}
			}
		}

		/** Called when the end of the path is reached */
		protected virtual void OnTargetReached () {
		}

		protected virtual Vector3 UpdateTarget (RichFunnel fn) {
			nextCorners.Clear();

			// Current position. We read and write to tr.position as few times as possible since doing so
			// is much slower than to read and write from/to a local variable
			bool requiresRepath;
			Vector3 position = fn.Update(tr.position, nextCorners, 2, out lastCorner, out requiresRepath);

			if (requiresRepath && !waitingForPathCalc) {
				UpdatePath();
			}

			return position;
		}

		/** Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not */
		protected override void MovementUpdate (float deltaTime) {
			RichPathPart currentPart = richPath.GetCurrentPart();
			var funnel = currentPart as RichFunnel;

			if (funnel != null) {
				TraverseFunnel(funnel, deltaTime);
			} else if (currentPart is RichSpecial) {
				// The current path part is a special part, for example a link
				// Movement during this part of the path is handled by the TraverseSpecial coroutine
				if (!traversingSpecialPath) {
					StartCoroutine(TraverseSpecial(currentPart as RichSpecial));
				}
			} else {
				// Unknown or null path part, just try to stand still
				if (rvoController != null && rvoController.enabled) {
					// Use RVOController
					rvoController.Move(Vector3.zero);
				}
				Move(tr.position, Vector3.zero);
			}

			var pos = tr.position;
			realVelocity = deltaTime > 0 ? (pos - prevPosition) / deltaTime : Vector3.zero;
			prevPosition = pos;
		}

		void TraverseFunnel (RichFunnel fn, float deltaTime) {
			// Clamp the current position to the navmesh
			// and update the list of upcoming corners in the path
			// and store that in the 'nextCorners' variable
			float elevation;
			Vector2 position = movementPlane.ToPlane(UpdateTarget(fn), out elevation);

			// Only find nearby walls every 5th frame to improve performance
			if (Time.frameCount % 5 == 0 && wallForce > 0 && wallDist > 0) {
				wallBuffer.Clear();
				fn.FindWalls(wallBuffer, wallDist);
			}

			// Target point
			Vector2 targetPoint = waypoint = movementPlane.ToPlane(nextCorners[0]);
			// Direction to target
			Vector2 dir = targetPoint - position;

			// Is the endpoint of the path (part) the current target point
			bool targetIsEndPoint = lastCorner && nextCorners.Count == 1;

			// Normalized direction to the target
			Vector2 normdir = VectorMath.Normalize(dir, out distanceToWaypoint);
			// Calculate force from walls
			Vector2 wallForceVector = CalculateWallForce(position, elevation, normdir);
			Vector2 targetVelocity;

			if (targetIsEndPoint) {
				targetVelocity = slowdownTime > 0 ? Vector2.zero : normdir * maxSpeed;

				// Reduce the wall avoidance force as we get closer to our target
				wallForceVector *= System.Math.Min(distanceToWaypoint/0.5f, 1);

				if (distanceToWaypoint <= endReachedDistance) {
					// END REACHED
					NextPart();
				}
			} else {
				var nextNextCorner = nextCorners.Count > 1 ? movementPlane.ToPlane(nextCorners[1]) : position + 2*dir;
				targetVelocity = (nextNextCorner - targetPoint).normalized * maxSpeed;
			}

			Vector2 accel = MovementUtilities.CalculateAccelerationToReachPoint(targetPoint - position, targetVelocity, velocity2D, acceleration, maxSpeed);

			// Update the velocity using the acceleration
			velocity2D += (accel + wallForceVector*wallForce)*deltaTime;

			// Distance to the end of the path (as the crow flies)
			var distToEndOfPath = fn.DistanceToEndOfPath;
			var slowdownFactor = slowdownTime > 0 ? distToEndOfPath / (maxSpeed * slowdownTime) : 1;

			velocity2D = MovementUtilities.ClampVelocity(velocity2D, maxSpeed, slowdownFactor, slowWhenNotFacingTarget, movementPlane.ToPlane(rotationIn2D ? tr.up : tr.forward));

			ApplyGravity(deltaTime);

			if (rvoController != null && rvoController.enabled) {
				// Send a message to the RVOController that we want to move
				// with this velocity. In the next simulation step, this
				// velocity will be processed and it will be fed back to the
				// rvo controller and finally it will be used by this script
				// when calling the CalculateMovementDelta method below

				// Make sure that we don't move further than to the end point
				// of the path. If the RVO simulation FPS is low and we did
				// not do this, the agent might overshoot the target a lot.
				var rvoTarget = movementPlane.ToWorld(position + Vector2.ClampMagnitude(velocity2D, distToEndOfPath), elevation);
				rvoController.SetTarget(rvoTarget, velocity2D.magnitude, maxSpeed);
			}

			// Direction and distance to move during this frame
			var deltaPosition = CalculateDeltaToMoveThisFrame(position, distToEndOfPath, deltaTime);

			// Rotate towards the direction we are moving in
			// Slow down the rotation of the character very close to the endpoint of the path to prevent oscillations
			var rotationSpeedFactor = targetIsEndPoint ? Mathf.Clamp01(1.1f * slowdownFactor - 0.1f) : 1f;
			RotateTowards(deltaPosition, rotationSpeed * rotationSpeedFactor * deltaTime);

			Move(movementPlane.ToWorld(position, elevation), movementPlane.ToWorld(deltaPosition, verticalVelocity * deltaTime));
		}

		protected override Vector3 ClampToNavmesh (Vector3 position) {
			if (richPath != null) {
				var funnel = richPath.GetCurrentPart() as RichFunnel;
				if (funnel != null) return funnel.ClampToNavmesh(position);
			}
			return position;
		}

		Vector2 CalculateWallForce (Vector2 position, float elevation, Vector2 directionToTarget) {
			if (wallForce <= 0 || wallDist <= 0) return Vector2.zero;

			float wLeft = 0;
			float wRight = 0;

			var position3D = movementPlane.ToWorld(position, elevation);
			for (int i = 0; i < wallBuffer.Count; i += 2) {
				Vector3 closest = VectorMath.ClosestPointOnSegment(wallBuffer[i], wallBuffer[i+1], position3D);
				float dist = (closest-position3D).sqrMagnitude;

				if (dist > wallDist*wallDist) continue;

				Vector2 tang = movementPlane.ToPlane(wallBuffer[i+1]-wallBuffer[i]).normalized;

				// Using the fact that all walls are laid out clockwise (looking from inside the obstacle)
				// Then left and right (ish) can be figured out like this
				float dot = Vector2.Dot(directionToTarget, tang);
				float weight = 1 - System.Math.Max(0, (2*(dist / (wallDist*wallDist))-1));
				if (dot > 0) wRight = System.Math.Max(wRight, dot * weight);
				else wLeft = System.Math.Max(wLeft, -dot * weight);
			}

			Vector2 normal = new Vector2(directionToTarget.y, -directionToTarget.x);
			return normal*(wRight-wLeft);
		}

		protected static readonly Color GizmoColorPath = new Color(8.0f/255, 78.0f/255, 194.0f/255);

		protected override void OnDrawGizmos () {
			base.OnDrawGizmos();

			if (tr != null) {
				Gizmos.color = GizmoColorPath;
				Vector3 p = tr.position;
				for (int i = 0; i < nextCorners.Count; p = nextCorners[i], i++) {
					Gizmos.DrawLine(p, nextCorners[i]);
				}
			}
		}

		protected virtual IEnumerator TraverseSpecial (RichSpecial rs) {
			traversingSpecialPath = true;
			velocity2D = Vector3.zero;

			var link = rs.nodeLink as AnimationLink;
			if (link == null) {
				Debug.LogError("Unhandled RichSpecial");
				yield break;
			}

			// Rotate character to face the correct direction
			// Magic number, should expose as variable
			while (Vector2.Angle(movementPlane.ToPlane(rotationIn2D ? tr.up : tr.forward), movementPlane.ToPlane(rs.first.forward)) > 5f) {
				RotateTowards(movementPlane.ToPlane(rs.first.forward), rotationSpeed * Time.deltaTime);
				yield return null;
			}

			// Reposition
			tr.parent.position = tr.position;

			tr.parent.rotation = tr.rotation;
			tr.localPosition = Vector3.zero;
			tr.localRotation = Quaternion.identity;

			// Set up animation speeds
			if (rs.reverse && link.reverseAnim) {
				anim[link.clip].speed = -link.animSpeed;
				anim[link.clip].normalizedTime = 1;
				anim.Play(link.clip);
				anim.Sample();
			} else {
				anim[link.clip].speed = link.animSpeed;
				anim.Rewind(link.clip);
				anim.Play(link.clip);
			}

			// Fix required for animations in reverse direction
			tr.parent.position -= tr.position-tr.parent.position;

			// Wait for the animation to finish
			yield return new WaitForSeconds(Mathf.Abs(anim[link.clip].length/link.animSpeed));

			traversingSpecialPath = false;
			NextPart();

			// If a path completed during the time we traversed the special connection, we need to recalculate it
			if (delayUpdatePath) {
				delayUpdatePath = false;
				UpdatePath();
			}
		}
	}
}
