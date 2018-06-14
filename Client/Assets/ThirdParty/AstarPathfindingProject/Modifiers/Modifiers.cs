using UnityEngine;

namespace Pathfinding {
	/** Base for all path modifiers.
	 * \see MonoModifier
	 * Modifier */
	public interface IPathModifier {
		int Order { get; }

		void Apply (Path p);
		void PreProcess (Path p);
	}

	/** Base class for path modifiers which are not attached to GameObjects.
	 * \see MonoModifier */
	[System.Serializable]
	public abstract class PathModifier : IPathModifier {
		[System.NonSerialized]
		public Seeker seeker;

		/** Modifiers will be executed from lower order to higher order.
		 * This value is assumed to stay constant.
		 */
		public abstract int Order { get; }

		public void Awake (Seeker s) {
			seeker = s;
			if (s != null) {
				s.RegisterModifier(this);
			}
		}

		public void OnDestroy (Seeker s) {
			if (s != null) {
				s.DeregisterModifier(this);
			}
		}

		public virtual void PreProcess (Path p) {
			// Required by IPathModifier
		}

		/** Main Post-Processing function */
		public abstract void Apply (Path p);
	}

	/** Base class for path modifiers which can be attached to GameObjects.
	 * \see[AddComponentMenu("CONTEXT/Seeker/Something")] Modifier
	 */
	[System.Serializable]
	public abstract class MonoModifier : VersionedMonoBehaviour, IPathModifier {
		public void OnEnable () {}
		public void OnDisable () {}

		[System.NonSerialized]
		public Seeker seeker;

		/** Modifiers will be executed from lower order to higher order.
		 * This value is assumed to stay constant.
		 */
		public abstract int Order { get; }

		/** Alerts the Seeker that this modifier exists */
		protected override void Awake () {
			base.Awake();
			seeker = GetComponent<Seeker>();

			if (seeker != null) {
				seeker.RegisterModifier(this);
			}
		}

		public virtual void OnDestroy () {
			if (seeker != null) {
				seeker.DeregisterModifier(this);
			}
		}

		public virtual void PreProcess (Path p) {
			// Required by IPathModifier
		}

		/** Main Post-Processing function */
		public abstract void Apply (Path p);
	}
}
