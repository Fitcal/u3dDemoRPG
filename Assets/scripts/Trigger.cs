using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ChuMeng {

	public class Trigger : MonoBehaviour {
		public List<string> OutputTrigger;
		public List<string> InputTrigger;
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public virtual List<string> GetOutputTrigger()
		{
			return OutputTrigger;
		}

		public virtual List<string> GetInputTrigger()
		{
			return InputTrigger;
		}
	}

}
