#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace ParadoxNotion.Design {

    ///Will show value only if another field is equal to target
	public class ShowIfDrawer : AttributeDrawer<ShowIfAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			var targetField = context.GetType().GetField(attribute.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (targetField != null){
				var fieldValue = (int)System.Convert.ChangeType( targetField.GetValue(context), typeof(int) );
				if (fieldValue != attribute.checkValue){
					return instance; //return instance without any editor (thus hide it)
				}
			}
			return MoveNextDrawer();
		}
	}

    ///Will show in red if value is null or empty
	public class RequiredFieldDrawer : AttributeDrawer<RequiredFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			var isNull = instance == null || instance.Equals(null) || ( (instance is string) && string.IsNullOrEmpty((string)instance) );
			GUI.backgroundColor = isNull? Colors.lightRed : Color.white;
			instance = MoveNextDrawer();
			GUI.backgroundColor = Color.white;
			return instance;
		}
	}

    ///Will invoke a callback method when value change
	public class CallbackDrawer : AttributeDrawer<CallbackAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			var newValue = MoveNextDrawer();
			if ( !Equals(newValue, instance) ){
				var method = context.GetType().RTGetMethod(attribute.methodName);
				if (method != null){
					fieldInfo.SetValue(context, newValue); //manual set field before invoke
					method.Invoke(context, null);
				}				
			}
			return newValue;
		}
	}

	///----------------------------------------------------------------------------------------------

	///Will clamp float or int value to min
	public class MinValueDrawer : AttributeDrawer<MinValueAttribute> {
		public override object OnGUI(GUIContent content, object instance){
			instance = MoveNextDrawer();
			if (fieldInfo.FieldType == typeof(float)){
				instance = Mathf.Max( (float)instance, (float)attribute.min );
			}
			if (fieldInfo.FieldType == typeof(int)){
				instance = Mathf.Max( (int)instance, (int)attribute.min );
			}
			return instance;
		}
	}

	///----------------------------------------------------------------------------------------------

	///Will make float, int or string field show in a delayed control
	public class DelayedFieldDrawer : AttributeDrawer<DelayedFieldAttribute>{
		public override object OnGUI(GUIContent content, object instance){
			if (fieldInfo.FieldType == typeof(float)){
				return EditorGUILayout.DelayedFloatField(content, (float)instance);
			}
			if (fieldInfo.FieldType == typeof(int)){
				return EditorGUILayout.DelayedIntField(content, (int)instance);
			}
			if (fieldInfo.FieldType == typeof(string)){
				return EditorGUILayout.DelayedTextField(content, (string)instance);
			}
			return MoveNextDrawer();
		}
	}

    ///Will force to use ObjectField editor, usefull for interfaces
	public class ForceObjectFieldDrawer : AttributeDrawer<ForceObjectFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			if (typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType) || fieldInfo.FieldType.IsInterface){
				return EditorGUILayout.ObjectField(content, instance as UnityEngine.Object, fieldInfo.FieldType, true);
			}
			return MoveNextDrawer();
		}
	}

    ///Will restrict selection on provided values
	public class PopupFieldDrawer : AttributeDrawer<PopupFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			if (attribute.options != null){
				return EditorUtils.Popup<object>(content, instance, attribute.options.ToList());
			}
			return MoveNextDrawer();
        }
    }

	///Will show a slider for int and float values
    public class SliderFieldDrawer : AttributeDrawer<SliderFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
            if (fieldInfo.FieldType == typeof(float)){
				return EditorGUILayout.Slider(content, (float)instance, (float)attribute.min, (float)attribute.max);
			}
			if (fieldInfo.FieldType == typeof(int)){
				return EditorGUILayout.IntSlider(content, (int)instance, (int)attribute.min, (int)attribute.max);
			}
			return MoveNextDrawer();
        }
    }

    ///Will show a layer selection for int values
	public class LayerFieldDrawer : AttributeDrawer<LayerFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			if (fieldInfo.FieldType == typeof(int)){
				return EditorGUILayout.LayerField(content, (int)instance);
			}
			return MoveNextDrawer();
        }
    }

    ///Will show a Tag selection for string values
	public class TagFieldDrawer : AttributeDrawer<TagFieldAttribute> {
        public override object OnGUI(GUIContent content, object instance) {
			if (fieldInfo.FieldType == typeof(string)){
				return EditorGUILayout.TagField(content, (string)instance);
			}
			return MoveNextDrawer();
        }
    }

    ///Will show a text area for string values
	public class TextAreaDrawer : AttributeDrawer<TextAreaFieldAttribute> {
		private static GUIStyle areaStyle;
		static TextAreaDrawer(){
			areaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
			areaStyle.wordWrap = true;			
		}
        public override object OnGUI(GUIContent content, object instance) {
			if (fieldInfo.FieldType == typeof(string)){
				GUILayout.Label(content);
				return EditorGUILayout.TextArea((string)instance, areaStyle, GUILayout.Height(attribute.numberOfLines * areaStyle.lineHeight));
			}
			return MoveNextDrawer();
        }
    }
}

#endif