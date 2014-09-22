using UnityEngine;
using System.Collections;


public enum CharacterState {
	Idle,
	Running,
	Attacking,
};

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour {
	CharacterState _characterState;


	Vector3 moveDirection;
	float moveSpeed = 0;
	float verticalSpeed = 0;
	Vector3 inAirVelocity = Vector3.zero;
	bool movingBack = false;
	bool isMoving = false;
	float walkSpeed = 2.0f;
	float rotateSpeed = 500;
	float speedSmoothing = 10;
	float gravity = 20.0f;



	CollisionFlags collisionFlags;



	void Awake() {
		_characterState = CharacterState.Idle;
		moveDirection = transform.TransformDirection(Vector3.forward);
	}

	// Use this for initialization
	void Start () {
		animation.CrossFade("idle");

	}

	void UpdateSmoothedMovementDirection() {
		bool grounded = IsGrounded();

		var forward = transform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		var v = Input.GetAxisRaw("Vertical");
		var h = Input.GetAxisRaw("Horizontal");
		if (v < -0.2)
			movingBack = true;
		else
            movingBack = false;

		bool wasMoving = isMoving;
		isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;

		Vector3 targetDirection = h * right + v * forward;
	

		if (grounded)
		{

			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			if (targetDirection != Vector3.zero)
			{
				// If we are really slow, just snap to the target direction
				if (moveSpeed < walkSpeed * 0.9 && grounded)
				{
					moveDirection = targetDirection.normalized;
				}
				// Otherwise smoothly turn towards it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
					moveDirection = moveDirection.normalized;
				}
			}
			
			// Smooth the speed based on the current target direction
			var curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			targetSpeed *= walkSpeed;

            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
			if(targetSpeed > 0.1f) 
			{
				_characterState = CharacterState.Running;
            } else {
				_characterState = CharacterState.Idle;
			}

        }else {
			
		}


	}

	void ApplyGravity() {
		if(IsGrounded()) {
			verticalSpeed = 0.0f;
		}else {
			verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	bool IsGrounded() {
		return (collisionFlags & CollisionFlags.Below) != 0;
	}
	// Update is called once per frame
	void HandleInput() {
		if(Input.GetAxisRaw("Horizontal") > 0.5f) {
			
		}
	}	

	void Update () {
		HandleInput();

		UpdateSmoothedMovementDirection();
		ApplyGravity();

		Vector3 upSpeed = new Vector3(0, verticalSpeed, 0);
		var movement = moveDirection * moveSpeed + upSpeed + inAirVelocity;
		movement *= Time.deltaTime;

		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);

		if(_characterState == CharacterState.Idle) {

			animation.CrossFade("idle");
		}else if(_characterState == CharacterState.Running) {
			animation["run"].speed = 2;

			animation.CrossFade("run");
		}
	}

}
