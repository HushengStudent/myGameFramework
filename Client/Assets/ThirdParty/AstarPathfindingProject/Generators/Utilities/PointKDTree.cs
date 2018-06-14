using System.Collections.Generic;

namespace Pathfinding {
	/** Represents a collection of GraphNodes. It allows for fast lookups of the closest node to a point */
	public class PointKDTree {
		// TODO: Make constant
		public static int LeafSize = 10;

		Node[] tree = new Node[16];

		int numNodes = 0;

		readonly List<GraphNode> largeList = new List<GraphNode>();
		readonly Stack<List<GraphNode> > listCache = new Stack<List<GraphNode> >();
		static readonly IComparer<GraphNode>[] comparers = new IComparer<GraphNode>[] { new CompareX(), new CompareY(), new CompareZ() };

		struct Node {
			public List<GraphNode> data;
			public int split;
			public byte splitAxis;
		}

		// Pretty ugly with one class for each axis, but it has been verified to make the tree around 5% faster
		class CompareX : IComparer<GraphNode> {
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.x.CompareTo(rhs.position.x); }
		}

		class CompareY : IComparer<GraphNode> {
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.y.CompareTo(rhs.position.y); }
		}

		class CompareZ : IComparer<GraphNode> {
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.z.CompareTo(rhs.position.z); }
		}

		public PointKDTree() {
			tree[1] = new Node { data = GetOrCreateList() };
		}

		/** Add the node to the tree */
		public void Add (GraphNode node) {
			numNodes++;
			Add(node, 1);
		}

		/** Rebuild the tree starting with all nodes in the array between index start (inclusive) and end (exclusive) */
		public void Rebuild (GraphNode[] nodes, int start, int end) {
			if (start < 0 || end < start || end > nodes.Length)
				throw new System.ArgumentException();

			for (int i = 0; i < tree.Length; i++) {
				if (tree[i].data != null) {
					tree[i].data.Clear();
					listCache.Push(tree[i].data);
					tree[i].data = null;
				}
			}

			numNodes = end - start;
			Build(1, new List<GraphNode>(nodes), start, end);
		}

		List<GraphNode> GetOrCreateList () {
			// Note, the lists will never become larger than this initial capacity, so possibly they should be replaced by arrays
			return listCache.Count > 0 ? listCache.Pop() : new List<GraphNode>(LeafSize*2 + 1);
		}

		int Size (int index) {
			return tree[index].data != null ? tree[index].data.Count : Size(2 * index) + Size(2 * index + 1);
		}

		void CollectAndClear (int index, List<GraphNode> buffer) {
			var nodes = tree[index].data;

			if (nodes != null) {
				tree[index] = new Node();
				for (int i = 0; i < nodes.Count; i++) buffer.Add(nodes[i]);
				nodes.Clear();
				listCache.Push(nodes);
			} else {
				CollectAndClear(index*2, buffer);
				CollectAndClear(index*2 + 1, buffer);
			}
		}

		static int MaxAllowedSize (int numNodes, int depth) {
			// Allow a node to be 2.5 times as full as it should ideally be
			// but do not allow it to contain more than 3/4ths of the total number of nodes
			// (important to make sure nodes near the top of the tree also get rebalanced).
			// A node should ideally contain numNodes/2^depth nodes below it (^ is exponentiation, not xor)
			return System.Math.Min(((5 * numNodes) / 2) >> depth, (3 * numNodes) / 4);
		}

		void Rebalance (int index) {
			CollectAndClear(index, largeList);
			Build(index, largeList, 0, largeList.Count);
			largeList.Clear();
		}

		void EnsureSize (int index) {
			if (index >= tree.Length) {
				var newLeaves = new Node[System.Math.Max(index + 1, tree.Length*2)];
				tree.CopyTo(newLeaves, 0);
				tree = newLeaves;
			}
		}

		void Build (int index, List<GraphNode> nodes, int start, int end) {
			EnsureSize(index);
			if (end - start <= LeafSize) {
				tree[index].data = GetOrCreateList();
				for (int i = start; i < end; i++)
					tree[index].data.Add(nodes[i]);
			} else {
				Int3 mn, mx;
				mn = mx = nodes[start].position;
				for (int i = start; i < end; i++) {
					var p = nodes[i].position;
					mn = new Int3(System.Math.Min(mn.x, p.x), System.Math.Min(mn.y, p.y), System.Math.Min(mn.z, p.z));
					mx = new Int3(System.Math.Max(mx.x, p.x), System.Math.Max(mx.y, p.y), System.Math.Max(mx.z, p.z));
				}
				Int3 diff = mx - mn;
				var axis = diff.x > diff.y ? (diff.x > diff.z ? 0 : 2) : (diff.y > diff.z ? 1 : 2);

				nodes.Sort(start, end - start, comparers[axis]);
				int mid = (start+end)/2;
				tree[index].split = (nodes[mid-1].position[axis] + nodes[mid].position[axis] + 1)/2;
				tree[index].splitAxis = (byte)axis;
				Build(index*2 + 0, nodes, start, mid);
				Build(index*2 + 1, nodes, mid, end);
			}
		}

		void Add (GraphNode point, int index, int depth = 0) {
			// Move down in the tree until the leaf node is found that this point is inside of
			while (tree[index].data == null) {
				index = 2 * index + (point.position[tree[index].splitAxis] < tree[index].split ? 0 : 1);
				depth++;
			}

			// Add the point to the leaf node
			tree[index].data.Add(point);

			// Check if the leaf node is large enough that we need to do some rebalancing
			if (tree[index].data.Count > LeafSize*2) {
				int levelsUp = 0;

				// Search upwards for nodes that are too large and should be rebalanced
				// Rebalance the node above the node that had a too large size so that it can
				// move children over to the sibling
				while (depth - levelsUp > 0 && Size(index >> levelsUp) > MaxAllowedSize(numNodes, depth-levelsUp)) {
					levelsUp++;
				}

				Rebalance(index >> levelsUp);
			}
		}

		/** Closest node to the point which satisfies the constraint */
		public GraphNode GetNearest (Int3 point, NNConstraint constraint) {
			GraphNode best = null;
			long bestSqrDist = long.MaxValue;

			GetNearestInternal(1, point, constraint, ref best, ref bestSqrDist);
			return best;
		}

		void GetNearestInternal (int index, Int3 point, NNConstraint constraint, ref GraphNode best, ref long bestSqrDist) {
			var data = tree[index].data;

			if (data != null) {
				for (int i = data.Count-1; i >= 0; i--) {
					var dist = (data[i].position - point).sqrMagnitudeLong;
					if (dist < bestSqrDist && (constraint == null || constraint.Suitable(data[i]))) {
						bestSqrDist = dist;
						best = data[i];
					}
				}
			} else {
				var dist = (long)(point[tree[index].splitAxis] - tree[index].split);
				var childIndex = 2 * index + (dist < 0 ? 0 : 1);
				GetNearestInternal(childIndex, point, constraint, ref best, ref bestSqrDist);

				// Try the other one if it is possible to find a valid node on the other side
				if (dist*dist < bestSqrDist) {
					// childIndex ^ 1 will flip the last bit, so if childIndex is odd, then childIndex ^ 1 will be even
					GetNearestInternal(childIndex ^ 0x1, point, constraint, ref best, ref bestSqrDist);
				}
			}
		}

		/** Add all nodes within a squared distance of the point to the buffer */
		public void GetInRange (Int3 point, long sqrRadius, List<GraphNode> buffer) {
			GetInRangeInternal(1, point, sqrRadius, buffer);
		}

		void GetInRangeInternal (int index, Int3 point, long sqrRadius, List<GraphNode> buffer) {
			var data = tree[index].data;

			if (data != null) {
				for (int i = data.Count-1; i >= 0; i--) {
					var dist = (data[i].position - point).sqrMagnitudeLong;
					if (dist < sqrRadius) {
						buffer.Add(data[i]);
					}
				}
			} else {
				var dist = (long)(point[tree[index].splitAxis] - tree[index].split);
				var childIndex = 2 * index + (dist < 0 ? 0 : 1);
				GetInRangeInternal(childIndex, point, sqrRadius, buffer);

				// Try the other one if it is possible to find a valid node on the other side
				if (dist*dist < sqrRadius) {
					// childIndex ^ 1 will flip the last bit, so if childIndex is odd, then childIndex ^ 1 will be even
					GetInRangeInternal(childIndex ^ 0x1, point, sqrRadius, buffer);
				}
			}
		}
	}
}
