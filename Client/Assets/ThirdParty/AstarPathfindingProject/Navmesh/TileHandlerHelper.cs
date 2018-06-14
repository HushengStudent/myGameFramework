using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Util;

namespace Pathfinding {
#if !ASTAR_NO_RECAST_GRAPH || !ASTAR_NO_NAVMESH_GRAPH
	/** Helper for navmesh cut objects.
	 * Adding an instance of this component into the scene makes
	 * sure that NavmeshCut components update the recast graph correctly when they move around.
	 *
	 * \astarpro
	 */
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_tile_handler_helper.php")]
	public class TileHandlerHelper : VersionedMonoBehaviour {
		TileHandler handler;

		/** How often to check if an update needs to be done (real seconds between checks).
		 * For very large worlds with lots of NavmeshCut objects, it might be a performance penalty to do this check every frame.
		 * If you think this is a performance penalty, increase this number to check less often.
		 *
		 * For almost all games, this can be kept at 0.
		 *
		 * If negative, no updates will be done. They must be manually triggered using #ForceUpdate
		 */
		public float updateInterval;

		float lastUpdateTime = float.NegativeInfinity;

		readonly List<IntRect> forcedReloadRects = new List<IntRect>();

		/** Use the specified handler, will create one at start if not called */
		public void UseSpecifiedHandler (TileHandler newHandler) {
			if (!enabled) throw new System.InvalidOperationException("TileHandlerHelper is disabled");

			if (handler != null) {
				NavmeshClipper.RemoveEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
				handler.graph.OnRecalculatedTiles -= OnRecalculatedTiles;
			}

			handler = newHandler;

			if (handler != null) {
				NavmeshClipper.AddEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
				handler.graph.OnRecalculatedTiles += OnRecalculatedTiles;
			}
		}

		void OnEnable () {
			if (handler != null) {
				NavmeshClipper.AddEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
				handler.graph.OnRecalculatedTiles += OnRecalculatedTiles;
			}
			forcedReloadRects.Clear();
		}

		void OnDisable () {
			if (handler != null) {
				NavmeshClipper.RemoveEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
				forcedReloadRects.Clear();
				handler.graph.OnRecalculatedTiles -= OnRecalculatedTiles;
			}
		}

		/** Discards all pending updates caused by moved or modified navmesh cuts */
		public void DiscardPending () {
			if (handler != null) {
				for (var cut = handler.cuts.AllItems; cut != null; cut = cut.next) {
					if (cut.obj.RequiresUpdate()) {
						cut.obj.NotifyUpdated();
					}
				}
			}
			forcedReloadRects.Clear();
		}

		void Start () {
			if (FindObjectsOfType(typeof(TileHandlerHelper)).Length > 1) {
				Debug.LogError("There should only be one TileHandlerHelper per scene. Destroying.");
				Destroy(this);
				return;
			}

			if (handler == null) FindGraph();
		}

		void FindGraph () {
			if (AstarPath.active != null) {
				var graph = AstarPath.active.data.recastGraph as NavmeshBase ?? AstarPath.active.data.navmesh;
				if (graph != null) {
					UseSpecifiedHandler(new TileHandler(graph));
					handler.CreateTileTypesFromGraph();
				}
			}
		}

		/** Called when some recast graph tiles have been completely recalculated */
		void OnRecalculatedTiles (NavmeshTile[] tiles) {
			if (!handler.isValid) {
				UseSpecifiedHandler(new TileHandler(handler.graph));
			}

			handler.OnRecalculatedTiles(tiles);
		}

		/** Called when a NavmeshCut or NavmeshAdd is enabled */
		void HandleOnEnableCallback (NavmeshClipper obj) {
			var graphSpaceBounds = obj.GetBounds(handler.graph.transform);
			var touchingTiles = handler.graph.GetTouchingTilesInGraphSpace(graphSpaceBounds);

			handler.cuts.Add(obj, touchingTiles);
			obj.ForceUpdate();
		}

		/** Called when a NavmeshCut or NavmeshAdd is disabled */
		void HandleOnDisableCallback (NavmeshClipper obj) {
			var root = handler.cuts.GetRoot(obj);

			if (root != null) {
				forcedReloadRects.Add(root.previousBounds);
				handler.cuts.Remove(obj);
			}
			lastUpdateTime = float.NegativeInfinity;
		}

		/** Update is called once per frame */
		void Update () {
			if (handler == null) FindGraph();

			if (handler != null && !AstarPath.active.isScanning && ((updateInterval >= 0 && Time.realtimeSinceStartup - lastUpdateTime > updateInterval) || !handler.isValid)) {
				ForceUpdate();
			}
		}

		/** Checks all NavmeshCut instances and updates graphs if needed.
		 * \note This schedules updates for all necessary tiles to happen as soon as possible.
		 * The pathfinding threads will continue to calculate the paths that they were calculating when this function
		 * was called and then they will be paused and the graph updates will be carried out (this may be several frames into the
		 * future and the graph updates themselves may take several frames to complete).
		 * If you want to force all navmesh cutting to be completed in a single frame call this method
		 * and immediately after call AstarPath.FlushWorkItems.
		 */
		public void ForceUpdate () {
			if (handler == null) throw new System.Exception("Cannot update graphs. No TileHandler. Do not call the ForceUpdate method in Awake.");

			lastUpdateTime = Time.realtimeSinceStartup;

			if (!handler.isValid) {
				// Check if the graph has been destroyed
				if (!handler.graph.exists) {
					UseSpecifiedHandler(null);
					return;
				}

				Debug.Log("TileHandler no longer matched the underlaying graph (possibly because of a graph scan). Recreating TileHandler...");
				UseSpecifiedHandler(new TileHandler(handler.graph));
				handler.CreateTileTypesFromGraph();

				// Reload in huge bounds. This will cause all tiles to be updated.
				forcedReloadRects.Add(new IntRect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
			}

			// Get all navmesh cuts in the scene
			var allCuts = handler.cuts.AllItems;

			if (forcedReloadRects.Count == 0) {
				int any = 0;

				// Check if any navmesh cuts need updating
				for (var cut = allCuts; cut != null; cut = cut.next) {
					if (cut.obj.RequiresUpdate()) {
						any++;
						break;
					}
				}

				// Nothing needs to be done for now
				if (any == 0) return;
			}

			// Start batching tile updates which is good for performance
			// if we are updating a lot of them
			bool end = handler.StartBatchLoad();

			for (int i = 0; i < forcedReloadRects.Count; i++) {
				handler.ReloadInBounds(forcedReloadRects[i]);
			}
			forcedReloadRects.Clear();

			// Reload all bounds touching the previous bounds and current bounds
			// of navmesh cuts that have moved or changed in some other way
			for (var cut = allCuts; cut != null; cut = cut.next) {
				if (cut.obj.RequiresUpdate()) {
					// Make sure the tile where it was is updated
					handler.ReloadInBounds(cut.previousBounds);

					var newGraphSpaceBounds = cut.obj.GetBounds(handler.graph.transform);
					var newTouchingTiles = handler.graph.GetTouchingTilesInGraphSpace(newGraphSpaceBounds);
					handler.cuts.Move(cut.obj, newTouchingTiles);
					handler.ReloadInBounds(newTouchingTiles);
				}
			}

			// Notify navmesh cuts that they have been updated
			// This will cause RequiresUpdate to return false
			// until it is changed again
			for (var cut = allCuts; cut != null; cut = cut.next) {
				if (cut.obj.RequiresUpdate()) {
					cut.obj.NotifyUpdated();
				}
			}

			if (end) handler.EndBatchLoad();
		}
	}
#endif
}
