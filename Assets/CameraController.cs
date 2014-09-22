using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public float offsetX = -5;
	public float offsetZ = 0;
	public float maxinumDistance = 2;
	public float playerVelocity = 10;

	private float movementX;
	private float movementZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		movementX = (player.transform.position.x+offsetX-this.transform.position.x)/maxinumDistance;
		movementZ = (player.transform.position.z+offsetZ-this.transform.position.z)/maxinumDistance;
		this.transform.position += new Vector3(movementX*playerVelocity*Time.deltaTime, 0, movementZ*playerVelocity*Time.deltaTime);

	}
}
