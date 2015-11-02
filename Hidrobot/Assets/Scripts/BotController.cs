using UnityEngine;
using System.Collections;

public class BotController : MonoBehaviour {
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;
	
	Rigidbody m_Rigidbody;
	Animator m_Animator;
	AudioSource m_AudioSource;
	bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;
	
	private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;

	public float speed = 6.0f;
	public float runSpeed = 9.0f;
	public float rotateSpeed = 90.0f;
	
	public float gravity = 20.0f;
	
	private Vector3 moveDirection = Vector3.zero;
	private bool grounded = false;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_AudioSource = GetComponent<AudioSource> ();
	}

	void FixedUpdate() {
		if (GameControl.controller.state != GameControl.State.Bot || GameControl.controller.paused) {
			return;
		}
		// read inputs
		float h = Input.GetAxis("LeftHorizontal");
		float v = Input.GetAxis("LeftVertical");
		
		// calculate move direction to pass to character
		if (m_Cam != null)
		{
			// calculate camera relative direction to move:
			m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
			m_Move = v*m_CamForward + h*m_Cam.right;
		}
		else
		{
			// we use world-relative directions in the case of no main camera
			m_Move = v*Vector3.forward + h*Vector3.right;
		}
		
		// pass all parameters to the character control script
		Move(m_Move);
	}

	void Move(Vector3 move) {
		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);

		move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;
		
		ApplyExtraTurnRotation();


		if (grounded) {
			// We are grounded, so recalculate movedirection directly from axes
//			moveDirection = new Vector3(0, 0, Input.GetAxis("LeftVertical")); //Determine the player's forward speed based upon the input.
			
			moveDirection = transform.TransformDirection(move); //make the direction relative to the player.
			if(Input.GetButton("Jump")) {
				moveDirection *= runSpeed;
			}
			else {
				moveDirection *= speed;
			}
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		CollisionFlags flags = controller.Move(moveDirection * runSpeed * Time.deltaTime);

//		transform.Rotate(0, rotateSpeed * Time.deltaTime * Input.GetAxis("LeftHorizontal"), 0);
		
		grounded = true;

		if (move.magnitude > 0.0f) {
			m_Animator.SetBool ("running", true);
			if(!m_AudioSource.isPlaying) {
				m_AudioSource.Play();
			}
		} else {
			m_Animator.SetBool ("running", false);
			m_AudioSource.Stop();
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}
}
