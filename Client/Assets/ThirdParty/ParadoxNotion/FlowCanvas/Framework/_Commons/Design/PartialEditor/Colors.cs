#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ParadoxNotion.Design {

    [InitializeOnLoad]
	///
	public static class Colors {

		static Colors(){ Load(); }

		[InitializeOnLoadMethod]
		static void Load(){
			lightOrange = EditorGUIUtility.isProSkin? new Color(1, 0.9f, 0.4f)    : Color.white;
			lightBlue   = EditorGUIUtility.isProSkin? new Color(0.8f,0.8f,1)      : Color.white;
			lightRed    = EditorGUIUtility.isProSkin? new Color(1,0.5f,0.5f, 0.8f): Color.white;
		}

		///----------------------------------------------------------------------------------------------

		public static Color lightOrange{get; private set;}
		public static Color lightBlue{get; private set;}
		public static Color lightRed{get; private set;}

		public const string HEX_LIGHT = "eeeeee";
		public const string HEX_DARK = "333333";

		///----------------------------------------------------------------------------------------------

		///Convert Color to Hex.
		private static Dictionary<Color32, string> colorHexCache = new Dictionary<Color32, string>();
		public static string ColorToHex(Color32 color){
			if (!EditorGUIUtility.isProSkin){
				if (color == Color.white){
					return "#000000";
				}
			}
			string result;
			if (colorHexCache.TryGetValue(color, out result)){
				return result;
			}
			result = ("#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2")).ToUpper();
			return colorHexCache[color] = result;
		}
		 
		///Convert Hex to Color.
		private static Dictionary<string, Color> hexColorCache = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
		public static Color HexToColor(string hex){
			Color result;
			if (hexColorCache.TryGetValue(hex, out result)){
				return result;
			}
			if (hex.Length != 6){
				throw new System.Exception("Invalid length for hex color provided");
			}
			var r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			result = new Color32(r,g,b, 255);
			return hexColorCache[hex] = result;
		}

		///----------------------------------------------------------------------------------------------

		///A greyscale color
		public static Color Grey(float value){
			return new Color(value, value, value);
		}

		///The color with alpha
		public static Color WithAlpha(this Color color, float alpha){
			color.a = alpha;
			return color;
		}

		///----------------------------------------------------------------------------------------------

		///Return a color for a type.
		public static Color GetTypeColor(System.Type type){
			return UserTypePrefs.GetTypeColor(type);
		}

		///Return a string hex color for a type.
		public static string GetTypeHexColor(System.Type type){
			return UserTypePrefs.GetTypeHexColor(type);
		}

	}
}

#endif