using UnityEngine;
using Pathfinding.Serialization;

namespace Pathfinding {
	public interface INavmeshHolder {
		Int3 GetVertex (int i);
		Int3 GetVertexInGraphSpace (int i);
		int GetVertexArrayIndex (int index);
		void GetTileCoordinates (int tileIndex, out int x, out int z);
	}

	/** Node represented by a triangle */
	public class TriangleMeshNode : MeshNode {
		public TriangleMeshNode (AstarPath astar) : base(astar) {}

		/** Internal vertex index for the first vertex */
		public int v0;

		/** Internal vertex index for the second vertex */
		public int v1;

		/** Internal vertex index for the third vertex */
		public int v2;

		/** Holds INavmeshHolder references for all graph indices to be able to access them in a performant manner */
		protected static INavmeshHolder[] _navmeshHolders = new INavmeshHolder[0];

		/** Used for synchronised access to the #_navmeshHolders array */
		protected static readonly System.Object lockObject = new System.Object();

		public static INavmeshHolder GetNavmeshHolder (uint graphIndex) {
			return _navmeshHolders[(int)graphIndex];
		}

		/** Sets the internal navmesh holder for a given graph index.
		 * \warning Internal method
		 */
		public static void SetNavmeshHolder (int graphIndex, INavmeshHolder graph) {
			if (_navmeshHolders.Length <= graphIndex) {
				// We need to lock and then check again to make sure
				// that this the resize operation is thread safe
				lock (lockObject) {
					if (_navmeshHolders.Length <= graphIndex) {
						var gg = new INavmeshHolder[graphIndex+1];
						for (int i = 0; i < _navmeshHolders.Length; i++) gg[i] = _navmeshHolders[i];
						_navmeshHolders = gg;
					}
				}
			}
			_navmeshHolders[graphIndex] = graph;
		}

		/** Set the position of this node to the average of its 3 vertices */
		public void UpdatePositionFromVertices () {
			Int3 a, b, c;

			GetVertices(out a, out b, out c);
			position = (a + b + c) * 0.333333f;
		}

		/** Return a number identifying a vertex.
		 * This number does not necessarily need to be a index in an array but two different vertices (in the same graph) should
		 * not have the same vertex numbers.
		 */
		public int GetVertexIndex (int i) {
			return i == 0 ? v0 : (i == 1 ? v1 : v2);
		}

		/** Return a number specifying an index in the source vertex array.
		 * The vertex array can for example be contained in a recast tile, or be a navmesh graph, that is graph dependant.
		 * This is slower than GetVertexIndex, if you only need to compare vertices, use GetVertexIndex.
		 */
		public int GetVertexArrayIndex (int i) {
			return GetNavmeshHolder(GraphIndex).GetVertexArrayIndex(i == 0 ? v0 : (i == 1 ? v1 : v2));
		}

		/** Returns all 3 vertices of this node in world space */
		public void GetVertices (out Int3 v0, out Int3 v1, out Int3 v2) {
			// Get the object holding the vertex data for this node
			// This is usually a graph or a recast graph tile
			var holder = GetNavmeshHolder(GraphIndex);

			v0 = holder.GetVertex(this.v0);
			v1 = holder.GetVertex(this.v1);
			v2 = holder.GetVertex(this.v2);
		}

		public override Int3 GetVertex (int i) {
			return GetNavmeshHolder(GraphIndex).GetVertex(GetVertexIndex(i));
		}

		public Int3 GetVertexInGraphSpace (int i) {
			return GetNavmeshHolder(GraphIndex).GetVertexInGraphSpace(GetVertexIndex(i));
		}

		public override int GetVertexCount () {
			// A triangle has 3 vertices
			return 3;
		}

		public override Vector3 ClosestPointOnNode (Vector3 p) {
			Int3 a, b, c;

			GetVertices(out a, out b, out c);
			return Pathfinding.Polygon.ClosestPointOnTriangle((Vector3)a, (Vector3)b, (Vector3)c, p);
		}

