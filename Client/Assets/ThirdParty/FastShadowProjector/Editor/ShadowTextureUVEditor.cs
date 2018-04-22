using UnityEngine;
using UnityEditor;

class ShadowTextureUVEditor:  EditorWindow {

	public ShadowProjector _shadowProjector;
	public float _zoomLevel = 1.0f;

	public static void Open(ShadowProjector shadowProjector) {
		ShadowTextureUVEditor window = (ShadowTextureUVEditor)EditorWindow.GetWindow (typeof (ShadowTextureUVEditor));
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 
		window.titleContent = new GUIContent("UV Editor");
#else
		window.title = "UV Editor";
#endif


		window._shadowProjector = shadowProjector;
		window._zoomLevel = 1.0f;

		window.Show();
	}
	
	void OnGUI () {

		if (_shadowProjector != null && _shadowProjector._Material != null && _shadowProjector._Material.mainTexture != null) {
			Texture shadowTex = _shadowProjector._Material.mainTexture;

			EditorGUILayout.LabelField("Shadow texture UV:");
			EditorGUILayout.HelpBox("Tip: Hold ALT/OPTION and drag labels X/Y/W/H for precise adjusting. Hold SHIFT to adjust values in bigger steps.", MessageType.Info);
			_shadowProjector.UVRect = EditorGUILayout.RectField(	_shadowProjector.UVRect);

			Rect rectClamped = _shadowProjector.UVRect;

			rectClamped.width = Mathf.Clamp(_shadowProjector.UVRect.width, 0.0f, 1.0f);
			rectClamped.height = Mathf.Clamp(_shadowProjector.UVRect.height, 0.0f, 1.0f);

			_shadowProjector.UVRect = rectClamped;

			EditorGUILayout.LabelField("Zoom: " + (int)(_zoomLevel * 100) + "%");
			_zoomLevel = EditorGUILayout.Slider(_zoomLevel, 0.0f, 2.0f);

			Vector2 imagePos = new Vector2(20, 150);
			Rect imageRect = new Rect(imagePos.x, imagePos.y, shadowTex.width*_zoomLevel, shadowTex.height*_zoomLevel);

			EditorGUI.DrawRect(imageRect, new Color(0.7f, 0.7f, 0.7f, 0.7f));
			EditorGUI.DrawPreviewTexture(imageRect, shadowTex, _shadowProjector._Material, ScaleMode.ScaleToFit);
			DrawZones(shadowTex, new Rect(_shadowProjector.UVRect.x, 1.0f -_shadowProjector.UVRect.y, _shadowProjector.UVRect.width, _shadowProjector.UVRect.height), imagePos);

			if (GUI.changed) {
				EditorUtility.SetDirty(_shadowProjector);
			}

		}
	}

	void DrawZones(Texture tex, Rect rect, Vector2 offset) {
		float x1 = rect.x;
		float x2 = rect.x + rect.width;

		float y1 = rect.y;
		float y2 = rect.y - rect.height;

		if ((Mathf.Floor(x1) == Mathf.Floor(x2)) && (Mathf.Floor(y1) == Mathf.Floor(y2))) {
			Rect newRect = new Rect(rect);
			x1 = x1 - Mathf.Floor(x1);
			y1 = y1 - Mathf.Floor(y1);
			newRect.x = x1;
			newRect.y = y1;

			DrawRect(tex, newRect, offset);
		} else {
			if ((Mathf.Floor(x1) != Mathf.Floor(x2)) && (Mathf.Floor(y1) != Mathf.Floor(y2))) {
				float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
				float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));

				float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
				float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
			
				Rect rect1 = new Rect(rect);
				rect1.y = newRectY1;
				rect1.x = 0;
				rect1.height = newRectY1;
				rect1.width = newRectX1;
				
				DrawRect(tex, rect1, offset);

				rect1.y = newRectY1;
				rect1.x = newRectX2;
				rect1.height = newRectY1;
				rect1.width = 1.0f - newRectX2;;
				
				DrawRect(tex, rect1, offset);

				Rect rect2 = new Rect(rect);
				rect2.y = 1.0f;
				rect2.x = newRectX2;
				rect2.height = 1.0f - newRectY2;
				rect2.width = 1.0f - newRectX2;
				
				DrawRect(tex, rect2, offset);

				rect2.y = 1.0f;
				rect2.x = 0.0f;
				rect2.height = 1.0f - newRectY2;
				rect2.width = newRectX1;
				
				DrawRect(tex, rect2, offset);
			} else {

				if (Mathf.Floor(x1) !=Mathf.Floor(x2)) {
					float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
					float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));

