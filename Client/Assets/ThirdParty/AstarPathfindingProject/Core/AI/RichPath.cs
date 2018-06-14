using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Util;

namespace Pathfinding {
	public class RichPath {
		int currentPart;
		readonly List<RichPathPart> parts = new List<RichPathPart>();

		public Seeker seeker;

		/** Transforms points from path space to world space.
		 * If null the identity transform will be used.
		 *
		 * This is used when the world position of the agent does not match the
		 * corresponding position on the graph. This is the case in the example
		 * scene called 'Moving'.
		 *
		 * \see #Pathfinding.Examples.LocalSpaceRichAI
		 */
		public ITransform transform;

		public void Clear () {
			parts.Clear();
			currentPart = 0;
		}

		/** Use this for initialization.
		 *
		 * \param s Optionally provide in order to take tag penalties into account. May be null if you do not use a Seeker\
		 * \param p Path to follow
		 * \param mergePartEndpoints If true, then adjacent parts that the path is split up in will
		 * try to use the same start/end points. For example when using a link on a navmesh graph
		 * Instead of first following the path to the center of the node where the link is and then
		 * follow the link, the path will be adjusted to go to the exact point where the link starts
		 * which usually makes more sense.
		 * \param simplificationMode The path can optionally be simplified. This can be a bit expensive for long paths.
		 */
		public void Initialize (Seeker s, Path p, bool mergePartEndpoints, bool simplificationMode) {
			if (p.error) throw new System.ArgumentException("Path has an error");

			List<GraphNode> nodes = p.path;
			if (nodes.Count == 0) throw new System.ArgumentException("Path traverses no nodes");

			seeker = s;
			// Release objects back to object pool
			// Yeah, I know, it's casting... but this won't be called much
			for (int i = 0; i < parts.Count; i++) {
				var funnelPart = parts[i] as RichFunnel;
				var specialPart = parts[i] as RichSpecial;
				if (funnelPart != null) ObjectPool<RichFunnel>.Release(ref funnelPart);
				else if (specialPart != null) ObjectPool<RichSpecial>.Release(ref specialPart);
			}

			parts.Clear();
			currentPart = 0;

			// Initialize new

			//Break path into parts
			for (int i = 0; i < nodes.Count; i++) {
				if (nodes[i] is TriangleMeshNode) {
					var graph = AstarData.GetGraph(nodes[i]);
					RichFunnel f = ObjectPool<RichFunnel>.Claim().Initialize(this, graph);

					f.funnelSimplification = simplificationMode;

					int sIndex = i;
					uint currentGraphIndex = nodes[sIndex].GraphIndex;


					for (; i < nodes.Count; i++) {
						if (nodes[i].GraphIndex != currentGraphIndex && !(nodes[i] is NodeLink3Node)) {
							break;
						}
					}
					i--;

					if (sIndex == 0) {
						f.exactStart = p.vectorPath[0];
					} else {
						f.exactStart = (Vector3)nodes[mergePartEndpoints ? sIndex-1 : sIndex].position;
					}

					if (i == nodes.Count-1) {
						f.exactEnd = p.vectorPath[p.vectorPath.Count-1];
					} else {
						f.exactEnd = (Vector3)nodes[mergePartEndpoints ? i+1 : i].position;
					}

					f.BuildFunnelCorridor(nodes, sIndex, i);

					parts.Add(f);
				} else if (NodeLink2.GetNodeLink(nodes[i]) != null) {
					NodeLink2 nl = NodeLink2.GetNodeLink(nodes[i]);

					int sIndex = i;
					uint currentGraphIndex = nodes[sIndex].GraphIndex;

					for (i++; i < nodes.Count; i++) {
						if (nodes[i].GraphIndex != currentGraphIndex) {
							break;
						}
					}
					i--;

					if (i - sIndex > 1) {
						throw new System.Exception("NodeLink2 path length greater than two (2) nodes. " + (i - sIndex));
					} else if (i - sIndex == 0) {
						//Just continue, it might be the case that a NodeLink was the closest node
						continue;
					}

					RichSpecial rps = ObjectPool<RichSpecial>.Claim().Initialize(nl, nodes[sIndex]);
					parts.Add(rps);
				}
			}
		}

