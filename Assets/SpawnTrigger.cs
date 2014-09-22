using UnityEngine;
using System.Collections;

public class SpawnTrigger : Trigger {
	public float Angle;

	public int Count;
	public float Duration;

	public enum ReleaseOrderEnum {
		Clockwise,

	}
	ReleaseOrderEnum ReleaseOrder;

	public enum GroupEnum {
		Monster,
	}
	public GroupEnum Group;

	public GameObject Resource;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
