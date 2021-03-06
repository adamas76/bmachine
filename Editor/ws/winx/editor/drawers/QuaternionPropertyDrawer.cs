using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.attributes;

namespace ws.winx.editor.drawers
{
	[CustomPropertyDrawer (typeof(QuaternionPropertyAttribute))]
	[CustomPropertyDrawer (typeof(Quaternion))]
	public class QuaternionPropertyDrawer:PropertyDrawer
	{

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{

				return 32f;// base.GetPropertyHeight (property, label);
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{


			Quaternion value = property.quaternionValue;
			Vector4 vector = new Vector4 (value.x, value.y, value.z, value.w);

			//if(Event.current.type==EventType.Layout)
			//GUILayoutUtility.GetRect (position.width, 16f);


			EditorGUI.BeginProperty (position, label, property);

			EditorGUI.BeginChangeCheck ();
			Vector4 vector2 = EditorGUI.Vector4Field(position, label.text, vector);
			if (EditorGUI.EndChangeCheck () && vector != vector2) {
						property.quaternionValue=new Quaternion (vector2.x, vector2.y, vector2.z, vector2.w);
			}

			EditorGUI.EndProperty ();
		}	
	}
}

