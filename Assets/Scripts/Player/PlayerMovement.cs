using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
	public float walkSpeed;
	public float groundDrag;

	[Header("Sprinting")]
	private bool canSprint;
	public float sprintSpeed;
	public MovementState state;
	public float currSprintEnergy;
	public float maxSprintEnergy;
	public float sprintDrainspeed;
	private bool sprintCanRegen;
	public float sprintCoolDown;


	[Header("Jumping")]
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	public bool readyToJump;
	public float jumpStamCost;

	[Header("Croucing")]
	public float crouchSpeed;
	public float crouchYScale;
	public float startYScale;

	[Header("KeyBinds")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask whatIsGround;
	public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

	public enum MovementState
	{
		walking,
		sprinting,
		crouching,
		air
	}

	private void Start()
	{
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
		canSprint = true;
		ResetJump();
		startYScale = transform.localScale.y;
	}

	private void Update()
	{
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        HandleInput();
		SpeedControl();
		StateHandler();

		if(currSprintEnergy <= 0)
		{
			sprintCanRegen = false;
			canSprint = false;
		}

		if (grounded)
			rb.drag = groundDrag;
		else
			rb.drag = 0;
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void HandleInput()
	{
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

		if(Input.GetKey(jumpKey) && readyToJump && grounded && (currSprintEnergy - jumpStamCost) >= 0)
		{
			readyToJump = false;

			Jump();

			Invoke(nameof(ResetJump), jumpCooldown);
		}

		if (Input.GetKey(sprintKey) && canSprint)
		{
			currSprintEnergy -= Time.deltaTime;
		}
		if (Input.GetKeyUp(sprintKey))
		{
			Invoke(nameof(StartSprintRegen), sprintCoolDown);
		}

		if (Input.GetKeyDown(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}
		if (Input.GetKeyUp(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
	}

	private void StateHandler()
	{
		if (Input.GetKeyDown(crouchKey))
		{
			moveSpeed = crouchSpeed;
		}
		if(grounded && Input.GetKey(sprintKey) && currSprintEnergy > 0 && canSprint)
		{
			state = MovementState.sprinting;
			moveSpeed = sprintSpeed;

		}
		else if (grounded)
		{
			state = MovementState.walking;
			moveSpeed = walkSpeed;

			if(currSprintEnergy < maxSprintEnergy && sprintCanRegen)
			{
				currSprintEnergy += Time.deltaTime;
			}
			if(currSprintEnergy >= maxSprintEnergy)
			{
				canSprint = true;
			}
		}
		else
		{
			state = MovementState.air;
		}
	}
    private void MovePlayer()
	{
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

		if(grounded)
			rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
		else if(!grounded)
			rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
	}

	private void SpeedControl()
	{
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		if(flatVel.magnitude > moveSpeed)
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
		}
	}

	private void Jump()
	{
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

		currSprintEnergy -= jumpStamCost;
	}

	private void ResetJump()
	{
		readyToJump = true;
	}

	private void StartSprintRegen()
	{
		sprintCanRegen = true;
	}
}
