using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	[System.Serializable]
	/** Adjusts start and end points of a path.
	 * \ingroup modifiers
	 */
	public class StartEndModifier : PathModifier {
		public override int Order { get { return 0; } }

		/** Add points to the path instead of replacing them */
		public bool addPoints;
		public Exactness exactStartPoint = Exactness.ClosestOnNode;
		public Exactness exactEndPoint = Exactness.ClosestOnNode;

		/** Will be called when a path is processed.
		 * The value which is returned will be used as the start point of the path
		 * and potentially clamped depending on the value of the #exactStartPoint field.
		 * Only used for the Original, Interpolate and NodeConnection modes.
		 */
		public System.Func<Vector3> adjustStartPoint;

		/** Sets where the start and end points of a path should be placed */
		public enum Exactness {
			/** The point is snapped to the first/last node in the path*/
			SnapToNode,
			/** The point is set to the exact point which was passed when calling the pathfinding */
			Original,
			/** The point is set to the closest point on the line between either the two first points or the two last points.
			 * Usually you will want to use the NodeConnection mode instead since that is usually the behaviour that you really want.
			 */
			Interpolate,
			/** The point is set to the closest point on the node. Note that for some node types (point nodes) the "closest point" is the node's position which makes this identical to Exactness.SnapToNode */
			ClosestOnNode,
			/** The point is set to the closest point on one of the connections from the start/end node */
			NodeConnection,
		}

		public bool useRaycasting;
		public LayerMask mask = -1;

		public bool useGraphRaycasting;

		List<GraphNode> connectionBuffer;
		System.Action<GraphNode> connectionBufferAddDelegate;

		public override void Apply (Path _p) {
			var p = _p as ABPath;

			// This modifier only supports ABPaths (doesn't make much sense for other paths anyway)
			if (p == null || p.vectorPath.Count == 0) return;

			if (p.vectorPath.Count == 1 && !addPoints) {
				// Duplicate first point
				p.vectorPath.Add(p.vectorPath[0]);
			}

			// Add instead of replacing points
			bool forceAddStartPoint, forceAddEndPoint;

			Vector3 pStart = Snap(p, exactStartPoint, true, out forceAddStartPoint);
			Vector3 pEnd = Snap(p, exactEndPoint, false, out forceAddEndPoint);

			// Add or replace the start point
			// Disable adding of points if the mode is SnapToNode since then
			// the first item in vectorPath will very likely be the same as the
			// position of the first node
			if ((forceAddStartPoint || addPoints) && exactStartPoint != Exactness.SnapToNode) {
				p.vectorPath.Insert(0, pStart);
			} else {
				p.vectorPath[0] = pStart;
			}

			if ((forceAddEndPoint || addPoints) && exactEndPoint != Exactness.SnapToNode) {
				p.vectorPath.Add(pEnd);
			} else {
				p.vectorPath[p.vectorPath.Count-1] = pEnd;
			}
		}

		Vector3 Snap (ABPath path, Exactness mode, bool start, out bool forceAddPoint) {
			var index = start ? 0 : path.path.Count - 1;
			var node = path.path[index];
			var nodePos = (Vector3)node.position;

			forceAddPoint = false;

			switch (mode) {
			case Exactness.ClosestOnNode:
				return GetClampedPoint(nodePos, start ? path.startPoint : path.endPoint, node);
			case Exactness.SnapToNode:
				return nodePos;
			case Exactness.Original:
			case Exactness.Interpolate:
			case Exactness.NodeConnection:
				Vector3 relevantPoint;
				if (start) {
					relevantPoint = adjustStartPoint != null ? adjustStartPoint() : path.originalStartPoint;
				} else {
					relevantPoint = path.originalEndPoint;
				}

				switch (mode) {
				case Exactness.Original:
					return GetClampedPoint(nodePos, relevantPoint, node);
				case Exactness.Interpolate:
					var clamped = GetClampedPoint(nodePos, relevantPoint, node);
					// Adjacent node to either the start node or the end node in the path
					var adjacentNode = path.path[Mathf.Clamp(index + (start ? 1 : -1), 0, path.path.Count-1)];
					return VectorMath.ClosestPointOnSegment(nodePos, (Vector3)adjacentNode.position, clamped);
				case Exactness.NodeConnection:
					// This code uses some tricks to avoid allocations
					// even though it uses delegates heavily
					// The connectionBufferAddDelegate delegate simply adds whatever node
					// it is called with to the connectionBuffer
					connectionBuffer = connectionBuffer ?? new List<GraphNode>();
					connectionBufferAddDelegate = connectionBufferAddDelegate ?? (System.Action<GraphNode>)connectionBuffer.Add;

					// Adjacent node to either the start node or the end node in the path
					adjacentNode = path.path[Mathf.Clamp(index + (start ? 1 : -1), 0, path.path.Count-1)];

					// Add all neighbours of #node to the connectionBuffer
					node.GetConnections(connectionBufferAddDelegate);
					var bestPos = nodePos;
					var bestDist = float.PositiveInfinity;

					// Loop through all neighbours
					// Do it in reverse order because the length of the connectionBuffer
					// will change during iteration
					for (int i = connectionBuffer.Count - 1; i >= 0; i--) {
						var neighbour = connectionBuffer[i];

						// Find the closest point on the connection between the nodes
						// and check if the distance to that point is lower than the previous best
						var closest = VectorMath.ClosestPointOnSegment(nodePos, (Vector3)neighbour.position, relevantPoint);

						var dist = (closest - relevantPoint).sqrMagnitude;
						if (dist < bestDist) {
							bestPos = closest;
							bestDist = dist;

							// If this node is not the adjacent node
							// then the path should go through the start node as well
							forceAddPoint = neighbour != adjacentNode;
						}
					}

					connectionBuffer.Clear();
					return bestPos;
				default:
					throw new System.ArgumentException("Cannot reach this point, but the compiler is not smart enough to realize that.");
				}
			default:
				throw new System.ArgumentException("Invalid mode");
			}
		}

		public Vector3 GetClampedPoint (Vector3 from, Vector3 to, GraphNode hint) {
			Vector3 point = to;
			RaycastHit hit;

			if (useRaycasting && Physics.Linecast(from, to, out hit, mask)) {
				point = hit.point;
			}

			if (useGraphRaycasting && hint != null) {
				var rayGraph = AstarData.GetGraph(hint) as IRaycastableGraph;

				if (rayGraph != null) {
					GraphHitInfo graphHit;
					if (rayGraph.Linecast(from, point, hint, out graphHit)) {
						point = graphHit.point;
					}
				}
			}

			return point;
		}
	}
}
