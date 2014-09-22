using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileManager))]
public class TileEditor : Editor {

	void OnSceneGUI()
	{

		Event e = Event.current;
        /*
		switch(e.type)
		{

		}
		Debug.Log("event type "+e.type);
		*/
		Handles.BeginGUI();
		GUILayout.Window(2, new Rect(100, 100, 100, 100), (id) => {
			if(GUILayout.Button("Order all Block"))
			{
				Debug.Log("target "+target);
				TileManager g = (TileManager)target;
				var count = 0;
				foreach(Transform t in g.transform)
				{
					count++;
					t.position = Vector3.zero;
					foreach(Transform child in t)
					{
						float x = child.position.x;
						int xc = Mathf.RoundToInt(x/4.0f)*4;
						float z = child.position.z;
						int zc = Mathf.RoundToInt(z/4.0f)*4;

						float y = child.position.y;
						child.position = new Vector3(xc, y, zc);
					}
				}
				Debug.Log(count);
			}

		}, "Title");
		Handles.EndGUI();
        

		if(Event.current.type == EventType.KeyDown)
		{
			Debug.Log(Event.current.keyCode);


			if(Event.current.keyCode == KeyCode.UpArrow)
			{
				Debug.Log("UpArrow");

			}
		}
	}
}
