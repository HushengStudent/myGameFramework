using UnityEngine;
using System.Collections;

[AddComponentMenu("Fast Shadow Projector/Shadow Projector")]
public class ShadowProjector : MonoBehaviour {
	
	private static class MeshGen {
		
		public static Mesh CreatePlane(Vector3 up, Vector3 right, Rect uvRect, Color color, ShadowProjector parent) {
			Mesh planeMesh = new Mesh();

			bool shouldFlipX = (GlobalProjectorManager.Exists() ? GlobalProjectorManager.GlobalFlipX : parent.GlobalFlipX);
			Vector2 flipX = new Vector2(uvRect.width, -uvRect.width);
			
			bool shouldFlipY = (GlobalProjectorManager.Exists() ? GlobalProjectorManager.GlobalFlipY : parent.GlobalFlipY);
			Vector2 flipY = new Vector2(-uvRect.height, uvRect.height);
			
			Vector3[] vertices = new Vector3[] {
				(up * 0.5f - right * 0.5f),
				(up * 0.5f + right * 0.5f),
				(-up * 0.5f - right * 0.5f),
				(-up * 0.5f + right * 0.5f)
			};
			
			Vector2[] uvs = new Vector2[] {
				new Vector2(uvRect.x, uvRect.y + uvRect.height),
				new Vector2(uvRect.x + uvRect.width, uvRect.y + uvRect.height),
				new Vector2(uvRect.x, uvRect.y),
				new Vector2(uvRect.x + uvRect.width, uvRect.y),
			};
			
			Color[] colors = new Color[] {
				color,
				color,
				color,
				color
			};
			
			int[] indices = new int[] { 0, 1, 3, 0, 3, 2 };

			if (shouldFlipX) {
				uvs[0].x += flipX[0];
				uvs[1].x += flipX[1];
				uvs[2].x += flipX[0];
				uvs[3].x += flipX[1];
			}

			if (shouldFlipY) {
				uvs[0].y += flipY[0];
				uvs[1].y += flipY[0];
				uvs[2].y += flipY[1];
				uvs[3].y += flipY[1];
			}
			
			planeMesh.vertices = vertices;
			planeMesh.uv = uvs;
			planeMesh.colors = colors;
			planeMesh.SetTriangles(indices, 0);
			
			return planeMesh;
		}
	}
	
