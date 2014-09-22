using UnityEngine;
using System.Collections;


public class SetShadowPlane : MonoBehaviour {
	public GameObject plane;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if(null != plane) {
			//renderer.sharedMaterial.SetMatrix("_World2Receiver", plane.renderer.worldToLocalMatrix);
		//Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, Time.time*-1, 0), Quaternion.Euler(new Vector3(0, 0, 0)), Vector3.one);
		//renderer.sharedMaterial.SetMatrix("_World2Receiver", mat);

		renderer.sharedMaterial.SetMatrix("_World2Receiver", 
		                                  plane.renderer.worldToLocalMatrix);


		//renderer.sharedMaterial.SetMatrix("_World2Receiver", plane.renderer.worldToLocalMatrix);

		/*
		for(int i = 0; i < renderer.materials.Length; i++)
		{
			//renderer.materials[i].SetMatrix("_World2Receiver", plane.renderer.worldToLocalMatrix);
			renderer.materials[i].SetMatrix("_World2Receiver", mat);

		}
		*/
		/*
			foreach(Material mat in renderer.materials) {
				mat.SetMatrix("_World2Receiver", plane.renderer.worldToLocalMatrix);
			}
		*/
			
		//}
	}
}
