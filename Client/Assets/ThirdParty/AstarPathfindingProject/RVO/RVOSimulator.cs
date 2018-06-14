using UnityEngine;

namespace Pathfinding.RVO {
	/** Unity front end for an RVO simulator.
	 * Attached to any GameObject in a scene, scripts such as the RVOController will use the
	 * simulator exposed by this class to handle their movement.
	 * In pretty much all cases you should only have a single RVOSimulator in the scene.
	 *
	 * You can have more than one of these, however most script which make use of the RVOSimulator
	 * will find it by FindObjectOfType, and thus only one will be used.
	 *
	 * This is only a wrapper class for a Pathfinding.RVO.Simulator which simplifies exposing it
	 * for a unity scene.
	 *
	 * \see Pathfinding.RVO.Simulator
	 * \see \ref local-avoidance
	 *
	 * \astarpro
	 */
	[ExecuteInEditMode]
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Simulator")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_r_v_o_1_1_r_v_o_simulator.php")]
	public class RVOSimulator : VersionedMonoBehaviour {
		/** First RVOSimulator in the scene (usually there is only one) */
		public static RVOSimulator active { get; private set; }

		/** Desired FPS for rvo simulation.
		 * It is usually not necessary to run a crowd simulation at a very high fps.
		 * Usually 10-30 fps is enough, but it can be increased for better quality.
		 * The rvo simulation will never run at a higher fps than the game
		 */
		[Tooltip("Desired FPS for rvo simulation. It is usually not necessary to run a crowd simulation at a very high fps.\n" +
			 "Usually 10-30 fps is enough, but can be increased for better quality.\n"+
			 "The rvo simulation will never run at a higher fps than the game")]
		public int desiredSimulationFPS = 20;

		/** Number of RVO worker threads.
		 * If set to None, no multithreading will be used.
		 * Using multithreading can significantly improve performance by offloading work to other CPU cores.
		 */
		[Tooltip("Number of RVO worker threads. If set to None, no multithreading will be used.")]
		public ThreadCount workerThreads = ThreadCount.Two;

		/** Calculate local avoidance in between frames.
		 * If this is enabled and multithreading is used, the local avoidance calculations will continue to run
		 * until the next frame instead of waiting for them to be done the same frame. This can increase the performance
		 * but it can make the agents seem a little less responsive.
		 *
		 * This will only be read at Awake.
		 * \see Pathfinding.RVO.Simulator.DoubleBuffering
		 */
		[Tooltip("Calculate local avoidance in between frames.\nThis can increase jitter in the agents' movement so use it only if you really need the performance boost. " +
			 "It will also reduce the responsiveness of the agents to the commands you send to them.")]
		public bool doubleBuffering;

		/** \copydoc Pathfinding::RVO::Simulator::symmetryBreakingBias */
		[Tooltip("Bias agents to pass each other on the right side.\n" +
			 "If the desired velocity of an agent puts it on a collision course with another agent or an obstacle " +
			 "its desired velocity will be rotated this number of radians (1 radian is approximately 57°) to the right. " +
			 "This helps to break up symmetries and makes it possible to resolve some situations much faster.\n\n" +
			 "When many agents have the same goal this can however have the side effect that the group " +
			 "clustered around the target point may as a whole start to spin around the target point.")]
		[Range(0, 0.2f)]
		public float symmetryBreakingBias = 0.1f;

		/** Determines if the XY (2D) or XZ (3D) plane is used for movement.
		 * For 2D games you would set this to XY and for 3D games you would usually set it to XZ.
		 */
		[Tooltip("Determines if the XY (2D) or XZ (3D) plane is used for movement")]
		public MovementPlane movementPlane = MovementPlane.XZ;

		/** Reference to the internal simulator */
		Pathfinding.RVO.Simulator simulator;

		/** Get the internal simulator.
		 * Will never be null when the game is running
		 */
		public Simulator GetSimulator () {
			if (simulator == null) {
				Awake();
			}
			return simulator;
		}

		void OnEnable () {
			active = this;
		}

		protected override void Awake () {
			base.Awake();
			if (simulator == null && Application.isPlaying) {
				int threadCount = AstarPath.CalculateThreadCount(workerThreads);
				simulator = new Pathfinding.RVO.Simulator(threadCount, doubleBuffering, movementPlane);
			}
		}

		/** Update the simulation */
		void Update () {
			if (!Application.isPlaying) return;

			if (desiredSimulationFPS < 1) desiredSimulationFPS = 1;

			var sim = GetSimulator();
			sim.DesiredDeltaTime = 1.0f / desiredSimulationFPS;
			sim.symmetryBreakingBias = symmetryBreakingBias;
			sim.Update();
		}

		void OnDestroy () {
			active = null;
			if (simulator != null) simulator.OnDestroy();
		}
	}
}
