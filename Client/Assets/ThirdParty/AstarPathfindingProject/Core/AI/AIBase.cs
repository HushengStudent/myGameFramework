using UnityEngine;

namespace Pathfinding {
	using Pathfinding.RVO;
	using Pathfinding.Util;

	/** Base class for AIPath and RichAI */
	public abstract class AIBase : VersionedMonoBehaviour {
		/** Gravity to use.
		 * If set to (NaN,NaN,NaN) then Physics.Gravity (configured in the Unity project settings) will be used.
		 * If set to (0,0,0) then no gravity will be used and no raycast to check for ground penetration will be performed.
		 */
		public Vector3 gravity = new Vector3(float.NaN, float.NaN, float.NaN);

		/** Layer mask to use for ground placement.
		 * Make sure this does not include the layer of any colliders attached to this gameobject.
		 *
		 * \see #gravity
		 * \see https://docs.unity3d.com/Manual/Layers.html
		 */
		public LayerMask groundMask = -1;

		/** Offset along the Y coordinate for the ground raycast start position.
		 * Normally the pivot of the character is at the character's feet, but you usually want to fire the raycast
		 * from the character's center, so this value should be half of the character's height.
		 *
		 * A green gizmo line will be drawn upwards from the pivot point of the character to indicate where the raycast will start.
		 *
		 * \see #gravity
		 */
		public float centerOffset = 1;

		/** If true, the forward axis of the character will be along the Y axis instead of the Z axis.
		 *
		 * For 3D games you most likely want to leave this the default value which is false.
		 * For 2D games you most likely want to change this to true as in 2D games you usually
		 * want the Y axis to be the forwards direction of the character.
		 *
		 * \shadowimage{aibase_forward_axis.png}
		 */
		public bool rotationIn2D = false;

		/** Current desired velocity of the agent.
		 * Lies in the movement plane.
		 */
		protected Vector2 velocity2D;

		/** Velocity due to gravity.
		 * Perpendicular to the movement plane.
		 */
		protected float verticalVelocity;

		/** Cached Seeker component */
		protected Seeker seeker;

		/** Cached Transform component */
		protected Transform tr;

		/** Cached Rigidbody component */
		protected Rigidbody rigid;

		/** Cached Rigidbody component */
		protected Rigidbody2D rigid2D;

		/** Cached CharacterController component */
		protected CharacterController controller;

		/** Cached RVOController component */
		protected RVOController rvoController;

		protected IMovementPlane movementPlane = GraphTransform.identityTransform;

		/** Indicates if gravity is used during this frame */
		protected bool usingGravity { get; private set; }

		/** Initializes reference variables.
		 * If you override this function you should in most cases call base.Awake () at the start of it.
		 */
		protected override void Awake () {
			base.Awake();
			seeker = GetComponent<Seeker>();
			controller = GetComponent<CharacterController>();
			rigid = GetComponent<Rigidbody>();
			rigid2D = GetComponent<Rigidbody2D>();
			rvoController = GetComponent<RVOController>();
			tr = transform;
		}

		/** Called every frame.
		 * If no rigidbodies are used then all movement happens here
		 */
		protected virtual void Update () {
			if (rigid == null && rigid2D == null) {
				usingGravity = !(gravity == Vector3.zero);
				MovementUpdate(Time.deltaTime);
			}
		}

		/** Called every physics update.
		 * If rigidbodies are used then all movement happens here
		 */
		protected virtual void FixedUpdate () {
			if (!(rigid == null && rigid2D == null)) {
				usingGravity = !(gravity == Vector3.zero) && (rigid == null || rigid.isKinematic) && (rigid2D == null || rigid2D.isKinematic);
				MovementUpdate(Time.fixedDeltaTime);
			}
		}

		/** Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not */
		protected abstract void MovementUpdate (float deltaTime);

		protected void ApplyGravity (float deltaTime) {
			// Apply gravity
			if (usingGravity) {
				float verticalGravity;
				velocity2D += movementPlane.ToPlane(deltaTime * (float.IsNaN(gravity.x) ? Physics.gravity : gravity), out verticalGravity);
				verticalVelocity += verticalGravity;
			} else {
				verticalVelocity = 0;
			}
		}

		protected Vector2 CalculateDeltaToMoveThisFrame (Vector2 position, float distanceToEndOfPath, float deltaTime) {
			if (rvoController != null && rvoController.enabled) {
				// Use RVOController to get a processed delta position
				// such that collisions will be avoided if possible
				return movementPlane.ToPlane(rvoController.CalculateMovementDelta(movementPlane.ToWorld(position, 0), deltaTime));
			}
			// Direction and distance to move during this frame
			return Vector2.ClampMagnitude(velocity2D * deltaTime, distanceToEndOfPath);
		}