		public override Vector3 ClosestPointOnNodeXZ (Vector3 p) {
			// Get all 3 vertices for this node
			Int3 tp1, tp2, tp3;

			GetVertices(out tp1, out tp2, out tp3);

			Vector2 closest = Polygon.ClosestPointOnTriangle(
				new Vector2(tp1.x*Int3.PrecisionFactor, tp1.z*Int3.PrecisionFactor),
				new Vector2(tp2.x*Int3.PrecisionFactor, tp2.z*Int3.PrecisionFactor),
				new Vector2(tp3.x*Int3.PrecisionFactor, tp3.z*Int3.PrecisionFactor),
				new Vector2(p.x, p.z)
				);

			return new Vector3(closest.x, p.y, closest.y);
		}

		public override bool ContainsPoint (Int3 p) {
			// Get all 3 vertices for this node
			Int3 a, b, c;

			GetVertices(out a, out b, out c);

			if ((long)(b.x - a.x) * (long)(p.z - a.z) - (long)(p.x - a.x) * (long)(b.z - a.z) > 0) return false;

			if ((long)(c.x - b.x) * (long)(p.z - b.z) - (long)(p.x - b.x) * (long)(c.z - b.z) > 0) return false;

			if ((long)(a.x - c.x) * (long)(p.z - c.z) - (long)(p.x - c.x) * (long)(a.z - c.z) > 0) return false;

			return true;
			// Equivalent code, but the above code is faster
			//return Polygon.IsClockwiseMargin (a,b, p) && Polygon.IsClockwiseMargin (b,c, p) && Polygon.IsClockwiseMargin (c,a, p);

			//return Polygon.ContainsPoint(g.GetVertex(v0),g.GetVertex(v1),g.GetVertex(v2),p);
		}

		public override void UpdateRecursiveG (Path path, PathNode pathNode, PathHandler handler) {
			UpdateG(path, pathNode);

			handler.heap.Add(pathNode);

			if (connections == null) return;

			for (int i = 0; i < connections.Length; i++) {
				GraphNode other = connections[i].node;
				PathNode otherPN = handler.GetPathNode(other);
				if (otherPN.parent == pathNode && otherPN.pathID == handler.PathID) other.UpdateRecursiveG(path, otherPN, handler);
			}
		}