		/** True if we have completed (called NextPart for) the last part in the path */
		public bool CompletedAllParts {
			get {
				return currentPart >= parts.Count;
			}
		}

		/** True if we are traversing the last part of the path */
		public bool IsLastPart {
			get {
				return currentPart >= parts.Count - 1;
			}
		}

		public void NextPart () {
			currentPart = Mathf.Min(currentPart + 1, parts.Count);
		}

		public RichPathPart GetCurrentPart () {
			if (parts.Count == 0) return null;
			return currentPart < parts.Count ? parts[currentPart] : parts[parts.Count - 1];
		}
	}

	public abstract class RichPathPart : Pathfinding.Util.IAstarPooledObject {
		public abstract void OnEnterPool ();
	}

	public class RichFunnel : RichPathPart {
		readonly List<Vector3> left;
		readonly List<Vector3> right;
		List<TriangleMeshNode> nodes;
		public Vector3 exactStart;
		public Vector3 exactEnd;
		NavGraph graph;
		int currentNode;
		Vector3 currentPosition;
		int checkForDestroyedNodesCounter;
		RichPath path;
		int[] triBuffer = new int[3];

		/** Post process the funnel corridor or not */
		public bool funnelSimplification = true;

		public RichFunnel () {
			left = Pathfinding.Util.ListPool<Vector3>.Claim();
			right = Pathfinding.Util.ListPool<Vector3>.Claim();
			nodes = new List<TriangleMeshNode>();
			this.graph = null;
		}

		/** Works like a constructor, but can be used even for pooled objects. Returns \a this for easy chaining */
		public RichFunnel Initialize (RichPath path, NavGraph graph) {
			if (graph == null) throw new System.ArgumentNullException("graph");
			if (this.graph != null) throw new System.InvalidOperationException("Trying to initialize an already initialized object. " + graph);

			this.graph = graph;
			this.path = path;
			return this;
		}

		public override void OnEnterPool () {
			left.Clear();
			right.Clear();
			nodes.Clear();
			graph = null;
			currentNode = 0;
			checkForDestroyedNodesCounter = 0;
		}

		public TriangleMeshNode CurrentNode {
			get {
				var node = nodes[currentNode];
				if (!node.Destroyed) {
					return node;
				}
				return null;
			}
		}

		/** Build a funnel corridor from a node list slice.
		 * The nodes are assumed to be of type TriangleMeshNode.
		 *
		 * \param nodes Nodes to build the funnel corridor from
		 * \param start Start index in the nodes list
		 * \param end End index in the nodes list, this index is inclusive
		 */
		public void BuildFunnelCorridor (List<GraphNode> nodes, int start, int end) {
			//Make sure start and end points are on the correct nodes
			exactStart = (nodes[start] as MeshNode).ClosestPointOnNode(exactStart);
			exactEnd = (nodes[end] as MeshNode).ClosestPointOnNode(exactEnd);

			left.Clear();
			right.Clear();
			left.Add(exactStart);
			right.Add(exactStart);


			this.nodes.Clear();

			var raycastableGraph = graph as IRaycastableGraph;
			if (raycastableGraph != null && funnelSimplification) {
				List<GraphNode> tmp = Pathfinding.Util.ListPool<GraphNode>.Claim(end-start);

				SimplifyPath(raycastableGraph, nodes, start, end, tmp, exactStart, exactEnd);

				if (this.nodes.Capacity < tmp.Count) this.nodes.Capacity = tmp.Count;

				for (int i = 0; i < tmp.Count; i++) {
					//Guaranteed to be TriangleMeshNodes since they are all in the same graph
					var node = tmp[i] as TriangleMeshNode;
					if (node != null) this.nodes.Add(node);
				}

				Pathfinding.Util.ListPool<GraphNode>.Release(tmp);
			} else {
				if (this.nodes.Capacity < end-start) this.nodes.Capacity = (end-start);
				for (int i = start; i <= end; i++) {
					//Guaranteed to be TriangleMeshNodes since they are all in the same graph
					var node = nodes[i] as TriangleMeshNode;
					if (node != null) this.nodes.Add(node);
				}
			}

			for (int i = 0; i < this.nodes.Count-1; i++) {
				/** \todo should use return value in future versions */
				this.nodes[i].GetPortal(this.nodes[i+1], left, right, false);
			}

			left.Add(exactEnd);
			right.Add(exactEnd);
		}