		/** Rotates in the specified direction.
		 * Rotates around the Y-axis.
		 * \see turningSpeed
		 */
		protected virtual void RotateTowards (Vector2 direction, float maxDegrees) {
			if (direction != Vector2.zero) {
				Quaternion targetRotation = Quaternion.LookRotation(movementPlane.ToWorld(direction, 0), movementPlane.ToWorld(Vector2.zero, 1));
				if (rotationIn2D) targetRotation *= Quaternion.Euler(90, 0, 0);
				tr.rotation = Quaternion.RotateTowards(tr.rotation, targetRotation, maxDegrees);
			}
		}

		protected void Move (Vector3 position3D, Vector3 deltaPosition) {
			bool positionDirty = false;

			if (controller != null && controller.enabled) {
				// Use CharacterController
				tr.position = position3D;
				controller.Move(deltaPosition);
				// Grab the position after the movement to be able to take physics into account
				// TODO: Add this into the clampedPosition calculation below to make RVO better respond to physics
				position3D = tr.position;
				if (controller.isGrounded) verticalVelocity = 0;
			} else {
				// Use Transform, Rigidbody or Rigidbody2D
				float lastElevation;
				movementPlane.ToPlane(position3D, out lastElevation);
				position3D += deltaPosition;

				// Position the character on the ground
				if (usingGravity) position3D = RaycastPosition(position3D, lastElevation);
				positionDirty = true;
			}

			// Clamp the position to the navmesh after movement is done
			var clampedPosition = ClampToNavmesh(position3D);

			// We cannot simply check for equality because some precision may be lost
			// if any coordinate transformations are used.
			if ((clampedPosition - position3D).sqrMagnitude > 0.001f*0.001f) {
				// The agent was outside the navmesh. Remove that component of the velocity
				// so that the velocity only goes along the direction of the wall, not into it
				var difference = movementPlane.ToPlane(clampedPosition - position3D);
				velocity2D -= difference * Vector2.Dot(difference, velocity2D) / difference.sqrMagnitude;

				// Make sure the RVO system knows that there was a collision here
				// Otherwise other agents may think this agent continued
				// to move forwards and avoidance quality may suffer
				if (rvoController != null && rvoController.enabled) {
					rvoController.SetCollisionNormal(difference);
				}
				position3D = clampedPosition;
				positionDirty = true;
			}

			// Assign the final position to the character if we haven't already set it
			if (positionDirty) {
				// Note that rigid.MovePosition may or may not move the character immediately.
				// Check the Unity documentation for the special cases.
				if (rigid != null) rigid.MovePosition(position3D);
				else if (rigid2D != null) rigid2D.MovePosition(position3D);
				else tr.position = position3D;
			}
		}

		/** Constrains the character's position to lie on the navmesh.
		 * Not all movement scripts have support for this.
		 */
		protected virtual Vector3 ClampToNavmesh (Vector3 position) {
			return position;
		}

		/** Find the world position of the ground below the character */
		protected Vector3 RaycastPosition (Vector3 position, float lastElevation) {
			RaycastHit hit;
			float elevation;

			movementPlane.ToPlane(position, out elevation);
			float rayLength = centerOffset + Mathf.Max(0, lastElevation-elevation);
			Vector3 rayOffset = movementPlane.ToWorld(Vector2.zero, rayLength);

			if (Physics.Raycast(position + rayOffset, -rayOffset, out hit, rayLength, groundMask, QueryTriggerInteraction.Ignore)) {
				// Grounded
				verticalVelocity = 0;
				return hit.point;
			}
			return position;
		}

		protected static readonly Color GizmoColorRaycast = new Color(118.0f/255, 206.0f/255, 112.0f/255);

		protected virtual void OnDrawGizmos () {
			if (!Application.isPlaying && controller == null) {
				controller = GetComponent<CharacterController>();
				rigid = GetComponent<Rigidbody>();
				rigid2D = GetComponent<Rigidbody2D>();
			}

			var usingGravity = (rigid == null || rigid.isKinematic) && (rigid2D == null || rigid2D.isKinematic) && !(gravity == Vector3.zero);
			if (usingGravity && (controller == null || !controller.enabled)) {
				Gizmos.color = GizmoColorRaycast;
				Gizmos.DrawLine(transform.position, transform.position + transform.up*centerOffset);
				Gizmos.DrawLine(transform.position - transform.right*0.1f, transform.position + transform.right*0.1f);
				Gizmos.DrawLine(transform.position - transform.forward*0.1f, transform.position + transform.forward*0.1f);
			}
		}
	}
}
