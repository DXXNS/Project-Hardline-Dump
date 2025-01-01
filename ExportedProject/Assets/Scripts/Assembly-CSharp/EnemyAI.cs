using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Human
{
	public enum EnemyDifficulties
	{
		EASY = 0,
		MEDIUM = 1,
		HARD = 2
	}

	public enum EnemyBehaviourStates
	{
		GUARD = 0,
		GUARD_HOLD = 1,
		PATROL = 2,
		RUN_HIDE = 3,
		COVER_HIDE = 4,
		HUNT = 5
	}

	public enum EnemyAttackStates
	{
		none = 0,
		attack = 1
	}

	private static int playerLayerMask = 4096;

	private static int characterLayerMask = 64;

	private static int projectileMask = 524288;

	private static int physicsParticleMask = 256;

	private static int glassLayer = 16;

	private static int hitboxLayerMask = 128;

	private static int ignoreRaycast = 4;

	private static int interactableLayerMask = 16384;

	private static float UPDATE_FREQ = 0.25f;

	private static float HIDE_FIND_RADIUS = 25f;

	private static float MIN_OBSTACLE_HEIGHT = 0.15f;

	[SerializeField]
	private LayerMask HidableLayers;

	private const float AIM_KP = 0.1f;

	private const float PATROL_DIST_MIN = 2f;

	private const float AIM_ACCURACY_TURN_LOSS = 0.005f;

	private const float AIM_RECOVER_SPEED = 0.04f;

	private const float PLAYER_VISION_GAIN = 0.007f;

	private const float PLAYER_SENSE_GAIN = 0.1f;

	private const float PLAYER_VISION_LOSS = 0.1f;

	private const float MIN_PLAYER_DISTANCE = 10f;

	private const float HIDE_SENSITIVITY = 0f;

	private const float GUARD_HOLD_TIME = 10f;

	private const float GUARD_TIME_MIN = 3f;

	private const float GUARD_TIME_MAX = 7f;

	private const float RESPOND_SUPPRESS_ANGLE = 160f;

	private const float EASY_DISPERSION = 7f;

	private const float MEDIUM_DISPERSION = 3f;

	private const float HARD_DISPERSION = 1f;

	private const float EASY_SENSESPEED = 0.3f;

	private const float MEDIUM_SENSESPEED = 0.75f;

	private const float HARD_SENSESPEED = 0.75f;

	private const float EASY_AIMSPEED = 0.3f;

	private const float MEDIUM_AIMSPEED = 0.75f;

	private const float HARD_AIMSPEED = 0.75f;

	private const float EASY_COOLDOWN = 4f;

	private const float MEDIUM_COOLDOWN = 2f;

	private const float HARD_COOLDOWN = 1f;

	private float aimYOffset = -2f;

	[SerializeField]
	private float dispersion = 3f;

	[SerializeField]
	private Human target;

	[SerializeField]
	private Transform aimTarget;

	private bool isAiming;

	private NavMeshAgent navMeshAgent;

	private Vector3 desVelocity;

	private bool isShooting;

	[SerializeField]
	private EnemyBehaviourStates behaviourState;

	[SerializeField]
	private float fieldOfFiew = 65f;

	[SerializeField]
	private float enemyViewRange = 35f;

	[SerializeField]
	private float enemySenseRange = 7f;

	[SerializeField]
	private float aimStability;

	[SerializeField]
	private float playerVision;

	[SerializeField]
	private GameObject nametag;

	private bool seesPlayer;

	private bool playerInLightOfSight;

	private float randomTargetAimRotation;

	[SerializeField]
	private Transform targetMovePosition;

	private Collider[] hideColliders = new Collider[5];

	private EnemyDifficulties difficulty;

	private EnemyPatrolLocations patrolLocations;

	private float difficultyDispersionMultiplier = 1f;

	private float difficultySenseSpeedMultiplier = 1f;

	private float difficultyAimSpeedMultiplier = 1f;

	private float difficultyCooldownMultiplier = 1f;

	public override void Start()
	{
		base.Start();
		patrolLocations = UnityEngine.Object.FindObjectOfType<EnemyPatrolLocations>();
		gameManager = UnityEngine.Object.FindObjectOfType<HardlineGameManager>();
		networkManager = UnityEngine.Object.FindObjectOfType<HardlineNetworkManager>();
		LoadItem("M416", localPlayer: false);
		UpdateTeamAppearance();
		base.UseThirdPersonAnimations = true;
		charController = GetComponent<CharacterController>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.updateRotation = false;
		target = UnityEngine.Object.FindObjectOfType<Player>();
		aimTarget = target.GetComponent<Human>().HeadAimTransform;
		behaviourState = EnemyBehaviourStates.PATROL;
		isAiming = false;
		if ((bool)UnityEngine.Object.FindObjectOfType<PracticeModeGameManager>())
		{
			difficulty = PracticeModeGameManager.difficulty;
			UpdateDifficulty();
		}
		SetNameTagVisibility(visible: false);
	}

	public void SetNameTagVisibility(bool visible)
	{
		nametag.SetActive(visible);
	}

	public void UpdateDifficulty()
	{
		if (difficulty == EnemyDifficulties.EASY)
		{
			difficultyDispersionMultiplier = 7f;
			difficultySenseSpeedMultiplier = 0.3f;
			difficultyAimSpeedMultiplier = 0.3f;
			difficultyCooldownMultiplier = 4f;
		}
		else if (difficulty == EnemyDifficulties.MEDIUM)
		{
			difficultyDispersionMultiplier = 3f;
			difficultySenseSpeedMultiplier = 0.75f;
			difficultyAimSpeedMultiplier = 0.75f;
			difficultyCooldownMultiplier = 2f;
		}
		else if (difficulty == EnemyDifficulties.HARD)
		{
			difficultyDispersionMultiplier = 1f;
			difficultySenseSpeedMultiplier = 0.75f;
			difficultyAimSpeedMultiplier = 0.75f;
			difficultyCooldownMultiplier = 1f;
		}
	}

	private bool CheckAimTargetInLineOfSight()
	{
		if (Physics.Linecast(base.HeadAimTransform.position, aimTarget.transform.position, ~(playerLayerMask | hitboxLayerMask | interactableLayerMask | ignoreRaycast | characterLayerMask | projectileMask | physicsParticleMask | glassLayer)))
		{
			return false;
		}
		return true;
	}

	public override void Update()
	{
		base.Update();
		RunBehaviour();
		AIAnimationStateUpdate();
	}

	private void UpdateAim()
	{
		if (isAiming)
		{
			navMeshAgent.updateRotation = false;
			Vector3 targetPos = aimTarget.transform.position + new Vector3(0f, aimYOffset, 0f);
			SmoothLookAt(targetPos);
		}
		else
		{
			navMeshAgent.updateRotation = true;
		}
		if (isAiming)
		{
			if (aimStability < 1f)
			{
				aimStability += 0.04f * difficultyAimSpeedMultiplier;
				aimStability = Mathf.Clamp(aimStability, 0f, 1f);
			}
		}
		else
		{
			aimStability = 0.7f;
		}
	}

	public void CheckAndReturnToPatrolling()
	{
		Vector3 normalized = (aimTarget.transform.position - base.transform.position).normalized;
		bool flag = CheckAimTargetInLineOfSight();
		float num = Vector3.Distance(aimTarget.transform.position, base.transform.position);
		if (IsInvoking("TurnToRandomDirection"))
		{
			CancelInvoke("TurnToRandomDirection");
		}
		navMeshAgent.destination = patrolLocations.GetRandomPatrolLocation().position;
		if (Vector3.Angle(base.transform.forward, normalized) < fieldOfFiew / 2f)
		{
			if (flag && num < enemyViewRange && target.Health > 0f)
			{
				playerVision += Time.deltaTime * 0.007f * Mathf.Clamp(150f - num, 0f, 150f) * difficultySenseSpeedMultiplier;
				return;
			}
			targetMovePosition = null;
			behaviourState = EnemyBehaviourStates.PATROL;
		}
		else
		{
			targetMovePosition = null;
			behaviourState = EnemyBehaviourStates.PATROL;
		}
	}

	public void TurnToRandomDirection()
	{
		randomTargetAimRotation = UnityEngine.Random.Range(0, 360);
	}

	public void SetAggressiveMode()
	{
		behaviourState = EnemyBehaviourStates.HUNT;
	}

	private void RunBehaviour()
	{
		if (behaviourState == EnemyBehaviourStates.GUARD)
		{
			targetMovePosition = null;
			SmoothLookAtRotation(new Vector3(0f, randomTargetAimRotation, 0f));
			Vector3 normalized = (aimTarget.transform.position - base.transform.position).normalized;
			if (Vector3.Angle(base.transform.forward, normalized) < fieldOfFiew / 2f && CheckAimTargetInLineOfSight() && target.Health > 0f)
			{
				behaviourState = EnemyBehaviourStates.HUNT;
				if (IsInvoking("TurnToRandomDirection"))
				{
					CancelInvoke("TurnToRandomDirection");
				}
			}
			if (!IsInvoking("TurnToRandomDirection"))
			{
				InvokeRepeating("TurnToRandomDirection", 0.1f, UnityEngine.Random.Range(3, 5));
			}
			if (!IsInvoking("CheckAndReturnToPatrolling"))
			{
				Invoke("CheckAndReturnToPatrolling", UnityEngine.Random.Range(3f, 7f));
			}
		}
		if (behaviourState == EnemyBehaviourStates.GUARD_HOLD)
		{
			targetMovePosition = null;
			AimAndShootPlayer();
			if (!IsInvoking("CheckAndReturnToPatrolling"))
			{
				Invoke("CheckAndReturnToPatrolling", 10f);
			}
		}
		else if (behaviourState == EnemyBehaviourStates.RUN_HIDE)
		{
			base.Sprinting = true;
			if (IsInvoking("FireWeapon"))
			{
				CancelInvoke("FireWeapon");
			}
			isAiming = false;
			Hide(target.transform);
			if (Vector3.Distance(base.transform.position, navMeshAgent.destination) < 2f || !CheckAimTargetInLineOfSight())
			{
				behaviourState = EnemyBehaviourStates.GUARD_HOLD;
			}
		}
		else if (behaviourState == EnemyBehaviourStates.COVER_HIDE)
		{
			Hide(target.transform);
			AimAndShootPlayer();
			if (Vector3.Distance(base.transform.position, navMeshAgent.destination) < 2f || !CheckAimTargetInLineOfSight())
			{
				behaviourState = EnemyBehaviourStates.GUARD_HOLD;
			}
		}
		else if (behaviourState == EnemyBehaviourStates.PATROL)
		{
			if (IsInvoking("FireWeapon"))
			{
				CancelInvoke("FireWeapon");
			}
			base.Sprinting = false;
			isAiming = false;
			if (targetMovePosition == null)
			{
				targetMovePosition = patrolLocations.GetRandomPatrolLocation();
			}
			if (Vector3.Distance(base.transform.position, navMeshAgent.destination) < 2f)
			{
				behaviourState = EnemyBehaviourStates.GUARD;
			}
			Vector3 normalized2 = (aimTarget.transform.position - base.transform.position).normalized;
			bool flag = CheckAimTargetInLineOfSight();
			float num = Vector3.Distance(aimTarget.transform.position, base.transform.position);
			if (flag && num < enemySenseRange && target.Health > 0f)
			{
				playerVision += 0.1f;
			}
			else if (Vector3.Angle(base.transform.forward, normalized2) < fieldOfFiew / 2f)
			{
				if (flag && num < enemyViewRange && target.Health > 0f)
				{
					playerVision += Time.deltaTime * 0.007f * Mathf.Clamp(150f - num, 0f, 150f) * difficultySenseSpeedMultiplier;
				}
				else
				{
					playerVision -= Time.deltaTime * 0.1f;
				}
			}
			else if (flag && num < enemySenseRange && target.Health > 0f)
			{
				playerVision += 0.007f * (Mathf.Clamp(enemyViewRange - num, 0f, enemyViewRange) / enemyViewRange) * difficultySenseSpeedMultiplier;
			}
			playerVision = Mathf.Clamp(playerVision, 0f, 1f);
			if (playerVision >= 1f)
			{
				behaviourState = EnemyBehaviourStates.HUNT;
			}
			navMeshAgent.destination = targetMovePosition.position;
		}
		else if (behaviourState == EnemyBehaviourStates.HUNT)
		{
			AimAndShootPlayer();
			navMeshAgent.SetDestination(target.transform.position);
		}
	}

	private void AimAndShootPlayer()
	{
		base.Sprinting = false;
		isAiming = true;
		if (CheckAimTargetInLineOfSight() && aimStability > 0.9f && !base.Sprinting)
		{
			if (!isShooting)
			{
				InvokeRepeating("FireWeapon", 0.1f, (base.Item as PlayerFirearm).Cooldown * difficultyCooldownMultiplier);
				isShooting = true;
			}
		}
		else
		{
			CancelInvoke("FireWeapon");
			isShooting = false;
		}
		if (target.Health <= 0f)
		{
			behaviourState = EnemyBehaviourStates.PATROL;
		}
	}

	public void FireWeapon()
	{
		if (base.Item is PlayerFirearm)
		{
			(base.Item as PlayerFirearm).Shoot(dispersion * difficultyDispersionMultiplier);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMovement();
		UpdateImpactForce();
		UpdateAim();
	}

	public override void Suppress(float intensity)
	{
		Vector3 normalized = (aimTarget.transform.position - base.transform.position).normalized;
		if (Vector3.Angle(base.transform.forward, normalized) < fieldOfFiew / 2f)
		{
			behaviourState = EnemyBehaviourStates.HUNT;
		}
		else if (Vector3.Angle(base.transform.forward, normalized) < 80f)
		{
			behaviourState = EnemyBehaviourStates.COVER_HIDE;
		}
		else
		{
			behaviourState = EnemyBehaviourStates.RUN_HIDE;
		}
	}

	public override void ReplicateDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		Vector3 normalized = (aimTarget.transform.position - base.transform.position).normalized;
		base.ReplicateDealDamage(value, causer, hitPos, hitRot);
		if (Vector3.Angle(base.transform.forward, normalized) < 80f)
		{
			behaviourState = EnemyBehaviourStates.COVER_HIDE;
		}
		else
		{
			behaviourState = EnemyBehaviourStates.RUN_HIDE;
		}
	}

	public override void UpdateMovement()
	{
		if (charController.isGrounded)
		{
			if (isSliding)
			{
				base.Velocity = new Vector3(base.Velocity.x / groundSlidingFriction, 0f - base.FloorGravity, base.Velocity.z / groundSlidingFriction);
			}
			else if (base.Walking || base.Sprinting)
			{
				base.Velocity = new Vector3(base.Velocity.x / groundMovingFriction, 0f - base.FloorGravity, base.Velocity.z / groundMovingFriction);
			}
			else
			{
				base.Velocity = new Vector3(base.Velocity.x / groundFriction, 0f - base.FloorGravity, base.Velocity.z / groundFriction);
			}
		}
		else
		{
			base.Velocity = new Vector3(base.Velocity.x / airFriction, base.Velocity.y - base.Gravity, base.Velocity.z / airFriction);
		}
		desVelocity = navMeshAgent.desiredVelocity;
		if (base.Sprinting)
		{
			charController.Move(desVelocity.normalized * baseMovementSpeed * sprintSpeedMultiplier);
		}
		else
		{
			charController.Move(desVelocity.normalized * baseMovementSpeed);
		}
		navMeshAgent.velocity = charController.velocity;
	}

	public void AIAnimationStateUpdate()
	{
		if (navMeshAgent.velocity.magnitude > 0f)
		{
			base.Walking = true;
		}
		else
		{
			base.Walking = false;
		}
		if (base.Walking)
		{
			if (base.Sprinting)
			{
				base.Animator.SetInteger("MoveState", 4);
			}
			else
			{
				base.Animator.SetInteger("MoveState", 1);
			}
		}
		else
		{
			base.Animator.SetInteger("MoveState", 0);
		}
	}

	protected override void Death()
	{
		if ((bool)UnityEngine.Object.FindObjectOfType<PracticeModeGameManager>())
		{
			UnityEngine.Object.FindObjectOfType<PracticeModeGameManager>().FindEnemiesAlive();
		}
		SpawnRagdoll(base.transform);
		base.Death();
	}

	public void LateUpdate()
	{
		UpdateMoveStateAnimationsValues();
		UpdateAnimations();
		UpdateCharacterControllerDimensions();
	}

	private void SmoothLookAt(Vector3 targetPos)
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		base.transform.LookAt(targetPos);
		Vector3 eulerAngles2 = base.transform.eulerAngles;
		base.transform.eulerAngles = eulerAngles;
		float y = base.transform.eulerAngles.y;
		base.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(eulerAngles.y, eulerAngles2.y, 0.1f), Mathf.LerpAngle(eulerAngles.z, eulerAngles2.z, 0.1f));
		float num = Mathf.Abs(y - base.transform.eulerAngles.y);
		aimStability -= num * 0.005f;
		base.SyncedAimY = Mathf.LerpAngle(base.SyncedAimY, (0f - eulerAngles2.x) * 2f, 0.1f);
	}

	private void SmoothLookAtRotation(Vector3 rotation)
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		Vector3 vector = rotation;
		float y = base.transform.eulerAngles.y;
		base.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(eulerAngles.y, vector.y, 0.1f), Mathf.LerpAngle(eulerAngles.z, vector.z, 0.1f));
		float num = Mathf.Abs(y - base.transform.eulerAngles.y);
		aimStability -= num * 0.005f;
		base.SyncedAimY = Mathf.LerpAngle(base.SyncedAimY, (0f - vector.x) * 2f, 0.1f);
	}

	private void OnDestroy()
	{
		CancelInvoke("FireWeapon");
	}

	protected override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		base.OnControllerColliderHit(hit);
		if ((bool)hit.gameObject.GetComponent<DoorInteractive>())
		{
			if (!hit.gameObject.GetComponent<DoorInteractive>().DoorOpen())
			{
				hit.gameObject.GetComponent<DoorInteractive>().Interact(base.transform.position);
			}
			else if (!hit.gameObject.GetComponent<DoorInteractive>().CheckOpenedCorrectly(base.transform.position))
			{
				hit.gameObject.GetComponent<DoorInteractive>().SwingOtherWay(base.transform.position);
			}
		}
	}

	private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += base.transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * (MathF.PI / 180f)), 0f, Mathf.Cos(angleInDegrees * (MathF.PI / 180f)));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		if (behaviourState == EnemyBehaviourStates.RUN_HIDE)
		{
			for (int i = 0; i < hideColliders.Length; i++)
			{
				Gizmos.DrawSphere(hideColliders[i].transform.position, 1f);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(navMeshAgent.destination, 1f);
		}
	}

	private void Hide(Transform Target)
	{
		for (int i = 0; i < hideColliders.Length; i++)
		{
			hideColliders[i] = null;
		}
		int num = Physics.OverlapSphereNonAlloc(navMeshAgent.transform.position, HIDE_FIND_RADIUS, hideColliders, HidableLayers);
		int num2 = 0;
		for (int j = 0; j < num; j++)
		{
			if (Vector3.Distance(hideColliders[j].transform.position, Target.position) < 10f || hideColliders[j].bounds.size.y < MIN_OBSTACLE_HEIGHT)
			{
				hideColliders[j] = null;
				num2++;
			}
		}
		num -= num2;
		Array.Sort(hideColliders, ColliderArraySortComparer);
		for (int k = 0; k < num; k++)
		{
			if (NavMesh.SamplePosition(hideColliders[k].transform.position, out var hit, 5f, navMeshAgent.areaMask))
			{
				if (!NavMesh.FindClosestEdge(hit.position, out hit, navMeshAgent.areaMask))
				{
					Debug.LogError($"Unable to find edge close to {hit.position}");
				}
				NavMeshPath navMeshPath = new NavMeshPath();
				NavMeshHit hit2;
				if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < 0f && NavMesh.CalculatePath(base.transform.position, hit.position, navMeshAgent.areaMask, navMeshPath))
				{
					if (navMeshPath.status == NavMeshPathStatus.PathComplete)
					{
						navMeshAgent.SetDestination(hit.position);
						break;
					}
				}
				else if (NavMesh.SamplePosition(hideColliders[k].transform.position - (Target.position - hit.position).normalized * 2f, out hit2, 5f, navMeshAgent.areaMask))
				{
					if (!NavMesh.FindClosestEdge(hit2.position, out hit2, navMeshAgent.areaMask))
					{
						Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
					}
					if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < 0f && NavMesh.CalculatePath(base.transform.position, hit.position, navMeshAgent.areaMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
					{
						navMeshAgent.SetDestination(hit2.position);
						break;
					}
				}
			}
			else
			{
				Debug.LogError($"Unable to find NavMesh near object {hideColliders[k].name} at {hideColliders[k].transform.position}");
			}
		}
	}

	public int ColliderArraySortComparer(Collider A, Collider B)
	{
		if (A == null && B != null)
		{
			return 1;
		}
		if (A != null && B == null)
		{
			return -1;
		}
		if (A == null && B == null)
		{
			return 0;
		}
		return Vector3.Distance(navMeshAgent.transform.position, A.transform.position).CompareTo(Vector3.Distance(navMeshAgent.transform.position, B.transform.position));
	}

	private void MirrorProcessed()
	{
	}
}
