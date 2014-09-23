using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace ChuMeng
{
	public enum CharacterState
	{
		Idle,
		Running,
		Attacking,
		Around,
	};

	[RequireComponent(typeof(CharacterController))]
	public class PlayerMoveController : MonoBehaviour
	{
		public VirtualJoystickRegion VJR;
		CharacterState _characterState;
		Vector3 moveDirection;
		float moveSpeed = 0;
		float verticalSpeed = 0;
		Vector3 inAirVelocity = Vector3.zero;
		bool movingBack = false;
		bool isMoving = false;
		float walkSpeed = 6.0f;
		float rotateSpeed = 500;
		float speedSmoothing = 10;
		float gravity = 20.0f;
		Vector3 camRight;
		Vector3 camForward;
		CollisionFlags collisionFlags;
		public GameObject AttackButton;

		bool inAttack = false;
		int attackCount = 0;
		List<string> allAttackAni;
		int attackId = 0;
		void Awake ()
		{
			allAttackAni = new List<string>(){"attack", "attack2"};

			_characterState = CharacterState.Idle;
			moveDirection = transform.TransformDirection (Vector3.forward);
		}

		// Use this for initialization
		void Start ()
		{
			animation.CrossFade ("idle");
			camRight = Camera.main.transform.TransformDirection (Vector3.right);
			camForward = Camera.main.transform.TransformDirection (Vector3.forward);
			camRight.y = 0;
			camForward.y = 0;
			camRight.Normalize ();
			camForward.Normalize ();

			UIEventListener.Get(AttackButton).onClick += OnAttack;

		}

		void OnAttack(GameObject g) {
			Debug.Log("Attack animatin");

			if(!inAttack) {
				StartAttack(allAttackAni[attackId]);
				attackId++;
				attackId %= allAttackAni.Count;

			}else {
				attackCount++;
				attackCount = Mathf.Min(1, attackCount);
			}
			_characterState = CharacterState.Attacking;
		}

		void StartAttack(string name) {
			inAttack = true;
			animation.CrossFade(name);
			animation[name].speed = 2;
			StartCoroutine(WaitForAnimation(animation));
		}

		IEnumerator WaitForAnimation(Animation animation) {
			do {
				yield return null;
			}while(animation.isPlaying);
			if(attackCount > 0) {
				attackCount--;
				StartAttack(allAttackAni[attackId]);
				attackId++;
				attackId %= allAttackAni.Count;
			}else {
				inAttack = false;
				//_characterState = CharacterState.Idle;
			}
		}




		void UpdateSmoothedMovementDirection ()
		{
			var curSmooth = speedSmoothing * Time.deltaTime;
			if(inAttack) {
				moveSpeed = Mathf.Lerp (moveSpeed, 0, curSmooth);
				return;
			}

			bool grounded = IsGrounded ();

			//var forward = transform.TransformDirection(Vector3.forward);
			//forward.y = 0;
			//forward = forward.normalized;
			//Vector3 right = new Vector3(forward.z, 0, -forward.x);


			var v = Input.GetAxisRaw ("Vertical");
			var h = Input.GetAxisRaw ("Horizontal");

			//Camera direction
			//Vector3 camRight = Camera.main.transform.TransformDirection(Vector3.right);
			//Vector3 camForward = Camera.main.transform.TransformDirection(Vector3.up);


			if (Mathf.Abs (v) < 0.1f && Mathf.Abs (h) < 0.1f && VJR != null) {
				Vector2 vec = VirtualJoystickRegion.VJRnormals;
				//Camera direction f
				Debug.Log ("joyStick pos " + vec);

				h = vec.x;
				v = vec.y;
			}

			/*
		if (v < -0.2)
			movingBack = true;
		else
            movingBack = false;
		*/

			bool wasMoving = isMoving;
			isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;

			//Vector3 targetDirection = h * right + v * forward;

			Vector3 targetDirection = h * camRight + v * camForward;


			if (grounded) {

				// We store speed and direction seperately,
				// so that when the character stands still we still have a valid forward direction
				// moveDirection is always normalized, and we only update it if there is user input.
				if (targetDirection != Vector3.zero) {
					// If we are really slow, just snap to the target direction

					if (moveSpeed < walkSpeed * 0.3f && grounded) {
						//moveDirection = targetDirection.normalized;
						moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * 2 * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
						moveDirection = moveDirection.normalized;
					}
				// Otherwise smoothly turn towards it
				else {

				
						moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
						moveDirection = moveDirection.normalized;
					}
				}
			
				// Smooth the speed based on the current target direction


				//backward move first need to rotate then run
				//if run first to stop then run

				// Choose target speed
				//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways

				var targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);
				if (Vector3.Dot (targetDirection, moveDirection) < 0) {
					//targetSpeed = 0.1f;
				}

				targetSpeed *= walkSpeed;


				moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);
				if (targetSpeed > 0.1f) {
					_characterState = CharacterState.Running;
				} else {
					_characterState = CharacterState.Idle;
				}

			} else {
			
			}


		}

		void ApplyGravity ()
		{
			if (IsGrounded ()) {
				verticalSpeed = 0.0f;
			} else {
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}

		bool IsGrounded ()
		{
			return (collisionFlags & CollisionFlags.Below) != 0;
		}
		// Update is called once per frame
		void HandleInput ()
		{
			if (Input.GetAxisRaw ("Horizontal") > 0.5f) {
			
			}
		}

		void Update ()
		{
			HandleInput ();

			UpdateSmoothedMovementDirection ();
			ApplyGravity ();

			Vector3 upSpeed = new Vector3 (0, verticalSpeed, 0);
			var movement = moveDirection * moveSpeed + upSpeed + inAirVelocity;
			movement *= Time.deltaTime;

			CharacterController controller = GetComponent<CharacterController> ();
			collisionFlags = controller.Move (movement);

			if (_characterState == CharacterState.Idle) {

				animation.CrossFade ("idle");
			} else if (_characterState == CharacterState.Running) {
				animation ["run"].speed = 2;

				animation.CrossFade ("run");
			}

			if (IsGrounded ()) {
				transform.rotation = Quaternion.LookRotation (moveDirection);	
			}
		}

	}

}