		/** Simplifies a funnel path using linecasting.
		 * Running time is roughly O(n^2 log n) in the worst case (where n = end-start)
		 * Actually it depends on how the graph looks, so in theory the actual upper limit on the worst case running time is O(n*m log n) (where n = end-start and m = nodes in the graph)
		 * but O(n^2 log n) is a much more realistic worst case limit.
		 *
		 * Requires #graph to implement IRaycastableGraph
		 */
		void SimplifyPath (IRaycastableGraph graph, List<GraphNode> nodes, int start, int end, List<GraphNode> result, Vector3 startPoint, Vector3 endPoint) {
			if (graph == null) throw new System.ArgumentNullException("graph");

			if (start > end) {
				throw new System.ArgumentException("start >= end");
			}

			int ostart = start;

			int count = 0;
			while (true) {
				if (count++ > 1000) {
					Debug.LogError("Was the path really long or have we got cought in an infinite loop?");
					break;
				}

				if (start == end) {
					result.Add(nodes[end]);
					return;
				}

				int resCount = result.Count;

				int mx = end+1;
				int mn = start+1;
				bool anySucceded = false;
				while (mx > mn+1) {
					int mid = (mx+mn)/2;

					GraphHitInfo hit;
					Vector3 sp = start == ostart ? startPoint : (Vector3)nodes[start].position;
					Vector3 ep = mid == end ? endPoint : (Vector3)nodes[mid].position;

					if (graph.Linecast(sp, ep, nodes[start], out hit)) {
						mx = mid;
					} else {
						anySucceded = true;
						mn = mid;
					}
				}

				if (!anySucceded) {
					result.Add(nodes[start]);

					//It is guaranteed that mn = start+1
					start = mn;
				} else {
					//Need to redo the linecast to get the trace
					GraphHitInfo hit;
					Vector3 sp = start == ostart ? startPoint : (Vector3)nodes[start].position;
					Vector3 ep = mn == end ? endPoint : (Vector3)nodes[mn].position;
					graph.Linecast(sp, ep, nodes[start], out hit, result);

					long penaltySum = 0;
					long penaltySum2 = 0;
					for (int i = start; i <= mn; i++) {
						penaltySum += nodes[i].Penalty + (path.seeker != null ? path.seeker.tagPenalties[nodes[i].Tag] : 0);
					}

					for (int i = resCount; i < result.Count; i++) {
						penaltySum2 += result[i].Penalty + (path.seeker != null ? path.seeker.tagPenalties[result[i].Tag] : 0);
					}

					// Allow 40% more penalty on average per node
					if ((penaltySum*1.4*(mn-start+1)) < (penaltySum2*(result.Count-resCount)) || result[result.Count-1] != nodes[mn]) {
						//Debug.Log ((penaltySum*1.4*(mn-start+1)) + " < "+ (penaltySum2*(result.Count-resCount)));
						//Debug.DrawLine ((Vector3)nodes[start].Position, (Vector3)nodes[mn].Position, Color.red);
						//Linecast hit the wrong node
						result.RemoveRange(resCount, result.Count-resCount);

						result.Add(nodes[start]);
						//Debug.Break();
						start = start+1;
					} else {
						//Debug.Log ("!! " + (penaltySum*1.4*(mn-start+1)) + " < "+ (penaltySum2*(result.Count-resCount)));
						//Debug.DrawLine ((Vector3)nodes[start].Position, (Vector3)nodes[mn].Position, Color.green);
						//Debug.Break ();
						//Remove nodes[end]
						result.RemoveAt(result.Count-1);
						start = mn;
					}
				}
			}
		}

		/** Split funnel at node index \a splitIndex and throw the nodes up to that point away and replace with \a prefix.
		 * Used when the AI has happened to get sidetracked and entered a node outside the funnel.
		 */
		void UpdateFunnelCorridor (int splitIndex, TriangleMeshNode prefix) {
			if (splitIndex > 0) {
				nodes.RemoveRange(0, splitIndex-1);
				//This is a node which should be removed, we replace it with the prefix
				nodes[0] = prefix;
			} else {
				nodes.Insert(0, prefix);
			}

			left.Clear();
			right.Clear();
			left.Add(exactStart);
			right.Add(exactStart);

			for (int i = 0; i < nodes.Count-1; i++) {
				//NOTE should use return value in future versions
				nodes[i].GetPortal(nodes[i+1], left, right, false);
			}

			left.Add(exactEnd);
			right.Add(exactEnd);
		}

