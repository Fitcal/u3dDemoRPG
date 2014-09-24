using UnityEngine;
using System.Collections;
namespace ChuMeng {
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(MyAnimationEvent))]

	public class NpcAI : MonoBehaviour {
		CharacterState _characterState;

		public float Radius = 5;
		public float ApproachDistance = 4;

		bool inStunned = false;


		public float AttackRange = 1.0f;


		public float WalkSpeed = 0.5f;
		public float RunSpeed = 5;
		public float FastRotateSpeed = 10;


		public float directionChangeInterval = 1;
		public float maxHeadingChange = 30;
		MyAnimationEvent myAnimationEvent;


		CharacterController controller;
		float heading;
		Vector3 targetRotation;

		Vector3 birthPoint;

		GameObject targetPlayer;

		//inside birth circle
		void Awake() {
			myAnimationEvent = GetComponent<MyAnimationEvent>();
			_characterState = CharacterState.Idle;

			controller = GetComponent<CharacterController>();
			heading = Random.Range(0, 360);
			transform.eulerAngles = new Vector3(0, heading, 0);
			StartCoroutine(NewHeading());
			StartCoroutine(FindTarget());
			//StartCoroutine(CheckOnHit());
		}
		// Use this for initialization
		void Start () {
			birthPoint = transform.position;
			birthPoint.y = 0;
		}

		IEnumerator WaitForAnimation(Animation animation) {
			do {
				yield return null;
			}while(animation.isPlaying);
		}

		void OnControllerColliderHit(ControllerColliderHit hit) {
			Debug.Log("hit gameobject "+hit.gameObject);
		}

		//swallow onHit
		bool CheckOnHit() {
			if(myAnimationEvent.onHit) {
				myAnimationEvent.onHit = false;
				return true;
			}
			return false;
		}


		IEnumerator Stunned() {
			Debug.Log("Stunned");
            if(inStunned) {
				animation.Rewind("stunned");
			}else {
				_characterState = CharacterState.Stunned;

				animation.Play("stunned");
				animation["stunned"].wrapMode = WrapMode.Once;
				animation["stunned"].speed = 2;
				inStunned = true;
				
				yield return StartCoroutine(WaitForAnimation(animation));
				_characterState = CharacterState.Idle;
				inStunned = false;
			}
			yield return null;
		}



		IEnumerator AroundMoveAttack(){
			_characterState = CharacterState.Around;


			targetPlayer = GameObject.FindGameObjectWithTag("Player");

			while(true) {
				if(CheckOnHit()) {
					yield return StartCoroutine(Stunned());
					yield break;
				}

				float passTime = 0;
				animation.CrossFade("run");
				animation["run"].speed = 2f;
				animation["run"].wrapMode = WrapMode.Loop;

				while(true) {
					Vector3 dir = targetPlayer.transform.position-transform.position;
					dir.y = 0;
					var rotation = Quaternion.LookRotation(dir);

					transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Mathf.Min(1, Time.deltaTime*FastRotateSpeed));
					//yield return null;
					break;
				}
				yield return null;
				var waitTime = Random.Range(1, 1.5f);

				while(passTime < waitTime) {
					if(CheckOnHit()) {
						Debug.Log("try to move around onhit");
						StartCoroutine(Stunned());
						yield break;
					}


					Vector3 dir = targetPlayer.transform.position-transform.position;
					dir.y = 0;
					var rotation = Quaternion.LookRotation(dir);
					transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*FastRotateSpeed);


					var right = transform.TransformDirection(Vector3.right);
					var back = transform.TransformDirection(Vector3.back);
					if(dir.magnitude > AttackRange*1.2f) {
						back = Vector3.zero;
                    }

					controller.SimpleMove(right*WalkSpeed+back*WalkSpeed);

					passTime += Time.deltaTime;
					yield return null;
				}

				//Rush to Player Follow
				while(true) {
					if(CheckOnHit()) {
						StartCoroutine(Stunned());
                        yield break;
                    }

					Vector3 dir = targetPlayer.transform.position-transform.position;
					dir.y = 0;
					if(dir.magnitude < AttackRange) {
						break;
					}

					var rotation = Quaternion.LookRotation(dir);


					transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*FastRotateSpeed);
					var forward = transform.TransformDirection(Vector3.forward);

					controller.SimpleMove(forward*RunSpeed);
					yield return null;
				}

				animation.CrossFade("attack1");

				while(animation.isPlaying) {
					if(CheckOnHit()) {
						StartCoroutine(Stunned());
                        yield break;
                    }

					Vector3 dir = targetPlayer.transform.position-transform.position;
					dir.y = 0;
					var rotation = Quaternion.LookRotation(dir);
					transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*FastRotateSpeed);
					yield return null;
				}

			}

			_characterState = CharacterState.Idle;
		}

		//Find nearby Target notify player
		IEnumerator FindTarget() {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			while(true) {
				if(player && _characterState == CharacterState.Idle) {
					float distance = (player.transform.position-transform.position).magnitude;
					if(distance < ApproachDistance) {
						yield return StartCoroutine(AroundMoveAttack());
						//_characterState = CharacterState.Around;
					}
				}else {
					if(CheckOnHit()) {
						StartCoroutine(Stunned());
					}
				}
				yield return null;
			}
		}


		IEnumerator NewHeadingRoutine() {
			//var floor = heading-maxHeadingChange;
			//var ceil = heading+maxHeadingChange;
			//Vector2 dir = Random.insideUnitCircle();

			//heading = Random.Range(floor, ceil);

			/*
			Vector3 curPos = transform.position;
			curPos.y = 0;
			
			float dist = (curPos-birthPoint).magnitude;
			if(dist >= Radius) {
				heading = Quaternion.Euler(birthPoint-curPos).eulerAngles.y;
			}
			*/

			while(true) {
				if(_characterState == CharacterState.Idle) {
					heading = Random.Range(0, 360);

					targetRotation = new Vector3(0, heading, 0);
					Quaternion qua = Quaternion.Euler(targetRotation);
					Vector3 dir = (qua*Vector3.forward);


					RaycastHit hitInfo;
					if(!Physics.Raycast(transform.position, dir, out hitInfo, 3)) {
						break;
					}
				}else {
					break;
				}
				yield return null;
			}
		}


		IEnumerator NewHeading() {
			while(true) {
				if(_characterState == CharacterState.Idle) {
					yield return new WaitForSeconds(Random.Range(1, 3));
					//NewHeadingRoutine();
					yield return StartCoroutine(NewHeadingRoutine());
					if(_characterState != CharacterState.Idle) {
						continue;
					}

					_characterState = CharacterState.Running;
					yield return new WaitForSeconds(directionChangeInterval);
					_characterState = CharacterState.Idle;
				}else {
					yield return null;
				}
			}
		}


		
		// Update is called once per frame
		void Update () {
			if(_characterState == CharacterState.Idle) {
				animation.CrossFade("idle");
				animation["idle"].wrapMode = WrapMode.Loop;
			}else if(_characterState == CharacterState.Running) {
				animation.CrossFade("run");
				animation["run"].speed = 2;
				animation["run"].wrapMode = WrapMode.Loop;

				transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime*directionChangeInterval);
				var forward = transform.TransformDirection(Vector3.forward);
				//Debug.Log("move speed "+forward*RunSpeed);
				controller.SimpleMove(forward*RunSpeed);
			}else if(_characterState == CharacterState.Around) {
				//animation.CrossFade("run");
			}
		}



	}
}
