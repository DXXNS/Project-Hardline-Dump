using UnityEngine;

public class MasterController : MonoBehaviour
{
	public CharacterState state;

	[SerializeField]
	private float walkSpeed = 3f;

	[SerializeField]
	private float runSpeed = 7f;

	[SerializeField]
	private float gravity = 9.81f;

	[SerializeField]
	[Tooltip("Determines how fast character turns.")]
	private float turnSmoothTime = 0.35f;

	[SerializeField]
	[Tooltip("Determines how high will character float above ground.")]
	private float groundDistance = 0.1f;

	[SerializeField]
	[Tooltip("Determines how fast animator will be pushed back towards ragdoll if no input is provided.")]
	private float pushBackSpeed = 7f;

	[SerializeField]
	[Tooltip("Maximum distance that animator can move away from ragdoll.")]
	private float maxPossibleDistanceFromRagdoll = 2f;

	[SerializeField]
	[Tooltip("All layers that are considered to be ground.")]
	private LayerMask groundMask;

	private Camera characterCamera;

	private Transform masterRoot;

	private Transform slaveRoot;

	private Animator anim;

	private bool isGrounded;

	private float currentFallVelocity;

	private float currentTurnSmoothVelocity;

	private void Start()
	{
		HumanoidSetUp componentInParent = GetComponentInParent<HumanoidSetUp>();
		characterCamera = componentInParent.characterCamera;
		masterRoot = componentInParent.masterRoot;
		slaveRoot = componentInParent.slaveRoot;
		anim = componentInParent.anim;
	}

	private void FixedUpdate()
	{
		isGrounded = Physics.CheckSphere(base.transform.position, groundDistance, groundMask);
		state = GetCharacterState();
		if (state != CharacterState.FALLING)
		{
			currentFallVelocity = 0f;
		}
		Vector3 directionFromInput = GetDirectionFromInput();
		float y = characterCamera.transform.eulerAngles.y;
		float num = CalculateCharacterAngle(directionFromInput, y);
		SetCharacterRotation(num);
		Vector3 moveDirection = Quaternion.Euler(0f, num, 0f) * Vector3.forward;
		MoveCharacter(moveDirection);
		SetAnimation();
	}

	private CharacterState GetCharacterState()
	{
		bool flag = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
		if (!isGrounded)
		{
			return CharacterState.FALLING;
		}
		if (flag && Input.GetKey(KeyCode.LeftShift))
		{
			return CharacterState.RUNNING;
		}
		if (flag)
		{
			return CharacterState.WALKING;
		}
		return CharacterState.IDLE;
	}

	private Vector3 GetDirectionFromInput()
	{
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		return new Vector3(axisRaw, 0f, axisRaw2).normalized;
	}

	private float CalculateCharacterAngle(Vector3 direction, float cameraAngleY)
	{
		float target = Mathf.Atan2(direction.x, direction.z) * 57.29578f + cameraAngleY;
		return Mathf.SmoothDampAngle(base.transform.eulerAngles.y, target, ref currentTurnSmoothVelocity, turnSmoothTime);
	}

	private void MoveCharacter(Vector3 moveDirection)
	{
		moveDirection = moveDirection.normalized;
		Debug.DrawRay(base.transform.position, moveDirection * 5f, Color.red);
		Vector3 vector = CalculatePushBackMovement();
		Vector3 vector2 = moveDirection + vector;
		switch (state)
		{
		case CharacterState.FALLING:
			currentFallVelocity -= gravity * Time.fixedDeltaTime;
			base.transform.position += new Vector3(0f, currentFallVelocity * Time.fixedDeltaTime, 0f);
			break;
		case CharacterState.RUNNING:
			base.transform.position += vector2 * runSpeed * Time.fixedDeltaTime;
			break;
		case CharacterState.WALKING:
			base.transform.position += vector2 * walkSpeed * Time.fixedDeltaTime;
			break;
		case CharacterState.IDLE:
			base.transform.position += vector * pushBackSpeed * Time.fixedDeltaTime;
			break;
		}
	}

	private void SetCharacterRotation(float angleY)
	{
		base.transform.rotation = Quaternion.Euler(0f, angleY, 0f);
	}

	private void SetAnimation()
	{
		switch (state)
		{
		case CharacterState.RUNNING:
			anim.SetInteger("Cond", 2);
			break;
		case CharacterState.WALKING:
			anim.SetInteger("Cond", 1);
			break;
		case CharacterState.IDLE:
			anim.SetInteger("Cond", 0);
			break;
		case CharacterState.FALLING:
			break;
		}
	}

	private Vector3 CalculatePushBackMovement()
	{
		Vector3 vector = slaveRoot.position - masterRoot.position;
		float t = Mathf.Clamp(vector.magnitude / maxPossibleDistanceFromRagdoll, 0f, 1f);
		float maxLength = Mathf.SmoothStep(0f, 1f, t);
		Vector3 result = Vector3.ClampMagnitude(vector, maxLength);
		result.y = 0f;
		return result;
	}
}