		/** True if any node in the path is destroyed */
		bool CheckForDestroyedNodes () {
			// Loop through all nodes and check if they are destroyed
			// If so, we really need a recalculation of our path quickly
			// since there might be an obstacle blocking our path after
			// a graph update or something similar
			for (int i = 0, t = nodes.Count; i < t; i++) {
				if (nodes[i].Destroyed) {
					return true;
				}
			}
			return false;
		}

		/** Approximate distance (as the crow flies) to the endpoint of this path part.
		 * \see #exactEnd
		 */
		public float DistanceToEndOfPath {
			get {
				var currentNode = CurrentNode;
				Vector3 closestOnNode = currentNode != null ? currentNode.ClosestPointOnNode(currentPosition) : currentPosition;
				return (exactEnd - closestOnNode).magnitude;
			}
		}

		/** Clamps the position to the navmesh and repairs the path if the agent has moved slightly outside it.
		 * You should not call this method with anything other than the agent's position.
		 */
		public Vector3 ClampToNavmesh (Vector3 position) {
			if (path.transform != null) position = path.transform.InverseTransform(position);
			ClampToNavmeshInternal(ref position);
			if (path.transform != null) position = path.transform.Transform(position);
			return position;
		}

		/** Find the next points to move towards and clamp the position to the navmesh.
		 *
		 * \param position The position of the agent.
		 * \param buffer Will be filled with up to \a numCorners points which are the next points in the path towards the target.
		 * \param numCorners See buffer.
		 * \param lastCorner True if the buffer contains the end point of the path.
		 * \param requiresRepath True if nodes along the path have been destroyed and a path recalculation is necessary.
		 *
		 * \returns The position of the agent clamped to make sure it is inside the navmesh.
		 */
		public Vector3 Update (Vector3 position, List<Vector3> buffer, int numCorners, out bool lastCorner, out bool requiresRepath) {
			if (path.transform != null) position = path.transform.InverseTransform(position);

			lastCorner = false;
			requiresRepath = false;

			// Only check for destroyed nodes every 10 frames
			if (checkForDestroyedNodesCounter >= 10) {
				checkForDestroyedNodesCounter = 0;
				requiresRepath |= CheckForDestroyedNodes();
			} else {
				checkForDestroyedNodesCounter++;
			}

			bool nodesDestroyed = ClampToNavmeshInternal(ref position);

			currentPosition = position;

			if (nodesDestroyed) {
				// Some nodes on the path have been destroyed
				// we need to recalculate the path immediately
				requiresRepath = true;
				lastCorner = false;
				buffer.Add(position);
			} else if (!FindNextCorners(position, currentNode, buffer, numCorners, out lastCorner)) {
				Debug.LogError("Failed to find next corners in the path");
				buffer.Add(position);
			}

			if (path.transform != null) {
				for (int i = 0; i < buffer.Count; i++) {
					buffer[i] = path.transform.Transform(buffer[i]);
				}

				position = path.transform.Transform(position);
			}

			return position;
		}

		/** Searches for the node the agent is inside.
		 * This will also clamp the position to the navmesh
		 * and repair the funnel cooridor if the agent moves slightly outside it.
		 *
		 * Assumes the check for if the position is inside nodes[currentNode] has already been done.
		 *
		 * \returns True if nodes along the path have been destroyed so that a path recalculation is required
		 */
		bool ClampToNavmeshInternal (ref Vector3 position) {
			if (nodes[currentNode].Destroyed) {
				return true;
			}

			var i3Pos = (Int3)position;

			// Check if we are in the same node as we were in during the last frame and otherwise do a more extensive search
			if (nodes[currentNode].ContainsPoint(i3Pos)) {
				return false;
			}

			// This part of the code is relatively seldom called
			// Most of the time we are still on the same node as during the previous frame

			// Otherwise check the 2 nodes ahead and 2 nodes back
			// If they contain the node in XZ space, then we probably moved into those nodes

			// 2 nodes ahead
			for (int i = currentNode+1, t = System.Math.Min(currentNode+3, nodes.Count); i < t; i++) {
				// If the node is destroyed, make sure we recalculate a new path quickly
				if (nodes[i].Destroyed) {
					return true;
				}

				// We found a node which contains our current position in XZ space
				if (nodes[i].ContainsPoint(i3Pos)) {
					currentNode = i;
					return false;
				}
			}

			// 2 nodes behind
			for (int i = currentNode-1, t = System.Math.Max(currentNode-3, 0); i > t; i--) {
				if (nodes[i].Destroyed) {
					return true;
				}

				if (nodes[i].ContainsPoint(i3Pos)) {
					currentNode = i;
					return false;
				}
			}

			// We need to separate this into its own method because it will create a delegate and
			// if that delegate is created in the same method then the JIT may decide it needs to
			// allocate some things even though we never actually get to the point where the code
			// creates the delegate, and that would cause unnecessary allocations every frame.
			return ClampToNavmeshInternalFull(ref position);
		}

