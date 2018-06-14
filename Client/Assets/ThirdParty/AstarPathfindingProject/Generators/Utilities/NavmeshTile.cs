namespace Pathfinding {
	using Pathfinding.Util;

	public class NavmeshTile : INavmeshHolder, INavmesh {
		/** Tile triangles */
		public int[] tris;

		/** Tile vertices */
		public Int3[] verts;

		/** Tile vertices in graph space */
		public Int3[] vertsInGraphSpace;

		/** Tile X Coordinate */
		public int x;

		/** Tile Z Coordinate */
		public int z;

		/** Width, in tile coordinates.
		     * \warning Widths other than 1 are not supported. This is mainly here for possible future features.
		     */
		public int w;

		/** Depth, in tile coordinates.
		     * \warning Depths other than 1 are not supported. This is mainly here for possible future features.
		     */
		public int d;

		/** All nodes in the tile */
		public TriangleMeshNode[] nodes;

		/** Bounding Box Tree for node lookups */
		public BBTree bbTree;

		/** Temporary flag used for batching */
		public bool flag;

		#region INavmeshHolder implementation

		public void GetTileCoordinates (int tileIndex, out int x, out int z) {
			x = this.x;
			z = this.z;
		}

		public int GetVertexArrayIndex (int index) {
			return index & NavmeshBase.VertexIndexMask;
		}

		/** Get a specific vertex in the tile */
		public Int3 GetVertex (int index) {
			int idx = index & NavmeshBase.VertexIndexMask;

			return verts[idx];
		}

		public Int3 GetVertexInGraphSpace (int index) {
			return vertsInGraphSpace[index & NavmeshBase.VertexIndexMask];
		}

		#endregion

		public void GetNodes (System.Action<GraphNode> action) {
			if (nodes == null) return;
			for (int i = 0; i < nodes.Length; i++) action(nodes[i]);
		}

		internal void Destroy () {
			if (nodes.Length > 0) {
				// Get this tile's index from the first node
				var tileIndex = NavmeshBase.GetTileIndex(nodes[0].GetVertexIndex(0));
				var graphIndex = nodes[0].GraphIndex;

				// Destroy the nodes
				// To avoid removing connections one by one, which is very inefficient
				// we set all connections to other nodes in the same tile to null since
				// we already know that their connections will be destroyed as well.
				// This reduces the time it takes to destroy the nodes by approximately 50%
				for (int i = 0; i < nodes.Length; i++) {
					var node = nodes[i];
					if (node.connections != null) {
						for (int j = 0; j < node.connections.Length; j++) {
							var otherMesh = node.connections[j].node as TriangleMeshNode;
							// Check if the nodes are in the same graph and the same tile
							if (otherMesh != null && otherMesh.GraphIndex == graphIndex && NavmeshBase.GetTileIndex(otherMesh.GetVertexIndex(0)) == tileIndex) {
								node.connections[j].node = null;
							}
						}
					}
				}

				// This will also remove old connections
				for (int i = 0; i < nodes.Length; i++) {
					nodes[i].Destroy();
				}
			}

			nodes = null;
			ObjectPool<BBTree>.Release(ref bbTree);
		}
	}
}
