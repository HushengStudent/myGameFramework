using UnityEngine;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding.RVO {
	using Pathfinding.Util;

	/** Adds a navmesh as RVO obstacles.
	 * Add this to a scene in which has a navmesh or grid based graph, when scanning (or loading from cache) the graph
	 * it will be added as RVO obstacles to the RVOSimulator (which must exist in the scene).
	 *
	 * \warning You should only have a single instance of this script in the scene, otherwise it will add duplicate
	 * obstacles and thereby increasing the CPU usage.
	 *
	 * If you update a graph during runtime the obstacles need to be recalculated which has a performance penalty.
	 * This can be quite significant for larger graphs.
	 *
	 * \astarpro
	 */
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Navmesh")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_r_v_o_1_1_r_v_o_navmesh.php")]
	public class RVONavmesh : GraphModifier {
		/** Height of the walls added for each obstacle edge.
		 * If a graph contains overlapping regions (e.g multiple floor in a building)
		 * you should set this low enough so that edges on different levels do not interfere,
		 * but high enough so that agents cannot move over them by mistake.
		 */
		public float wallHeight = 5;

		/** Obstacles currently added to the simulator */
		readonly List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

		/** Last simulator used */
		Simulator lastSim;

		public override void OnPostCacheLoad () {
			OnLatePostScan();
		}

		public override void OnGraphsPostUpdate () {
			OnLatePostScan();
		}

		public override void OnLatePostScan () {
			if (!Application.isPlaying) return;

			Profiler.BeginSample("Update RVO Obstacles From Graphs");
			RemoveObstacles();
			NavGraph[] graphs = AstarPath.active.graphs;
			RVOSimulator rvosim = RVOSimulator.active;
			if (rvosim == null) throw new System.NullReferenceException("No RVOSimulator could be found in the scene. Please add one to any GameObject");

			// Remember which simulator these obstacles were added to
			lastSim = rvosim.GetSimulator();

			for (int i = 0; i < graphs.Length; i++) {
				RecastGraph recast = graphs[i] as RecastGraph;
				INavmesh navmesh = graphs[i] as INavmesh;
				GridGraph grid = graphs[i] as GridGraph;
				if (recast != null) {
					foreach (var tile in recast.GetTiles()) {
						AddGraphObstacles(lastSim, tile);
					}
				} else if (navmesh != null) {
					AddGraphObstacles(lastSim, navmesh);
				} else if (grid != null) {
					AddGraphObstacles(lastSim, grid);
				}
			}
			Profiler.EndSample();
		}

		/** Removes all obstacles which have been added by this component */
		public void RemoveObstacles () {
			if (lastSim != null) {
				for (int i = 0; i < obstacles.Count; i++) lastSim.RemoveObstacle(obstacles[i]);
				lastSim = null;
			}

			obstacles.Clear();
		}

		void AddGraphObstacles (Pathfinding.RVO.Simulator sim, GridGraph grid) {
			FindAllContours(grid, (vertices, cycle) => obstacles.Add(sim.AddObstacle(vertices, wallHeight, true)));
		}

		/** Finds all contours of a collection of nodes in a grid graph.
		 *
		 * In the image below you can see the contour of a graph.
		 * \shadowimage{grid_contour.png}
		 *
		 * In the image below you can see the contour of just a part of a grid graph (when the \a nodes parameter is supplied)
		 * \shadowimage{grid_contour_partial.png}
		 *
		 * Contour of a hexagon graph
		 * \shadowimage{grid_contour_hexagon.png}
		 *
		 * \param grid The grid to find the contours of
		 * \param callback The callback will be called once for every contour that is found, the first parameter is the vertices
		 *      and the second parameter indicates if that contour is a cycle or if it is just a chain (chains can only occur when you explicitly specify some nodes to search
		 * \param nodes Only these nodes will be searched. If this parameter is null then all nodes in the grid graph will be searched.
		 *
		 *
		 * \code
		 * var grid = AstarPath.data.gridGraph;
		 * // Find all contours in the graph and draw them using debug lines
		 * FindAllContours(grid, (vertices, cycle) => {
		 *     int end = cycle ? vertices.Length : vertices.Length - 1;
		 *     for (int i = 0; i < end; i++) {
		 *         Debug.DrawLine(vertices[i], vertices[(i+1)%vertices.Length], Color.red, 4);
		 *     }
		 * });
		 * \endcode
		 */
		static void FindAllContours (GridGraph grid, System.Action<Vector3[], bool> callback, GridNodeBase[] nodes = null) {
			// Use all nodes if the nodes parameter is null
			if (grid is LayerGridGraph) nodes = nodes ?? (grid as LayerGridGraph).nodes;
			nodes = nodes ?? grid.nodes;

			int[] neighbourXOffsets = grid.neighbourXOffsets;
			int[] neighbourZOffsets = grid.neighbourZOffsets;
			var neighbourIndices = grid.neighbours == NumNeighbours.Six ? GridGraph.hexagonNeighbourIndices : new [] { 0, 1, 2, 3 };
			var offsetMultiplier = grid.neighbours == NumNeighbours.Six ? 1/3f : 0.5f;

			if (nodes != null) {
				var seenVertices = new Dictionary<Int3, int>();
				// A dictionary which has all items (a,b) such that there is a contour edge going from vertex a to vertex b
				// The indices correspond to an index in the vertices list
				var outline = new Dictionary<int, int>();
				// Contains all elements x such that the outline dictionary has an item (_,x)
				// This is all vertices that has some other vertex pointing to it
				var hasInEdge = new HashSet<int>();
				var vertices = ListPool<Vector3>.Claim();

				for (int i = 0; i < nodes.Length; i++) {
					var node = nodes[i];
					// The third check is a fast check for if the node has connections in all grid directions, if it has that we can skip processing it
					if (node != null && node.Walkable && !node.HasConnectionsToAllEightNeighbours) {
						for (int d = 0; d < neighbourIndices.Length; d++) {
							var d1 = neighbourIndices[d];
							// Check if there is an obstacle in that direction
							if (node.GetNeighbourAlongDirection(d1) == null) {
								var d0 = neighbourIndices[(d - 1 + neighbourIndices.Length) % neighbourIndices.Length];
								var d2 = neighbourIndices[(d + 1) % neighbourIndices.Length];

								// Position in graph space of the vertex
								Vector3 graphSpacePos = new Vector3(node.XCoordinateInGrid + 0.5f, 0, node.ZCoordinateInGrid + 0.5f);
								graphSpacePos.x += neighbourXOffsets[d1] * offsetMultiplier;
								graphSpacePos.z += neighbourZOffsets[d1] * offsetMultiplier;
								graphSpacePos.y = grid.transform.InverseTransform((Vector3)node.position).y;

								// Offset along diagonal to get the correct XZ coordinates
								Vector3 corner1, corner2;
								corner1 = corner2 = graphSpacePos;
								corner1.x += neighbourXOffsets[d0] * offsetMultiplier;
								corner1.z += neighbourZOffsets[d0] * offsetMultiplier;
								corner2.x += neighbourXOffsets[d2] * offsetMultiplier;
								corner2.z += neighbourZOffsets[d2] * offsetMultiplier;

								// Quantize vertices so that we can check for duplicates easier
								Int3 c1 = (Int3)corner1;
								Int3 c2 = (Int3)corner2;

								int i1, i2;
								if (seenVertices.TryGetValue(c1, out i1)) {
									// We only keep a vertex in the seenVertices dictionary until it has been used twice
									// This prevents more than 2 edges from enter or leaving a single vertex which would confuse
									// the algorithm that is used for following the contour.
									//
									// This can happen when in situations like this
									//   ____
									//  |    |
									//  |____|____
									//       |    |
									//       |____|
									//
									// I.e when a corner is shared by multiple contours
									// With this fix we will just duplicate those vertices when they are encountered
									seenVertices.Remove(c1);
								} else {
									i1 = seenVertices[c1] = vertices.Count;
									vertices.Add(corner1);
								}

								if (seenVertices.TryGetValue(c2, out i2)) {
									seenVertices.Remove(c2);
								} else {
									i2 = seenVertices[c2] = vertices.Count;
									vertices.Add(corner2);
								}

								outline.Add(i1, i2);
								hasInEdge.Add(i2);
							}
						}
					}
				}

				// Follow the pointers that we constructed above to trace out the contours
				var transform = grid.transform;
				var vertexBuffer = ListPool<Vector3>.Claim();
				CompressContour(outline, hasInEdge, (chain, cycle) => {
					vertexBuffer.Clear();
					var v0 = vertices[chain[0]];
					vertexBuffer.Add(v0);

					// Add all other points in the chain but compress lines to just 2 points
					for (int i = 1; i < chain.Count - 1; i++) {
						var v1 = vertices[chain[i]];
						var v1d = v1 - v0;
						var v2d = vertices[chain[i+1]] - v0;
					    // Skip points if they are colinear with the point just before it and just after it, because that point wouldn't add much information, but it would add CPU overhead
						if (((Mathf.Abs(v1d.x) > 0.01f || Mathf.Abs(v2d.x) > 0.01f) && (Mathf.Abs(v1d.z) > 0.01f || Mathf.Abs(v2d.z) > 0.01f)) || (Mathf.Abs(v1d.y) > 0.01f || Mathf.Abs(v2d.y) > 0.01f)) {
							vertexBuffer.Add(v1);
						}
						v0 = v1;
					}
					vertexBuffer.Add(vertices[chain[chain.Count - 1]]);
					var result = vertexBuffer.ToArray();
					// Convert to world space
					transform.Transform(result);
					callback(result, cycle);
				});

				ListPool<Vector3>.Release(vertexBuffer);
				ListPool<Vector3>.Release(vertices);
			}
		}

		/** Adds obstacles for a graph */
		void AddGraphObstacles (Pathfinding.RVO.Simulator sim, INavmesh ng) {
			// Assume 3 vertices per node
			var uses = new int[3];

			var outline = new Dictionary<int, int>();
			var vertexPositions = new Dictionary<int, Int3>();
			var hasInEdge = new HashSet<int>();

			ng.GetNodes(_node => {
				var node = _node as TriangleMeshNode;

				uses[0] = uses[1] = uses[2] = 0;

				if (node != null) {
				    // Find out which edges are shared with other nodes
					for (int j = 0; j < node.connections.Length; j++) {
						var other = node.connections[j].node as TriangleMeshNode;

				        // Not necessarily a TriangleMeshNode
						if (other != null) {
							int a = node.SharedEdge(other);
							if (a != -1) uses[a] = 1;
						}
					}

				    // Loop through all edges on the node
					for (int j = 0; j < 3; j++) {
				        // The edge is not shared with any other node
				        // I.e it is an exterior edge on the mesh
						if (uses[j] == 0) {
							var i1 = j;
							var i2 = (j+1) % node.GetVertexCount();

							outline[node.GetVertexIndex(i1)] = node.GetVertexIndex(i2);
							hasInEdge.Add(node.GetVertexIndex(i2));
							vertexPositions[node.GetVertexIndex(i1)] = node.GetVertex(i1);
							vertexPositions[node.GetVertexIndex(i2)] = node.GetVertex(i2);
						}
					}
				}
			});

			List<Vector3> vertices = ListPool<Vector3>.Claim();
			CompressContour(outline, hasInEdge, (chain, cycle) => {
				for (int i = 0; i < chain.Count; i++) vertices.Add((Vector3)vertexPositions[chain[i]]);
				obstacles.Add(sim.AddObstacle(vertices.ToArray(), wallHeight, cycle));
				vertices.Clear();
			});
			ListPool<Vector3>.Release(vertices);
		}

		static void CompressContour (Dictionary<int, int> outline, HashSet<int> hasInEdge, System.Action<List<int>, bool> results) {
			// Iterate through chains of the navmesh outline.
			// I.e segments of the outline that are not loops
			// we need to start these at the beginning of the chain.
			// Then iterate over all the loops of the outline.
			// Since they are loops, we can start at any point.
			var obstacleVertices = ListPool<int>.Claim();
			var outlineKeys = ListPool<int>.Claim();

			outlineKeys.AddRange(outline.Keys);
			for (int k = 0; k <= 1; k++) {
				bool cycles = k == 1;
				for (int i = 0; i < outlineKeys.Count; i++) {
					var startIndex = outlineKeys[i];

					// Chains (not cycles) need to start at the start of the chain
					// Cycles can start at any point
					if (!cycles && hasInEdge.Contains(startIndex)) {
						continue;
					}

					var index = startIndex;
					obstacleVertices.Clear();
					obstacleVertices.Add(index);

					while (outline.ContainsKey(index)) {
						var next = outline[index];
						outline.Remove(index);

						obstacleVertices.Add(next);

						// We traversed a full cycle
						if (next == startIndex) break;

						index = next;
					}

					if (obstacleVertices.Count > 1) {
						results(obstacleVertices, cycles);
					}
				}
			}

			ListPool<int>.Release(outlineKeys);
			ListPool<int>.Release(obstacleVertices);
		}
	}
}