		bool ClampToNavmeshInternalFull (ref Vector3 position) {
			int closestNodeInPath = 0;
			float closestDist = float.PositiveInfinity;
			bool closestIsInPath = false;
			TriangleMeshNode closestNode = null;

			// If we still couldn't find a good node
			// Check all nodes in the whole path

			// We are checking for if any node is destroyed in the loop
			// So we can reset this counter
			checkForDestroyedNodesCounter = 0;

			for (int i = 0, t = nodes.Count; i < t; i++) {
				if (nodes[i].Destroyed) {
					return true;
				}

				Vector3 close = nodes[i].ClosestPointOnNode(position);
				float d = (close-position).sqrMagnitude;
				if (d < closestDist) {
					closestDist = d;
					closestNodeInPath = i;
					closestNode = nodes[i];
					closestIsInPath = true;
				}
			}

			// Loop through all neighbours of all nodes in the path
			// and find the closet point on them
			// We cannot just look on the ones in the path since it is impossible
			// to know if we are outside the navmesh completely or if we have just
			// stepped in to an adjacent node

			// Need to make a copy because ref parameters cannot be used inside delegates
			var posCopy = position;
			int containingIndex = nodes.Count-1;
			int closestIsNeighbourOf = 0;

			System.Action<GraphNode> del = node => {
				// Check so that this neighbour we are processing is neither the node after the current node or the node before the current node in the path
				// This is done for optimization, we have already checked those nodes earlier
				if (!(containingIndex > 0 && node == nodes[containingIndex-1]) && !(containingIndex < nodes.Count-1 && node == nodes[containingIndex+1])) {
					// Check if the neighbour was a mesh node
					var triNode = node as TriangleMeshNode;
					if (triNode != null) {
						// Find the distance to the closest point on it from our current position
						var close = triNode.ClosestPointOnNode(posCopy);
						float dist = (close-posCopy).sqrMagnitude;

						// Is that distance better than the best distance seen so far
						if (dist < closestDist) {
							closestDist = dist;
							closestIsNeighbourOf = containingIndex;
							closestNode = triNode;
							closestIsInPath = false;
						}
					}
				}
			};

			// Loop through all the nodes in the path in reverse order
			// The callback needs to know about the index, so we store it
			// in a local variable which it can read
			for (; containingIndex >= 0; containingIndex--) {
				// Loop through all neighbours of the node
				nodes[containingIndex].GetConnections(del);
			}

			// Check if the closest node
			// was on the path already or if we need to adjust it
			if (closestIsInPath) {
				// If we have found a node
				// Snap to the closest point in XZ space (keep the Y coordinate)
				// If we would have snapped to the closest point in 3D space, the agent
				// might slow down when traversing slopes
				currentNode = closestNodeInPath;
				position = nodes[closestNodeInPath].ClosestPointOnNodeXZ(position);
			} else {
				// Snap to the closest point in XZ space on the node
				position = closestNode.ClosestPointOnNodeXZ(position);

				// We have found a node containing the position, but it is outside the funnel
				// Recalculate the funnel to include this node
				exactStart = position;
				UpdateFunnelCorridor(closestIsNeighbourOf, closestNode);

				// Restart from the first node in the updated path
				currentNode = 0;
			}

			return false;
		}

