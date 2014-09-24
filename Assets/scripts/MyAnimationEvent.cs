using UnityEngine;
using System.Collections;

namespace ChuMeng
{
	/*
	 * mailbox receive other send message
	 * message dispatch   registerListener notify listener  push
	 * other checkMessage     pull
	 */

	public class MyAnimationEvent : MonoBehaviour
	{
		[HideInInspector]
		public bool hit = false;

		[HideInInspector]
		public bool onHit = false;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void HIT ()
		{
			hit = true;	
		}

		public void OnHit(GameObject go) {
			onHit = true;
		}

	}

}