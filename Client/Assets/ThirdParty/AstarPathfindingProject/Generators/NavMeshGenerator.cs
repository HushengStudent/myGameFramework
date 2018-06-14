using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	using Pathfinding.Util;
	using Pathfinding.Serialization;

	public interface INavmesh {
		void GetNodes (System.Action<GraphNode> del);
	}

	/** Generates graphs based on navmeshes.
	 * \ingroup graphs
	 * Navmeshes are meshes where each triangle defines a walkable area.
	 * These are great because the AI can get so much more information on how it can walk.
	 * Polygons instead of points mean that the funnel smoother can produce really nice looking paths and the graphs are also really fast to search
	 * and have a low memory footprint because fewer nodes are usually needed to describe the same area compared to grid graphs.
	 *
	 * \see Pathfinding.RecastGraph
	 *
	 * \shadowimage{navmeshgraph_graph.png}
	 * \shadowimage{navmeshgraph_inspector.png}
	 */
	[JsonOptIn]
	public class NavMeshGraph : NavmeshBase, IUpdatableGraph {
		/** Mesh to construct navmesh from */
		[JsonMember]
		public Mesh sourceMesh;

		/** Offset in world space */
		[JsonMember]
		public Vector3 offset;

		/** Rotation in degrees */
		[JsonMember]
		public Vector3 rotation;

		/** Scale of the graph */
		[JsonMember]
		public float scale = 1;

		public override float TileWorldSizeX {
			get {
				return forcedBoundsSize.x;
			}
		}

		public override float TileWorldSizeZ {
			get {
				return forcedBoundsSize.z;
			}
		}

		protected override float MaxTileConnectionEdgeDistance {
			get {
				// Tiles are not supported, so this is irrelevant
				return 0f;
			}
		}

		public override GraphTransform CalculateTransform () {
			return new GraphTransform(Matrix4x4.TRS(offset, Quaternion.Euler(rotation), Vector3.one) * Matrix4x4.TRS(sourceMesh != null ? sourceMesh.bounds.min * scale : Vector3.zero, Quaternion.identity, Vector3.one));
		}

		public GraphUpdateThreading CanUpdateAsync (GraphUpdateObject o) {
			return GraphUpdateThreading.UnityThread;
		}

		public void UpdateAreaInit (GraphUpdateObject o) {}
		public void UpdateAreaPost (GraphUpdateObject o) {}

		public void UpdateArea (GraphUpdateObject o) {
			UpdateArea(o, this);
		}

		public static void UpdateArea (GraphUpdateObject o, INavmesh graph) {
			Bounds bounds = o.bounds;

			// Bounding rectangle with floating point coordinates
			Rect rect = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);

			// Bounding rectangle with integer coordinates
			var irect = new IntRect(
				Mathf.FloorToInt(bounds.min.x*Int3.Precision),
				Mathf.FloorToInt(bounds.min.z*Int3.Precision),
				Mathf.FloorToInt(bounds.max.x*Int3.Precision),
				Mathf.FloorToInt(bounds.max.z*Int3.Precision)
				);

			// Corners of the bounding rectangle
			var a = new Int3(irect.xmin, 0, irect.ymin);
			var b = new Int3(irect.xmin, 0, irect.ymax);
			var c = new Int3(irect.xmax, 0, irect.ymin);
			var d = new Int3(irect.xmax, 0, irect.ymax);

			var ymin = ((Int3)bounds.min).y;
			var ymax = ((Int3)bounds.max).y;

			// Loop through all nodes and check if they intersect the bounding box
			graph.GetNodes(_node => {
				var node = _node as TriangleMeshNode;

				bool inside = false;

				int allLeft = 0;
				int allRight = 0;
				int allTop = 0;
				int allBottom = 0;

				// Check bounding box rect in XZ plane
				for (int v = 0; v < 3; v++) {
					Int3 p = node.GetVertex(v);
					var vert = (Vector3)p;

					if (irect.Contains(p.x, p.z)) {
						inside = true;
						break;
					}

					if (vert.x < rect.xMin) allLeft++;
					if (vert.x > rect.xMax) allRight++;
					if (vert.z < rect.yMin) allTop++;
					if (vert.z > rect.yMax) allBottom++;
				}

				if (!inside) {
					if (allLeft == 3 || allRight == 3 || allTop == 3 || allBottom == 3) {
						return;
					}
				}

				// Check if the polygon edges intersect the bounding rect
				for (int v = 0; v < 3; v++) {
					int v2 = v > 1 ? 0 : v+1;

					Int3 vert1 = node.GetVertex(v);
					Int3 vert2 = node.GetVertex(v2);

					if (VectorMath.SegmentsIntersectXZ(a, b, vert1, vert2)) { inside = true; break; }
					if (VectorMath.SegmentsIntersectXZ(a, c, vert1, vert2)) { inside = true; break; }
					if (VectorMath.SegmentsIntersectXZ(c, d, vert1, vert2)) { inside = true; break; }
					if (VectorMath.SegmentsIntersectXZ(d, b, vert1, vert2)) { inside = true; break; }
				}

				// Check if the node contains any corner of the bounding rect
				if (inside || node.ContainsPoint(a) || node.ContainsPoint(b) || node.ContainsPoint(c) || node.ContainsPoint(d)) {
					inside = true;
				}

				if (!inside) {
					return;
				}

				int allAbove = 0;
				int allBelow = 0;

				// Check y coordinate
				for (int v = 0; v < 3; v++) {
					Int3 p = node.GetVertex(v);
					if (p.y < ymin) allBelow++;
					if (p.y > ymax) allAbove++;
				}

				// Polygon is either completely above the bounding box or completely below it
				if (allBelow == 3 || allAbove == 3) return;

				// Triangle is inside the bounding box!
				// Update it!
				o.WillUpdateNode(node);
				o.Apply(node);
			});
		}

		/** Scans the graph using the path to an .obj mesh */
		[System.Obsolete("Set the mesh to ObjImporter.ImportFile(...) and scan the graph the normal way instead")]
		public void ScanInternal (string objMeshPath) {
			Mesh mesh = ObjImporter.ImportFile(objMeshPath);

			if (mesh == null) {
				Debug.LogError("Couldn't read .obj file at '"+objMeshPath+"'");
				return;
			}

			sourceMesh = mesh;

			var scan = ScanInternal().GetEnumerator();
			while (scan.MoveNext()) {}
		}

		public override IEnumerable<Progress> ScanInternal () {
			transform = CalculateTransform();
			tileZCount = tileXCount = 1;
			tiles = new NavmeshTile[tileZCount*tileXCount];
			TriangleMeshNode.SetNavmeshHolder(AstarPath.active.data.GetGraphIndex(this), this);

			if (sourceMesh == null) {
				FillWithEmptyTiles();
				yield break;
			}

			yield return new Progress(0.0f, "Transforming Vertices");

			forcedBoundsSize = sourceMesh.bounds.size * scale;
			Vector3[] vectorVertices = sourceMesh.vertices;
			var intVertices = ListPool<Int3>.Claim(vectorVertices.Length);
			var matrix = Matrix4x4.TRS(-sourceMesh.bounds.min * scale, Quaternion.identity, Vector3.one * scale);
			// Convert the vertices to integer coordinates and also position them in graph space
			// so that the minimum of the bounding box of the mesh is at the origin
			// (the vertices will later be transformed to world space)
			for (int i = 0; i < vectorVertices.Length; i++) {
				intVertices.Add((Int3)matrix.MultiplyPoint3x4(vectorVertices[i]));
			}

			yield return new Progress(0.1f, "Compressing Vertices");

			// Remove duplicate vertices
			Int3[] compressedVertices = null;
			int[] compressedTriangles = null;
			Polygon.CompressMesh(intVertices, new List<int>(sourceMesh.triangles), out compressedVertices, out compressedTriangles);
			ListPool<Int3>.Release(intVertices);

			yield return new Progress(0.2f, "Building Nodes");

			ReplaceTile(0, 0, compressedVertices, compressedTriangles);

			// This may be used by the TileHandlerHelper script to update the tiles
			// while taking NavmeshCuts into account after the graph has been completely recalculated.
			if (OnRecalculatedTiles != null) {
				OnRecalculatedTiles(tiles.Clone() as NavmeshTile[]);
			}
		}

		public override void DeserializeSettingsCompatibility (GraphSerializationContext ctx) {
			base.DeserializeSettingsCompatibility(ctx);

			sourceMesh = ctx.DeserializeUnityObject() as Mesh;
			offset = ctx.DeserializeVector3();
			rotation = ctx.DeserializeVector3();
			scale = ctx.reader.ReadSingle();
			nearestSearchOnlyXZ = !ctx.reader.ReadBoolean();
		}
	}
}