	public Vector3 GlobalProjectionDir {
		set {
			_GlobalProjectionDir = value;
			
			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalProjectionDir = _GlobalProjectionDir; 
			}
		}
		get {
			return _GlobalProjectionDir;
		}
	}
	
	[UnityEngine.SerializeField]
	protected Vector3 _GlobalProjectionDir = new Vector3(0.0f, -1.0f, 0.0f);
	
	public int GlobalShadowResolution {
		set {
			_GlobalShadowResolution = value;
			
			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalShadowResolution = _GlobalShadowResolution; 
			}
		}
		get {
			return _GlobalShadowResolution;
		}
	}
	
	[UnityEngine.SerializeField]
	protected int _GlobalShadowResolution = 1;
	
	public GlobalProjectorManager.ProjectionCulling GlobalShadowCullingMode {
		set {
			_GlobalShadowCullingMode = value;
			
			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalShadowCullingMode = _GlobalShadowCullingMode; 
			}
		}
		get {
			return _GlobalShadowCullingMode;
		}
	}
	
	[UnityEngine.SerializeField]
	protected GlobalProjectorManager.ProjectionCulling _GlobalShadowCullingMode;
	
	public bool EnableCutOff {
		set {
			if (_EnableCutOff != value) {
				_EnableCutOff = value;
			}
		}
		
		get {
			return _EnableCutOff;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _EnableCutOff = false;
	
	
	public float GlobalCutOffDistance {
		set {
			_GlobalCutOffDistance = value;
			
			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalCutOffDistance = _GlobalCutOffDistance; 
			}
		}
		get {
			return _GlobalCutOffDistance;
		}
	}
	
	[UnityEngine.SerializeField]
	protected float _GlobalCutOffDistance;

	public bool GlobalFlipX {
		set {
			_GlobalFlipX = value;

			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalFlipX = _GlobalFlipX; 
			}
		}
		get {
			return _GlobalFlipX;
		}
	}
	
	[UnityEngine.SerializeField]
	protected bool _GlobalFlipX;

	public bool GlobalFlipY {
		set {
			_GlobalFlipY = value;
			
			if (GlobalProjectorManager.Exists()) {
				GlobalProjectorManager.GlobalFlipY = _GlobalFlipY; 
			}
		}
		get {
			return _GlobalFlipY;
		}
	}
	
	[UnityEngine.SerializeField]
	protected bool _GlobalFlipY;
	
	public float ShadowSize {
		set {
			if (_ShadowSize != value) {
				_ShadowSize = value;
				if (_ShadowDummyMesh != null) {
					OnShadowSizeChanged();
				}
			}
		}
		
		get {
			return _ShadowSize;
		}
	}
	
	[UnityEngine.SerializeField]
	float _ShadowSize = 1.0f;
	
	public Color ShadowColor {
		set {
			if (_ShadowColor != value) {
				_ShadowColor = value;
				if (_ShadowDummyMesh != null) {
					OnShadowColorChanged();
				}
			}
		}
		
		get {
			return _ShadowColor;
		}
	}
	
	[UnityEngine.SerializeField]
	Color _ShadowColor = new Color(1.0f, 1.0f, 1.0f);
	
	public float ShadowOpacity {
		set {
			if (_ShadowOpacity != value) {
				_ShadowOpacity = value;
				if (_ShadowDummyMesh != null) {
					OnShadowColorChanged();
				}
			}
		}
		
		get {
			return _ShadowOpacity;
		}
	}
	
	[UnityEngine.SerializeField]
	float _ShadowOpacity = 1.0f;
	
	public Material _Material; 
	
	public bool IsLight {
		set {
			_IsLight = value;
		}
		
		get {
			return _IsLight;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _IsLight;
	
	
	public Vector3 ShadowLocalOffset {
		set {
			_ShadowLocalOffset = value;
			
			if (_ShadowDummy != null) {
				_ShadowDummy._ShadowLocalOffset = _ShadowLocalOffset;
			}
		}
		
		get {
			return _ShadowLocalOffset;
		}
	}
	
	[UnityEngine.SerializeField]
	Vector3 _ShadowLocalOffset;
	
	public Quaternion RotationAngleOffset {
		set {
			_RotationAngleOffset = value;
			
			if (_ShadowDummy != null) {
				_ShadowDummy._RotationAngleOffset = _RotationAngleOffset;
			}
		}
		
		get {
			return _RotationAngleOffset;
		}
	}
	
	[UnityEngine.SerializeField]
	Quaternion _RotationAngleOffset;
	
	// Freeze X Rotation ----------------------------------------------------
	
	public bool FreezeXRot {
		set {
			_FreezeXRot = value;
			
			if (_ShadowDummy != null) {
				_ShadowDummy._freezeXRot = _FreezeXRot;
			}
		}
		
		get {
			return _FreezeXRot;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _FreezeXRot = true;
	
	// Freeze Y Rotation ----------------------------------------------------
	
	public bool FreezeYRot {
		set {
			_FreezeYRot = value;
			
			if (_ShadowDummy != null) {
				_ShadowDummy._freezeYRot = _FreezeYRot;
			}
		}
		
		get {
			return _FreezeYRot;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _FreezeYRot = false;
	
	// Freeze Z Rotation ----------------------------------------------------
	
	public bool FreezeZRot {
		set {
			_FreezeZRot = value;
			
			if (_ShadowDummy != null) {
				_ShadowDummy._freezeZRot = _FreezeZRot;
			}
		}
		
		get {
			return _FreezeZRot;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _FreezeZRot = true;
	
	
	// UV RECT ----------------------------------------------------
	
	public Rect UVRect {
		set {
			_UVRect = value;
			if (_ShadowDummy != null) {
				OnUVRectChanged();
			}
		}
		
		get {
			return _UVRect;
		}
	}
	
	[UnityEngine.SerializeField]
	Rect _UVRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
	
	// Auto Size/Opacity ----------------------------------------------------
	
	public bool AutoSizeOpacity {
		set {
			_AutoSizeOpacity = value;
		}
		
		get {
			return _AutoSizeOpacity;
		}
	}
	
	[UnityEngine.SerializeField]
	bool _AutoSizeOpacity = false;
	
	// Auto Size/Opacity CutOff Distance -------------------------------------------------
	
	public float AutoSOCutOffDistance {
		set {
			_AutoSOCutOffDistance = value;
		}
		
		get {
			return _AutoSOCutOffDistance;
		}
	}
	
	[UnityEngine.SerializeField]
	float _AutoSOCutOffDistance = 10.0f;

	// Auto Size/Opacity Ray Origin Offset -------------------------------------------------
	
	public float AutoSORayOriginOffset {
		set {
			_AutoSORayOriginOffset = value;
		}
		
		get {
			return _AutoSORayOriginOffset;
		}
	}
	
	[UnityEngine.SerializeField]
	float _AutoSORayOriginOffset = 0.0f;
	
	
	// Auto Size/Opacity Max Scale Multiplier -------------------------------------------------
	public float AutoSOMaxScaleMultiplier {
		set {
			_AutoSOMaxScaleMultiplier = value;
		}
		
		get {
			return _AutoSOMaxScaleMultiplier;
		}
	}
	
	[UnityEngine.SerializeField]
	float _AutoSOMaxScaleMultiplier = 2.0f;
	
	// Auto Size/Opacity Layer -------------------------------------------------
	public int AutoSORaycastLayer {
		set {
			_AutoSORaycastLayer = value;
		}
		
		get {
			return _AutoSORaycastLayer;
		}
	}
	
	[UnityEngine.SerializeField]
	int _AutoSORaycastLayer = 0;
	
	MeshRenderer _Renderer;
	MeshFilter _MeshFilter;
	Mesh _ShadowDummyMesh;
	
	ProjectorShadowDummy _ShadowDummy;
	
	float _initialSize;
	float _initialOpacity;

	bool _discarded;
	
	void Awake() {
		_ShadowDummyMesh = MeshGen.CreatePlane(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), _UVRect, 
		                                       new Color(_ShadowColor.r, _ShadowColor.g, _ShadowColor.b, _ShadowOpacity), this);
		
		Transform parent = transform;
		
		_ShadowDummy = new GameObject("shadowDummy").AddComponent<ProjectorShadowDummy>();
		_ShadowDummy.transform.parent = parent;
		_ShadowDummy.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		_ShadowDummy.transform.localRotation = Quaternion.identity * parent.localRotation; 
		
		_ShadowDummy.gameObject.layer = LayerMask.NameToLayer(GlobalProjectorManager.GlobalProjectorLayer);
		
		_ShadowDummy._ShadowLocalOffset = _ShadowLocalOffset;
		_ShadowDummy._RotationAngleOffset = _RotationAngleOffset;
		
		_ShadowDummy._freezeXRot = _FreezeXRot;
		_ShadowDummy._freezeYRot = _FreezeYRot;
		_ShadowDummy._freezeZRot = _FreezeZRot;
		
		OnShadowSizeChanged();
		
		_Renderer = _ShadowDummy.gameObject.AddComponent<MeshRenderer>();
		_Renderer.receiveShadows = false;
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 
		_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#else
		_Renderer.castShadows = false;
#endif
		_Renderer.material = _Material;
		_Renderer.enabled = false;
		
		_MeshFilter = _ShadowDummy.gameObject.AddComponent<MeshFilter>();
		_MeshFilter.mesh = _ShadowDummyMesh;
		
		_initialSize = _ShadowSize;
		_initialOpacity = _ShadowOpacity;

		_discarded = false;
	}
	
	void Start () {	
		GlobalProjectorManager.Get().AddProjector(this);
		_Renderer.enabled = true;
	}
	
	void OnEnable() {
		GlobalProjectorManager.Get().AddProjector(this);
		_Renderer.enabled = true;
	}
	
	void OnDisable() {
		if (GlobalProjectorManager.Exists()) {
			GlobalProjectorManager.Get().RemoveProjector(this);
			if (_ShadowDummy != null) {
				_Renderer.enabled = false;
			}
		}
	}
	
	void OnDestroy() {
		if (GlobalProjectorManager.Exists()) {
			GlobalProjectorManager.Get().RemoveProjector(this);
		}
	}
	
	public Bounds GetBounds() {
		return _Renderer.bounds;
	}
	
	public bool IsVisible() {
		return _Renderer.isVisible;
	}
	
	public void SetVisible(bool visible) {
		_Renderer.enabled = visible;
	}

	public void Discard(bool discard) {
		_discarded = discard;
		SetVisible(!discard);
	}

	public bool IsDiscarded() {
		return _discarded;
	}
	
	void Update() {
		if (_AutoSizeOpacity) {
			RaycastHit hitInfo;
			bool hit = Physics.Raycast(new Ray(transform.position, _GlobalProjectionDir), out hitInfo, _AutoSOCutOffDistance, 1 << _AutoSORaycastLayer);
			
			if (hit) {
				float opacity = 1.0f - Mathf.Min(_AutoSOCutOffDistance, Mathf.Max(0, hitInfo.distance - _AutoSORayOriginOffset)) / _AutoSOCutOffDistance;
				float scaleMultiplier = Mathf.Lerp(_AutoSOMaxScaleMultiplier, 1.0f, opacity);
				
				ShadowSize = _initialSize * scaleMultiplier;
				ShadowOpacity = _initialOpacity * opacity;
			} else {
				ShadowOpacity = 0.0f;
			}
		}	
	}
	
	public void OnPreRenderShadowProjector(Camera camera) {
		if (_ShadowDummy != null) {
			_ShadowDummy.OnPreRenderShadowDummy(camera);
		}
	}
	
	public Matrix4x4 ShadowDummyLocalToWorldMatrix() {
		return _ShadowDummy.transform.localToWorldMatrix;
	}

	public float GetShadowWorldSize() {
		Matrix4x4 dummyMatrix = ShadowDummyLocalToWorldMatrix();
		return Mathf.Max ((dummyMatrix * new Vector3(1.0f, 0.0f, 0.0f)).magnitude, (dummyMatrix * new Vector3(0.0f, 1.0f, 0.0f)).magnitude);
	}

	public Vector3 GetShadowPos() {
		return _ShadowDummy.transform.position;
	}
	
	void OnShadowSizeChanged() {
		_ShadowDummy.transform.localScale = new Vector3(_ShadowSize, _ShadowSize, _ShadowSize);
	}
	
	public void OnUVRectChanged() {
		RebuildMesh();
	}
	
	public void OnShadowColorChanged() {
		Color color = new Color(_ShadowColor.r, _ShadowColor.g, _ShadowColor.b, _ShadowOpacity);
		_ShadowDummyMesh.colors = new Color[] { color, color, color, color };
	}
	
	void RebuildMesh() {
		_ShadowDummyMesh = MeshGen.CreatePlane(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), _UVRect,
		                                       new Color(_ShadowColor.r, _ShadowColor.g, _ShadowColor.b, _ShadowOpacity), this);
		_MeshFilter.mesh = _ShadowDummyMesh;
	}
}

