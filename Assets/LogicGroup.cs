using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TriggerEdge {
	public Trigger output;

	[PopupAttribute("output")]
	public string @outputEvent = "None";


	public Trigger input;
	[PopupAttribute("input")]
	public string inputEvent;


};


//connect from certer node to other node
//[System.Serializable]
public class TriggerConnect  {
	public Trigger from;
	public List<TriggerEdge> to;
}


public class LogicGroup : MonoBehaviour {
	public List<Trigger> nodes;
	

	public TriggerEdge[] triggerEdges; 


		// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
