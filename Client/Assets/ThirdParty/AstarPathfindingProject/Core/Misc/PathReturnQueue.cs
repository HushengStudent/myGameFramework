using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding {
	class PathReturnQueue {
		/**
		 * Holds all paths which are waiting to be flagged as completed.
		 * \see ReturnPaths
		 */
		Pathfinding.Util.LockFreeStack pathReturnStack = new Pathfinding.Util.LockFreeStack();

		/**
		 * A temporary queue for paths which weren't returned due to large processing time.
		 * When some time limit is exceeded in ReturnPaths, paths are put in this queue until the next frame.
		 *
		 * Paths contain a member called 'next', so this actually forms a linked list.
		 *
		 * \see ReturnPaths
		 */
		Path pathReturnPop;

		/**
		 * Paths are claimed silently by some object to prevent them from being recycled while still in use.
		 * This will be set to the AstarPath object.
		 */
		System.Object pathsClaimedSilentlyBy;

		public PathReturnQueue (System.Object pathsClaimedSilentlyBy) {
			this.pathsClaimedSilentlyBy = pathsClaimedSilentlyBy;
		}

		public void Enqueue (Path path) {
			pathReturnStack.Push(path);
		}

		/**
		 * Returns all paths in the return stack.
		 * Paths which have been processed are put in the return stack.
		 * This function will pop all items from the stack and return them to e.g the Seeker requesting them.
		 *
		 * \param timeSlice Do not return all paths at once if it takes a long time, instead return some and wait until the next call.
		 */
		public void ReturnPaths (bool timeSlice) {
			Profiler.BeginSample("Calling Path Callbacks");
			// Pop all items from the stack
			Path p = pathReturnStack.PopAll();

			if (pathReturnPop == null) {
				pathReturnPop = p;
			} else {
				Path tail = pathReturnPop;
				while (tail.next != null) tail = tail.next;
				tail.next = p;
			}

			// Hard coded limit on 1.0 ms
			long targetTick = timeSlice ? System.DateTime.UtcNow.Ticks + 1 * 10000 : 0;

			int counter = 0;
			// Loop through the linked list and return all paths
			while (pathReturnPop != null) {
				//Move to the next path
				Path prev = pathReturnPop;
				pathReturnPop = pathReturnPop.next;

				// Remove the reference to prevent possible memory leaks
				// If for example the first path computed was stored somewhere,
				// it would through the linked list contain references to all comming paths to be computed,
				// and thus the nodes those paths searched.
				// That adds up to a lot of memory not being released
				prev.next = null;

				//Return the path
				((IPathInternals)prev).ReturnPath();

				// Will increment to Returned
				// However since multithreading is annoying, it might be set to ReturnQueue for a small time until the pathfinding calculation
				// thread advanced the state as well
				((IPathInternals)prev).AdvanceState(PathState.Returned);

				prev.Release(pathsClaimedSilentlyBy, true);

				counter++;
				// At least 5 paths will be returned, even if timeSlice is enabled
				if (counter > 5 && timeSlice) {
					counter = 0;
					if (System.DateTime.UtcNow.Ticks >= targetTick) {
						return;
					}
				}
			}
			Profiler.EndSample();
		}
	}
}
