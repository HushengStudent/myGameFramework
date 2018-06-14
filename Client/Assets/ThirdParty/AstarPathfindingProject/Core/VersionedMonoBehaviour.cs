using UnityEngine;

namespace Pathfinding {
	/** Base class for all components in the package */
	public abstract class VersionedMonoBehaviour : MonoBehaviour, ISerializationCallbackReceiver {
		/** Version of the serialized data. Used for script upgrades. */
		[SerializeField]
		[HideInInspector]
		int version = 0;

		protected virtual void Awake () {
			// Make sure the version field is up to date for components created during runtime.
			// Reset is not called when in play mode.
			// If the data had to be upgraded then OnAfterDeserialize would have been called earlier.
			if (Application.isPlaying) version = OnUpgradeSerializedData(int.MaxValue);
		}

		/** Handle serialization backwards compatibility */
		void Reset () {
			// Set initial version when adding the component for the first time
			version = OnUpgradeSerializedData(int.MaxValue);
		}

		/** Handle serialization backwards compatibility */
		void ISerializationCallbackReceiver.OnBeforeSerialize () {
		}

		/** Handle serialization backwards compatibility */
		void ISerializationCallbackReceiver.OnAfterDeserialize () {
			version = OnUpgradeSerializedData(version);
		}

		/** Handle serialization backwards compatibility */
		protected virtual int OnUpgradeSerializedData (int version) {
			return 1;
		}
	}
}