		/** Fill wallBuffer with all navmesh wall segments close to the current position.
		 * A wall segment is a node edge which is not shared by any other neighbour node, i.e an outer edge on the navmesh.
		 */
		public void FindWalls (List<Vector3> wallBuffer, float range) {
			FindWalls(currentNode, wallBuffer, currentPosition, range);
		}

		void FindWalls (int nodeIndex, List<Vector3> wallBuffer, Vector3 position, float range) {
			if (range <= 0) return;

			bool negAbort = false;
			bool posAbort = false;

			range *= range;

			position.y = 0;
			//Looping as 0,-1,1,-2,2,-3,3,-4,4 etc. Avoids code duplication by keeping it to one loop instead of two
			for (int i = 0; !negAbort || !posAbort; i = i < 0 ? -i : -i-1) {
				if (i < 0 && negAbort) continue;
				if (i > 0 && posAbort) continue;

				if (i < 0 && nodeIndex+i < 0) {
					negAbort = true;
					continue;
				}

				if (i > 0 && nodeIndex+i >= nodes.Count) {
					posAbort = true;
					continue;
				}

				TriangleMeshNode prev = nodeIndex+i-1 < 0 ? null : nodes[nodeIndex+i-1];
				TriangleMeshNode node = nodes[nodeIndex+i];
				TriangleMeshNode next = nodeIndex+i+1 >= nodes.Count ? null : nodes[nodeIndex+i+1];

				if (node.Destroyed) {
					break;
				}

				if ((node.ClosestPointOnNodeXZ(position)-position).sqrMagnitude > range) {
					if (i < 0) negAbort = true;
					else posAbort = true;
					continue;
				}

				for (int j = 0; j < 3; j++) triBuffer[j] = 0;

				for (int j = 0; j < node.connections.Length; j++) {
					var other = node.connections[j].node as TriangleMeshNode;
					if (other == null) continue;

					int va = -1;
					for (int a = 0; a < 3; a++) {
						for (int b = 0; b < 3; b++) {
							if (node.GetVertex(a) == other.GetVertex((b+1) % 3) && node.GetVertex((a+1) % 3) == other.GetVertex(b)) {
								va = a;
								a = 3;
								break;
							}
						}
					}
					if (va == -1) {
						//No direct connection
					} else {
						triBuffer[va] = other == prev || other == next ? 2 : 1;
					}
				}

				for (int j = 0; j < 3; j++) {
					//Tribuffer values
					// 0 : Navmesh border, outer edge
					// 1 : Inner edge, to node inside funnel
					// 2 : Inner edge, to node outside funnel
					if (triBuffer[j] == 0) {
						//Add edge to list of walls
						wallBuffer.Add((Vector3)node.GetVertex(j));
						wallBuffer.Add((Vector3)node.GetVertex((j+1) % 3));
					}
				}
			}

			if (path.transform != null) {
				for (int i = 0; i < wallBuffer.Count; i++) {
					wallBuffer[i] = path.transform.Transform(wallBuffer[i]);
				}
			}
		}

