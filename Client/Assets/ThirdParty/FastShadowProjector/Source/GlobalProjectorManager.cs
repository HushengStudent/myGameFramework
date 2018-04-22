using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalProjectorManager : MonoBehaviour {

	public enum ProjectionCulling {
		None,
		ProjectorBounds,
		ProjectionVolumeBounds
	}

	ProjectorEyeTexture _Tex;
	ProjectorEyeTexture _TexLight;

	public Material _ProjectorMaterialShadow;
	public Material _ProjectorMaterialLight;
	
	Matrix4x4 _ProjectorMatrix;
	Matrix4x4 _ProjectorClipMatrix;
	Matrix4x4 _BiasMatrix;
	Matrix4x4 _ViewMatrix;
	Matrix4x4 _BPV;
	Matrix4x4 _BPVClip;
	Matrix4x4 _ModelMatrix;
	Matrix4x4 _FinalMatrix;
	Matrix4x4 _FinalClipMatrix;
	
	MaterialPropertyBlock _MBP;

	int[] _ShadowResolutions =  new int[] { 128, 256, 512, 1024, 2048 };

	public static readonly string GlobalProjectorLayer = "GlobalProjectorLayer";
	
	static GlobalProjectorManager _Instance;

	public static Vector3 GlobalProjectionDir {
		set {
			if (_Instance._GlobalProjectionDir != value) {
				_Instance._GlobalProjectionDir = value;
				_Instance.OnProjectionDirChange();
			}
		}

		get {
			return _Instance._GlobalProjectionDir;
		}
	}
	Vector3 _GlobalProjectionDir = new Vector3(0.0f, -1.0f, 0.0f);
	

	public static int GlobalShadowResolution {
		set {
			if (_Instance._GlobalShadowResolution != value) {
				_Instance._GlobalShadowResolution = value;
				_Instance.OnShadowResolutionChange();
			}
		}
		
		get {
			return _Instance._GlobalShadowResolution;
		}
	}
	int _GlobalShadowResolution = 2;

	public static ProjectionCulling GlobalShadowCullingMode {
		set {
			_Instance._GlobalShadowCullingMode = value;
		}
		
		get {
			return _Instance._GlobalShadowCullingMode;
		}
	}

	ProjectionCulling _GlobalShadowCullingMode = ProjectionCulling.None;
	
	public static float GlobalCutOffDistance {
		set {
			_Instance._GlobalCutOffDistance = value;
		}
		
		get {
			return _Instance._GlobalCutOffDistance;
		}
	}
	
	float _GlobalCutOffDistance = 1000.0f;
	
	bool _renderShadows = true;
	
	public bool ShadowsOn { 
		set {
			_renderShadows = value;
		}
		get {
			return _renderShadows;
		}
	}
	
	Camera _ProjectorCamera;
	Camera _ProjectorCameraLight;
	
	List<ShadowProjector> _ShadowProjectors;
	List<ShadowProjector> _LightProjectors;
	List<ShadowReceiver> _ShadowReceivers;
	List<ShadowTrigger> _ShadowTriggers;
	
	Texture2D _textureRead;

	bool _shouldCheckTriggers = false;

	Plane[] _mainCameraPlains;
	bool _cameraPlainsCalculated;
	Bounds _projectorBounds;
	
	public static GlobalProjectorManager Get() {
		if (_Instance == null) {
			_Instance = new GameObject("_FSPGlobalProjectorManager").AddComponent<GlobalProjectorManager>();
			_Instance.Initialize();
		}
		return _Instance;
	}
	
	public Shader _globalProjectorShader;

	void Initialize() {

		gameObject.layer = LayerMask.NameToLayer(GlobalProjectorLayer);

		_ProjectorMaterialShadow = new Material(Shader.Find("Fast Shadow Projector/Multiply"));
		_ProjectorMaterialLight = new Material(Shader.Find("Fast Shadow Projector/Add"));

		_ProjectorCamera = gameObject.AddComponent<Camera>();
		_ProjectorCamera.clearFlags = CameraClearFlags.SolidColor;
		_ProjectorCamera.backgroundColor = new Color32(255, 255, 255, 0);
		_ProjectorCamera.cullingMask = 1 << LayerMask.NameToLayer(GlobalProjectorManager.GlobalProjectorLayer);
		_ProjectorCamera.orthographic = true;
		_ProjectorCamera.nearClipPlane = -1;
		_ProjectorCamera.farClipPlane = 100000;
		_ProjectorCamera.aspect = 1.0f;
		_ProjectorCamera.depth = float.MinValue;

		_BiasMatrix = new Matrix4x4();
		_BiasMatrix.SetRow(0, new Vector4(0.5f, 0.0f, 0.0f, 0.5f));
		_BiasMatrix.SetRow(1, new Vector4(0.0f, 0.5f, 0.0f, 0.5f));
		_BiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 0.5f, 0.5f));
		_BiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

		_ProjectorMatrix = new Matrix4x4();
		_ProjectorClipMatrix = new Matrix4x4();

		_MBP = new MaterialPropertyBlock();

		_ShadowProjectors = new List<ShadowProjector>();
		_LightProjectors = new List<ShadowProjector>();
		_ShadowReceivers = new List<ShadowReceiver>();
		_ShadowTriggers = new List<ShadowTrigger>();
	
		CreateLightCamera();
		
		_ProjectorCamera.enabled = false;
		_ProjectorCameraLight.enabled = false;
		
		if (_LightProjectors.Count > 0) {
			CreateProjectorEyeTexture();
		} else {
			CreateProjectorEyeTexture(true, false);
		}
		
		_textureRead = new Texture2D(512, 1, TextureFormat.ARGB32, false);

		_projectorBounds = new Bounds();
	}
	
	void CreateLightCamera() {
		GameObject cameraGameObject = new GameObject("_lightCamera");
		GlobalProjectorLightCamera lightCamera;
		
		cameraGameObject.transform.parent = transform;
		lightCamera =  cameraGameObject.AddComponent<GlobalProjectorLightCamera>();
		lightCamera.PreCullCallback = OnLightPreCull;
		lightCamera.PostRenderCallback = OnLightPostRender;
		
		_ProjectorCameraLight = cameraGameObject.AddComponent<Camera>();
		_ProjectorCameraLight.CopyFrom(_ProjectorCamera);
		_ProjectorCameraLight.backgroundColor = new Color32(0, 0, 0, 0);
		_ProjectorCameraLight.depth = -2;
		
		_ProjectorCameraLight.enabled = false;
	}

	void Awake() {
		OnProjectionDirChange();
	}

	void Start() {
		OnProjectionDirChange();       
	}

	void OnDestroy() {
		_Instance = null;
	}

	public static bool Exists() {
		return (_Instance != null);
	}
	
	public Texture GetShadowTexture() {
		return _Tex.GetTexture();
	}

	public void AddProjector(ShadowProjector projector) {
		bool added = false;
		
		if (!projector.IsLight) {
			if (_Tex == null) {
				CreateProjectorEyeTexture(true, false);
			}

			if (!_ShadowProjectors.Contains(projector)) {
				_ShadowProjectors.Add(projector);
				added = true;
				
				if (_ProjectorCamera.enabled == false) { 
					_ProjectorCamera.enabled = true;
				}
			}
		} else {
			if (_TexLight == null) {
				CreateProjectorEyeTexture(false, true);
			}
			if (!_LightProjectors.Contains(projector)) {
				_LightProjectors.Add(projector);
				added = true;
				
				if (_ProjectorCameraLight.enabled == false) { 
					_ProjectorCameraLight.enabled = true;
				}
			}
		}
		
		if (added && projector.GlobalProjectionDir != _GlobalProjectionDir) {
			GlobalProjectionDir = projector.GlobalProjectionDir;
		}

		if (added && projector.GlobalShadowCullingMode != _GlobalShadowCullingMode) {
			_GlobalShadowCullingMode = projector.GlobalShadowCullingMode;
		}

		if (added && projector.GlobalShadowResolution != _GlobalShadowResolution) {
			_GlobalShadowResolution = projector.GlobalShadowResolution;
		}
		
	}

	public void RemoveProjector(ShadowProjector projector) {
		if (!projector.IsLight) {
			if (_ShadowProjectors.Contains(projector)) {
				_ShadowProjectors.Remove(projector);
				
				if (_ShadowProjectors.Count == 0) {
					_ProjectorCamera.enabled = false;
				}
			}
		} else {
			if (_LightProjectors.Contains(projector)) {
				_LightProjectors.Remove(projector);
				
				if (_LightProjectors.Count == 0) {
					_ProjectorCameraLight.enabled = false;
				}
			}
		}
	
	}

	public void AddReceiver(ShadowReceiver receiver) {
		if (!_ShadowReceivers.Contains(receiver)) {
			CheckForTerrain(receiver);
			_ShadowReceivers.Add(receiver);
		}
	}
	
	void CheckForTerrain(ShadowReceiver receiver) { 
	#if !UNITY_3_5 && !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0 && !UNITY_3_0_0
		if (receiver.IsTerrain()) {
			if (receiver.GetTerrain().materialTemplate != null) {
				receiver.GetTerrain().materialTemplate.SetTexture("_ShadowTex", _Tex.GetTexture());
			}
		}
	#endif
	}
	
	public void AddShadowTrigger(ShadowTrigger trigger) {
		if (!_ShadowTriggers.Contains(trigger)) {
			_ShadowTriggers.Add(trigger);
		}
	}

	public void RemoveShadowTrigger(ShadowTrigger trigger) {
		if (_ShadowTriggers.Contains(trigger)) {
			_ShadowTriggers.Remove(trigger);
		}
	}
	
	public void RemoveReceiver(ShadowReceiver receiver) {
		if (_ShadowReceivers.Contains(receiver)) {
			_ShadowReceivers.Remove(receiver);
		}
	}

	void OnProjectionDirChange() {
		if (GetComponent<Camera>() != null) {
			GetComponent<Camera>().transform.rotation = Quaternion.LookRotation(_GlobalProjectionDir);
		}	
	}

	void OnShadowResolutionChange() {
		if (_LightProjectors.Count > 0) {
			CreateProjectorEyeTexture();
		} else {
			CreateProjectorEyeTexture(true, false);
		}
	}

	void CreateProjectorEyeTexture() {
		CreateProjectorEyeTexture(true, true);
	}

	void CreateProjectorEyeTexture(bool shadow, bool light) {
		if (shadow) { 
			if (_Tex != null) {
				_Tex.CleanUp();
			}
			
			_Tex = new ProjectorEyeTexture(_ProjectorCamera, _ShadowResolutions[_GlobalShadowResolution]);
			_ProjectorMaterialShadow.SetTexture("_ShadowTex", _Tex.GetTexture());
		}

		if (light) {
			if (_TexLight != null) {
				_TexLight.CleanUp();
			}

			_TexLight = new ProjectorEyeTexture(_ProjectorCameraLight, _ShadowResolutions[_GlobalShadowResolution]);
			_ProjectorMaterialLight.SetTexture("_ShadowTex", _TexLight.GetTexture());
		}
	}

	void CalculateShadowBounds(Camera targetCamera, List<ShadowProjector> projectors) {
		_cameraPlainsCalculated = false;

		Vector2 xRange = new Vector2(float.MaxValue, float.MinValue);
		Vector2 yRange = new Vector2(float.MaxValue, float.MinValue);

		Vector2 shadowCoords;
		float maxShadowSize = 0.0f;

		bool noVisibleProjectors = true;
		int projectorIndex = 0;


		ShadowProjector shadowProjector;
		for (int n = 0; n < projectors.Count; n++) {
			shadowProjector = projectors[n];
			
			if (shadowProjector.EnableCutOff) {
				if ((shadowProjector.transform.position - Camera.main.transform.position).magnitude > _GlobalCutOffDistance) {
					shadowProjector.SetVisible(false);
					continue;
				}
			}

			switch(_GlobalShadowCullingMode) {
				case ProjectionCulling.ProjectorBounds: 
				{
					CheckMainCameraPlains();

					if (!GeometryUtility.TestPlanesAABB(_mainCameraPlains, shadowProjector.GetBounds())) {
						shadowProjector.SetVisible(false);
						continue;
					}
				}
					break;
				
				case ProjectionCulling.ProjectionVolumeBounds: 
				{
					CheckMainCameraPlains();

					if (!IsProjectionVolumeVisible(_mainCameraPlains, shadowProjector)) {
						shadowProjector.SetVisible(false);
						continue;
					}
				}
					break;

				default:
					break;
			}

			noVisibleProjectors = false;
			shadowProjector.SetVisible(true);

			shadowCoords = targetCamera.WorldToViewportPoint(shadowProjector.GetShadowPos());

			if (projectorIndex == 0) {
				_projectorBounds.center = shadowProjector.GetShadowPos();
				_projectorBounds.size = Vector3.zero;
			} else {
				_projectorBounds.Encapsulate(shadowProjector.GetShadowPos());
			}

			if (shadowCoords.x < xRange.x) xRange.x = shadowCoords.x;
			if (shadowCoords.x > xRange.y) xRange.y = shadowCoords.x;

			if (shadowCoords.y < yRange.x) yRange.x = shadowCoords.y;
			if (shadowCoords.y > yRange.y) yRange.y = shadowCoords.y;

			float shadowSize = shadowProjector.GetShadowWorldSize();

			if (shadowSize > maxShadowSize) {
				maxShadowSize = shadowSize;
			}

			projectorIndex++;
		}

		if (noVisibleProjectors) {
			return;
		}

		float cameraWorldSize = targetCamera.orthographicSize * 2.0f;
		float maxShadowSizeViewport = Mathf.Max(0.08f, maxShadowSize / cameraWorldSize);

		Vector3 camPos = _projectorBounds.center + _projectorBounds.extents.magnitude * -_GlobalProjectionDir.normalized;
		targetCamera.transform.position = camPos;

		float maxRange = Mathf.Max(xRange[1] - xRange[0] + maxShadowSizeViewport * 2.0f, yRange[1] - yRange[0] + maxShadowSizeViewport* 2.0f);
		targetCamera.orthographicSize = targetCamera.orthographicSize * maxRange;
	}

	void CheckMainCameraPlains() {
		if (!_cameraPlainsCalculated) {
			_mainCameraPlains = GeometryUtility.CalculateFrustumPlanes(Camera.main);
			_cameraPlainsCalculated = true;
		}
	}

	bool IsProjectionVolumeVisible(Plane[] planes, ShadowProjector projector) {
		float boundSize = 1000000.0f;

		Vector3 center = projector.GetShadowPos() + GlobalProjectionDir.normalized * (boundSize * 0.5f);
		Vector3 size = new Vector3(Mathf.Abs(GlobalProjectionDir.normalized.x), Mathf.Abs(GlobalProjectionDir.normalized.y), Mathf.Abs(GlobalProjectionDir.normalized.z))  * boundSize;
		Bounds bounds = new Bounds(center, size);

		float shadowSize = projector.GetShadowWorldSize();

		bounds.Encapsulate(new Bounds(projector.GetShadowPos(), new Vector3(shadowSize, shadowSize, shadowSize)));

		return GeometryUtility.TestPlanesAABB(planes, bounds);
	}
	
	public void SetTriggerTexPixel(Vector3 point, bool checkShadow, bool checkLight, int triggerID) {
		if (checkShadow) {
			SetTriggerTexPixel(_ProjectorCamera, _Tex, point, triggerID);
		} else {
			if (checkLight) {
				SetTriggerTexPixel(_ProjectorCameraLight, _TexLight, point, triggerID);
			}
		}
	}
	
	void SetTriggerTexPixel(Camera camera, ProjectorEyeTexture tex, Vector3 point, int triggerID) {
		Vector3 screenPoint = camera.WorldToScreenPoint(point);

		Texture texture = (Texture)tex.GetTexture();
	
		int x = (int)((screenPoint.x / camera.pixelWidth) * texture.width);
		int y = (int)((screenPoint.y / camera.pixelHeight) * texture.height);

		if (x < 0.0f || y < 0.0f || x >= texture.width || y >= texture.height) {
			return;
		}

		Color pixelColor;
		if (!tex.RenderTextureSupported()) {
			pixelColor = ((Texture2D)texture).GetPixel(x, y);
			_textureRead.SetPixel(triggerID, 0, pixelColor);
		} else {
			RenderTexture.active = tex.GetRenderTexture();
			_textureRead.ReadPixels(new Rect(x,  y, 1, 1), triggerID, 0, false);
		}

	}

	void Update() {
		ShadowReceiver receiver;
		
		for (int i = 0; i < _ShadowReceivers.Count; i++) {
			receiver = _ShadowReceivers[i];
			if (receiver.IsTerrain()) {
				receiver.GetTerrain().materialTemplate = null;
			}
		}
	}


	void LateUpdate() {
		if (!_renderShadows) {
			return;
		}
		
		_shouldCheckTriggers = !_shouldCheckTriggers;
		RenderProjectors(_ProjectorCamera, _ShadowProjectors, _ProjectorMaterialShadow);
		RenderProjectors(_ProjectorCameraLight, _LightProjectors, _ProjectorMaterialLight);
	}
	
	void RenderProjectors(Camera targetCamera, List<ShadowProjector> projectors, Material material) {
		if (!_renderShadows) {
			return;
		}
		
		if (projectors.Count > 0 && _ShadowReceivers.Count > 0) {
			CalculateShadowBounds(targetCamera, projectors);
			
			float n = targetCamera.nearClipPlane;
			float f = targetCamera.farClipPlane;
			float r = targetCamera.orthographicSize;
			float t = targetCamera.orthographicSize;
			float clipN = 0.1f;
			float clipF = 100.0f;


			_ProjectorMatrix.SetRow(0, new Vector4(1 / r, 0.0f, 0.0f, 0));
			_ProjectorMatrix.SetRow(1, new Vector4(0.0f, 1 / t, 0.0f, 0));
			_ProjectorMatrix.SetRow(2, new Vector4(0.0f, 0.0f, -2 / (f - n), 0));
			_ProjectorMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

			_ProjectorClipMatrix.SetRow(0, new Vector4(clipN / r, 0.0f, 0.0f, 0));
			_ProjectorClipMatrix.SetRow(1, new Vector4(0.0f, clipN / t, 0.0f, 0));
			_ProjectorClipMatrix.SetRow(2, new Vector4(0.0f, 0.0f, -(clipF + clipN)/(clipF-clipN), -2*clipF*clipN/(clipF-clipN)));
			_ProjectorClipMatrix.SetRow(3, new Vector4(0.0f, 0.0f, -1.0f, 0.0f));
	
			
			_ViewMatrix = targetCamera.transform.localToWorldMatrix.inverse;
			
			_BPV =  _BiasMatrix * _ProjectorMatrix * _ViewMatrix;
			_BPVClip = _BiasMatrix * _ProjectorClipMatrix * _ViewMatrix;
			
			Render(material);
		}
	}
		
	void Render(Material material) {
		if (!_renderShadows) {
			return;
		}
		
		bool useMBP = true; // WP8 doesn't support MBP's correctly - only one receiver will work for now.

#if UNITY_WP8
		useMBP = false;
#endif
		
		ShadowReceiver receiver;
		
		for (int i = 0; i < _ShadowReceivers.Count; i++) {
			receiver = _ShadowReceivers[i];
			_ModelMatrix = receiver.transform.localToWorldMatrix;
			_FinalMatrix = _BPV * _ModelMatrix;
			_FinalClipMatrix = _BPVClip * _ModelMatrix;
			
			if (receiver.IsTerrain()) { 
				ApplyTerrainTextureMatrix(receiver);
			} else {

				if (useMBP) {
		
					_MBP.Clear();
					_MBP.SetMatrix("_GlobalProjector", _FinalMatrix);
					_MBP.SetMatrix("_GlobalProjectorClip", _FinalClipMatrix);

					for (int n = 0; n < _ShadowReceivers[i].GetMesh().subMeshCount; n++) {
						Graphics.DrawMesh( _ShadowReceivers[i].GetMesh(), _ModelMatrix, material, LayerMask.NameToLayer("Default"), null, n, _MBP);
					}
				} else {
					material.SetMatrix("_GlobalProjector", _FinalMatrix);
					material.SetMatrix("_GlobalProjector", _FinalClipMatrix);

					Graphics.DrawMesh(_ShadowReceivers[i].GetMesh(), _ModelMatrix, material, LayerMask.NameToLayer("Default"));
				}
			}
		}
	}
	
	void CheckTriggers(bool shadow) {
		if (!_shouldCheckTriggers) {
			return;
		}

		RaycastHit raycastHit;

		for (int n = 0; n < _ShadowTriggers.Count; n++) {
			if (Physics.Raycast(new Ray(_ShadowTriggers[n].transform.position, GlobalProjectorManager.GlobalProjectionDir), out raycastHit)) {
				if (_ShadowTriggers[n].DetectShadow && shadow) {
					SetTriggerTexPixel(_ProjectorCamera, _Tex, raycastHit.point, n);
				} else if (_ShadowTriggers[n].DetectLight && !shadow) {
					SetTriggerTexPixel(_ProjectorCameraLight, _TexLight, raycastHit.point, n);
				}
			}
		}

		_textureRead.Apply();

		Color[] pixels;
		pixels = _textureRead.GetPixels(0, 0, _ShadowTriggers.Count, 1);

		for (int n = 0; n < _ShadowTriggers.Count; n++) {
		
				if (_ShadowTriggers[n].DetectShadow && shadow) {
					_ShadowTriggers[n].OnTriggerCheckDone(pixels[n].a > 0.0f ? true : false);
				} else if (_ShadowTriggers[n].DetectLight && !shadow) {
					_ShadowTriggers[n].OnTriggerCheckDone(pixels[n].a > 0.0f ? true : false);
				}

		}
	}
	
	void ApplyTerrainTextureMatrix(ShadowReceiver receiver) {

		if (receiver._terrainMaterial != null) {
			receiver._terrainMaterial.SetMatrix("_GlobalProjector", _FinalMatrix);
			receiver._terrainMaterial.SetMatrix("_GlobalProjectorClip", _FinalClipMatrix);
		} 

		receiver.GetTerrain().materialTemplate = receiver._terrainMaterial;

	}

	void OnPreCull() {
		foreach (ShadowProjector shadowProjector in _ShadowProjectors) {
			shadowProjector.SetVisible(true);
			shadowProjector.OnPreRenderShadowProjector(_ProjectorCamera);
		}
	}

	void OnPostRender() {
		_Tex.GrabScreenIfNeeded();
		CheckTriggers(true);

		foreach (ShadowProjector shadowProjector in _ShadowProjectors) {
			shadowProjector.SetVisible(false);
		}
	}
	
	void OnLightPreCull() {		
		foreach (ShadowProjector lightProjector in _LightProjectors) {
			lightProjector.SetVisible(true);
			lightProjector.OnPreRenderShadowProjector(_ProjectorCameraLight);
		}
	}

	void OnLightPostRender() {
		_TexLight.GrabScreenIfNeeded();
		CheckTriggers(false);
		
		foreach (ShadowProjector lightProjector in _LightProjectors) {
			lightProjector.SetVisible(false);
		}
	}
}