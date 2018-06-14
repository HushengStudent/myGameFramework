using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	/** Simplifies a path using raycasting.
	 * \ingroup modifiers
	 * This modifier will try to remove as many nodes as possible from the path using raycasting (linecasting) to validate the node removal.
	 * You can use either graph raycasting or Physics.Raycast.
	 * When using graph raycasting, the graph will be traversed and checked for obstacles. When physics raycasting is used, the Unity physics system
	 * will be asked if there are any colliders which intersect the line that is currently being checked.
	 */
	[AddComponentMenu("Pathfinding/Modifiers/Raycast Simplifier")]
	[RequireComponent(typeof(Seeker))]
	[System.Serializable]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_raycast_modifier.php")]
	public class RaycastModifier : MonoModifier {
	#if UNITY_EDITOR
		[UnityEditor.MenuItem("CONTEXT/Seeker/Add Raycast Simplifier Modifier")]
		public static void AddComp (UnityEditor.MenuCommand command) {
			(command.context as Component).gameObject.AddComponent(typeof(RaycastModifier));
		}
	#endif

		public override int Order { get { return 40; } }

		/** Use Physics.Raycast to simplify the path */
		public bool useRaycasting = true;

		/** Layer mask used for physics raycasting */
		public LayerMask mask = -1;

		/** Checks around the line between two points, not just the exact line.
		 * Make sure the ground is either too far below or is not inside the mask since otherwise the raycast might always hit the ground
		 */
		[Tooltip("Checks around the line between two points, not just the exact line.\nMake sure the ground is either too far below or is not inside the mask since otherwise the raycast might always hit the ground.")]
		public bool thickRaycast;

		/** Distance from the ray which will be checked for colliders */
		[Tooltip("Distance from the ray which will be checked for colliders")]
		public float thickRaycastRadius;

		/** Offset from the original positions to perform the raycast.
		 * Can be useful to avoid the raycast intersecting the ground or similar things you do not want to it intersect
		 */
		[Tooltip("Offset from the original positions to perform the raycast.\nCan be useful to avoid the raycast intersecting the ground or similar things you do not want to it intersect")]
		public Vector3 raycastOffset = Vector3.zero;

		/** Subdivides the path every iteration to be able to find shorter paths */
		[Tooltip("Subdivides the path every iteration to be able to find shorter paths")]
		public bool subdivideEveryIter;

		/** How many iterations to try to simplify the path.
		 * If the path is changed in one iteration, the next iteration may find more simplification oppourtunities */
		[Tooltip("How many iterations to try to simplify the path. If the path is changed in one iteration, the next iteration may find more simplification oppourtunities")]
		public int iterations = 2;

		/** Use raycasting on the graphs. Only currently works with GridGraph and NavmeshGraph and RecastGraph. \astarpro */
		[Tooltip("Use raycasting on the graphs. Only currently works with GridGraph and NavmeshGraph and RecastGraph. This is a pro version feature.")]
		public bool useGraphRaycasting;

		public override void Apply (Path p) {
			if (iterations <= 0) return;

			if (!useRaycasting && !useGraphRaycasting) {
				Debug.LogWarning("RaycastModifier is configured to not use either raycasting or graph raycasting. This would simplify the path to a straight line. The modifier will not be applied.");
				return;
			}

			var points = p.vectorPath;

			for (int it = 0; it < iterations; it++) {
				if (subdivideEveryIter && it != 0) {
					Subdivide(points);
				}

				int i = 0;
				while (i < points.Count-2) {
					Vector3 start = points[i];
					Vector3 end = points[i+2];

					if (ValidateLine(null, null, start, end)) {
						points.RemoveAt(i+1);
					} else {
						i++;
					}
				}
			}
		}

		/** Divides each segment in the list into 3 segments */
		static void Subdivide (List<Vector3> points) {
			if (points.Capacity < points.Count * 3) {
				points.Capacity = points.Count * 3;
			}

			int preLength = points.Count;

			for (int j = 0; j < preLength-1; j++) {
				points.Add(Vector3.zero);
				points.Add(Vector3.zero);
			}

			for (int j = preLength-1; j > 0; j--) {
				Vector3 p1 = points[j];
				Vector3 p2 = points[j + 1];

				points[j * 3] = points[j];

				if (j != preLength - 1) {
					points[j * 3 + 1] = Vector3.Lerp(p1, p2, 0.33F);
					points[j * 3 + 2] = Vector3.Lerp(p1, p2, 0.66F);
				}
			}
		}

		/** Check if a straight path between v1 and v2 is valid */
		public bool ValidateLine (GraphNode n1, GraphNode n2, Vector3 v1, Vector3 v2) {
			if (useRaycasting) {
				// Use raycasting to check if a straight path between v1 and v2 is valid
				if (thickRaycast && thickRaycastRadius > 0) {
					if (Physics.SphereCast(new Ray(v1+raycastOffset, v2-v1), thickRaycastRadius, (v2-v1).magnitude, mask)) {
						return false;
					}
				} else {
					if (Physics.Linecast(v1+raycastOffset, v2+raycastOffset, mask)) {
						return false;
					}
				}
			}

			if (useGraphRaycasting && n1 == null) {
				n1 = AstarPath.active.GetNearest(v1).node;
				n2 = AstarPath.active.GetNearest(v2).node;
			}

			if (useGraphRaycasting && n1 != null && n2 != null) {
				// Use graph raycasting to check if a straight path between v1 and v2 is valid
				NavGraph graph = AstarData.GetGraph(n1);
				NavGraph graph2 = AstarData.GetGraph(n2);

				if (graph != graph2) {
					return false;
				}

				if (graph != null) {
					var rayGraph = graph as IRaycastableGraph;

					if (rayGraph != null) {
						return !rayGraph.Linecast(v1, v2, n1);
					}
				}
			}
			return true;
		}
	}
}