					float newY = y1 - Mathf.Floor(y1);

					Rect rect1 = new Rect(rect);
					rect1.x = 0;
					rect1.y = newY;
					rect1.width = newRectX1;

					DrawRect(tex, rect1, offset);

					Rect rect2 = new Rect(rect);
					rect2.x = newRectX2;
					rect2.y = newY;
					rect2.width = 1.0f - newRectX2;
					
					DrawRect(tex, rect2, offset);
				} 

				if (Mathf.Floor(y1) != Mathf.Floor(y2)) {
					float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
					float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));

					
					float newX = x1 - Mathf.Floor(x1);

					Rect rect1 = new Rect(rect);
					rect1.y = newRectY1;
					rect1.x = newX;
					rect1.height = newRectY1;
					
					DrawRect(tex, rect1, offset);
					
					Rect rect2 = new Rect(rect);
					rect2.y = 1.0f;
					rect2.x = newX;
					rect2.height = 1.0f - newRectY2;
					
					DrawRect(tex, rect2, offset);
				}
			}
		}
	}

	void DrawRect(Texture tex, Rect rect, Vector2 offset) {
		Rect clampedRect = new Rect(rect);

		clampedRect.x = Mathf.Clamp(clampedRect.x, 0.0f, 1.0f);
		clampedRect.y = Mathf.Clamp(clampedRect.y, 0.0f, 1.0f);
		clampedRect.width = Mathf.Clamp(clampedRect.width, 0.0f, 1.0f);
		clampedRect.height = Mathf.Clamp(clampedRect.height, 0.0f, 1.0f);

		float endCoordX = clampedRect.x + clampedRect.width;
		if (endCoordX > 1.0f) {
			clampedRect.width = 1.0f - clampedRect.x;
		}

		float endCoordY = clampedRect.y - clampedRect.height;
		if (endCoordY < 0.0f) {
			clampedRect.height = clampedRect.y;
		}

		EditorGUI.DrawRect(new Rect(clampedRect.x * tex.width * _zoomLevel+ offset.x,
		                            clampedRect.y * tex.height * _zoomLevel + offset.y,
		                            clampedRect.width * tex.width * _zoomLevel, 
		                            1.0f), 
		                   			new Color(0.5f, 0.9f, 0.0f));


		EditorGUI.DrawRect(new Rect(clampedRect.x * tex.width * _zoomLevel + clampedRect.width * tex.width * _zoomLevel + offset.x,
		                            clampedRect.y * tex.height * _zoomLevel+ offset.y,
		                            1.0f,
		                            -clampedRect.height * tex.height * _zoomLevel),  new Color(0.5f, 0.9f, 0.0f));


		EditorGUI.DrawRect(new Rect(clampedRect.x * tex.width * _zoomLevel+ offset.x, 
		                            clampedRect.y * tex.height * _zoomLevel -  clampedRect.height * tex.height * _zoomLevel+ offset.y,
		                            clampedRect.width * tex.width * _zoomLevel, 
		                            -1.0f ), 
		                  			 new Color(0.5f, 0.9f, 0.0f));

		EditorGUI.DrawRect(new Rect(clampedRect.x * tex.width * _zoomLevel+ offset.x,
		                            clampedRect.y * tex.height * _zoomLevel + offset.y,
		                            1.0f,
		                            -clampedRect.height * tex.height * _zoomLevel),  new Color(0.5f, 0.9f, 0.0f));
	}
}