		public override void Open (Path path, PathNode pathNode, PathHandler handler) {
			if (connections == null) return;

			// Flag2 indicates if this node needs special treatment
			// with regard to connection costs
			bool flag2 = pathNode.flag2;

			// Loop through all connections
			for (int i = connections.Length-1; i >= 0; i--) {
				var conn = connections[i];
				var other = conn.node;

				// Make sure we can traverse the neighbour
				if (path.CanTraverse(conn.node)) {
					PathNode pathOther = handler.GetPathNode(conn.node);

					// Fast path out, worth it for triangle mesh nodes since they usually have degree 2 or 3
					if (pathOther == pathNode.parent) {
						continue;
					}

					uint cost = conn.cost;

					if (flag2 || pathOther.flag2) {
						// Get special connection cost from the path
						// This is used by the start and end nodes
						cost = path.GetConnectionSpecialCost(this, conn.node, cost);
					}

					// Test if we have seen the other node before
					if (pathOther.pathID != handler.PathID) {
						// We have not seen the other node before
						// So the path from the start through this node to the other node
						// must be the shortest one so far

						// Might not be assigned
						pathOther.node = conn.node;

						pathOther.parent = pathNode;
						pathOther.pathID = handler.PathID;

						pathOther.cost = cost;

						pathOther.H = path.CalculateHScore(other);
						other.UpdateG(path, pathOther);

						handler.heap.Add(pathOther);
					} else {
						// If not we can test if the path from this node to the other one is a better one than the one already used
						if (pathNode.G + cost + path.GetTraversalCost(other) < pathOther.G) {
							pathOther.cost = cost;
							pathOther.parent = pathNode;

							other.UpdateRecursiveG(path, pathOther, handler);
						} else if (pathOther.G+cost+path.GetTraversalCost(this) < pathNode.G && other.ContainsConnection(this)) {
							// Or if the path from the other node to this one is better

							pathNode.parent = pathOther;
							pathNode.cost = cost;

							UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
		}

		/** Returns the edge which is shared with \a other.
		 * If no edge is shared, -1 is returned.
		 * The edge is GetVertex(result) - GetVertex((result+1) % GetVertexCount()).
		 * See GetPortal for the exact segment shared.
		 * \note Might return that an edge is shared when the two nodes are in different tiles and adjacent on the XZ plane, but do not line up perfectly on the Y-axis.
		 * Therefore it is recommended that you only test for neighbours of this node or do additional checking afterwards.
		 */
		public int SharedEdge (GraphNode other) {
			int a, b;

			GetPortal(other, null, null, false, out a, out b);
			return a;
		}

		public override bool GetPortal (GraphNode _other, System.Collections.Generic.List<Vector3> left, System.Collections.Generic.List<Vector3> right, bool backwards) {
			int aIndex, bIndex;

			return GetPortal(_other, left, right, backwards, out aIndex, out bIndex);
		}

		public bool GetPortal (GraphNode _other, System.Collections.Generic.List<Vector3> left, System.Collections.Generic.List<Vector3> right, bool backwards, out int aIndex, out int bIndex) {
			aIndex = -1;
			bIndex = -1;

			//If the nodes are in different graphs, this function has no idea on how to find a shared edge.
			if (_other.GraphIndex != GraphIndex) return false;

			// Since the nodes are in the same graph, they are both TriangleMeshNodes
			// So we don't need to care about other types of nodes
			var other = _other as TriangleMeshNode;

			//Get tile indices
			int tileIndex = (GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;
			int tileIndex2 = (other.GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;

			//When the nodes are in different tiles, the edges might not be completely identical
			//so another technique is needed
			//Only do this on recast graphs
			if (tileIndex != tileIndex2 && (GetNavmeshHolder(GraphIndex) is RecastGraph)) {
				for (int i = 0; i < connections.Length; i++) {
					if (connections[i].node.GraphIndex != GraphIndex) {
#if !ASTAR_NO_POINT_GRAPH
						var mid = connections[i].node as NodeLink3Node;
						if (mid != null && mid.GetOther(this) == other) {
							// We have found a node which is connected through a NodeLink3Node

							if (left != null) {
								mid.GetPortal(other, left, right, false);
								return true;
							}
						}
#endif
					}
				}

				//Get the tile coordinates, from them we can figure out which edge is going to be shared
				int x1, x2, z1, z2;
				int coord;
				INavmeshHolder nm = GetNavmeshHolder(GraphIndex);
				nm.GetTileCoordinates(tileIndex, out x1, out z1);
				nm.GetTileCoordinates(tileIndex2, out x2, out z2);

				if (System.Math.Abs(x1-x2) == 1) coord = 0;
				else if (System.Math.Abs(z1-z2) == 1) coord = 2;
				else throw new System.Exception("Tiles not adjacent (" + x1+", " + z1 +") (" + x2 + ", " + z2+")");

				int av = GetVertexCount();
				int bv = other.GetVertexCount();

				//Try the X and Z coordinate. For one of them the coordinates should be equal for one of the two nodes' edges
				//The midpoint between the tiles is the only place where they will be equal

				int first = -1, second = -1;

				//Find the shared edge
				for (int a = 0; a < av; a++) {
					int va = GetVertex(a)[coord];
					for (int b = 0; b < bv; b++) {
						if (va == other.GetVertex((b+1)%bv)[coord] && GetVertex((a+1) % av)[coord] == other.GetVertex(b)[coord]) {
							first = a;
							second = b;
							a = av;
							break;
						}
					}
				}

				aIndex = first;
				bIndex = second;

				if (first != -1) {
					Int3 a = GetVertex(first);
					Int3 b = GetVertex((first+1)%av);

					//The coordinate which is not the same for the vertices
					int ocoord = coord == 2 ? 0 : 2;

					//When the nodes are in different tiles, they might not share exactly the same edge
					//so we clamp the portal to the segment of the edges which they both have.
					int mincoord = System.Math.Min(a[ocoord], b[ocoord]);
					int maxcoord = System.Math.Max(a[ocoord], b[ocoord]);

					mincoord = System.Math.Max(mincoord, System.Math.Min(other.GetVertex(second)[ocoord], other.GetVertex((second+1)%bv)[ocoord]));
					maxcoord = System.Math.Min(maxcoord, System.Math.Max(other.GetVertex(second)[ocoord], other.GetVertex((second+1)%bv)[ocoord]));

					if (a[ocoord] < b[ocoord]) {
						a[ocoord] = mincoord;
						b[ocoord] = maxcoord;
					} else {
						a[ocoord] = maxcoord;
						b[ocoord] = mincoord;
					}

					if (left != null) {
						//All triangles should be clockwise so second is the rightmost vertex (seen from this node)
						left.Add((Vector3)a);
						right.Add((Vector3)b);
					}
					return true;
				}
			} else
			if (!backwards) {
				int first = -1;
				int second = -1;

				int av = GetVertexCount();
				int bv = other.GetVertexCount();

				/** \todo Maybe optimize with pa=av-1 instead of modulus... */
				for (int a = 0; a < av; a++) {
					int va = GetVertexIndex(a);
					for (int b = 0; b < bv; b++) {
						if (va == other.GetVertexIndex((b+1)%bv) && GetVertexIndex((a+1) % av) == other.GetVertexIndex(b)) {
							first = a;
							second = b;
							a = av;
							break;
						}
					}
				}

				aIndex = first;
				bIndex = second;

				if (first != -1) {
					if (left != null) {
						//All triangles should be clockwise so second is the rightmost vertex (seen from this node)
						left.Add((Vector3)GetVertex(first));
						right.Add((Vector3)GetVertex((first+1)%av));
					}
				} else {
					for (int i = 0; i < connections.Length; i++) {
						if (connections[i].node.GraphIndex != GraphIndex) {
#if !ASTAR_NO_POINT_GRAPH
							var mid = connections[i].node as NodeLink3Node;
							if (mid != null && mid.GetOther(this) == other) {
								// We have found a node which is connected through a NodeLink3Node

								if (left != null) {
									mid.GetPortal(other, left, right, false);
									return true;
								}
							}
#endif
						}
					}
					return false;
				}
			}

			return true;
		}

		public override float SurfaceArea () {
			// TODO: This is the area in XZ space, use full 3D space for higher correctness maybe?
			var holder = GetNavmeshHolder(GraphIndex);

			return System.Math.Abs(VectorMath.SignedTriangleAreaTimes2XZ(holder.GetVertex(v0), holder.GetVertex(v1), holder.GetVertex(v2))) * 0.5f;
		}

		public override Vector3 RandomPointOnSurface () {
			// Find a random point inside the triangle
			// This generates uniformly distributed trilinear coordinates
			// See http://mathworld.wolfram.com/TrianglePointPicking.html
			float r1;
			float r2;

			do {
				r1 = Random.value;
				r2 = Random.value;
			} while (r1+r2 > 1);

			var holder = GetNavmeshHolder(GraphIndex);
			// Pick the point corresponding to the trilinear coordinate
			return ((Vector3)(holder.GetVertex(v1)-holder.GetVertex(v0)))*r1 + ((Vector3)(holder.GetVertex(v2)-holder.GetVertex(v0)))*r2 + (Vector3)holder.GetVertex(v0);
		}

		public override void SerializeNode (GraphSerializationContext ctx) {
			base.SerializeNode(ctx);
			ctx.writer.Write(v0);
			ctx.writer.Write(v1);
			ctx.writer.Write(v2);
		}

		public override void DeserializeNode (GraphSerializationContext ctx) {
			base.DeserializeNode(ctx);
			v0 = ctx.reader.ReadInt32();
			v1 = ctx.reader.ReadInt32();
			v2 = ctx.reader.ReadInt32();
		}
	}
}