		bool FindNextCorners (Vector3 origin, int startIndex, List<Vector3> funnelPath, int numCorners, out bool lastCorner) {
			lastCorner = false;

			if (left == null) throw new System.Exception("left list is null");
			if (right == null) throw new System.Exception("right list is null");
			if (funnelPath == null) throw new System.ArgumentNullException("funnelPath");

			if (left.Count != right.Count) throw new System.ArgumentException("left and right lists must have equal length");

			int diagonalCount = left.Count;

			if (diagonalCount == 0) throw new System.ArgumentException("no diagonals");

			if (diagonalCount-startIndex < 3) {
				//Direct path
				funnelPath.Add(left[diagonalCount-1]);
				lastCorner = true;
				return true;
			}

			#if ASTARDEBUG
			for (int i = startIndex; i < left.Count-1; i++) {
				Debug.DrawLine(left[i], left[i+1], Color.red);
				Debug.DrawLine(right[i], right[i+1], Color.magenta);
				Debug.DrawRay(right[i], Vector3.up, Color.magenta);
			}
			for (int i = 0; i < left.Count; i++) {
				Debug.DrawLine(right[i], left[i], Color.cyan);
			}
			#endif

			//Remove identical vertices
			while (left[startIndex+1] == left[startIndex+2] && right[startIndex+1] == right[startIndex+2]) {
				//System.Console.WriteLine ("Removing identical left and right");
				//left.RemoveAt (1);
				//right.RemoveAt (1);
				startIndex++;

				if (diagonalCount-startIndex <= 3) {
					return false;
				}
			}

			Vector3 swPoint = left[startIndex+2];
			if (swPoint == left[startIndex+1]) {
				swPoint = right[startIndex+2];
			}


			//Test
			while (VectorMath.IsColinearXZ(origin, left[startIndex+1], right[startIndex+1]) || VectorMath.RightOrColinearXZ(left[startIndex+1], right[startIndex+1], swPoint) == VectorMath.RightOrColinearXZ(left[startIndex+1], right[startIndex+1], origin)) {
	#if ASTARDEBUG
				Debug.DrawLine(left[startIndex+1], right[startIndex+1], new Color(0, 0, 0, 0.5F));
				Debug.DrawLine(origin, swPoint, new Color(0, 0, 0, 0.5F));
	#endif
				//left.RemoveAt (1);
				//right.RemoveAt (1);
				startIndex++;

				if (diagonalCount-startIndex < 3) {
					//Debug.Log ("#2 " + left.Count + " - " + startIndex + " = " + (left.Count-startIndex));
					//Direct path
					funnelPath.Add(left[diagonalCount-1]);
					lastCorner = true;
					return true;
				}

				swPoint = left[startIndex+2];
				if (swPoint == left[startIndex+1]) {
					swPoint = right[startIndex+2];
				}
			}


			//funnelPath.Add (origin);

			Vector3 portalApex = origin;
			Vector3 portalLeft = left[startIndex+1];
			Vector3 portalRight = right[startIndex+1];

			int apexIndex = startIndex+0;
			int rightIndex = startIndex+1;
			int leftIndex = startIndex+1;

			for (int i = startIndex+2; i < diagonalCount; i++) {
				if (funnelPath.Count >= numCorners) {
					return true;
				}

				if (funnelPath.Count > 2000) {
					Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
					break;
				}

				Vector3 pLeft = left[i];
				Vector3 pRight = right[i];

				/*Debug.DrawLine (portalApex,portalLeft,Color.red);
				 * Debug.DrawLine (portalApex,portalRight,Color.yellow);
				 * Debug.DrawLine (portalApex,left,Color.cyan);
				 * Debug.DrawLine (portalApex,right,Color.cyan);*/

				if (VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalRight, pRight) >= 0) {
					if (portalApex == portalRight || VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalLeft, pRight) <= 0) {
						portalRight = pRight;
						rightIndex = i;
					} else {
						funnelPath.Add(portalLeft);
						portalApex = portalLeft;
						apexIndex = leftIndex;

						portalLeft = portalApex;
						portalRight = portalApex;

						leftIndex = apexIndex;
						rightIndex = apexIndex;

						i = apexIndex;

						continue;
					}
				}

				if (VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalLeft, pLeft) <= 0) {
					if (portalApex == portalLeft || VectorMath.SignedTriangleAreaTimes2XZ(portalApex, portalRight, pLeft) >= 0) {
						portalLeft = pLeft;
						leftIndex = i;
					} else {
						funnelPath.Add(portalRight);
						portalApex = portalRight;
						apexIndex = rightIndex;

						portalLeft = portalApex;
						portalRight = portalApex;

						leftIndex = apexIndex;
						rightIndex = apexIndex;

						i = apexIndex;

						continue;
					}
				}
			}

			lastCorner = true;
			funnelPath.Add(left[diagonalCount-1]);

			return true;
		}
	}

	public class RichSpecial : RichPathPart {
		public NodeLink2 nodeLink;
		public Transform first;
		public Transform second;
		public bool reverse;

		public override void OnEnterPool () {
			nodeLink = null;
		}

		/** Works like a constructor, but can be used even for pooled objects. Returns \a this for easy chaining */
		public RichSpecial Initialize (NodeLink2 nodeLink, GraphNode first) {
			this.nodeLink = nodeLink;
			if (first == nodeLink.startNode) {
				this.first = nodeLink.StartTransform;
				this.second = nodeLink.EndTransform;
				reverse = false;
			} else {
				this.first = nodeLink.EndTransform;
				this.second = nodeLink.StartTransform;
				reverse = true;
			}
			return this;
		}
	}
}
