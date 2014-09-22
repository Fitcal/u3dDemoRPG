using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;


//getPopList from target Object  GetOutPutTri
public class PopupAttribute : PropertyAttribute {
	public List<string> popList;
	public string name;
	public PopupAttribute(string n)
	{
		name = n;
	}

}


[CustomPropertyDrawer(typeof(PopupAttribute))]
public class PopupDrawer : PropertyDrawer{
	const int textHeight = 16;

	PopupAttribute popupAttribute {
		get{ return (PopupAttribute)attribute; }
	}

	public override float GetPropertyHeight(SerializedProperty prop,
	                                        GUIContent label)
	{
		return base.GetPropertyHeight(prop, label);
	}

	public override void OnGUI(Rect position, 
	                           SerializedProperty prop,
	                           GUIContent label
	                           )
	{
		Rect textFieldPosition = position;
		textFieldPosition.height = textHeight;
		//DrawTextField(textFieldPosition, prop, label);
		DrawPopupField(position, prop, label);
	}
	public object GetParent(SerializedProperty prop)
	{
		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
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
		return obj;
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

	void DrawPopupField(Rect position, SerializedProperty prop, GUIContent label)
	{
		object obj = prop.serializedObject.targetObject;

		Debug.Log("obj is who? "+obj);
		var path = prop.propertyPath.Replace(".Array.data[", "[");
		Debug.Log(path);

		obj = GetParent(prop);

		obj = GetValue(obj, popupAttribute.name);
		if(obj != null) {
			var tr = (Trigger)obj;
			List<string> o;
			if(popupAttribute.name == "output"){
				o = tr.GetOutputTrigger();
			}else {
				o = tr.GetInputTrigger();
			}
			List<GUIContent> gc = new List<GUIContent>();
			foreach(var i in o)
			{
				gc.Add(new GUIContent(i));
			}
			
			//GUIContent[] t = new GUIContent[]{new GUIContent("Test")};
			EditorGUI.Popup(position, label, 0, gc.ToArray());
		}else {
			GUIContent[] t = new GUIContent[]{new GUIContent("None")};
			EditorGUI.Popup(position, label, 0, t);
		}


	}
	void DrawTextField(Rect position, SerializedProperty prop, GUIContent label) 
	{
		EditorGUI.BeginChangeCheck();
		string value = EditorGUI.TextField(position, label, prop.stringValue);
		if(EditorGUI.EndChangeCheck()){
			prop.stringValue = value;
		}
	}
}