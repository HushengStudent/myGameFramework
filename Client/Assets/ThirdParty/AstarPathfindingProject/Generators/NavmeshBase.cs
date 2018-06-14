using UnityEngine;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding {
	using System.IO;
	using Pathfinding.Util;
	using Pathfinding.Serialization;
	using Math = System.Math;

	/** Base class for RecastGraph and NavMeshGraph */
	public abstract class NavmeshBase : NavGraph, INavmesh, INavmeshHolder, ITransformedGraph
		, IRaycastableGraph {
#if ASTAR_RECAST_LARGER_TILES
		// Larger tiles
		public const int VertexIndexMask = 0xFFFFF;

		public const int TileIndexMask = 0x7FF;
		public const int TileIndexOffset = 20;
#else
		// Larger worlds
		public const int VertexIndexMask = 0xFFF;

		public const int TileIndexMask = 0x7FFFF;
		public const int TileIndexOffset = 12;
#endif

		/** Size of the bounding box. */
		[JsonMember]
		public Vector3 forcedBoundsSize = new Vector3(100, 40, 100);

		/** Size of a tile in world units along the X axis */
		public abstract float TileWorldSizeX { get; }

		/** Size of a tile in world units along the Z axis */
		public abstract float TileWorldSizeZ { get; }

		/** Maximum (vertical) distance between the sides of two nodes for them to be connected across a tile edge.
		 * When tiles are connected to each other, the nodes sometimes do not line up perfectly
		 * so some allowance must be made to allow tiles that do not match exactly to be connected with each other.
		 */
		protected abstract float MaxTileConnectionEdgeDistance { get; }

		/** Show an outline of the polygons in the Unity Editor */
		[JsonMember]
		public bool showMeshOutline = true;

		/** Show the connections between the polygons in the Unity Editor */
		[JsonMember]
		public bool showNodeConnections;

		/** Show the surface of the navmesh */
		[JsonMember]
		public bool showMeshSurface;

		/** Number of tiles along the X-axis */
		public int tileXCount;
		/** Number of tiles along the Z-axis */
		public int tileZCount;

		/** All tiles.
		 *
		 * \see #GetTile
		 */
		protected NavmeshTile[] tiles;

		/** Perform nearest node searches in XZ space only.
		 * Recomended for single-layered environments. Faster but can be inaccurate esp. in multilayered contexts.
		 * You should not use this if the graph is rotated since then the XZ plane no longer corresponds to the ground plane.
		 */
		[JsonMember]
		public bool nearestSearchOnlyXZ;

		/** Currently updating tiles in a batch */
		bool batchTileUpdate;

		/** List of tiles updating during batch */
		List<int> batchUpdatedTiles = new List<int>();

		/** Determines how the graph transforms graph space to world space.
		 * \see #CalculateTransform
		 */
		public GraphTransform transform = new GraphTransform(Matrix4x4.identity);

		GraphTransform ITransformedGraph.transform { get { return transform; } }

		/** Returns a new transform which transforms graph space to world space.
		 * Does not update the #transform field.
		 * \see #RelocateNodes(GraphTransform)
		 */
		public abstract GraphTransform CalculateTransform ();

		/** Called when tiles have been completely recalculated.
		 * This is called after scanning the graph and after
		 * performing graph updates that completely recalculate tiles
		 * (not ones that simply modify e.g penalties).
		 * It is not called after NavmeshCut updates.
		 */
		public System.Action<NavmeshTile[]> OnRecalculatedTiles;

		/** Tile at the specified x, z coordinate pair.
		 * The first tile is at (0,0), the last tile at (tileXCount-1, tileZCount-1).
		 */
		public NavmeshTile GetTile (int x, int z) {
			return tiles[x + z * tileXCount];
		}

		/** Vertex coordinate for the specified vertex index.
		 *
		 * \throws IndexOutOfRangeException if the vertex index is invalid.
		 * \throws NullReferenceException if the tile the vertex is in is not calculated.
		 *
		 * \see NavmeshTile.GetVertex
		 */
		public Int3 GetVertex (int index) {
			int tileIndex = (index >> TileIndexOffset) & TileIndexMask;

			return tiles[tileIndex].GetVertex(index);
		}

		/** Vertex coordinate in graph space for the specified vertex index */
		public Int3 GetVertexInGraphSpace (int index) {
			int tileIndex = (index >> TileIndexOffset) & TileIndexMask;

			return tiles[tileIndex].GetVertexInGraphSpace(index);
		}

		/** Tile index from a vertex index */
		public static int GetTileIndex (int index) {
			return (index >> TileIndexOffset) & TileIndexMask;
		}

		public int GetVertexArrayIndex (int index) {
			return index & VertexIndexMask;
		}

		/** Tile coordinates from a tile index */
		public void GetTileCoordinates (int tileIndex, out int x, out int z) {
			//z = System.Math.DivRem (tileIndex, tileXCount, out x);
			z = tileIndex/tileXCount;
			x = tileIndex - z*tileXCount;
		}

		/** All tiles.
		 * \warning Do not modify this array
		 */
		public NavmeshTile[] GetTiles () {
			return tiles;
		}

		/** Returns an XZ bounds object with the bounds of a group of tiles.
		 * The bounds object is defined in world units.
		 */
		public Bounds GetTileBounds (IntRect rect) {
			return GetTileBounds(rect.xmin, rect.ymin, rect.Width, rect.Height);
		}

		/** Returns an XZ bounds object with the bounds of a group of tiles.
		 * The bounds object is defined in world units.
		 */
		public Bounds GetTileBounds (int x, int z, int width = 1, int depth = 1) {
			return transform.Transform(GetTileBoundsInGraphSpace(x, z, width, depth));
		}

		public Bounds GetTileBoundsInGraphSpace (IntRect rect) {
			return GetTileBoundsInGraphSpace(rect.xmin, rect.ymin, rect.Width, rect.Height);
		}

		/** Returns an XZ bounds object with the bounds of a group of tiles in graph space */
		public Bounds GetTileBoundsInGraphSpace (int x, int z, int width = 1, int depth = 1) {
			var b = new Bounds();

			b.SetMinMax(
				new Vector3(x*TileWorldSizeX, 0, z*TileWorldSizeZ),
				new Vector3((x+width)*TileWorldSizeX, forcedBoundsSize.y, (z+depth)*TileWorldSizeZ)
				);
			return b;
		}

		/** Returns the tile coordinate which contains the point \a p.
		 * It is not necessarily a valid tile (i.e it could be out of bounds).
		 */
		public Int2 GetTileCoordinates (Vector3 p) {
			p = transform.InverseTransform(p);
			p.x /= TileWorldSizeX;
			p.z /= TileWorldSizeZ;
			return new Int2((int)p.x, (int)p.z);
		}

		public override void OnDestroy () {
			base.OnDestroy();

			// Cleanup
			TriangleMeshNode.SetNavmeshHolder(active.data.GetGraphIndex(this), null);

			if (tiles != null) {
				for (int i = 0; i < tiles.Length; i++) {
					Pathfinding.Util.ObjectPool<BBTree>.Release(ref tiles[i].bbTree);
				}
			}
		}

		public override void RelocateNodes (Matrix4x4 deltaMatrix) {
			RelocateNodes(deltaMatrix * transform);
		}

		/** Moves the nodes in this graph.
		 * Moves all the nodes in such a way that the specified transform is the new graph space to world space transformation for the graph.
		 * You usually use this together with the #CalculateTransform method.
		 *
		 * So for example if you want to move and rotate all your nodes in e.g a recast graph you can do
		 * \code
		 * var graph = AstarPath.data.recastGraph;
		 * graph.rotation = new Vector3(45, 0, 0);
		 * graph.forcedBoundsCenter = new Vector3(20, 10, 10);
		 * var transform = graph.CalculateTransform();
		 * graph.RelocateNodes(transform);
		 * \endcode
		 * This will move all the nodes to new positions as if the new graph settings had been there from the start.
		 *
		 * \note RelocateNodes(deltaMatrix) is not equivalent to RelocateNodes(new GraphTransform(deltaMatrix)).
		 *  The overload which takes a matrix multiplies all existing node positions with the matrix while this
		 *  overload does not take into account the current positions of the nodes.
		 *
		 * \see #CalculateTransform
		 */
		public void RelocateNodes (GraphTransform newTransform) {
			transform = newTransform;
			if (tiles != null) {
				// Move all the vertices in each tile
				for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++) {
					var tile = tiles[tileIndex];
					if (tile != null) {
						tile.vertsInGraphSpace.CopyTo(tile.verts, 0);
						// Transform the graph space vertices to world space
						transform.Transform(tile.verts);

						for (int nodeIndex = 0; nodeIndex < tile.nodes.Length; nodeIndex++) {
							tile.nodes[nodeIndex].UpdatePositionFromVertices();
						}
						tile.bbTree.RebuildFrom(tile.nodes);
					}
				}
			}
		}

		/** Creates a single new empty tile */
		protected static NavmeshTile NewEmptyTile (int x, int z) {
			return new NavmeshTile {
					   x = x,
					   z = z,
					   w = 1,
					   d = 1,
					   verts = new Int3[0],
					   vertsInGraphSpace = new Int3[0],
					   tris = new int[0],
					   nodes = new TriangleMeshNode[0],
					   bbTree = ObjectPool<BBTree>.Claim()
			};
		}

		public override void GetNodes (System.Action<GraphNode> action) {
			if (tiles == null) return;

			for (int i = 0; i < tiles.Length; i++) {
				if (tiles[i] == null || tiles[i].x+tiles[i].z*tileXCount != i) continue;
				TriangleMeshNode[] nodes = tiles[i].nodes;

				if (nodes == null) continue;

				for (int j = 0; j < nodes.Length; j++) action(nodes[j]);
			}
		}

		/** Returns a rect containing the indices of all tiles touching the specified bounds */
		public IntRect GetTouchingTiles (Bounds bounds) {
			bounds = transform.InverseTransform(bounds);

			// Calculate world bounds of all affected tiles
			var r = new IntRect(Mathf.FloorToInt(bounds.min.x / TileWorldSizeX), Mathf.FloorToInt(bounds.min.z / TileWorldSizeZ), Mathf.FloorToInt(bounds.max.x / TileWorldSizeX), Mathf.FloorToInt(bounds.max.z / TileWorldSizeZ));
			// Clamp to bounds
			r = IntRect.Intersection(r, new IntRect(0, 0, tileXCount-1, tileZCount-1));
			return r;
		}

		/** Returns a rect containing the indices of all tiles touching the specified bounds.
		 * \param rect Graph space rectangle (in graph space all tiles are on the XZ plane regardless of graph rotation and other transformations, the first tile has a corner at the origin)
		 */
		public IntRect GetTouchingTilesInGraphSpace (Rect rect) {
			// Calculate world bounds of all affected tiles
			var r = new IntRect(Mathf.FloorToInt(rect.xMin / TileWorldSizeX), Mathf.FloorToInt(rect.yMin / TileWorldSizeZ), Mathf.FloorToInt(rect.xMax / TileWorldSizeX), Mathf.FloorToInt(rect.yMax / TileWorldSizeZ));

			// Clamp to bounds
			r = IntRect.Intersection(r, new IntRect(0, 0, tileXCount-1, tileZCount-1));
			return r;
		}

		/** Returns a rect containing the indices of all tiles by rounding the specified bounds to tile borders.
		 * This is different from GetTouchingTiles in that the tiles inside the rectangle returned from this method
		 * may not contain the whole bounds, while that is guaranteed for GetTouchingTiles.
		 */
		public IntRect GetTouchingTilesRound (Bounds bounds) {
			bounds = transform.InverseTransform(bounds);

			//Calculate world bounds of all affected tiles
			var r = new IntRect(Mathf.RoundToInt(bounds.min.x / TileWorldSizeX), Mathf.RoundToInt(bounds.min.z / TileWorldSizeZ), Mathf.RoundToInt(bounds.max.x / TileWorldSizeX)-1, Mathf.RoundToInt(bounds.max.z / TileWorldSizeZ)-1);
			//Clamp to bounds
			r = IntRect.Intersection(r, new IntRect(0, 0, tileXCount-1, tileZCount-1));
			return r;
		}

		protected void ConnectTileWithNeighbours (NavmeshTile tile, bool onlyUnflagged = false) {
			if (tile.w != 1 || tile.d != 1) {
				throw new System.ArgumentException("Tile widths or depths other than 1 are not supported. The fields exist mainly for possible future expansions.");
			}

			// Loop through z and x offsets to adjacent tiles
			// _ x _
			// x _ x
			// _ x _
			for (int zo = -1; zo <= 1; zo++) {
				var z = tile.z + zo;
				if (z < 0 || z >= tileZCount) continue;

				for (int xo = -1; xo <= 1; xo++) {
					var x = tile.x + xo;
					if (x < 0 || x >= tileXCount) continue;

					// Ignore diagonals and the tile itself
					if ((xo == 0) == (zo == 0)) continue;

					var otherTile = tiles[x + z*tileXCount];
					if (!onlyUnflagged || !otherTile.flag) {
						ConnectTiles(otherTile, tile);
					}
				}
			}
		}

		protected void RemoveConnectionsFromTile (NavmeshTile tile) {
			if (tile.x > 0) {
				int x = tile.x-1;
				for (int z = tile.z; z < tile.z+tile.d; z++) RemoveConnectionsFromTo(tiles[x + z*tileXCount], tile);
			}
			if (tile.x+tile.w < tileXCount) {
				int x = tile.x+tile.w;
				for (int z = tile.z; z < tile.z+tile.d; z++) RemoveConnectionsFromTo(tiles[x + z*tileXCount], tile);
			}
			if (tile.z > 0) {
				int z = tile.z-1;
				for (int x = tile.x; x < tile.x+tile.w; x++) RemoveConnectionsFromTo(tiles[x + z*tileXCount], tile);
			}
			if (tile.z+tile.d < tileZCount) {
				int z = tile.z+tile.d;
				for (int x = tile.x; x < tile.x+tile.w; x++) RemoveConnectionsFromTo(tiles[x + z*tileXCount], tile);
			}
		}

		protected void RemoveConnectionsFromTo (NavmeshTile a, NavmeshTile b) {
			if (a == null || b == null) return;
			//Same tile, possibly from a large tile (one spanning several x,z tile coordinates)
			if (a == b) return;

			int tileIdx = b.x + b.z*tileXCount;

			for (int i = 0; i < a.nodes.Length; i++) {
				TriangleMeshNode node = a.nodes[i];
				if (node.connections == null) continue;
				for (int j = 0;; j++) {
					//Length will not be constant if connections are removed
					if (j >= node.connections.Length) break;

					var other = node.connections[j].node as TriangleMeshNode;

					//Only evaluate TriangleMeshNodes
					if (other == null) continue;

					int tileIdx2 = other.GetVertexIndex(0);
					tileIdx2 = (tileIdx2 >> TileIndexOffset) & TileIndexMask;

					if (tileIdx2 == tileIdx) {
						node.RemoveConnection(node.connections[j].node);
						j--;
					}
				}
			}
		}

		public override NNInfoInternal GetNearest (Vector3 position, NNConstraint constraint, GraphNode hint) {
			return GetNearestForce(position, null);
		}

		public override NNInfoInternal GetNearestForce (Vector3 position, NNConstraint constraint) {
			if (tiles == null) return new NNInfoInternal();

			var tileCoords = GetTileCoordinates(position);

			// Clamp to graph borders
			tileCoords.x = Mathf.Clamp(tileCoords.x, 0, tileXCount-1);
			tileCoords.y = Mathf.Clamp(tileCoords.y, 0, tileZCount-1);

			int wmax = Math.Max(tileXCount, tileZCount);

			var best = new NNInfoInternal();
			float bestDistance = float.PositiveInfinity;

			bool xzSearch = nearestSearchOnlyXZ || (constraint != null && constraint.distanceXZ);

			// Search outwards in a diamond pattern from the closest tile
			//     2
			//   2 1 2
			// 2 1 0 1 2  etc.
			//   2 1 2
			//     2
			for (int w = 0; w < wmax; w++) {
				// Stop the loop when we can guarantee that no nodes will be closer than the ones we have already searched
				if (bestDistance < (w-2)*Math.Max(TileWorldSizeX, TileWorldSizeX)) break;

				int zmax = Math.Min(w+tileCoords.y +1, tileZCount);
				for (int z = Math.Max(-w+tileCoords.y, 0); z < zmax; z++) {
					// Solve for z such that abs(x-tx) + abs(z-tx) == w
					// Delta X coordinate
					int originalDx = Math.Abs(w - Math.Abs(z-tileCoords.y));
					var dx = originalDx;
					// Solution is dx + tx and -dx + tx
					// This loop will first check +dx and then -dx
					// If dx happens to be zero, then it will not run twice
					do {
						// Absolute x coordinate
						int x = -dx + tileCoords.x;
						if (x >= 0 && x < tileXCount) {
							NavmeshTile tile = tiles[x + z*tileXCount];

							if (tile != null) {
								if (xzSearch) {
									best = tile.bbTree.QueryClosestXZ(position, constraint, ref bestDistance, best);
								} else {
									best = tile.bbTree.QueryClosest(position, constraint, ref bestDistance, best);
								}
							}
						}

						dx = -dx;
					} while (dx != originalDx);
				}
			}

			best.node = best.constrainedNode;
			best.constrainedNode = null;
			best.clampedPosition = best.constClampedPosition;

			return best;
		}

		/** Finds the first node which contains \a position.
		 * "Contains" is defined as \a position is inside the triangle node when seen from above. So only XZ space matters.
		 * In case of a multilayered environment, which node of the possibly several nodes
		 * containing the point is undefined.
		 *
		 * Returns null if there was no node containing the point. This serves as a quick
		 * check for "is this point on the navmesh or not".
		 *
		 * Note that the behaviour of this method is distinct from the GetNearest method.
		 * The GetNearest method will return the closest node to a point,
		 * which is not necessarily the one which contains it in XZ space.
		 *
		 * \see GetNearest
		 */
		public GraphNode PointOnNavmesh (Vector3 position, NNConstraint constraint) {
			if (tiles == null) return null;

			var tileCoords = GetTileCoordinates(position);

			// Graph borders
			if (tileCoords.x < 0 || tileCoords.y < 0 || tileCoords.x >= tileXCount || tileCoords.y >= tileZCount) return null;

			NavmeshTile tile = GetTile(tileCoords.x, tileCoords.y);

			if (tile != null) {
				GraphNode node = tile.bbTree.QueryInside(position, constraint);
				return node;
			}

			return null;
		}

		/** Fills graph with tiles created by NewEmptyTile */
		protected void FillWithEmptyTiles () {
			for (int z = 0; z < tileZCount; z++) {
				for (int x = 0; x < tileXCount; x++) {
					tiles[z*tileXCount + x] = NewEmptyTile(x, z);
				}
			}
		}

		/** Create connections between all nodes.
		 * \version Since 3.7.6 the implementation is thread safe
		 */
		protected static void CreateNodeConnections (TriangleMeshNode[] nodes) {
			List<Connection> connections = ListPool<Connection>.Claim();

			var nodeRefs = ObjectPoolSimple<Dictionary<Int2, int> >.Claim();
			nodeRefs.Clear();

			// Build node neighbours
			for (int i = 0; i < nodes.Length; i++) {
				TriangleMeshNode node = nodes[i];

				int av = node.GetVertexCount();

				for (int a = 0; a < av; a++) {
					// Recast can in some very special cases generate degenerate triangles which are simply lines
					// In that case, duplicate keys might be added and thus an exception will be thrown
					// It is safe to ignore the second edge though... I think (only found one case where this happens)
					var key = new Int2(node.GetVertexIndex(a), node.GetVertexIndex((a+1) % av));
					if (!nodeRefs.ContainsKey(key)) {
						nodeRefs.Add(key, i);
					}
				}
			}

			for (int i = 0; i < nodes.Length; i++) {
				TriangleMeshNode node = nodes[i];

				connections.Clear();

				int av = node.GetVertexCount();

				for (int a = 0; a < av; a++) {
					int first = node.GetVertexIndex(a);
					int second = node.GetVertexIndex((a+1) % av);
					int connNode;

					if (nodeRefs.TryGetValue(new Int2(second, first), out connNode)) {
						TriangleMeshNode other = nodes[connNode];

						int bv = other.GetVertexCount();

						for (int b = 0; b < bv; b++) {
							/** \todo This will fail on edges which are only partially shared */
							if (other.GetVertexIndex(b) == second && other.GetVertexIndex((b+1) % bv) == first) {
								connections.Add(new Connection {
									node = other,
									cost = (uint)(node.position - other.position).costMagnitude
								});
								break;
							}
						}
					}
				}

				node.connections = connections.ToArrayFromPool();
			}

			nodeRefs.Clear();
			ObjectPoolSimple<Dictionary<Int2, int> >.Release(ref nodeRefs);
			ListPool<Connection>.Release(connections);
		}

		/** Generate connections between the two tiles.
		 * The tiles must be adjacent.
		 */
		protected void ConnectTiles (NavmeshTile tile1, NavmeshTile tile2) {
			if (tile1 == null || tile2 == null) return;

			if (tile1.nodes == null) throw new System.ArgumentException("tile1 does not contain any nodes");
			if (tile2.nodes == null) throw new System.ArgumentException("tile2 does not contain any nodes");

			int t1x = Mathf.Clamp(tile2.x, tile1.x, tile1.x+tile1.w-1);
			int t2x = Mathf.Clamp(tile1.x, tile2.x, tile2.x+tile2.w-1);
			int t1z = Mathf.Clamp(tile2.z, tile1.z, tile1.z+tile1.d-1);
			int t2z = Mathf.Clamp(tile1.z, tile2.z, tile2.z+tile2.d-1);

			int coord, altcoord;
			int t1coord, t2coord;

			float tileWorldSize;

			// Figure out which side that is shared between the two tiles
			// and what coordinate index is fixed along that edge (x or z)
			if (t1x == t2x) {
				coord = 2;
				altcoord = 0;
				t1coord = t1z;
				t2coord = t2z;
				tileWorldSize = TileWorldSizeZ;
			} else if (t1z == t2z) {
				coord = 0;
				altcoord = 2;
				t1coord = t1x;
				t2coord = t2x;
				tileWorldSize = TileWorldSizeX;
			} else {
				throw new System.ArgumentException("Tiles are not adjacent (neither x or z coordinates match)");
			}

			if (Math.Abs(t1coord-t2coord) != 1) {
				throw new System.ArgumentException("Tiles are not adjacent (tile coordinates must differ by exactly 1. Got '" + t1coord + "' and '" + t2coord + "')");
			}

			// Midpoint between the two tiles
			int midpoint = (int)Math.Round((Math.Max(t1coord, t2coord) * tileWorldSize) * Int3.Precision);

			#if ASTARDEBUG
			Vector3 v1 = new Vector3(-100, 0, -100);
			Vector3 v2 = new Vector3(100, 0, 100);
			v1[coord] = midpoint*Int3.PrecisionFactor;
			v2[coord] = midpoint*Int3.PrecisionFactor;

			Debug.DrawLine(v1, v2, Color.magenta);
			#endif

			TriangleMeshNode[] nodes1 = tile1.nodes;
			TriangleMeshNode[] nodes2 = tile2.nodes;

			// Find adjacent nodes on the border between the tiles
			for (int i = 0; i < nodes1.Length; i++) {
				TriangleMeshNode nodeA = nodes1[i];
				int aVertexCount = nodeA.GetVertexCount();

				// Loop through all *sides* of the node
				for (int a = 0; a < aVertexCount; a++) {
					// Vertices that the segment consists of
					Int3 aVertex1 = nodeA.GetVertexInGraphSpace(a);
					Int3 aVertex2 = nodeA.GetVertexInGraphSpace((a+1) % aVertexCount);

					// Check if it is really close to the tile border
					if (Math.Abs(aVertex1[coord] - midpoint) < 2 && Math.Abs(aVertex2[coord] - midpoint) < 2) {
						int minalt = Math.Min(aVertex1[altcoord], aVertex2[altcoord]);
						int maxalt = Math.Max(aVertex1[altcoord], aVertex2[altcoord]);

						// Degenerate edge
						if (minalt == maxalt) continue;

						for (int j = 0; j < nodes2.Length; j++) {
							TriangleMeshNode nodeB = nodes2[j];
							int bVertexCount = nodeB.GetVertexCount();
							for (int b = 0; b < bVertexCount; b++) {
								Int3 bVertex1 = nodeB.GetVertexInGraphSpace(b);
								Int3 bVertex2 = nodeB.GetVertexInGraphSpace((b+1) % aVertexCount);
								if (Math.Abs(bVertex1[coord] - midpoint) < 2 && Math.Abs(bVertex2[coord] - midpoint) < 2) {
									int minalt2 = Math.Min(bVertex1[altcoord], bVertex2[altcoord]);
									int maxalt2 = Math.Max(bVertex1[altcoord], bVertex2[altcoord]);

									// Degenerate edge
									if (minalt2 == maxalt2) continue;

									if (maxalt > minalt2 && minalt < maxalt2) {
										// The two nodes seem to be adjacent

										// Test shortest distance between the segments (first test if they are equal since that is much faster and pretty common)
										if ((aVertex1 == bVertex1 && aVertex2 == bVertex2) || (aVertex1 == bVertex2 && aVertex2 == bVertex1) ||
											VectorMath.SqrDistanceSegmentSegment((Vector3)aVertex1, (Vector3)aVertex2, (Vector3)bVertex1, (Vector3)bVertex2) < MaxTileConnectionEdgeDistance*MaxTileConnectionEdgeDistance) {
											uint cost = (uint)(nodeA.position - nodeB.position).costMagnitude;

											nodeA.AddConnection(nodeB, cost);
											nodeB.AddConnection(nodeA, cost);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/** Start batch updating of tiles.
		 * During batch updating, tiles will not be connected if they are updating with ReplaceTile.
		 * When ending batching, all affected tiles will be connected.
		 * This is faster than not using batching.
		 */
		public void StartBatchTileUpdate () {
			if (batchTileUpdate) throw new System.InvalidOperationException("Calling StartBatchLoad when batching is already enabled");
			batchTileUpdate = true;
		}

		/** End batch updating of tiles.
		 * During batch updating, tiles will not be connected if they are updating with ReplaceTile.
		 * When ending batching, all affected tiles will be connected.
		 * This is faster than not using batching.
		 */
		public void EndBatchTileUpdate () {
			if (!batchTileUpdate) throw new System.InvalidOperationException("Calling EndBatchLoad when batching not enabled");

			batchTileUpdate = false;

			int tw = tileXCount;
			int td = tileZCount;

			//Clear all flags
			for (int z = 0; z < td; z++) {
				for (int x = 0; x < tw; x++) {
					tiles[x + z*tileXCount].flag = false;
				}
			}

			for (int i = 0; i < batchUpdatedTiles.Count; i++) tiles[batchUpdatedTiles[i]].flag = true;

			for (int z = 0; z < td; z++) {
				for (int x = 0; x < tw; x++) {
					if (x < tw-1
						&& (tiles[x + z*tileXCount].flag || tiles[x+1 + z*tileXCount].flag)
						&& tiles[x + z*tileXCount] != tiles[x+1 + z*tileXCount]) {
						ConnectTiles(tiles[x + z*tileXCount], tiles[x+1 + z*tileXCount]);
					}

					if (z < td-1
						&& (tiles[x + z*tileXCount].flag || tiles[x + (z+1)*tileXCount].flag)
						&& tiles[x + z*tileXCount] != tiles[x + (z+1)*tileXCount]) {
						ConnectTiles(tiles[x + z*tileXCount], tiles[x + (z+1)*tileXCount]);
					}
				}
			}

			batchUpdatedTiles.Clear();
		}

		/** Clear all tiles within the rectangle with one corner at (x,z), width w and depth d */
		protected void ClearTiles (int x, int z, int w, int d) {
			for (int cz = z; cz < z+d; cz++) {
				for (int cx = x; cx < x+w; cx++) {
					int tileIndex = cx + cz*tileXCount;
					NavmeshTile otile = tiles[tileIndex];
					if (otile == null) continue;

					otile.Destroy();

					for (int qz = otile.z; qz < otile.z+otile.d; qz++) {
						for (int qx = otile.x; qx < otile.x+otile.w; qx++) {
							NavmeshTile qtile = tiles[qx + qz*tileXCount];
							if (qtile == null || qtile != otile) throw new System.Exception("This should not happen");

							if (qz < z || qz >= z+d || qx < x || qx >= x+w) {
								// if out of this tile's bounds, replace with empty tile
								tiles[qx + qz*tileXCount] = NewEmptyTile(qx, qz);

								if (batchTileUpdate) {
									batchUpdatedTiles.Add(qx + qz*tileXCount);
								}
							} else {
								//Will be replaced by the new tile
								tiles[qx + qz*tileXCount] = null;
							}
						}
					}
				}
			}
		}

		/** Replace tile at index with nodes created from specified navmesh.
		 * \see StartBatchTileUpdating
		 */
		public void ReplaceTile (int x, int z, Int3[] verts, int[] tris) {
			ReplaceTile(x, z, 1, 1, verts, tris);
		}

		/** Replaces a tile with a new mesh.
		 * This will create new nodes and link them to the adjacent tile (unless batching has been started in which case that will be done when batching ends).
		 *
		 * The vertices are assumed to be in 'tile space', that is being in a rectangle with
		 * one corner at the origin and one at (#TileWorldSizeX, 0, #TileWorldSizeZ).
		 *
		 * \note The vertex and triangle arrays may be modified and will be stored with the tile data.
		 * do not modify them after this method has been called.
		 */
		public void ReplaceTile (int x, int z, int w, int d, Int3[] verts, int[] tris) {
			if (x + w > tileXCount || z+d > tileZCount || x < 0 || z < 0) {
				throw new System.ArgumentException("Tile is placed at an out of bounds position or extends out of the graph bounds ("+x+", " + z + " [" + w + ", " + d+ "] " + tileXCount + " " + tileZCount + ")");
			}

			if (w < 1 || d < 1) throw new System.ArgumentException("width and depth must be greater or equal to 1. Was " + w + ", " + d);
			if (tris.Length % 3 != 0) throw new System.ArgumentException("Triangle array's length must be a multiple of 3 (tris)");
			if (verts.Length > 0xFFFF) throw new System.ArgumentException("Too many vertices per tile (more than 65535). Try using a smaller tile size.");

			Profiler.BeginSample("Clear Previous Tiles");

			// Remove previous tiles
			ClearTiles(x, z, w, d);

			Profiler.EndSample();
			Profiler.BeginSample("Tile Initialization");

			//Create a new navmesh tile and assign its settings
			var tile = new NavmeshTile {
				x = x,
				z = z,
				w = w,
				d = d,
				tris = tris,
				bbTree = ObjectPool<BBTree>.Claim()
			};

			if (!Mathf.Approximately(x*TileWorldSizeX*Int3.FloatPrecision, (float)Math.Round(x*TileWorldSizeX*Int3.FloatPrecision))) Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");
			if (!Mathf.Approximately(z*TileWorldSizeZ*Int3.FloatPrecision, (float)Math.Round(z*TileWorldSizeZ*Int3.FloatPrecision))) Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");

			var offset = (Int3) new Vector3((x * TileWorldSizeX), 0, (z * TileWorldSizeZ));

			for (int i = 0; i < verts.Length; i++) {
				verts[i] += offset;
			}
			tile.vertsInGraphSpace = verts;
			tile.verts = (Int3[])verts.Clone();
			transform.Transform(tile.verts);

			//Here we are faking a new graph
			//The tile is not added to any graphs yet, but to get the position querys from the nodes
			//to work correctly (not throw exceptions because the tile is not calculated) we fake a new graph
			//and direct the position queries directly to the tile
			int graphIndex = AstarPath.active.data.graphs.Length;

			TriangleMeshNode.SetNavmeshHolder(graphIndex, tile);

			//This index will be ORed to the triangle indices
			int tileIndex = x + z*tileXCount;
			tileIndex <<= TileIndexOffset;

			Profiler.EndSample();

			if (tile.verts.Length > VertexIndexMask) {
				Debug.LogError("Too many vertices in the tile (" + tile.verts.Length + " > " + VertexIndexMask +")\nYou can enable ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector to raise this limit.");
				tiles[tileIndex] = NewEmptyTile(x, z);
				return;
			}

			Profiler.BeginSample("Assign Node Data");

			// Create nodes and assign triangle indices
			var nodes = tile.nodes = CreateNodes(tile.tris, tileIndex, (uint)graphIndex);

			Profiler.EndSample();
			Profiler.BeginSample("AABBTree Rebuild");
			tile.bbTree.RebuildFrom(nodes);
			Profiler.EndSample();

			Profiler.BeginSample("Create Node Connections");
			CreateNodeConnections(tile.nodes);
			Profiler.EndSample();

			Profiler.BeginSample("Connect With Neighbours");

			// Set tile
			for (int cz = z; cz < z+d; cz++) {
				for (int cx = x; cx < x+w; cx++) {
					tiles[cx + cz*tileXCount] = tile;
				}
			}

			if (batchTileUpdate) {
				batchUpdatedTiles.Add(x + z*tileXCount);
			} else {
				ConnectTileWithNeighbours(tile);
			}

			//Remove the fake graph
			TriangleMeshNode.SetNavmeshHolder(graphIndex, null);
			Profiler.EndSample();

			Profiler.BeginSample("Set graph index");

			//Real graph index
			//TODO, could this step be changed for this function, is a fake index required?
			graphIndex = AstarPath.active.data.GetGraphIndex(this);

			for (int i = 0; i < nodes.Length; i++) nodes[i].GraphIndex = (uint)graphIndex;

			Profiler.EndSample();
		}

		protected TriangleMeshNode[] CreateNodes (int[] tris, int tileIndex, uint graphIndex) {
			var nodes = new TriangleMeshNode[tris.Length/3];

			// Create nodes and assign vertex indices
			for (int i = 0; i < nodes.Length; i++) {
				var node = nodes[i] = new TriangleMeshNode(active);
				node.GraphIndex = graphIndex;
				// The vertices stored on the node are composed
				// out of the triangle index and the tile index
				node.v0 = tris[i*3+0] | tileIndex;
				node.v1 = tris[i*3+1] | tileIndex;
				node.v2 = tris[i*3+2] | tileIndex;

				// Make sure the triangle is clockwise in graph space (it may not be in world space since the graphs can be rotated)
				if (!VectorMath.IsClockwiseXZ(node.GetVertexInGraphSpace(0), node.GetVertexInGraphSpace(1), node.GetVertexInGraphSpace(2))) {
					int tmp = node.v0;
					node.v0 = node.v2;
					node.v2 = tmp;
				}

				node.Walkable = true;
				node.Penalty = initialPenalty;
				node.UpdatePositionFromVertices();
			}

			return nodes;
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end) {
			return Linecast(origin, end, GetNearest(origin, NNConstraint.None).node);
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * \param [in] origin Point to linecast from
		 * \param [in] end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point
		 *
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end, GraphNode hint, out GraphHitInfo hit) {
			return Linecast(this as INavmesh, origin, end, hint, out hit, null);
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * \param [in] origin Point to linecast from
		 * \param [in] end Point to linecast to
		 * \param [in] hint You need to pass the node closest to the start point
		 *
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end, GraphNode hint) {
			GraphHitInfo hit;

			return Linecast(this as INavmesh, origin, end, hint, out hit, null);
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * \param [in] origin Point to linecast from
		 * \param [in] end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point
		 * \param trace If a list is passed, then it will be filled with all nodes the linecast traverses
		 *
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace) {
			return Linecast(this as INavmesh, origin, end, hint, out hit, trace);
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * \param [in] graph The graph to perform the search on
		 * \param [in] tmp_origin Point to start from
		 * \param [in] tmp_end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point, if null, a search for the closest node will be done
		 *
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public static bool Linecast (INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, GraphNode hint, out GraphHitInfo hit) {
			return Linecast(graph, tmp_origin, tmp_end, hint, out hit, null);
		}

		/** Returns if there is an obstacle between \a origin and \a end on the graph.
		 * \param [in] graph The graph to perform the search on
		 * \param [in] tmp_origin Point to start from
		 * \param [in] tmp_end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point, if null, a search for the closest node will be done
		 * \param trace If a list is passed, then it will be filled with all nodes the linecast traverses
		 *
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public static bool Linecast (INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace) {
			var end = (Int3)tmp_end;
			var origin = (Int3)tmp_origin;

			hit = new GraphHitInfo();

			if (float.IsNaN(tmp_origin.x + tmp_origin.y + tmp_origin.z)) throw new System.ArgumentException("origin is NaN");
			if (float.IsNaN(tmp_end.x + tmp_end.y + tmp_end.z)) throw new System.ArgumentException("end is NaN");

			var node = hint as TriangleMeshNode;
			if (node == null) {
				node = (graph as NavGraph).GetNearest(tmp_origin, NNConstraint.None).node as TriangleMeshNode;

				if (node == null) {
					Debug.LogError("Could not find a valid node to start from");
					hit.point = tmp_origin;
					return true;
				}
			}

			if (origin == end) {
				hit.node = node;
				return false;
			}

			origin = (Int3)node.ClosestPointOnNode((Vector3)origin);
			hit.origin = (Vector3)origin;

			if (!node.Walkable) {
				hit.point = (Vector3)origin;
				hit.tangentOrigin = (Vector3)origin;
				return true;
			}


			List<Vector3> left = Pathfinding.Util.ListPool<Vector3>.Claim();
			List<Vector3> right = Pathfinding.Util.ListPool<Vector3>.Claim();

			int counter = 0;
			while (true) {
				counter++;
				if (counter > 2000) {
					Debug.LogError("Linecast was stuck in infinite loop. Breaking.");
					Pathfinding.Util.ListPool<Vector3>.Release(left);
					Pathfinding.Util.ListPool<Vector3>.Release(right);
					return true;
				}

				TriangleMeshNode newNode = null;

				if (trace != null) trace.Add(node);

				if (node.ContainsPoint(end)) {
					Pathfinding.Util.ListPool<Vector3>.Release(left);
					Pathfinding.Util.ListPool<Vector3>.Release(right);
					return false;
				}

				for (int i = 0; i < node.connections.Length; i++) {
					//Nodes on other graphs should not be considered
					//They might even be of other types (not MeshNode)
					if (node.connections[i].node.GraphIndex != node.GraphIndex) continue;

					left.Clear();
					right.Clear();

					if (!node.GetPortal(node.connections[i].node, left, right, false)) continue;

					Vector3 a = left[0];
					Vector3 b = right[0];

					//i.e Left or colinear
					if (!VectorMath.RightXZ(a, b, hit.origin)) {
						if (VectorMath.RightXZ(a, b, tmp_end)) {
							//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
							continue;
						}
					}

					float factor1, factor2;

					if (VectorMath.LineIntersectionFactorXZ(a, b, hit.origin, tmp_end, out factor1, out factor2)) {
						//Intersection behind the start
						if (factor2 < 0) continue;

						if (factor1 >= 0 && factor1 <= 1) {
							newNode = node.connections[i].node as TriangleMeshNode;
							break;
						}
					}
				}

				if (newNode == null) {
					//Possible edge hit
					int vs = node.GetVertexCount();

					for (int i = 0; i < vs; i++) {
						// The two vertices of edge i on the node
						var a = (Vector3)node.GetVertex(i);
						var b = (Vector3)node.GetVertex((i + 1) % vs);

						//i.e left or colinear
						if (!VectorMath.RightXZ(a, b, hit.origin)) {
							//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
							if (VectorMath.RightXZ(a, b, tmp_end)) {
								//Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
								continue;
							}
						}

						float factor1, factor2;
						if (VectorMath.LineIntersectionFactorXZ(a, b, hit.origin, tmp_end, out factor1, out factor2)) {
							if (factor2 < 0) continue;

							if (factor1 >= 0 && factor1 <= 1) {
								Vector3 intersectionPoint = a + (b-a)*factor1;
								hit.point = intersectionPoint;
								hit.node = node;
								hit.tangent = b-a;
								hit.tangentOrigin = a;

								Pathfinding.Util.ListPool<Vector3>.Release(left);
								Pathfinding.Util.ListPool<Vector3>.Release(right);
								return true;
							}
						}
					}

					//Ok, this is wrong...
					Debug.LogWarning("Linecast failing because point not inside node, and line does not hit any edges of it");

					Pathfinding.Util.ListPool<Vector3>.Release(left);
					Pathfinding.Util.ListPool<Vector3>.Release(right);
					return false;
				}

				node = newNode;
			}
		}

		public override void OnDrawGizmos (Pathfinding.Util.RetainedGizmos gizmos, bool drawNodes) {
			if (!drawNodes) {
				return;
			}

			using (var helper = gizmos.GetSingleFrameGizmoHelper(active)) {
				var bounds = new Bounds();
				bounds.SetMinMax(Vector3.zero, forcedBoundsSize);
				// Draw a write cube using the latest transform
				// (this makes the bounds update immediately if some field is changed in the editor)
				helper.builder.DrawWireCube(CalculateTransform(), bounds, Color.white);
			}

			if (tiles != null) {
				// Update navmesh vizualizations for
				// the tiles that have been changed
				for (int i = 0; i < tiles.Length; i++) {
					// Calculate a hash of the tile
					var hasher = new RetainedGizmos.Hasher(active);
					hasher.AddHash(showMeshOutline ? 1 : 0);
					hasher.AddHash(showMeshSurface ? 1 : 0);
					hasher.AddHash(showNodeConnections ? 1 : 0);

					var nodes = tiles[i].nodes;
					for (int j = 0; j < nodes.Length; j++) {
						hasher.HashNode(nodes[j]);
					}

					if (!gizmos.Draw(hasher)) {
						using (var helper = gizmos.GetGizmoHelper(active, hasher)) {
							if (showMeshSurface || showMeshOutline) CreateNavmeshSurfaceVisualization(tiles[i], helper);
							if (showMeshSurface || showMeshOutline) CreateNavmeshOutlineVisualization(tiles[i], helper);

							if (showNodeConnections) {
								for (int j = 0; j < nodes.Length; j++) {
									helper.DrawConnections(nodes[j]);
								}
							}
						}
					}

					gizmos.Draw(hasher);
				}
			}

			if (active.showUnwalkableNodes) DrawUnwalkableNodes(active.unwalkableNodeDebugSize);
		}

		/** Creates a mesh of the surfaces of the navmesh for use in OnDrawGizmos in the editor */
		void CreateNavmeshSurfaceVisualization (NavmeshTile tile, GraphGizmoHelper helper) {
			// Vertex array might be a bit larger than necessary, but that's ok
			var vertices = ArrayPool<Vector3>.Claim(tile.nodes.Length*3);
			var colors = ArrayPool<Color>.Claim(tile.nodes.Length*3);

			for (int j = 0; j < tile.nodes.Length; j++) {
				var node = tile.nodes[j];
				Int3 v0, v1, v2;
				node.GetVertices(out v0, out v1, out v2);
				vertices[j*3 + 0] = (Vector3)v0;
				vertices[j*3 + 1] = (Vector3)v1;
				vertices[j*3 + 2] = (Vector3)v2;

				var color = helper.NodeColor(node);
				colors[j*3 + 0] = colors[j*3 + 1] = colors[j*3 + 2] = color;
			}

			if (showMeshSurface) helper.DrawTriangles(vertices, colors, tile.nodes.Length);
			if (showMeshOutline) helper.DrawWireTriangles(vertices, colors, tile.nodes.Length);

			// Return lists to the pool
			ArrayPool<Vector3>.Release(ref vertices);
			ArrayPool<Color>.Release(ref colors);
		}

		/** Creates an outline of the navmesh for use in OnDrawGizmos in the editor */
		static void CreateNavmeshOutlineVisualization (NavmeshTile tile, GraphGizmoHelper helper) {
			var sharedEdges = new bool[3];

			for (int j = 0; j < tile.nodes.Length; j++) {
				sharedEdges[0] = sharedEdges[1] = sharedEdges[2] = false;

				var node = tile.nodes[j];
				for (int c = 0; c < node.connections.Length; c++) {
					var other = node.connections[c].node as TriangleMeshNode;

					// Loop through neighbours to figure out which edges are shared
					if (other != null && other.GraphIndex == node.GraphIndex) {
						for (int v = 0; v < 3; v++) {
							for (int v2 = 0; v2 < 3; v2++) {
								if (node.GetVertexIndex(v) == other.GetVertexIndex((v2+1)%3) && node.GetVertexIndex((v+1)%3) == other.GetVertexIndex(v2)) {
									// Found a shared edge with the other node
									sharedEdges[v] = true;
									v = 3;
									break;
								}
							}
						}
					}
				}

				var color = helper.NodeColor(node);
				for (int v = 0; v < 3; v++) {
					if (!sharedEdges[v]) {
						helper.builder.DrawLine((Vector3)node.GetVertex(v), (Vector3)node.GetVertex((v+1)%3), color);
					}
				}
			}
		}

		/** Serializes Node Info.
		 * Should serialize:
		 * - Base
		 *    - Node Flags
		 *    - Node Penalties
		 *    - Node
		 * - Node Positions (if applicable)
		 * - Any other information necessary to load the graph in-game
		 * All settings marked with json attributes (e.g JsonMember) have already been
		 * saved as graph settings and do not need to be handled here.
		 *
		 * It is not necessary for this implementation to be forward or backwards compatible.
		 *
		 * \see
		 */
		public override void SerializeExtraInfo (GraphSerializationContext ctx) {
			BinaryWriter writer = ctx.writer;

			if (tiles == null) {
				writer.Write(-1);
				return;
			}
			writer.Write(tileXCount);
			writer.Write(tileZCount);

			for (int z = 0; z < tileZCount; z++) {
				for (int x = 0; x < tileXCount; x++) {
					NavmeshTile tile = tiles[x + z*tileXCount];

					if (tile == null) {
						throw new System.Exception("NULL Tile");
						//writer.Write (-1);
						//continue;
					}

					writer.Write(tile.x);
					writer.Write(tile.z);

					if (tile.x != x || tile.z != z) continue;

					writer.Write(tile.w);
					writer.Write(tile.d);

					writer.Write(tile.tris.Length);

					for (int i = 0; i < tile.tris.Length; i++) writer.Write(tile.tris[i]);

					writer.Write(tile.verts.Length);
					for (int i = 0; i < tile.verts.Length; i++) {
						ctx.SerializeInt3(tile.verts[i]);
					}

					writer.Write(tile.vertsInGraphSpace.Length);
					for (int i = 0; i < tile.vertsInGraphSpace.Length; i++) {
						ctx.SerializeInt3(tile.vertsInGraphSpace[i]);
					}

					writer.Write(tile.nodes.Length);
					for (int i = 0; i < tile.nodes.Length; i++) {
						tile.nodes[i].SerializeNode(ctx);
					}
				}
			}
		}

		public override void DeserializeExtraInfo (GraphSerializationContext ctx) {
			BinaryReader reader = ctx.reader;

			tileXCount = reader.ReadInt32();

			if (tileXCount < 0) return;

			tileZCount = reader.ReadInt32();
			transform = CalculateTransform();

			tiles = new NavmeshTile[tileXCount * tileZCount];

			//Make sure mesh nodes can reference this graph
			TriangleMeshNode.SetNavmeshHolder((int)ctx.graphIndex, this);

			for (int z = 0; z < tileZCount; z++) {
				for (int x = 0; x < tileXCount; x++) {
					int tileIndex = x + z*tileXCount;
					int tx = reader.ReadInt32();
					if (tx < 0) throw new System.Exception("Invalid tile coordinates (x < 0)");

					int tz = reader.ReadInt32();
					if (tz < 0) throw new System.Exception("Invalid tile coordinates (z < 0)");

					// This is not the origin of a large tile. Refer back to that tile.
					if (tx != x || tz != z) {
						tiles[tileIndex] = tiles[tz*tileXCount + tx];
						continue;
					}

					var tile = new NavmeshTile();

					tile.x = tx;
					tile.z = tz;
					tile.w = reader.ReadInt32();
					tile.d = reader.ReadInt32();
					tile.bbTree = ObjectPool<BBTree>.Claim();

					tiles[tileIndex] = tile;

					int trisCount = reader.ReadInt32();

					if (trisCount % 3 != 0) throw new System.Exception("Corrupt data. Triangle indices count must be divisable by 3. Got " + trisCount);

					tile.tris = new int[trisCount];
					for (int i = 0; i < tile.tris.Length; i++) tile.tris[i] = reader.ReadInt32();

					tile.verts = new Int3[reader.ReadInt32()];
					for (int i = 0; i < tile.verts.Length; i++) {
						tile.verts[i] = ctx.DeserializeInt3();
					}

					if (ctx.meta.version.Major >= 4) {
						tile.vertsInGraphSpace = new Int3[reader.ReadInt32()];
						if (tile.vertsInGraphSpace.Length != tile.verts.Length) throw new System.Exception("Corrupt data. Array lengths did not match");
						for (int i = 0; i < tile.verts.Length; i++) {
							tile.vertsInGraphSpace[i] = ctx.DeserializeInt3();
						}
					} else {
						// Compatibility
						tile.vertsInGraphSpace = new Int3[tile.verts.Length];
						tile.verts.CopyTo(tile.vertsInGraphSpace, 0);
						transform.InverseTransform(tile.vertsInGraphSpace);
					}

					int nodeCount = reader.ReadInt32();
					tile.nodes = new TriangleMeshNode[nodeCount];

					//Prepare for storing in vertex indices
					tileIndex <<= TileIndexOffset;

					for (int i = 0; i < tile.nodes.Length; i++) {
						var node = new TriangleMeshNode(active);
						tile.nodes[i] = node;

						node.DeserializeNode(ctx);

						node.v0 = tile.tris[i*3+0] | tileIndex;
						node.v1 = tile.tris[i*3+1] | tileIndex;
						node.v2 = tile.tris[i*3+2] | tileIndex;
						node.UpdatePositionFromVertices();
					}

					tile.bbTree.RebuildFrom(tile.nodes);
				}
			}
		}

		public override void PostDeserialization () {
			// Make sure that the transform is up to date.
			// It is assumed that the current graph settings correspond to the correct
			// transform as it is not serialized itself.
			transform = CalculateTransform();
		}
	}
}
