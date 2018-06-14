using Math = System.Math;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding {
	using Pathfinding.Voxels;
	using Pathfinding.Serialization;
	using Pathfinding.Recast;
	using Pathfinding.Util;
	using System.Threading;

	/** Automatically generates navmesh graphs based on world geometry.
	 * The recast graph is based on Recast (http://code.google.com/p/recastnavigation/).\n
	 * I have translated a good portion of it to C# to run it natively in Unity. The Recast process is described as follows:
	 * - The voxel mold is build from the input triangle mesh by rasterizing the triangles into a multi-layer heightfield.
	 * Some simple filters are then applied to the mold to prune out locations where the character would not be able to move.
	 * - The walkable areas described by the mold are divided into simple overlayed 2D regions.
	 * The resulting regions have only one non-overlapping contour, which simplifies the final step of the process tremendously.
	 * - The navigation polygons are peeled off from the regions by first tracing the boundaries and then simplifying them.
	 * The resulting polygons are finally converted to convex polygons which makes them perfect for pathfinding and spatial reasoning about the level.
	 *
	 * It works exactly like that in the C# version as well, except that everything is triangulated to triangles instead of n-gons.
	 * The recast generation process usually works directly on the visiable geometry in the world, this is usually a good thing, because world geometry is usually more detailed than the colliders.
	 * You can however specify that colliders should be rasterized, if you have very detailed world geometry, this can speed up the scan.
	 *
	 * Check out the second part of the Get Started Tutorial which discusses recast graphs.
	 *
	 * \section export Exporting for manual editing
	 * In the editor there is a button for exporting the generated graph to a .obj file.
	 * Usually the generation process is good enough for the game directly, but in some cases you might want to edit some minor details.
	 * So you can export the graph to a .obj file, open it in your favourite 3D application, edit it, and export it to a mesh which Unity can import.
	 * You can then use that mesh in a navmesh graph.
	 *
	 * Since many 3D modelling programs use different axis systems (unity uses X=right, Y=up, Z=forward), it can be a bit tricky to get the rotation and scaling right.
	 * For blender for example, what you have to do is to first import the mesh using the .obj importer. Don't change anything related to axes in the settings.
	 * Then select the mesh, open the transform tab (usually the thin toolbar to the right of the 3D view) and set Scale -> Z to -1.
	 * If you transform it using the S (scale) hotkey, it seems to set both Z and Y to -1 for some reason.
	 * Then make the edits you need and export it as an .obj file to somewhere in the Unity project.
	 * But this time, edit the setting named "Forward" to "Z forward" (not -Z as it is per default).
	 *
	 * \shadowimage{recastgraph_graph.png}
	 * \shadowimage{recastgraph_inspector.png}
	 *
	 *
	 * \ingroup graphs
	 *
	 * \astarpro
	 */
	[JsonOptIn]
	public class RecastGraph : NavmeshBase, IUpdatableGraph {
		[JsonMember]
		/** Radius of the agent which will traverse the navmesh.
		 * The navmesh will be eroded with this radius.
		 * \shadowimage{recast/character_radius.gif}
		 */
		public float characterRadius = 1.5F;

		/** Max distance from simplified edge to real edge.
		 * \shadowimage{recast/max_edge_error.gif}
		 */
		[JsonMember]
		public float contourMaxError = 2F;

		/** Voxel sample size (x,z).
		 * Lower values will yield higher quality navmeshes, however the graph will be slower to scan.
		 *
		 * \shadowimage{recast/cell_size.gif}
		 */
		[JsonMember]
		public float cellSize = 0.5F;

		/** Character height.
		 * \shadowimage{recast/walkable_height.gif}
		 */
		[JsonMember]
		public float walkableHeight = 2F;

		/** Height the character can climb.
		 * \shadowimage{recast/walkable_climb.gif}
		 */
		[JsonMember]
		public float walkableClimb = 0.5F;

		/** Max slope in degrees the character can traverse.
		 * \shadowimage{recast/max_slope.gif}
		 */
		[JsonMember]
		public float maxSlope = 30;

		/** Longer edges will be subdivided.
		 * Reducing this value can improve path quality since similarly sized polygons
		 * yield better paths than really large and really small next to each other
		 */
		[JsonMember]
		public float maxEdgeLength = 20;

		/** Minumum region size.
		 * Small regions will be removed from the navmesh.
		 * Measured in square world units (square meters in most games).
		 *
		 * \shadowimage{recast/min_region_size.gif}
		 *
		 * If a region is adjacent to a tile border, it will not be removed
		 * even though it is small since the adjacent tile might join it
		 * to form a larger region.
		 *
		 * \shadowimage{recast_minRegionSize_1.png}
		 * \shadowimage{recast_minRegionSize_2.png}
		 */
		[JsonMember]
		public float minRegionSize = 3;

		/** Size in voxels of a single tile.
		 * This is the width of the tile.
		 *
		 * A large tile size can be faster to initially scan (but beware of out of memory issues if you try with a too large tile size in a large world)
		 * smaller tile sizes are (much) faster to update.
		 *
		 * Different tile sizes can affect the quality of paths. It is often good to split up huge open areas into several tiles for
		 * better quality paths, but too small tiles can lead to effects looking like invisible obstacles.
		 */
		[JsonMember]
		public int editorTileSize = 128;

		/** Size of a tile along the X axis in voxels.
		 * \warning Do not modify, it is set from #editorTileSize at Scan
		 */
		[JsonMember]
		public int tileSizeX = 128;

		/** Size of a tile along the Z axis in voxels.
		 * \warning Do not modify, it is set from #editorTileSize at Scan
		 */
		[JsonMember]
		public int tileSizeZ = 128;


		/** If true, divide the graph into tiles, otherwise use a single tile covering the whole graph */
		[JsonMember]
		public bool useTiles;

		/** If true, scanning the graph will yield a completely empty graph.
		 * Useful if you want to replace the graph with a custom navmesh for example
		 */
		public bool scanEmptyGraph;

		public enum RelevantGraphSurfaceMode {
			DoNotRequire,
			OnlyForCompletelyInsideTile,
			RequireForAll
		}

		/** Require every region to have a RelevantGraphSurface component inside it.
		 * A RelevantGraphSurface component placed in the scene specifies that
		 * the navmesh region it is inside should be included in the navmesh.
		 *
		 * If this is set to OnlyForCompletelyInsideTile
		 * a navmesh region is included in the navmesh if it
		 * has a RelevantGraphSurface inside it, or if it
		 * is adjacent to a tile border. This can leave some small regions
		 * which you didn't want to have included because they are adjacent
		 * to tile borders, but it removes the need to place a component
		 * in every single tile, which can be tedious (see below).
		 *
		 * If this is set to RequireForAll
		 * a navmesh region is included only if it has a RelevantGraphSurface
		 * inside it. Note that even though the navmesh
		 * looks continous between tiles, the tiles are computed individually
		 * and therefore you need a RelevantGraphSurface component for each
		 * region and for each tile.
		 *
		 *
		 *
		 * \shadowimage{relevantgraphsurface/dontreq.png}
		 * In the above image, the mode OnlyForCompletelyInsideTile was used. Tile borders
		 * are highlighted in black. Note that since all regions are adjacent to a tile border,
		 * this mode didn't remove anything in this case and would give the same result as DoNotRequire.
		 * The RelevantGraphSurface component is shown using the green gizmo in the top-right of the blue plane.
		 *
		 * \shadowimage{relevantgraphsurface/no_tiles.png}
		 * In the above image, the mode RequireForAll was used. No tiles were used.
		 * Note that the small region at the top of the orange cube is now gone, since it was not the in the same
		 * region as the relevant graph surface component.
		 * The result would have been identical with OnlyForCompletelyInsideTile since there are no tiles (or a single tile, depending on how you look at it).
		 *
		 * \shadowimage{relevantgraphsurface/req_all.png}
		 * The mode RequireForAll was used here. Since there is only a single RelevantGraphSurface component, only the region
		 * it was in, in the tile it is placed in, will be enabled. If there would have been several RelevantGraphSurface in other tiles,
		 * those regions could have been enabled as well.
		 *
		 * \shadowimage{relevantgraphsurface/tiled_uneven.png}
		 * Here another tile size was used along with the OnlyForCompletelyInsideTile.
		 * Note that the region on top of the orange cube is gone now since the region borders do not intersect that region (and there is no
		 * RelevantGraphSurface component inside it).
		 *
		 * \note When not using tiles. OnlyForCompletelyInsideTile is equivalent to RequireForAll.
		 */
		[JsonMember]
		public RelevantGraphSurfaceMode relevantGraphSurfaceMode = RelevantGraphSurfaceMode.DoNotRequire;

		[JsonMember]
		/** Use colliders to calculate the navmesh */
		public bool rasterizeColliders;

		[JsonMember]
		/** Use scene meshes to calculate the navmesh */
		public bool rasterizeMeshes = true;

		/** Include the Terrain in the scene. */
		[JsonMember]
		public bool rasterizeTerrain = true;

		/** Rasterize tree colliders on terrains.
		 *
		 * If the tree prefab has a collider, that collider will be rasterized.
		 * Otherwise a simple box collider will be used and the script will
		 * try to adjust it to the tree's scale, it might not do a very good job though so
		 * an attached collider is preferable.
		 *
		 * \see rasterizeTerrain
		 * \see colliderRasterizeDetail
		 */
		[JsonMember]
		public bool rasterizeTrees = true;

		/** Controls detail on rasterization of sphere and capsule colliders.
		 * This controls the number of rows and columns on the generated meshes.
		 * A higher value does not necessarily increase quality of the mesh, but a lower
		 * value will often speed it up.
		 *
		 * You should try to keep this value as low as possible without affecting the mesh quality since
		 * that will yield the fastest scan times.
		 *
		 * \see rasterizeColliders
		 */
		[JsonMember]
		public float colliderRasterizeDetail = 10;

		/** Layer mask which filters which objects to include.
		 * \see tagMask
		 */
		[JsonMember]
		public LayerMask mask = -1;

		/** Objects tagged with any of these tags will be rasterized.
		 * Note that this extends the layer mask, so if you only want to use tags, set #mask to 'Nothing'.
		 *
		 * \see mask
		 */
		[JsonMember]
		public List<string> tagMask = new List<string>();

		/** Controls how large the sample size for the terrain is.
		 * A higher value is faster to scan but less accurate
		 */
		[JsonMember]
		public int terrainSampleSize = 3;

		/** Rotation of the graph in degrees */
		[JsonMember]
		public Vector3 rotation;

		/** Center of the bounding box.
		 * Scanning will only be done inside the bounding box */
		[JsonMember]
		public Vector3 forcedBoundsCenter;

		private Voxelize globalVox;

		public const int BorderVertexMask = 1;
		public const int BorderVertexOffset = 31;

		/** List of tiles that have been calculated in a graph update, but have not yet been added to the graph.
		 * When updating the graph in a separate thread, large changes cannot be made directly to the graph
		 * as other scripts might use the graph data structures at the same time in another thread.
		 * So the tiles are calculated, but they are not yet connected to the existing tiles
		 * that will be done in UpdateAreaPost which runs in the Unity thread.
		 */
		List<NavmeshTile> stagingTiles = new List<NavmeshTile>();

		public override float TileWorldSizeX {
			get {
				return tileSizeX*cellSize;
			}
		}

		public override float TileWorldSizeZ {
			get {
				return tileSizeZ*cellSize;
			}
		}

		protected override float MaxTileConnectionEdgeDistance {
			get {
				return walkableClimb;
			}
		}

		/** World bounds for the graph.
		 * Defined as a bounds object with size #forcedBoundsSize and centered at #forcedBoundsCenter
		 * \deprecated Obsolete since this is not accurate when the graph is rotated (rotation was not supported when this property was created)
		 */
		[System.Obsolete("Obsolete since this is not accurate when the graph is rotated (rotation was not supported when this property was created)")]
		public Bounds forcedBounds {
			get {
				return new Bounds(forcedBoundsCenter, forcedBoundsSize);
			}
		}

		/** Returns the closest point of the node.
		 * \deprecated Use #Pathfinding.TriangleMeshNode.ClosestPointOnNode instead
		 */
		[System.Obsolete("Use node.ClosestPointOnNode instead")]
		public Vector3 ClosestPointOnNode (TriangleMeshNode node, Vector3 pos) {
			return node.ClosestPointOnNode(pos);
		}

		/** Returns if the point is inside the node in XZ space.
		 * \deprecated Use #Pathfinding.TriangleMeshNode.ContainsPoint instead
		 */
		[System.Obsolete("Use node.ContainsPoint instead")]
		public bool ContainsPoint (TriangleMeshNode node, Vector3 pos) {
			return node.ContainsPoint((Int3)pos);
		}

		/** Changes the bounds of the graph to precisely encapsulate all objects in the scene that can be included in the scanning process based on the settings.
		 * Which objects are used depends on the settings. If an object would have affected the graph with the current settings if it would have
		 * been inside the bounds of the graph, it will be detected and the bounds will be expanded to contain that object.
		 *
		 * This method corresponds to the 'Snap bounds to scene' button in the inspector.
		 *
		 * \see rasterizeMeshes
		 * \see rasterizeTerrain
		 * \see rasterizeColliders
		 * \see mask
		 * \see tagMask
		 *
		 * \see forcedBoundsCenter
		 * \see forcedBoundsSize
		 */
		public void SnapForceBoundsToScene () {
			var meshes = CollectMeshes(new Bounds(Vector3.zero, new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)));

			if (meshes.Count == 0) {
				return;
			}

			var bounds = meshes[0].bounds;

			for (int i = 1; i < meshes.Count; i++) {
				bounds.Encapsulate(meshes[i].bounds);
				meshes[i].Pool();
			}

			forcedBoundsCenter = bounds.center;
			forcedBoundsSize = bounds.size;
		}

		GraphUpdateThreading IUpdatableGraph.CanUpdateAsync (GraphUpdateObject o) {
			return o.updatePhysics ? GraphUpdateThreading.UnityInit | GraphUpdateThreading.SeparateThread | GraphUpdateThreading.UnityPost : GraphUpdateThreading.SeparateThread;
		}

		void IUpdatableGraph.UpdateAreaInit (GraphUpdateObject o) {
			if (!o.updatePhysics) {
				return;
			}

			AstarProfiler.Reset();
			AstarProfiler.StartProfile("UpdateAreaInit");
			AstarProfiler.StartProfile("CollectMeshes");

			RelevantGraphSurface.UpdateAllPositions();

			//Calculate world bounds of all affected tiles
			IntRect touchingTiles = GetTouchingTiles(o.bounds);
			Bounds tileBounds = GetTileBounds(touchingTiles);

			// Expand TileBorderSizeInWorldUnits voxels on each side
			tileBounds.Expand(new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2);

			var meshes = CollectMeshes(tileBounds);

			if (globalVox == null) {
				// Create the voxelizer and set all settings
				globalVox = new Voxelize(CellHeight, cellSize, walkableClimb, walkableHeight, maxSlope, maxEdgeLength);
			}

			globalVox.inputMeshes = meshes;

			AstarProfiler.EndProfile("CollectMeshes");
			AstarProfiler.EndProfile("UpdateAreaInit");
		}

		void IUpdatableGraph.UpdateArea (GraphUpdateObject guo) {
			// Figure out which tiles are affected
			var affectedTiles = GetTouchingTiles(guo.bounds);

			if (!guo.updatePhysics) {
				for (int z = affectedTiles.ymin; z <= affectedTiles.ymax; z++) {
					for (int x = affectedTiles.xmin; x <= affectedTiles.xmax; x++) {
						NavmeshTile tile = tiles[z*tileXCount + x];
						NavMeshGraph.UpdateArea(guo, tile);
					}
				}
				return;
			}

			Voxelize vox = globalVox;

			if (vox == null) {
				throw new System.InvalidOperationException("No Voxelizer object. UpdateAreaInit should have been called before this function.");
			}

			AstarProfiler.StartProfile("Build Tiles");

			// Build the new tiles
			for (int x = affectedTiles.xmin; x <= affectedTiles.xmax; x++) {
				for (int z = affectedTiles.ymin; z <= affectedTiles.ymax; z++) {
					stagingTiles.Add(BuildTileMesh(vox, x, z));
				}
			}

			uint graphIndex = (uint)AstarPath.active.data.GetGraphIndex(this);

			// Set the correct graph index
			for (int i = 0; i < stagingTiles.Count; i++) {
				NavmeshTile tile = stagingTiles[i];
				GraphNode[] nodes = tile.nodes;

				for (int j = 0; j < nodes.Length; j++) nodes[j].GraphIndex = graphIndex;
			}

			for (int i = 0; i < vox.inputMeshes.Count; i++) vox.inputMeshes[i].Pool();
			ListPool<RasterizationMesh>.Release(vox.inputMeshes);
			vox.inputMeshes = null;
			AstarProfiler.EndProfile("Build Tiles");
		}

		/** Called on the Unity thread to complete a graph update */
		void IUpdatableGraph.UpdateAreaPost (GraphUpdateObject guo) {
			Profiler.BeginSample("RemoveConnections");
			// Remove connections from existing tiles destroy the nodes
			// Replace the old tile by the new tile
			for (int i = 0; i < stagingTiles.Count; i++) {
				var tile = stagingTiles[i];
				int index = tile.x + tile.z * tileXCount;
				var oldTile = tiles[index];

				// Destroy the previous nodes
				for (int j = 0; j < oldTile.nodes.Length; j++) {
					oldTile.nodes[j].Destroy();
				}

				tiles[index] = tile;
			}

			Profiler.EndSample();

			Profiler.BeginSample("Connect With Neighbours");
			// Connect the new tiles with their neighbours
			for (int i = 0; i < stagingTiles.Count; i++) {
				var tile = stagingTiles[i];
				ConnectTileWithNeighbours(tile, false);
			}

			// This may be used to update the tile again to take into
			// account NavmeshCut components.
			// It is not the super efficient, but it works.
			// Usually you only use either normal graph updates OR navmesh
			// cutting, not both.
			if (OnRecalculatedTiles != null) {
				OnRecalculatedTiles(stagingTiles.ToArray());
			}

			stagingTiles.Clear();
			Profiler.EndSample();
		}

		public override IEnumerable<Progress> ScanInternal () {
			TriangleMeshNode.SetNavmeshHolder(AstarPath.active.data.GetGraphIndex(this), this);

			if (!Application.isPlaying) {
				RelevantGraphSurface.FindAllGraphSurfaces();
			}

			RelevantGraphSurface.UpdateAllPositions();


			foreach (var progress in ScanAllTiles()) {
				yield return progress;
			}


#if DEBUG_REPLAY
			DebugReplay.WriteToFile();
#endif
		}

		public override GraphTransform CalculateTransform () {
			return new GraphTransform(Matrix4x4.TRS(forcedBoundsCenter, Quaternion.Euler(rotation), Vector3.one) * Matrix4x4.TRS(-forcedBoundsSize*0.5f, Quaternion.identity, Vector3.one));
		}

		void InitializeTileInfo () {
			// Voxel grid size
			int totalVoxelWidth = (int)(forcedBoundsSize.x/cellSize + 0.5f);
			int totalVoxelDepth = (int)(forcedBoundsSize.z/cellSize + 0.5f);

			if (!useTiles) {
				tileSizeX = totalVoxelWidth;
				tileSizeZ = totalVoxelDepth;
			} else {
				tileSizeX = editorTileSize;
				tileSizeZ = editorTileSize;
			}

			// Number of tiles
			tileXCount = (totalVoxelWidth + tileSizeX-1) / tileSizeX;
			tileZCount = (totalVoxelDepth + tileSizeZ-1) / tileSizeZ;

			if (tileXCount * tileZCount > TileIndexMask+1) {
				throw new System.Exception("Too many tiles ("+(tileXCount * tileZCount)+") maximum is "+(TileIndexMask+1)+
					"\nTry disabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* inspector.");
			}

			tiles = new NavmeshTile[tileXCount*tileZCount];
		}

		void BuildTiles (Queue<Int2> tileQueue, List<RasterizationMesh>[] meshBuckets, ManualResetEvent doneEvent, int threadIndex) {
			try {
				// Create the voxelizer and set all settings
				var vox = new Voxelize(CellHeight, cellSize, walkableClimb, walkableHeight, maxSlope, maxEdgeLength);

				while (true) {
					Int2 tile;
					lock (tileQueue) {
						if (tileQueue.Count == 0) {
							return;
						}

						tile = tileQueue.Dequeue();
					}

					vox.inputMeshes = meshBuckets[tile.x + tile.y*tileXCount];
					tiles[tile.x + tile.y*tileXCount] = BuildTileMesh(vox, tile.x, tile.y, threadIndex);
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			} finally {
				if (doneEvent != null) doneEvent.Set();
			}
		}

		/** Connects all tiles in the queue with the tiles to the right (x+1) side of them or above them (z+1) depending on xDirection and zDirection */
		void ConnectTiles (Queue<Int2> tileQueue, ManualResetEvent doneEvent, bool xDirection, bool zDirection) {
			try {
				while (true) {
					Int2 tile;
					lock (tileQueue) {
						if (tileQueue.Count == 0) {
							return;
						}

						tile = tileQueue.Dequeue();
					}

					// Connect with tile at (x+1,z) and (x,z+1)
					if (xDirection && tile.x < tileXCount - 1)
						ConnectTiles(tiles[tile.x + tile.y * tileXCount], tiles[tile.x + 1 + tile.y * tileXCount]);
					if (zDirection && tile.y < tileZCount - 1)
						ConnectTiles(tiles[tile.x + tile.y * tileXCount], tiles[tile.x + (tile.y + 1) * tileXCount]);
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			} finally {
				// See BuildTiles
				if (doneEvent != null) doneEvent.Set();
			}
		}

		/** Creates a list for every tile and adds every mesh that touches a tile to the corresponding list */
		List<RasterizationMesh>[] PutMeshesIntoTileBuckets (List<RasterizationMesh> meshes) {
			var result = new List<RasterizationMesh>[tiles.Length];
			var borderExpansion = new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2;

			for (int i = 0; i < result.Length; i++) {
				result[i] = ListPool<RasterizationMesh>.Claim();
			}

			for (int i = 0; i < meshes.Count; i++) {
				var mesh = meshes[i];
				var bounds = mesh.bounds;
				// Expand borderSize voxels on each side
				bounds.Expand(borderExpansion);

				var rect = GetTouchingTiles(bounds);
				for (int z = rect.ymin; z <= rect.ymax; z++) {
					for (int x = rect.xmin; x <= rect.xmax; x++) {
						result[x + z*tileXCount].Add(mesh);
					}
				}
			}

			return result;
		}

		protected IEnumerable<Progress> ScanAllTiles () {
			transform = CalculateTransform();
			InitializeTileInfo();

			// If this is true, just fill the graph with empty tiles
			if (scanEmptyGraph) {
				FillWithEmptyTiles();
				yield break;
			}

			// A walkableClimb higher than walkableHeight can cause issues when generating the navmesh since then it can in some cases
			// Both be valid for a character to walk under an obstacle and climb up on top of it (and that cannot be handled with navmesh without links)
			// The editor scripts also enforce this but we enforce it here too just to be sure
			walkableClimb = Mathf.Min(walkableClimb, walkableHeight);

			yield return new Progress(0, "Finding Meshes");
			var bounds = transform.Transform(new Bounds(forcedBoundsSize*0.5f, forcedBoundsSize));
			var meshes = CollectMeshes(bounds);
			var buckets = PutMeshesIntoTileBuckets(meshes);

			Queue<Int2> tileQueue = new Queue<Int2>();

			// Put all tiles in the queue
			for (int z = 0; z < tileZCount; z++) {
				for (int x = 0; x < tileXCount; x++) {
					tileQueue.Enqueue(new Int2(x, z));
				}
			}

#if UNITY_WEBGL && !UNITY_EDITOR
			// WebGL does not support multithreading so we will do everything synchronously instead
			BuildTiles(tileQueue, buckets, null, 0);
#else
			// Fire up a bunch of threads to scan the graph in parallel
			int threadCount = Mathf.Min(tileQueue.Count, Mathf.Max(1, AstarPath.CalculateThreadCount(ThreadCount.AutomaticHighLoad)));
			var waitEvents = new ManualResetEvent[threadCount];

			for (int i = 0; i < waitEvents.Length; i++) {
				waitEvents[i] = new ManualResetEvent(false);
#if NETFX_CORE
				// Need to make a copy here, otherwise it may refer to some other index when the task actually runs
				var threadIndex = i;
				System.Threading.Tasks.Task.Run(() => BuildTiles(tileQueue, buckets, waitEvents[threadIndex], threadIndex));
#else
				ThreadPool.QueueUserWorkItem(state => BuildTiles(tileQueue, buckets, waitEvents[(int)state], (int)state), i);
#endif
			}

			// Prioritize responsiveness while playing
			// but when not playing prioritize throughput
			// (the Unity progress bar is also pretty slow to update)
			int timeoutMillis = Application.isPlaying ? 1 : 200;

			while (!WaitHandle.WaitAll(waitEvents, timeoutMillis)) {
				int count;
				lock (tileQueue) count = tileQueue.Count;

				yield return new Progress(Mathf.Lerp(0.1f, 0.9f, (tiles.Length - count + 1) / (float)tiles.Length), "Generating Tile " + (tiles.Length - count + 1) + "/" + tiles.Length);
			}
#endif

			yield return new Progress(0.9f, "Assigning Graph Indices");

			// Assign graph index to nodes
			uint graphIndex = (uint)AstarPath.active.data.GetGraphIndex(this);

			GetNodes(node => node.GraphIndex = graphIndex);

#if UNITY_WEBGL && !UNITY_EDITOR
			// Put all tiles in the queue to be connected
			for (int i = 0; i < tiles.Length; i++) tileQueue.Enqueue(new Int2(tiles[i].x, tiles[i].z));

			// Calculate synchronously
			ConnectTiles(tileQueue, null, true, true);
#else
			// First connect all tiles with an EVEN coordinate sum
			// This would be the white squares on a chess board.
			// Then connect all tiles with an ODD coordinate sum (which would be all black squares).
			// This will prevent the different threads that do all
			// this in parallel from conflicting with each other.
			// The directions are also done separately
			// first they are connected along the X direction and then along the Z direction.
			// Looping over 0 and then 1
			for (int coordinateSum = 0; coordinateSum <= 1; coordinateSum++) {
				for (int direction = 0; direction <= 1; direction++) {
					for (int i = 0; i < tiles.Length; i++) {
						if ((tiles[i].x + tiles[i].z) % 2 == coordinateSum) {
							tileQueue.Enqueue(new Int2(tiles[i].x, tiles[i].z));
						}
					}

					int numTilesInQueue = tileQueue.Count;
					for (int i = 0; i < waitEvents.Length; i++) {
						waitEvents[i].Reset();
#if NETFX_CORE
						var waitEvent = waitEvents[i];
						System.Threading.Tasks.Task.Run(() => ConnectTiles(tileQueue, waitEvent, direction == 0, direction == 1));
#else
						ThreadPool.QueueUserWorkItem(state => ConnectTiles(tileQueue, state as ManualResetEvent, direction == 0, direction == 1), waitEvents[i]);
#endif
					}

					while (!WaitHandle.WaitAll(waitEvents, timeoutMillis)) {
						int count;
						lock (tileQueue) {
							count = tileQueue.Count;
						}

						yield return new Progress(0.95f, "Connecting Tile " + (numTilesInQueue - count) + "/" + numTilesInQueue + " (Phase " + (direction + 1 + 2*coordinateSum) + " of 4)");
					}
				}
			}
#endif

			for (int i = 0; i < meshes.Count; i++) meshes[i].Pool();
			ListPool<RasterizationMesh>.Release(meshes);

			// This may be used by the TileHandlerHelper script to update the tiles
			// while taking NavmeshCuts into account after the graph has been completely recalculated.
			if (OnRecalculatedTiles != null) {
				OnRecalculatedTiles(tiles.Clone() as NavmeshTile[]);
			}
		}

		List<RasterizationMesh> CollectMeshes (Bounds bounds) {
			Profiler.BeginSample("Find Meshes for rasterization");
			var result = ListPool<RasterizationMesh>.Claim();

			var meshGatherer = new RecastMeshGatherer(bounds, terrainSampleSize, mask, tagMask, colliderRasterizeDetail);

			if (rasterizeMeshes) {
				Profiler.BeginSample("Find meshes");
				meshGatherer.CollectSceneMeshes(result);
				Profiler.EndSample();
			}

			Profiler.BeginSample("Find RecastMeshObj components");
			meshGatherer.CollectRecastMeshObjs(result);
			Profiler.EndSample();

			if (rasterizeTerrain) {
				Profiler.BeginSample("Find terrains");
				// Split terrains up into meshes approximately the size of a single chunk
				var desiredTerrainChunkSize = cellSize*Math.Max(tileSizeX, tileSizeZ);
				meshGatherer.CollectTerrainMeshes(rasterizeTrees, desiredTerrainChunkSize, result);
				Profiler.EndSample();
			}

			if (rasterizeColliders) {
				Profiler.BeginSample("Find colliders");
				meshGatherer.CollectColliderMeshes(result);
				Profiler.EndSample();
			}

			if (result.Count == 0) {
				Debug.LogWarning("No MeshFilters were found contained in the layers specified by the 'mask' variables");
			}

			Profiler.EndSample();
			return result;
		}

		float CellHeight {
			get {
				// Voxel y coordinates will be stored as ushorts which have 65536 values
				// Leave a margin to make sure things do not overflow
				return Mathf.Max(forcedBoundsSize.y / 64000, 0.001f);
			}
		}

		/** Convert character radius to a number of voxels */
		int CharacterRadiusInVoxels {
			get {
				// Round it up most of the time, but round it down
				// if it is very close to the result when rounded down
				return Mathf.CeilToInt((characterRadius / cellSize) - 0.1f);
			}
		}

		/** Number of extra voxels on each side of a tile to ensure accurate navmeshes near the tile border.
		 * The width of a tile is expanded by 2 times this value (1x to the left and 1x to the right)
		 */
		int TileBorderSizeInVoxels {
			get {
				return CharacterRadiusInVoxels + 3;
			}
		}

		float TileBorderSizeInWorldUnits {
			get {
				return TileBorderSizeInVoxels*cellSize;
			}
		}

		Bounds CalculateTileBoundsWithBorder (int x, int z) {
			var bounds = new Bounds();

			bounds.SetMinMax(new Vector3(x*TileWorldSizeX, 0, z*TileWorldSizeZ),
				new Vector3((x+1)*TileWorldSizeX, forcedBoundsSize.y, (z+1)*TileWorldSizeZ)
				);

			// Expand borderSize voxels on each side
			bounds.Expand(new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2);
			return bounds;
		}

		protected NavmeshTile BuildTileMesh (Voxelize vox, int x, int z, int threadIndex = 0) {
			AstarProfiler.StartProfile("Build Tile");
			AstarProfiler.StartProfile("Init");

			vox.borderSize = TileBorderSizeInVoxels;
			vox.forcedBounds = CalculateTileBoundsWithBorder(x, z);
			vox.width = tileSizeX + vox.borderSize*2;
			vox.depth = tileSizeZ + vox.borderSize*2;

			if (!useTiles && relevantGraphSurfaceMode == RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile) {
				// This best reflects what the user would actually want
				vox.relevantGraphSurfaceMode = RelevantGraphSurfaceMode.RequireForAll;
			} else {
				vox.relevantGraphSurfaceMode = relevantGraphSurfaceMode;
			}

			vox.minRegionSize = Mathf.RoundToInt(minRegionSize / (cellSize*cellSize));

			AstarProfiler.EndProfile("Init");


			// Init voxelizer
			vox.Init();
			vox.VoxelizeInput(transform, CalculateTileBoundsWithBorder(x, z));

			AstarProfiler.StartProfile("Filter Ledges");


			vox.FilterLedges(vox.voxelWalkableHeight, vox.voxelWalkableClimb, vox.cellSize, vox.cellHeight);

			AstarProfiler.EndProfile("Filter Ledges");

			AstarProfiler.StartProfile("Filter Low Height Spans");
			vox.FilterLowHeightSpans(vox.voxelWalkableHeight, vox.cellSize, vox.cellHeight);
			AstarProfiler.EndProfile("Filter Low Height Spans");

			vox.BuildCompactField();
			vox.BuildVoxelConnections();
			vox.ErodeWalkableArea(CharacterRadiusInVoxels);
			vox.BuildDistanceField();
			vox.BuildRegions();

			var cset = new VoxelContourSet();
			vox.BuildContours(contourMaxError, 1, cset, Voxelize.RC_CONTOUR_TESS_WALL_EDGES | Voxelize.RC_CONTOUR_TESS_TILE_EDGES);

			VoxelMesh mesh;
			vox.BuildPolyMesh(cset, 3, out mesh);

			AstarProfiler.StartProfile("Build Nodes");

			// Position the vertices correctly in graph space (all tiles are laid out on the xz plane with the (0,0) tile at the origin)
			for (int i = 0; i < mesh.verts.Length; i++) {
				mesh.verts[i] *= Int3.Precision;
			}
			vox.transformVoxel2Graph.Transform(mesh.verts);

			NavmeshTile tile = CreateTile(vox, mesh, x, z, threadIndex);

			AstarProfiler.EndProfile("Build Nodes");

			AstarProfiler.EndProfile("Build Tile");
			return tile;
		}

		/** Create a tile at tile index \a x, \a z from the mesh.
		 * \version Since version 3.7.6 the implementation is thread safe
		 */
		NavmeshTile CreateTile (Voxelize vox, VoxelMesh mesh, int x, int z, int threadIndex) {
			if (mesh.tris == null) throw new System.ArgumentNullException("mesh.tris");
			if (mesh.verts == null) throw new System.ArgumentNullException("mesh.verts");
			if (mesh.tris.Length % 3 != 0) throw new System.ArgumentException("Indices array's length must be a multiple of 3 (mesh.tris)");
			if (mesh.verts.Length >= VertexIndexMask) {
				if (tileXCount*tileZCount == 1) {
					throw new System.ArgumentException("Too many vertices per tile (more than " + VertexIndexMask + ")." +
						"\n<b>Try enabling tiling in the recast graph settings.</b>\n");
				} else {
					throw new System.ArgumentException("Too many vertices per tile (more than " + VertexIndexMask + ")." +
						"\n<b>Try reducing tile size or enabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector</b>");
				}
			}

			// Create a new navmesh tile and assign its settings
			var tile = new NavmeshTile {
				x = x,
				z = z,
				w = 1,
				d = 1,
				tris = mesh.tris,
				bbTree = new BBTree()
			};

			tile.vertsInGraphSpace = Utility.RemoveDuplicateVertices(mesh.verts, tile.tris);
			tile.verts = (Int3[])tile.vertsInGraphSpace.Clone();
			transform.Transform(tile.verts);

			// Here we are faking a new graph
			// The tile is not added to any graphs yet, but to get the position queries from the nodes
			// to work correctly (not throw exceptions because the tile is not calculated) we fake a new graph
			// and direct the position queries directly to the tile
			// The thread index is added to make sure that if multiple threads are calculating tiles at the same time
			// they will not use the same temporary graph index
			uint temporaryGraphIndex = (uint)(active.data.graphs.Length + threadIndex);

			if (temporaryGraphIndex > GraphNode.MaxGraphIndex) {
				// Multithreaded tile calculations use fake graph indices, see above.
				throw new System.Exception("Graph limit reached. Multithreaded recast calculations cannot be done because a few scratch graph indices are required.");
			}

			// This index will be ORed to the triangle indices
			int tileIndex = x + z*tileXCount;
			tileIndex <<= TileIndexOffset;

			TriangleMeshNode.SetNavmeshHolder((int)temporaryGraphIndex, tile);
			// We need to lock here because creating nodes is not thread safe
			// and we may be doing this from multiple threads at the same time
			lock (active) {
				tile.nodes = CreateNodes(tile.tris, tileIndex, temporaryGraphIndex);
			}

			tile.bbTree.RebuildFrom(tile.nodes);
			CreateNodeConnections(tile.nodes);
			// Remove the fake graph
			TriangleMeshNode.SetNavmeshHolder((int)temporaryGraphIndex, null);

			return tile;
		}

		public override void DeserializeSettingsCompatibility (GraphSerializationContext ctx) {
			base.DeserializeSettingsCompatibility(ctx);

			characterRadius = ctx.reader.ReadSingle();
			contourMaxError = ctx.reader.ReadSingle();
			cellSize = ctx.reader.ReadSingle();
			ctx.reader.ReadSingle(); // Backwards compatibility, cellHeight was previously read here
			walkableHeight = ctx.reader.ReadSingle();
			maxSlope = ctx.reader.ReadSingle();
			maxEdgeLength = ctx.reader.ReadSingle();
			editorTileSize = ctx.reader.ReadInt32();
			tileSizeX = ctx.reader.ReadInt32();
			nearestSearchOnlyXZ = ctx.reader.ReadBoolean();
			useTiles = ctx.reader.ReadBoolean();
			relevantGraphSurfaceMode = (RelevantGraphSurfaceMode)ctx.reader.ReadInt32();
			rasterizeColliders = ctx.reader.ReadBoolean();
			rasterizeMeshes = ctx.reader.ReadBoolean();
			rasterizeTerrain = ctx.reader.ReadBoolean();
			rasterizeTrees = ctx.reader.ReadBoolean();
			colliderRasterizeDetail = ctx.reader.ReadSingle();
			forcedBoundsCenter = ctx.DeserializeVector3();
			forcedBoundsSize = ctx.DeserializeVector3();
			mask = ctx.reader.ReadInt32();

			int count = ctx.reader.ReadInt32();
			tagMask = new List<string>(count);
			for (int i = 0; i < count; i++) {
				tagMask.Add(ctx.reader.ReadString());
			}

			showMeshOutline = ctx.reader.ReadBoolean();
			showNodeConnections = ctx.reader.ReadBoolean();
			terrainSampleSize = ctx.reader.ReadInt32();

			// These were originally forgotten but added in an upgrade
			// To keep backwards compatibility, they are only deserialized
			// If they exist in the streamed data
			walkableClimb = ctx.DeserializeFloat(walkableClimb);
			minRegionSize = ctx.DeserializeFloat(minRegionSize);

			// Make the world square if this value is not in the stream
			tileSizeZ = ctx.DeserializeInt(tileSizeX);

			showMeshSurface = ctx.reader.ReadBoolean();
		}
	}
}
