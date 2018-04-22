using UnityEngine;
using System.Collections;

[AddComponentMenu("Fast Shadow Projector/Shadow Receiver")]
public class ShadowReceiver : MonoBehaviour {
	
	MeshFilter _meshFilter;
	Mesh _mesh;
	Mesh _meshCopy;
	MeshRenderer _meshRenderer;
	Terrain _terrain = null;
	public Material _terrainMaterial;

	bool _isTerrain = false;
	bool _standardTerrain = false;

	public int _id;
	
	void Awake() {

		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_terrain = GetComponent<Terrain>();
	
		if (_terrain != null) {
			_isTerrain = true;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 
			if (_terrain.materialType == Terrain.MaterialType.BuiltInStandard) {
			
				_terrainMaterial = new Material(Shader.Find("FastShadowProjector/FSP_Standard-TerrainFirstPass"));
				_standardTerrain = true;
			} else {
				_terrainMaterial = new Material(Shader.Find("FastShadowProjector/FSP_TerrainFirstPass"));
				_standardTerrain = false;
			}

			if (_terrainMaterial == null) {
				Debug.LogWarning ("Could not find: FSP terrain FirstPass shader!");
			} else { 

				_terrain.materialType = Terrain.MaterialType.Custom;
				_terrain.materialTemplate = _terrainMaterial;
			}		
#else 
			_terrainMaterial = new Material(Shader.Find("FastShadowProjector/FSP_TerrainFirstPass"));
			_standardTerrain = false;

			
			if (_terrainMaterial == null) {
				Debug.LogWarning ("Could not find: FSP terrain FirstPass shader!");
			} else { 
				_terrain.materialTemplate = _terrainMaterial;
			}		
#endif
		}

		if (_terrain == null && !_meshRenderer.isPartOfStaticBatch && _meshFilter != null) {
			_mesh = _meshFilter.sharedMesh;
		}
		
		_meshCopy = null;
	}
	
	void Start () {	
		AddReceiver();

		if (_terrain == null && _meshRenderer.isPartOfStaticBatch && _meshRenderer != null && _terrain == null) {
			_meshCopy = FSPStaticMeshHolder.Get().GetMesh(_id);


		}
	}
	
	public Mesh GetMesh() {
		if (_meshCopy != null) {
			return _meshCopy;
		} else {
			return _mesh;
		}
	}
	
	public bool IsTerrain() {
		if (!Application.isPlaying) { 
			if (GetComponent<Terrain>() != null) {
				_isTerrain = true;
				return true;
			}
		}
		return _isTerrain;
	}

	public bool IsStandardTerrain() {
		return _standardTerrain;
	}
	
	public Terrain GetTerrain() {
		return _terrain;
	}
	
	void OnEnable() {
		AddReceiver();
	}
	
	void OnDisable() {
		RemoveReceiver();
	}

	void OnBecameVisible() {
		AddReceiver();
	}

	void OnBecameInvisible() {
		RemoveReceiver();
	}
	
	void OnDestroy() {
		RemoveReceiver();
	}

	void AddReceiver() {
		if (_meshFilter != null || _terrain != null) {
			if (IsTerrain()) {
				_terrain.enabled = true;
			}
			GlobalProjectorManager.Get().AddReceiver(this);
		}
	}

	void RemoveReceiver() {
		if (GlobalProjectorManager.Exists()) {
			if (_meshFilter != null || _terrain != null) {
				if (IsTerrain()) {
					_terrain.enabled = false;
				}
				GlobalProjectorManager.Get().RemoveReceiver(this);
			}
		}
	}
}