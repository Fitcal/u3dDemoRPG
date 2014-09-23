using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.Linq;

namespace ChuMeng {
//[CustomPropertyDrawer(typeof(TriggerEdge))]
public class TriggerEdgeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		//EditorGUI.BeginProperty(position, label, property);
		//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		Rect textFieldPosition = position;

		PopOutputTrigger(position, property);

		//EditorGUI.EndProperty();
		/*

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		var fromRect = new Rect(position.x, position.y, 30, position.height);
		EditorGUI.indentLevel = 1;
		var toRect = new Rect(position.x, position.y, )

		EditorGUI.PropertyField(fromRect, property.FindPropertyRelative("from"), GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
		*/
	}

	void PopOutputTrigger(Rect position, SerializedProperty property)
	{
		var path = property.propertyPath.Replace(".Array.data[", "[");
		Debug.Log(path);
		object obj = property.serializedObject.targetObject;

		var elements = path.Split('.');
		foreach(var element in elements.Take(elements.Length-1))
		{
			if(element.Contains("["))
			{
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
				obj = GetValue(obj, elementName, index);
			}
			else
			{
				obj = GetValue(obj, element);
			}
		}


		//TriggerEdge out
		TriggerEdge te = (TriggerEdge) obj;
		if(te.output != null) {
		}

		//property.serializedObject.targetObject;

		/*
		var t = (TriggerEdge)property.objectReferenceValue;
		if(t.output != null) {
			var o = t.output.GetOutputTrigger();
			EditorGUI.Popup(position, 0, o.ToArray());
		}
		(/
		/*
		Trigger f = ((TriggerConnect)(property.serializedObject.targetObject)).from;

		var o = f.GetOutputTrigger();

		EditorGUI.Popup(position, 0, o.ToArray());
*/
	}

	public object GetValue(object source, string name)
	{
		if(source == null)
			return null;

		var type = source.GetType();
		var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if(f == null)
		{
			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if(p == null)
				return null;
			return p.GetValue(source, null);
		}
		return f.GetValue(source);

	}

	public object GetValue(object source, string name, int index)
	{
		var enumerable = GetValue(source, name) as IEnumerable;
		var enm = enumerable.GetEnumerator();
		while(index-- >= 0)
		{
			enm.MoveNext();
		}
		return enm.Current;
	}

}

}