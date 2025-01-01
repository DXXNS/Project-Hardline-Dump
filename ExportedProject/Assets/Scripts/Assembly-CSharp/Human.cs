using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Human : NetworkBehaviour
{
	private int score;

	private int kills;

	private int assists;

	private int deaths;

	[SerializeField]
	protected float baseMovementSpeed;

	[SerializeField]
	protected float sprintSpeedMultiplier;

	[SerializeField]
	protected float sneakSpeedMultiplier;

	[SerializeField]
	protected float groundFriction;

	[SerializeField]
	protected float groundMovingFriction;

	[SerializeField]
	protected float groundSlidingFriction;

	[SerializeField]
	protected float airFriction;

	[SerializeField]
	protected float gravity;

	[SerializeField]
	protected float jumpStrength;

	[SerializeField]
	protected float pivotLimit;

	[SerializeField]
	private GameObject spine1;

	[SerializeField]
	private GameObject spine2;

	[SerializeField]
	private GameObject spine3;

	[SerializeField]
	private GameObject neck;

	[SerializeField]
	private GameObject itemAlignment;

	[SerializeField]
	private GameObject leftFootTargBase;

	[SerializeField]
	private GameObject rightFootTargBase;

	[SerializeField]
	private GameObject leftFootTarg;

	[SerializeField]
	private GameObject rightFootTarg;

	[SerializeField]
	private GameObject leftFootAngle;

	[SerializeField]
	private GameObject rightFootAngle;

	[SerializeField]
	private GameObject headCamPosition;

	[SerializeField]
	private Transform headAimTransform;

	[SerializeField]
	private GameObject[] ragdollParts;

	[SerializeField]
	private GameObject[] meshes;

	[SerializeField]
	private GameObject ragdoll;

	private Vector3 headClearanceCheckPos = new Vector3(0f, 1f, 0f);

	private float headClearanceCheckRange = 0.6f;

	protected bool crouchObstructed;

	private float crouchWalkSpeedMultiplier = 0.65f;

	private float sprintTransition_kP = 0.1f;

	private float floorGravity = 0.1f;

	private float feetIKCastHeight = 1f;

	private float feetIKOffset = 0.98f;

	private float feetCastRadius = 0.09f;

	private float feetIK_kP = 0.42f;

	private float feetIK_max = 0.8f;

	private float toeIK_dist = 0.2f;

	private float leftFootBaseAngle;

	private float rightFootBaseAngle;

	private float slopeAngleMultiplier = 1.5f;

	private float slopeDistanceClamp = 0.05f;

	private float pivot_kP = 0.11f;

	private float pivotTransition_kP = 0.03f;

	private float maxPivotAngle = 60f;

	private float walkPivotAngle = 35f;

	private float sprintPivotAngle = 35f;

	private float pivotMinimumForReset = 3f;

	private float playerAirSpeedMultiplier = 0.3f;

	private float fleshImpactCooldown = 0.04f;

	private float fallDamageMultiplier = 350f;

	private float fallDamageThreshold = 0.32f;

	private float landRegisterThreshold = 0.12f;

	private float movementColliderStandHeight = 1.7f;

	private float movementColliderStandPosY = 1f;

	private float movementColliderCrouchHeight = 1.3f;

	private float movementColliderCrouchPosY = 0.75f;

	private float movementColliderSize_kP = 0.05f;

	private float jumpRound_coeff = 5f;

	private float jumpStrengthRecovery_kP = 0.05f;

	private float jumpStrengthRecovery_loss = 0.07f;

	private float slideJumpStrength = 0.9f;

	private float slideBoostPenalty = 0.3f;

	private float slideBoostRecovery = 0.02f;

	private float slideBoostMinimum = 0.035f;

	private float killboxDamage = 1250f;

	private float IMPACT_MAX_MAG = 30f;

	private float LEAN_ANGLE = 25f;

	private float RECORD_VELOCITY_CONSTANT = 50f;

	private bool slidePenaltyFlag;

	protected float slideInitMinVelocity = 0.13f;

	protected float slideMinVelocity = 0.12f;

	protected float slideAddBaseVelocity = 0.055f;

	protected float slideSteeringVelocityMultiplier = 0.2f;

	protected float currentSlideAddVelocity;

	protected float jumpCooldown = 0.12f;

	protected float fallPenaltyCooldown = 0.2f;

	protected float fallPenalty = 0.3f;

	protected float slidePenalty = 0.6f;

	protected float currentSlidePenalty = 0.6f;

	protected float slideRecoveryRate = 0.04f;

	protected float slidePenaltyCooldown = 0.7f;

	protected float jumpStrengthMultiplier = 1f;

	private float charControllerTargHeight;

	private float charControllerTargPosY;

	[SerializeField]
	protected bool isSliding;

	public static int score_kill;

	private int hitboxLayer = 7;

	[SerializeField]
	private bool active;

	[SerializeField]
	protected bool jumpReady = true;

	[SerializeField]
	protected bool fallRecovered = true;

	[SerializeField]
	protected bool slideRecovered = true;

	private float jumpCooldownRemaining;

	private float fallPenaltyCooldownRemaining;

	private float slidePenaltyCooldownRemaining;

	private float forwardInput;

	private float rightInput;

	private float movementSpeed;

	private float spine2Multiplier;

	private float sprintWeight;

	private float neckMultiplier;

	private int fixedFrameNumber;

	private int maxFixedFrameNumber = 10;

	[SyncVar]
	private float syncForwardInput;

	[SyncVar]
	private float syncRightInput;

	[SerializeField]
	private float aimY;

	[SyncVar]
	[SerializeField]
	private float syncedAimY;

	[SyncVar]
	private float syncedAimX;

	private float lastAimX;

	[SerializeField]
	[SyncVar]
	private bool syncIsGrounded;

	[SerializeField]
	protected float baseHealth;

	[SerializeField]
	[SyncVar(hook = "HealthChanged")]
	private float health;

	[SerializeField]
	[SyncVar]
	private bool sprinting;

	[SerializeField]
	[SyncVar]
	protected bool sneaking;

	private GameObject preparedRagdoll;

	private Vector3 lastPosition;

	private Vector3 recordVelocity;

	private float pivot;

	private bool walking;

	private bool jumpQueued;

	private bool aimingDownSights;

	private bool callDeathFlag;

	private bool crouch;

	private Animator animator;

	protected CharacterController charController;

	[SerializeField]
	protected GameObject weaponParent;

	[SerializeField]
	private PlayerItem item;

	[SerializeField]
	[SyncVar]
	protected string itemString;

	[SerializeField]
	private Vector3 velocity;

	[SerializeField]
	protected string oldItem = "";

	[SyncVar]
	protected int leanState;

	[SerializeField]
	private GameObject leanLimb;

	[SerializeField]
	private float lean;

	private float lean_kP = 0.2f;

	[SyncVar]
	private int moveAnimationState;

	[SyncVar]
	private int overrideMoveAnimationState;

	[SyncVar]
	private int armAnimationState;

	[SyncVar]
	private bool leaveRepetitiveReload;

	[SerializeField]
	[SyncVar]
	private string humanName;

	[SerializeField]
	[SyncVar]
	private int team;

	private Inventory humanInventory = new Inventory();

	[SerializeField]
	protected Nametag playerNameTag;

	protected Player localPlayer;

	[Header("Footsteps")]
	[SerializeField]
	private GameObject[] FootstepGrassSprintSounds;

	[SerializeField]
	private GameObject[] FootstepGrassSounds;

	[SerializeField]
	private GameObject[] landGrassSounds;

	[SerializeField]
	private GameObject[] FootstepWoodSprintSounds;

	[SerializeField]
	private GameObject[] FootstepWoodSounds;

	[SerializeField]
	private GameObject[] landWoodSounds;

	[SerializeField]
	private GameObject[] FootstepConcreteSprintSounds;

	[SerializeField]
	private GameObject[] FootstepConcreteSounds;

	[SerializeField]
	private GameObject[] landConcreteSounds;

	[SerializeField]
	private GameObject[] FootstepMetalSprintSounds;

	[SerializeField]
	private GameObject[] FootstepMetalSounds;

	[SerializeField]
	private GameObject[] landMetalSounds;

	[SerializeField]
	private GameObject[] slideSounds;

	private int lastStepSound;

	private bool stepSound;

	private float stepCoolTime = 0.5f;

	[SerializeField]
	private float rightFootCastDistance;

	[SerializeField]
	private float leftFootCastDistance;

	[SerializeField]
	private float pControlledRightFootPos;

	[SerializeField]
	private float pControlledLeftFootPos;

	[SerializeField]
	private float rightToeCastDistance;

	[SerializeField]
	private float leftToeCastDistance;

	[SerializeField]
	private float pControlledRightFootAngle;

	[SerializeField]
	private float pControlledLeftFootAngle;

	[SerializeField]
	protected GameObject[] fleshImpactSounds;

	[Header("Clothing Items")]
	[SerializeField]
	protected Transform armatureToAssign;

	[SerializeField]
	protected GameObject shirtParent;

	[SerializeField]
	protected GameObject pantsParent;

	[SerializeField]
	protected GameObject vestsParent;

	[SerializeField]
	protected GameObject headwearParent;

	[SerializeField]
	protected GameObject hoodsParent;

	[SerializeField]
	protected string shirt;

	[SerializeField]
	protected string pants;

	[SerializeField]
	protected string headwear;

	[SerializeField]
	protected string vest;

	[SerializeField]
	protected string hood;

	protected Vector3 headImpactOffset;

	protected Vector3 spine3ImpactOffset;

	protected Vector3 spine2ImpactOffset;

	protected Vector3 headImpactVelocity;

	protected Vector3 spine3ImpactVelocity;

	protected Vector3 spine2ImpactVelocity;

	protected Vector3 headImpactTargVelocity;

	protected Vector3 spine3ImpactTargVelocity;

	protected Vector3 spine2ImpactTargVelocity;

	private float impactAnimationRecover_kP = 0.4f;

	private float impactAnimationVelocity_kP = 0.2f;

	private ClothesLibrary clothesLibrary;

	[SerializeField]
	private GameObject clothesLibraryToSpawn;

	private float targPivot;

	private float pivotWeight;

	private bool resetPivotAngle;

	private bool fleshImpactCool;

	[SerializeField]
	private bool useThirdPersonAnimations;

	[SerializeField]
	protected bool updateAppearanceFlag;

	protected HardlineGameManager gameManager;

	protected HardlineNetworkManager networkManager;

	private SpectatorCamera spectatorCamera;

	private List<Human> playersKilled = new List<Human>();

	private List<Human> playersDamaged = new List<Human>();

	public float AimY
	{
		get
		{
			return aimY;
		}
		set
		{
			aimY = value;
		}
	}

	public GameObject ItemAlignment
	{
		get
		{
			return itemAlignment;
		}
		set
		{
			itemAlignment = value;
		}
	}

	public bool AimingDownSights
	{
		get
		{
			return aimingDownSights;
		}
		set
		{
			aimingDownSights = value;
		}
	}

	public PlayerItem Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
		}
	}

	public float Health
	{
		get
		{
			return health;
		}
		set
		{
			Networkhealth = value;
		}
	}

	public bool CallDeathFlag
	{
		get
		{
			return callDeathFlag;
		}
		set
		{
			callDeathFlag = value;
		}
	}

	public GameObject[] Meshes
	{
		get
		{
			return meshes;
		}
		set
		{
			meshes = value;
		}
	}

	public GameObject Ragdoll
	{
		get
		{
			return ragdoll;
		}
		set
		{
			ragdoll = value;
		}
	}

	public bool Sprinting
	{
		get
		{
			return sprinting;
		}
		set
		{
			Networksprinting = value;
		}
	}

	public float ForwardInput
	{
		get
		{
			return forwardInput;
		}
		set
		{
			forwardInput = value;
		}
	}

	public float RightInput
	{
		get
		{
			return rightInput;
		}
		set
		{
			rightInput = value;
		}
	}

	public bool JumpQueued
	{
		get
		{
			return jumpQueued;
		}
		set
		{
			jumpQueued = value;
		}
	}

	public Animator Animator
	{
		get
		{
			return animator;
		}
		set
		{
			animator = value;
		}
	}

	public bool Crouch
	{
		get
		{
			return crouch;
		}
		set
		{
			crouch = value;
		}
	}

	public bool Walking
	{
		get
		{
			return walking;
		}
		set
		{
			walking = value;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	public float Gravity
	{
		get
		{
			return gravity;
		}
		set
		{
			gravity = value;
		}
	}

	public float FloorGravity
	{
		get
		{
			return floorGravity;
		}
		set
		{
			floorGravity = value;
		}
	}

	public float Lean
	{
		get
		{
			return lean;
		}
		set
		{
			lean = value;
		}
	}

	public bool LeaveRepetitiveReload
	{
		get
		{
			return leaveRepetitiveReload;
		}
		set
		{
			NetworkleaveRepetitiveReload = value;
		}
	}

	public float SyncedAimY
	{
		get
		{
			return syncedAimY;
		}
		set
		{
			NetworksyncedAimY = value;
		}
	}

	public int Team
	{
		get
		{
			return team;
		}
		set
		{
			Networkteam = value;
		}
	}

	public Inventory HumanInventory
	{
		get
		{
			return humanInventory;
		}
		set
		{
			humanInventory = value;
		}
	}

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public string HumanName
	{
		get
		{
			return humanName;
		}
		set
		{
			NetworkhumanName = value;
		}
	}

	public float Pivot
	{
		get
		{
			return pivot;
		}
		set
		{
			pivot = value;
		}
	}

	public float SyncedAimX
	{
		get
		{
			return syncedAimX;
		}
		set
		{
			NetworksyncedAimX = value;
		}
	}

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	public int Kills
	{
		get
		{
			return kills;
		}
		set
		{
			kills = value;
		}
	}

	public int Assists
	{
		get
		{
			return assists;
		}
		set
		{
			assists = value;
		}
	}

	public int Deaths
	{
		get
		{
			return deaths;
		}
		set
		{
			deaths = value;
		}
	}

	public List<Human> PlayersKilled
	{
		get
		{
			return playersKilled;
		}
		set
		{
			playersKilled = value;
		}
	}

	public List<Human> PlayersDamaged
	{
		get
		{
			return playersDamaged;
		}
		set
		{
			playersDamaged = value;
		}
	}

	public bool UseThirdPersonAnimations
	{
		get
		{
			return useThirdPersonAnimations;
		}
		set
		{
			useThirdPersonAnimations = value;
		}
	}

	public Transform HeadTransform
	{
		get
		{
			return HeadAimTransform;
		}
		set
		{
			HeadAimTransform = value;
		}
	}

	public GameObject Spine2
	{
		get
		{
			return spine2;
		}
		set
		{
			spine2 = value;
		}
	}

	public Transform HeadAimTransform
	{
		get
		{
			return headAimTransform;
		}
		set
		{
			headAimTransform = value;
		}
	}

	public int OverrideMoveAnimationState
	{
		get
		{
			return overrideMoveAnimationState;
		}
		set
		{
			NetworkoverrideMoveAnimationState = value;
		}
	}

	public GameObject Spine3
	{
		get
		{
			return spine3;
		}
		set
		{
			spine3 = value;
		}
	}

	public GameObject Neck
	{
		get
		{
			return neck;
		}
		set
		{
			neck = value;
		}
	}

	public GameObject HeadCamPosition
	{
		get
		{
			return headCamPosition;
		}
		set
		{
			headCamPosition = value;
		}
	}

	public SpectatorCamera SpectatorCamera
	{
		get
		{
			return spectatorCamera;
		}
		set
		{
			spectatorCamera = value;
		}
	}

	public float NetworksyncForwardInput
	{
		get
		{
			return syncForwardInput;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref syncForwardInput))
			{
				float num = syncForwardInput;
				SetSyncVar(value, ref syncForwardInput, 1uL);
			}
		}
	}

	public float NetworksyncRightInput
	{
		get
		{
			return syncRightInput;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref syncRightInput))
			{
				float num = syncRightInput;
				SetSyncVar(value, ref syncRightInput, 2uL);
			}
		}
	}

	public float NetworksyncedAimY
	{
		get
		{
			return syncedAimY;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref syncedAimY))
			{
				float num = syncedAimY;
				SetSyncVar(value, ref syncedAimY, 4uL);
			}
		}
	}

	public float NetworksyncedAimX
	{
		get
		{
			return syncedAimX;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref syncedAimX))
			{
				float num = syncedAimX;
				SetSyncVar(value, ref syncedAimX, 8uL);
			}
		}
	}

	public bool NetworksyncIsGrounded
	{
		get
		{
			return syncIsGrounded;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref syncIsGrounded))
			{
				bool flag = syncIsGrounded;
				SetSyncVar(value, ref syncIsGrounded, 16uL);
			}
		}
	}

	public float Networkhealth
	{
		get
		{
			return health;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref health))
			{
				float oldValue = health;
				SetSyncVar(value, ref health, 32uL);
				if (NetworkServer.localClientActive && !GetSyncVarHookGuard(32uL))
				{
					SetSyncVarHookGuard(32uL, value: true);
					HealthChanged(oldValue, value);
					SetSyncVarHookGuard(32uL, value: false);
				}
			}
		}
	}

	public bool Networksprinting
	{
		get
		{
			return sprinting;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref sprinting))
			{
				bool flag = sprinting;
				SetSyncVar(value, ref sprinting, 64uL);
			}
		}
	}

	public bool Networksneaking
	{
		get
		{
			return sneaking;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref sneaking))
			{
				bool flag = sneaking;
				SetSyncVar(value, ref sneaking, 128uL);
			}
		}
	}

	public string NetworkitemString
	{
		get
		{
			return itemString;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref itemString))
			{
				string text = itemString;
				SetSyncVar(value, ref itemString, 256uL);
			}
		}
	}

	public int NetworkleanState
	{
		get
		{
			return leanState;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref leanState))
			{
				int num = leanState;
				SetSyncVar(value, ref leanState, 512uL);
			}
		}
	}

	public int NetworkmoveAnimationState
	{
		get
		{
			return moveAnimationState;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref moveAnimationState))
			{
				int num = moveAnimationState;
				SetSyncVar(value, ref moveAnimationState, 1024uL);
			}
		}
	}

	public int NetworkoverrideMoveAnimationState
	{
		get
		{
			return overrideMoveAnimationState;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref overrideMoveAnimationState))
			{
				int num = overrideMoveAnimationState;
				SetSyncVar(value, ref overrideMoveAnimationState, 2048uL);
			}
		}
	}

	public int NetworkarmAnimationState
	{
		get
		{
			return armAnimationState;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref armAnimationState))
			{
				int num = armAnimationState;
				SetSyncVar(value, ref armAnimationState, 4096uL);
			}
		}
	}

	public bool NetworkleaveRepetitiveReload
	{
		get
		{
			return leaveRepetitiveReload;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref leaveRepetitiveReload))
			{
				bool flag = leaveRepetitiveReload;
				SetSyncVar(value, ref leaveRepetitiveReload, 8192uL);
			}
		}
	}

	public string NetworkhumanName
	{
		get
		{
			return humanName;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref humanName))
			{
				string text = humanName;
				SetSyncVar(value, ref humanName, 16384uL);
			}
		}
	}

	public int Networkteam
	{
		get
		{
			return team;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref team))
			{
				int num = team;
				SetSyncVar(value, ref team, 32768uL);
			}
		}
	}

	[Server]
	protected void SetLeanState(int leanState)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Human::SetLeanState(System.Int32)' called when server was not active");
		}
		else
		{
			NetworkleanState = leanState;
		}
	}

	[Server]
	protected void SetHealth(float health)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Human::SetHealth(System.Single)' called when server was not active");
		}
		else
		{
			Networkhealth = health;
		}
	}

	protected void ApplyImpact(Vector3 hitPos, Vector3 hitRot, float damage)
	{
		if (damage != 4f)
		{
			if (Random.Range(1, 3) == 1)
			{
				spine2ImpactVelocity += new Vector3(1f, 1f, 1f) * Random.Range(1f, 1.3f) * damage;
				spine3ImpactVelocity += new Vector3(1f, 1f, 1f) * Random.Range(1f, 1.3f) * damage;
				headImpactVelocity += new Vector3(1f, 1f, 1f) * Random.Range(1f, 1.3f) * damage;
			}
			else
			{
				spine2ImpactVelocity += new Vector3(-1f, -1f, -1f) * Random.Range(1f, 1.3f) * damage;
				spine3ImpactVelocity += new Vector3(-1f, -1f, -1f) * Random.Range(1f, 1.3f) * damage;
				headImpactVelocity += new Vector3(-1f, -1f, -1f) * Random.Range(1f, 1.3f) * damage;
			}
		}
	}

	protected virtual void ReduceHealth(float damage, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		ApplyImpact(hitPos, hitRot, damage);
		if (health > 0f)
		{
			if (health - damage < 100f)
			{
				Networkhealth = health - damage;
			}
			else
			{
				Networkhealth = 100f;
			}
		}
	}

	protected void UpdateAnimationStates(int movement, int arms, int overrideMovement)
	{
		if (!base.hasAuthority)
		{
			NetworkmoveAnimationState = movement;
			NetworkarmAnimationState = arms;
			OverrideMoveAnimationState = overrideMovement;
		}
	}

	protected void UpdateAimDownSightState()
	{
		if (armAnimationState == 1)
		{
			aimingDownSights = true;
		}
		else
		{
			aimingDownSights = false;
		}
	}

	[Server]
	protected void SetLeaveRepetitiveReload(bool enabled)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Human::SetLeaveRepetitiveReload(System.Boolean)' called when server was not active");
		}
		else
		{
			LeaveRepetitiveReload = enabled;
		}
	}

	protected void RunReloadAnimation()
	{
		Animator.SetTrigger("Reload");
	}

	public void PrimaryUseItem()
	{
		if (!(health <= 0f) && !base.hasAuthority && (bool)item && item is PlayerFirearm)
		{
			(item as PlayerFirearm).Shoot();
		}
	}

	public void SetAimY(float aimY)
	{
		NetworksyncedAimY = aimY;
	}

	public void SetAimX(float aimX)
	{
		NetworksyncedAimX = aimX;
	}

	public void SetItemString(string name)
	{
		if (!base.hasAuthority)
		{
			NetworkitemString = name;
			LoadItem(name, base.hasAuthority);
		}
	}

	public void ForceSetItemString(string name)
	{
		NetworkitemString = name;
		LoadItem(name, base.hasAuthority);
	}

	public void SetTeam(int team)
	{
		Team = team;
	}

	public void UpdateTeamAppearance()
	{
		if (!shirtParent || !pantsParent || !headwearParent || !vestsParent || !hoodsParent)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if ((bool)gameManager && gameManager is RoundsHardlineGameManager)
		{
			num = (gameManager as RoundsHardlineGameManager).Team1Wins;
			num2 = (gameManager as RoundsHardlineGameManager).Team2Wins;
		}
		if (team == 1)
		{
			if (num2 < 2)
			{
				shirt = "Shirt NATO";
				pants = "Pants NATO";
				vest = "Sling Bag";
				headwear = "NATO Baseball Cap";
				hood = "Empty";
			}
			else if (num2 < 3)
			{
				shirt = "Shirt NATO";
				pants = "Pants NATO";
				vest = "NATO Plate Carrier";
				headwear = "NATO Baseball Cap";
				hood = "Empty";
			}
			else if (num2 < 5)
			{
				shirt = "Shirt NATO";
				pants = "Pants NATO";
				vest = "NATO Chest Rig 1";
				headwear = "FastHelmeMasked";
				hood = "Empty";
			}
			else
			{
				shirt = "Shirt NATO";
				pants = "Pants NATO";
				vest = "NATO Chest Rig 1";
				headwear = "FastHelmeMasked";
				hood = "Empty";
			}
		}
		else if (num < 2)
		{
			shirt = "Shirt OPFOR";
			pants = "Pants OPFOR";
			vest = "OPFOR Chest Rig";
			headwear = "OpHelmet";
			hood = "Marauder Hood";
		}
		else if (num < 3)
		{
			shirt = "Shirt OPFOR";
			pants = "Pants OPFOR";
			vest = "OPFOR Plate Carrier";
			headwear = "OpHelmet";
			hood = "Marauder Hood";
		}
		else if (num < 5)
		{
			shirt = "Shirt OPFOR";
			pants = "Pants OPFOR";
			vest = "OPFOR Plate Carrier";
			headwear = "Altyn Helmet";
			hood = "Empty";
		}
		else
		{
			shirt = "Shirt OPFOR";
			pants = "Pants OPFOR";
			vest = "OPFOR Plate Carrier";
			headwear = "Altyn Helmet";
			hood = "Empty";
		}
		if (shirt != "" && pants != "" && vest != "" && headwear != "" && hood != "")
		{
			SetAllActive(shirtParent, enabled: false, self: false);
			SetAllActive(pantsParent, enabled: false, self: false);
			SetAllActive(headwearParent, enabled: false, self: false);
			SetAllActive(vestsParent, enabled: false, self: false);
			SetAllActive(hoodsParent, enabled: false, self: false);
			shirtParent.SetActive(value: true);
			pantsParent.SetActive(value: true);
			vestsParent.SetActive(value: true);
			headwearParent.SetActive(value: true);
			hoodsParent.SetActive(value: true);
			SetAllActive(GetChildWithName(shirtParent, shirt), enabled: true, self: true);
			SetAllActive(GetChildWithName(pantsParent, pants), enabled: true, self: true);
			SetAllActive(GetChildWithName(headwearParent, headwear), enabled: true, self: true);
			SetAllActive(GetChildWithName(vestsParent, vest), enabled: true, self: true);
			SetAllActive(GetChildWithName(hoodsParent, hood), enabled: true, self: true);
		}
	}

	private GameObject GetChildWithName(GameObject obj, string name)
	{
		Transform transform = obj.transform.Find(name);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	private void SetAllActive(GameObject item, bool enabled, bool self)
	{
		if (self)
		{
			item.SetActive(enabled);
		}
		Transform[] componentsInChildren = item.GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(enabled);
		}
	}

	public void DestroyClothesLibrary()
	{
		Object.Destroy(clothesLibrary.gameObject);
	}

	public void ConfigureClothingItem(GameObject clothingItem)
	{
		if ((bool)clothingItem.GetComponent<ReassignBoneWeigthsToNewMesh>())
		{
			clothingItem.GetComponent<ReassignBoneWeigthsToNewMesh>().newArmature = armatureToAssign;
			clothingItem.GetComponent<ReassignBoneWeigthsToNewMesh>().rootBoneName = "root";
			clothingItem.GetComponent<ReassignBoneWeigthsToNewMesh>().ReassignClothing();
		}
		ReassignBoneWeigthsToNewMesh[] componentsInChildren = clothingItem.GetComponentsInChildren<ReassignBoneWeigthsToNewMesh>();
		foreach (ReassignBoneWeigthsToNewMesh obj in componentsInChildren)
		{
			obj.GetComponent<ReassignBoneWeigthsToNewMesh>().newArmature = armatureToAssign;
			obj.GetComponent<ReassignBoneWeigthsToNewMesh>().rootBoneName = "root";
			obj.GetComponent<ReassignBoneWeigthsToNewMesh>().ReassignClothing();
		}
	}

	public void SetHumanName(string name)
	{
		HumanName = name;
	}

	[Command]
	protected void CmdUpdateLeanState(int value)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(value);
		SendCommandInternal(typeof(Human), "CmdUpdateLeanState", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(value);
		writer.WriteString(causer);
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		SendCommandInternal(typeof(Human), "CmdDealDamage", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(value);
		writer.WriteString(causer);
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		SendRPCInternal(typeof(Human), "RpcDealDamage", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetAimY(float aimY)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(aimY);
		SendCommandInternal(typeof(Human), "CmdSetAimY", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetAimY(float aimY)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(aimY);
		SendRPCInternal(typeof(Human), "RpcSetAimY", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetAimX(float aimX)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(aimX);
		SendCommandInternal(typeof(Human), "CmdSetAimX", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetForwardInput(float forward)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(forward);
		SendRPCInternal(typeof(Human), "RpcSetForwardInput", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetForwardInput(float forward)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(forward);
		SendCommandInternal(typeof(Human), "CmdSetForwardInput", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetRightInput(float right)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(right);
		SendRPCInternal(typeof(Human), "RpcSetRightInput", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetRightInput(float right)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(right);
		SendCommandInternal(typeof(Human), "CmdSetRightInput", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetAimX(float aimX)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(aimX);
		SendRPCInternal(typeof(Human), "RpcSetAimX", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdUpdateAnimationStates(int movement, int arms, int overrideMovement)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(movement);
		writer.WriteInt(arms);
		writer.WriteInt(overrideMovement);
		SendCommandInternal(typeof(Human), "CmdUpdateAnimationStates", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcUpdateAnimationStates(int movement, int arms, int overrideMovement)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(movement);
		writer.WriteInt(arms);
		writer.WriteInt(overrideMovement);
		SendRPCInternal(typeof(Human), "RpcUpdateAnimationStates", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdSetItemString(string item)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(item);
		SendCommandInternal(typeof(Human), "CmdSetItemString", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetItemString(string item)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(item);
		SendRPCInternal(typeof(Human), "RpcSetItemString", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdRunReloadAnimation()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(Human), "CmdRunReloadAnimation", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcRunReloadAnimation()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(Human), "RpcRunReloadAnimation", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetLeaveRepetitiveReload(bool enabled)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(enabled);
		SendCommandInternal(typeof(Human), "CmdSetLeaveRepetitiveReload", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdPrimaryUseItem()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(Human), "CmdPrimaryUseItem", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcPrimaryUseItem()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(Human), "RpcPrimaryUseItem", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetSprinting(bool sprinting)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sprinting);
		SendCommandInternal(typeof(Human), "CmdSetSprinting", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetSprinting(bool sprinting)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sprinting);
		SendRPCInternal(typeof(Human), "RpcSetSprinting", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetSneaking(bool sneaking)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sneaking);
		SendCommandInternal(typeof(Human), "CmdSetSneaking", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetSneaking(bool sneaking)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sneaking);
		SendRPCInternal(typeof(Human), "RpcSetSneaking", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetSliding(bool sliding)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sliding);
		SendCommandInternal(typeof(Human), "CmdSetSliding", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetSliding(bool sliding)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(sliding);
		SendRPCInternal(typeof(Human), "RpcSetSliding", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetTeam(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendCommandInternal(typeof(Human), "CmdSetTeam", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetTeam(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendRPCInternal(typeof(Human), "RpcSetTeam", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetHumanName(string humanName)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(humanName);
		SendCommandInternal(typeof(Human), "CmdSetHumanName", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetHumanName(string humanName)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(humanName);
		SendRPCInternal(typeof(Human), "RpcSetHumanName", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void SetIsGrounded(bool isGrounded)
	{
		NetworksyncIsGrounded = isGrounded;
	}

	[Command]
	public void CmdSetIsGrounded(bool isGrounded)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(isGrounded);
		SendCommandInternal(typeof(Human), "CmdSetIsGrounded", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetIsGrounded(bool isGrounded)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(isGrounded);
		SendRPCInternal(typeof(Human), "RpcSetIsGrounded", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetScore(int score)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(score);
		SendCommandInternal(typeof(Human), "CmdSetScore", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetScore(int score)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(score);
		SendRPCInternal(typeof(Human), "RpcSetScore", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetKills(int kills)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(kills);
		SendCommandInternal(typeof(Human), "CmdSetKills", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetKills(int kills)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(kills);
		SendRPCInternal(typeof(Human), "RpcSetKills", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetAssists(int assists)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(assists);
		SendCommandInternal(typeof(Human), "CmdSetAssists", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetAssists(int assists)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(assists);
		SendRPCInternal(typeof(Human), "RpcSetAssists", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetDeaths(int deaths)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(deaths);
		SendCommandInternal(typeof(Human), "CmdSetDeaths", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetDeaths(int deaths)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(deaths);
		SendRPCInternal(typeof(Human), "RpcSetDeaths", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSyncAttachments(int sight, int barrel, int grip)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(sight);
		writer.WriteInt(barrel);
		writer.WriteInt(grip);
		SendCommandInternal(typeof(Human), "CmdSyncAttachments", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSyncAttachments(int sight, int barrel, int grip)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(sight);
		writer.WriteInt(barrel);
		writer.WriteInt(grip);
		SendRPCInternal(typeof(Human), "RpcSyncAttachments", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientCallback]
	protected virtual void ClientUpdate()
	{
		if (NetworkClient.active)
		{
		}
	}

	protected virtual void FixedUpdate()
	{
		RecordVelocity();
	}

	private void RecordVelocity()
	{
		recordVelocity = (base.transform.position - lastPosition) * RECORD_VELOCITY_CONSTANT;
		lastPosition = base.transform.position;
	}

	[ClientCallback]
	protected virtual void ClientFixedUpdate()
	{
		if (NetworkClient.active && base.hasAuthority)
		{
			CmdUpdateLeanState(leanState);
			ReplicateIsGrounded(charController.isGrounded);
			ReplicateSliding(isSliding);
			ReplicateLoadItem(item.ItemName);
			ReplicateAimY(AimY);
			ReplicateAimX(base.transform.eulerAngles.y);
			if (fixedFrameNumber == 0)
			{
				ReplicateForwardInput(ForwardInput);
				ReplicateRightInput(RightInput);
			}
			if (fixedFrameNumber == 3)
			{
				ReplicateSprinting(Sprinting);
				ReplicateSneaking(sneaking);
				ReplicateAnimationStates(moveAnimationState, armAnimationState, OverrideMoveAnimationState);
			}
			if (fixedFrameNumber == 5)
			{
				CmdSetLeaveRepetitiveReload(leaveRepetitiveReload);
			}
			if (fixedFrameNumber == 7)
			{
				ReplicateScore(score);
				ReplicateKills(kills);
				ReplicateAssists(assists);
				ReplicateDeaths(deaths);
			}
			if (fixedFrameNumber == 9)
			{
				ReplicateTeam(team);
				ReplicateAttachments();
			}
			fixedFrameNumber++;
			if (fixedFrameNumber > maxFixedFrameNumber)
			{
				fixedFrameNumber = 0;
			}
		}
	}

	public void ReplicatePrimaryUseItem()
	{
		if (!(health < 0f))
		{
			if (!base.isServer)
			{
				CmdPrimaryUseItem();
			}
			else if (base.isServer)
			{
				RpcPrimaryUseItem();
			}
		}
	}

	public void ReplicateAnimationStates(int movement, int arms, int overrideMovement)
	{
		if (!base.isServer)
		{
			CmdUpdateAnimationStates(movement, arms, overrideMovement);
		}
		else if (base.isServer)
		{
			RpcUpdateAnimationStates(movement, arms, overrideMovement);
		}
	}

	public void ReplicateReloadAnimation()
	{
		if (!base.isServer)
		{
			CmdRunReloadAnimation();
		}
		else if (base.isServer)
		{
			RpcRunReloadAnimation();
		}
	}

	public void ReplicateTeam(int team)
	{
		if (!base.isServer)
		{
			CmdSetTeam(team);
		}
		else if (base.isServer)
		{
			RpcSetTeam(team);
		}
	}

	public void ReplicateAttachments()
	{
		if (item is PlayerFirearm)
		{
			PlayerFirearm playerFirearm = item as PlayerFirearm;
			if (!base.isServer)
			{
				CmdSyncAttachments(playerFirearm.SightAttachment, playerFirearm.BarrelAttachment, playerFirearm.GripAttachment);
			}
			else if (base.isServer)
			{
				RpcSyncAttachments(playerFirearm.SightAttachment, playerFirearm.BarrelAttachment, playerFirearm.GripAttachment);
			}
		}
	}

	public void ReplicateHumanName(string name)
	{
		if (!base.isServer)
		{
			CmdSetHumanName(name);
		}
		else if (base.isServer)
		{
			RpcSetHumanName(name);
		}
	}

	public void ReplicateIsGrounded(bool isGrounded)
	{
		if (!base.isServer)
		{
			CmdSetIsGrounded(isGrounded);
		}
		else if (base.isServer)
		{
			RpcSetIsGrounded(isGrounded);
		}
	}

	public void ReplicateLoadItem(string item)
	{
		if (!base.isServer)
		{
			SetItemString(item);
			CmdSetItemString(item);
		}
		else if (base.isServer)
		{
			RpcSetItemString(item);
		}
	}

	public void ReplicateLoadItem()
	{
		if (!base.isServer)
		{
			CmdSetItemString(itemString);
		}
		else if (base.isServer)
		{
			RpcSetItemString(itemString);
		}
	}

	public void ReplicateAimY(float aimY)
	{
		if (!base.isServer && base.hasAuthority)
		{
			CmdSetAimY(aimY);
		}
		else if (base.isServer && base.hasAuthority)
		{
			RpcSetAimY(aimY);
		}
	}

	public void ReplicateAimX(float aimX)
	{
		if (!base.isServer && base.hasAuthority)
		{
			CmdSetAimX(aimX);
		}
		else if (base.isServer && base.hasAuthority)
		{
			RpcSetAimX(aimX);
		}
	}

	public void ReplicateForwardInput(float forwardInput)
	{
		if (!base.isServer && base.hasAuthority)
		{
			CmdSetForwardInput(forwardInput);
		}
		else if (base.isServer && base.hasAuthority)
		{
			RpcSetForwardInput(forwardInput);
		}
	}

	public void ReplicateRightInput(float rightInput)
	{
		if (!base.isServer && base.hasAuthority)
		{
			CmdSetRightInput(rightInput);
		}
		else if (base.isServer && base.hasAuthority)
		{
			RpcSetRightInput(rightInput);
		}
	}

	public void ReplicateSprinting(bool sprinting)
	{
		if (!base.isServer)
		{
			CmdSetSprinting(sprinting);
		}
		else if (base.isServer)
		{
			RpcSetSprinting(sprinting);
		}
	}

	public void ReplicateSneaking(bool sneaking)
	{
		if (!base.isServer)
		{
			CmdSetSneaking(sneaking);
		}
		else if (base.isServer)
		{
			RpcSetSneaking(sneaking);
		}
	}

	public void ReplicateSliding(bool sliding)
	{
		if (!base.isServer)
		{
			CmdSetSliding(sliding);
		}
		else if (base.isServer)
		{
			RpcSetSliding(sliding);
		}
	}

	public virtual void ReplicateDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		if (base.isServer)
		{
			ReduceHealth(value, causer, hitPos, hitRot);
			RpcDealDamage(value, causer, hitPos, hitRot);
		}
		else
		{
			CmdDealDamage(value, causer, hitPos, hitRot);
		}
	}

	public void ReplicateScore(int value)
	{
		if (!base.isServer)
		{
			CmdSetScore(value);
		}
		else if (base.isServer)
		{
			RpcSetScore(value);
		}
	}

	public void ReplicateKills(int value)
	{
		if (!base.isServer)
		{
			CmdSetKills(value);
		}
		else if (base.isServer)
		{
			RpcSetKills(value);
		}
	}

	public void ReplicateAssists(int value)
	{
		if (!base.isServer)
		{
			CmdSetAssists(value);
		}
		else if (base.isServer)
		{
			RpcSetAssists(value);
		}
	}

	public void ReplicateDeaths(int value)
	{
		if (!base.isServer)
		{
			CmdSetDeaths(value);
		}
		else if (base.isServer)
		{
			RpcSetDeaths(value);
		}
	}

	public virtual void HealthChanged(float oldValue, float newValue)
	{
		if (newValue < oldValue)
		{
			FleshImpact();
		}
		Networkhealth = newValue;
		if (!CallDeathFlag && health <= 0f)
		{
			Death();
			CallDeathFlag = true;
		}
	}

	public void AddKill()
	{
		if (base.hasAuthority)
		{
			Kills++;
		}
	}

	public void AddScore(int score)
	{
		if (base.hasAuthority)
		{
			this.score += score;
		}
	}

	public virtual void Start()
	{
	}

	public virtual void Init()
	{
		gameManager = Object.FindObjectOfType<HardlineGameManager>();
		networkManager = Object.FindObjectOfType<HardlineNetworkManager>();
		if (base.gameObject != null)
		{
			CallDeathFlag = false;
			PlayersKilled.Clear();
			PlayersDamaged.Clear();
			stepSound = true;
			humanInventory.Init();
			Animator = GetComponent<Animator>();
			charController = GetComponent<CharacterController>();
			Networkhealth = baseHealth;
			oldItem = "";
			LeaveRepetitiveReload = true;
			SetClothingVisiblity(visible: true);
			if (!base.hasAuthority)
			{
				ClothingWithExtras[] componentsInChildren = GetComponentsInChildren<ClothingWithExtras>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ShowExtras();
				}
			}
			NetworkleanState = 0;
			animator.SetInteger("MoveState", 0);
			AimY = 0f;
		}
		UpdateTeamAppearance();
	}

	public void SetClothingVisiblity(bool visible)
	{
		headwearParent.SetActive(visible);
		vestsParent.SetActive(visible);
		shirtParent.SetActive(visible);
		pantsParent.SetActive(visible);
		hoodsParent.SetActive(visible);
	}

	protected void FindLocalPlayer()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.hasAuthority)
			{
				localPlayer = player;
			}
		}
	}

	public virtual void Update()
	{
		if (Health <= 0f && !CallDeathFlag)
		{
			Death();
		}
		else
		{
			CheckHeadClearance();
			CheckFootsteps();
			UpdateAnimatorValues();
			UpdatePivot();
			if (forwardInput != 0f || rightInput != 0f)
			{
				walking = true;
			}
			else
			{
				walking = false;
			}
		}
		if (localPlayer == null)
		{
			FindLocalPlayer();
		}
		else if ((bool)playerNameTag)
		{
			UpdateBillboard();
		}
		if (!base.hasAuthority)
		{
			UpdateAimDownSightState();
		}
		if (updateAppearanceFlag)
		{
			UpdateTeamAppearance();
			updateAppearanceFlag = false;
		}
	}

	public void UpdatePlayerRotations(float x, float y)
	{
		if (UseThirdPersonAnimations)
		{
			pivot += x;
		}
		else
		{
			base.transform.Rotate(0f, x, 0f);
		}
		AimY += y;
		AimY = Mathf.Clamp(AimY, -60f, 70f);
	}

	public virtual void UpdateMovement()
	{
		UpdateMovementPenalties();
		if (Sprinting)
		{
			movementSpeed = baseMovementSpeed * sprintSpeedMultiplier;
		}
		else if (crouch)
		{
			movementSpeed = baseMovementSpeed * crouchWalkSpeedMultiplier;
		}
		else
		{
			movementSpeed = baseMovementSpeed;
		}
		if (sneaking)
		{
			movementSpeed *= sneakSpeedMultiplier;
		}
		if (!fallRecovered)
		{
			movementSpeed *= fallPenalty;
		}
		if (!slideRecovered)
		{
			movementSpeed *= currentSlidePenalty;
		}
		if (isSliding)
		{
			if (!slidePenaltyFlag)
			{
				jumpStrengthMultiplier = slideJumpStrength;
				currentSlideAddVelocity *= slideBoostPenalty;
				slidePenaltyFlag = true;
			}
			movementSpeed *= slideSteeringVelocityMultiplier;
		}
		else
		{
			currentSlidePenalty = Mathf.Clamp(currentSlidePenalty + slideRecoveryRate, 0f, 1f);
			slidePenaltyFlag = false;
		}
		if (item is PlayerFirearm && aimingDownSights)
		{
			movementSpeed *= (item as PlayerFirearm).AdsMovementSpeedMultiplier;
		}
		if (velocity.magnitude <= slideMinVelocity && isSliding)
		{
			CancelSlide();
		}
		if (charController.isGrounded)
		{
			jumpStrengthMultiplier += (1f - jumpStrengthMultiplier) * jumpStrengthRecovery_kP;
			if (velocity.y <= 0f - landRegisterThreshold)
			{
				FallLand(velocity.y);
			}
			animator.SetBool("MidAir", value: false);
			Velocity += spine1.transform.forward * ForwardInput * movementSpeed + spine1.transform.right * RightInput * movementSpeed;
			if (isSliding)
			{
				Velocity = new Vector3(Velocity.x / groundSlidingFriction, 0f - FloorGravity, Velocity.z / groundSlidingFriction);
			}
			else if (Walking || Sprinting)
			{
				Velocity = new Vector3(Velocity.x / groundMovingFriction, 0f - FloorGravity, Velocity.z / groundMovingFriction);
			}
			else
			{
				Velocity = new Vector3(Velocity.x / groundFriction, 0f - FloorGravity, Velocity.z / groundFriction);
			}
			if (forwardInput == 0f && rightInput == 0f)
			{
				Walking = false;
			}
			else
			{
				Walking = true;
			}
			if (JumpQueued && jumpReady)
			{
				CancelSlide();
				Jump(jumpStrength);
				jumpReady = false;
				jumpCooldownRemaining = jumpCooldown;
			}
		}
		else
		{
			JumpQueued = false;
			jumpReady = false;
			jumpCooldownRemaining = jumpCooldown;
			Networksprinting = false;
			animator.SetBool("MidAir", value: true);
			Velocity = new Vector3(Velocity.x / airFriction, Velocity.y - Gravity, Velocity.z / airFriction);
		}
		charController.Move(Velocity);
	}

	protected void CancelSlide()
	{
		isSliding = false;
		currentSlidePenalty = slidePenalty;
		slideRecovered = false;
		slidePenaltyCooldownRemaining = slidePenaltyCooldown;
	}

	protected virtual void FallLand(float velocity)
	{
		jumpStrengthMultiplier -= jumpStrengthRecovery_loss;
		fallRecovered = false;
		fallPenaltyCooldownRemaining = fallPenaltyCooldown;
		if (velocity < 0f - fallDamageThreshold && base.hasAuthority)
		{
			ReplicateDealDamage(fallDamageMultiplier * (0f - (velocity + fallDamageThreshold)), HumanName, base.transform.position, Vector3.zero);
		}
		if (Physics.Raycast(base.transform.position + new Vector3(0f, 1f, 0f), base.transform.TransformDirection(Vector3.down), out var hitInfo, 1.5f))
		{
			if (hitInfo.transform.tag == "Terrain")
			{
				int num = Random.Range(0, landGrassSounds.Length - 1);
				Object.Instantiate(landGrassSounds[num], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Stone")
			{
				int num2 = Random.Range(0, landConcreteSounds.Length - 1);
				Object.Instantiate(landConcreteSounds[num2], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Metal")
			{
				int num3 = Random.Range(0, landMetalSounds.Length - 1);
				Object.Instantiate(landMetalSounds[num3], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Wood")
			{
				int num4 = Random.Range(0, landWoodSounds.Length - 1);
				Object.Instantiate(landWoodSounds[num4], base.transform.position, base.transform.rotation);
			}
			else
			{
				int num5 = Random.Range(0, landConcreteSounds.Length - 1);
				Object.Instantiate(landConcreteSounds[num5], base.transform.position, base.transform.rotation);
			}
		}
	}

	public void ResetInputs()
	{
		ForwardInput = 0f;
		RightInput = 0f;
		Sprinting = false;
		AimingDownSights = false;
	}

	protected void Jump(float strength)
	{
		if (charController.isGrounded)
		{
			Velocity += new Vector3(0f, strength * jumpStrengthMultiplier, 0f);
		}
	}

	public void UpdateFixedAnimationValues()
	{
		if (!Sprinting)
		{
			sprintWeight += (1f - sprintWeight) * sprintTransition_kP;
			spine2Multiplier += (1f - spine2Multiplier) * sprintTransition_kP;
			neckMultiplier += (0f - neckMultiplier) * sprintTransition_kP;
		}
		else
		{
			sprintWeight += (0f - sprintWeight) * sprintTransition_kP;
			spine2Multiplier += (0f - spine2Multiplier) * sprintTransition_kP;
			neckMultiplier += (1f - neckMultiplier) * sprintTransition_kP;
		}
	}

	public void AddImpactForce()
	{
	}

	protected void UpdateImpactForce()
	{
		spine3ImpactTargVelocity = (Vector3.zero - spine3ImpactOffset) * impactAnimationRecover_kP;
		spine2ImpactTargVelocity = (Vector3.zero - spine2ImpactOffset) * impactAnimationRecover_kP;
		headImpactTargVelocity = (Vector3.zero - headImpactOffset) * impactAnimationRecover_kP;
		spine3ImpactVelocity = (spine3ImpactTargVelocity - spine3ImpactVelocity) * impactAnimationVelocity_kP;
		spine2ImpactVelocity = (spine2ImpactTargVelocity - spine2ImpactVelocity) * impactAnimationVelocity_kP;
		headImpactVelocity = (headImpactTargVelocity - headImpactVelocity) * impactAnimationVelocity_kP;
		spine3ImpactOffset += spine3ImpactVelocity;
		spine2ImpactOffset += spine2ImpactVelocity;
		headImpactOffset += headImpactVelocity;
		spine3ImpactOffset = Vector3.ClampMagnitude(spine3ImpactOffset, IMPACT_MAX_MAG);
		spine2ImpactOffset = Vector3.ClampMagnitude(spine2ImpactOffset, IMPACT_MAX_MAG);
		headImpactOffset = Vector3.ClampMagnitude(spine2ImpactOffset, IMPACT_MAX_MAG);
	}

	protected void UpdateAnimations()
	{
		if ((bool)leftFootTarg && (bool)rightFootTarg)
		{
			leftFootTarg.transform.position = new Vector3(leftFootTargBase.transform.position.x, leftFootTargBase.transform.position.y - pControlledLeftFootPos, leftFootTargBase.transform.position.z);
			rightFootTarg.transform.position = new Vector3(rightFootTargBase.transform.position.x, rightFootTargBase.transform.position.y - pControlledRightFootPos, rightFootTargBase.transform.position.z);
			if (!sprinting && UseThirdPersonAnimations && syncIsGrounded)
			{
				leftFootAngle.transform.eulerAngles += new Vector3(0f - pControlledLeftFootAngle - leftFootBaseAngle, 0f, 0f);
				rightFootAngle.transform.eulerAngles += new Vector3(0f - pControlledRightFootAngle - rightFootBaseAngle, 0f, 0f);
			}
		}
		if (!useThirdPersonAnimations)
		{
			if (base.hasAuthority)
			{
				Spine2.transform.Rotate(base.transform.right * ((0f - AimY) / 2f) * spine2Multiplier, Space.World);
				Spine3.transform.Rotate(base.transform.right * ((0f - AimY) / 2f), Space.World);
				Neck.transform.Rotate(base.transform.right * ((0f - AimY) / 2f) * neckMultiplier, Space.World);
			}
			else
			{
				Spine2.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) * spine2Multiplier + spine2ImpactOffset, Space.World);
				Spine3.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) + spine3ImpactOffset, Space.World);
				Neck.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) * neckMultiplier + headImpactOffset, Space.World);
			}
		}
		else
		{
			Spine2.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) * sprintWeight + spine2ImpactOffset, Space.World);
			Spine3.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) * sprintWeight + spine3ImpactOffset, Space.World);
		}
		if (base.hasAuthority)
		{
			if (Item is PlayerFirearm)
			{
				UpdateAimDownSightsState((Item as PlayerFirearm).AimedDownSights);
			}
			UpdateMoveStateAnimationsValues();
		}
		else if (Item is PlayerFirearm)
		{
			(Item as PlayerFirearm).AimedDownSights = aimingDownSights;
		}
		if (UseThirdPersonAnimations)
		{
			if (!(this is EnemyAI))
			{
				UpdatePivotRot();
			}
			else
			{
				Spine2.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f) * spine2Multiplier, Space.World);
				Spine3.transform.Rotate(base.transform.right * ((0f - syncedAimY) / 2f), Space.World);
			}
		}
		UpdateLeaning(leanState);
		UpdateUseAnimationState();
	}

	public void UpdatePivot()
	{
		if (!UseThirdPersonAnimations)
		{
			return;
		}
		pivot += (syncedAimX - lastAimX) % 360f;
		lastAimX = syncedAimX;
		if (walking || sprinting || resetPivotAngle)
		{
			targPivot = 0f;
			if (!sprinting)
			{
				if (forwardInput > 0f && rightInput > 0f)
				{
					targPivot = 0f - walkPivotAngle;
				}
				if (forwardInput > 0f && rightInput < 0f)
				{
					targPivot = walkPivotAngle;
				}
				if (forwardInput < 0f && rightInput > 0f)
				{
					targPivot = walkPivotAngle;
				}
				if (forwardInput < 0f && rightInput < 0f)
				{
					targPivot = 0f - walkPivotAngle;
				}
			}
			else
			{
				if (forwardInput > 0f && rightInput > 0f)
				{
					targPivot = 0f - sprintPivotAngle;
				}
				if (forwardInput > 0f && rightInput < 0f)
				{
					targPivot = sprintPivotAngle;
				}
				if (forwardInput < 0f && rightInput > 0f)
				{
					targPivot = sprintPivotAngle;
				}
				if (forwardInput < 0f && rightInput < 0f)
				{
					targPivot = 0f - sprintPivotAngle;
				}
			}
			pivot = Mathf.LerpAngle(pivot, targPivot, pivot_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant) % 360f;
			if (Mathf.Abs(pivot) < pivotMinimumForReset)
			{
				resetPivotAngle = false;
			}
		}
		if (Mathf.Abs(pivot) > maxPivotAngle && !walking)
		{
			resetPivotAngle = true;
		}
	}

	public void UpdatePivotRot()
	{
		spine1.transform.localEulerAngles += new Vector3(0f, pivot, 0f);
		spine1.transform.eulerAngles = new Vector3(Mathf.LerpAngle(0f, spine1.transform.eulerAngles.x, pivotWeight), spine1.transform.eulerAngles.y, Mathf.LerpAngle(0f, spine1.transform.eulerAngles.z, pivotWeight));
		if (!sprinting)
		{
			pivotWeight += (0f - pivotWeight) * pivotTransition_kP;
		}
		else
		{
			pivotWeight += (1f - pivotWeight) * pivotTransition_kP;
		}
		if (!base.hasAuthority || this is EnemyAI)
		{
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, syncedAimX - pivot, base.transform.eulerAngles.z);
		}
	}

	public void UpdateAnimatorValues()
	{
		animator.SetBool("Sliding", isSliding);
		if (OverrideMoveAnimationState != 0 && !sprinting && !crouch && moveAnimationState == 0)
		{
			Animator.SetInteger("MoveState", OverrideMoveAnimationState);
		}
		else
		{
			Animator.SetInteger("MoveState", moveAnimationState);
		}
		Animator.SetInteger("ArmState", armAnimationState);
		if (item is PlayerFirearm && (item as PlayerFirearm).UseRepetitiveReload)
		{
			Animator.SetBool("LeaveRepetitiveReload", LeaveRepetitiveReload);
		}
		if (sneaking)
		{
			animator.SetFloat("MovementAnimationSpeed", sneakSpeedMultiplier);
		}
		else
		{
			animator.SetFloat("MovementAnimationSpeed", 1f);
		}
		if (useThirdPersonAnimations)
		{
			animator.SetLayerWeight(animator.GetLayerIndex("Arms 3rdPerson"), 1f);
			animator.SetLayerWeight(animator.GetLayerIndex("Arms"), 0f);
		}
		else
		{
			animator.SetLayerWeight(animator.GetLayerIndex("Arms 3rdPerson"), 0f);
			animator.SetLayerWeight(animator.GetLayerIndex("Arms"), 1f);
		}
	}

	public void UpdateFeetIK()
	{
		if (syncIsGrounded)
		{
			Vector3 vector = new Vector3(rightFootTarg.transform.position.x, base.transform.position.y + feetIKCastHeight, rightFootTarg.transform.position.z);
			Vector3 origin = vector + base.transform.forward * toeIK_dist;
			if (Physics.SphereCast(vector, feetCastRadius, Vector3.down, out var hitInfo, 1.5f, hitboxLayer))
			{
				rightFootCastDistance = Mathf.Clamp(hitInfo.distance - feetIKOffset + feetCastRadius, 0f - feetIK_max, feetIK_max);
			}
			if (Physics.SphereCast(origin, feetCastRadius, Vector3.down, out var hitInfo2, 1.5f, hitboxLayer))
			{
				rightToeCastDistance = hitInfo2.distance - feetIKOffset + feetCastRadius;
			}
			else
			{
				rightToeCastDistance = rightFootCastDistance;
			}
			Vector3 vector2 = new Vector3(leftFootTarg.transform.position.x, base.transform.position.y + feetIKCastHeight, leftFootTarg.transform.position.z);
			Vector3 origin2 = vector2 + base.transform.forward * toeIK_dist;
			if (Physics.SphereCast(vector2, feetCastRadius, Vector3.down, out var hitInfo3, 1.5f, hitboxLayer))
			{
				leftFootCastDistance = Mathf.Clamp(hitInfo3.distance - feetIKOffset + feetCastRadius, 0f - feetIK_max, feetIK_max);
			}
			if (Physics.SphereCast(origin2, feetCastRadius, Vector3.down, out var hitInfo4, 1.5f, hitboxLayer))
			{
				leftToeCastDistance = hitInfo4.distance - feetIKOffset + feetCastRadius;
			}
			else
			{
				leftToeCastDistance = leftFootCastDistance;
			}
		}
	}

	public void UpdatePControlIK()
	{
		float num = Mathf.Clamp(leftFootCastDistance - leftToeCastDistance, 0f - slopeDistanceClamp, slopeDistanceClamp);
		float num2 = Mathf.Clamp(rightFootCastDistance - rightToeCastDistance, 0f - slopeDistanceClamp, slopeDistanceClamp);
		float b = Mathf.Atan2(num * slopeAngleMultiplier, toeIK_dist) * 57.29578f;
		float b2 = Mathf.Atan2(num2 * slopeAngleMultiplier, toeIK_dist) * 57.29578f;
		pControlledLeftFootPos += (leftFootCastDistance - pControlledLeftFootPos) * feetIK_kP;
		pControlledRightFootPos += (rightFootCastDistance - pControlledRightFootPos) * feetIK_kP;
		pControlledLeftFootAngle = Mathf.LerpAngle(pControlledLeftFootAngle, b, feetIK_kP);
		pControlledRightFootAngle = Mathf.LerpAngle(pControlledRightFootAngle, b2, feetIK_kP);
	}

	public void UpdateCharacterControllerDimensions()
	{
		if (!crouch && !isSliding)
		{
			charControllerTargHeight += (movementColliderStandHeight - charControllerTargHeight) * movementColliderSize_kP;
			charControllerTargPosY += (movementColliderStandPosY - charControllerTargPosY) * movementColliderSize_kP;
		}
		else
		{
			charControllerTargHeight += (movementColliderCrouchHeight - charControllerTargHeight) * movementColliderSize_kP;
			charControllerTargPosY += (movementColliderCrouchPosY - charControllerTargPosY) * movementColliderSize_kP;
		}
		if (UseThirdPersonAnimations)
		{
			float num = Mathf.Abs(pControlledRightFootPos - pControlledLeftFootPos);
			charController.height = charControllerTargHeight - num / 2f;
			charController.center = new Vector3(0f, charControllerTargPosY + num / 4f);
		}
		else
		{
			charController.height = charControllerTargHeight;
			charController.center = new Vector3(0f, charControllerTargPosY);
		}
	}

	private void UpdateLeaning(float leanState)
	{
		float num = 0f;
		if (leanState == 1f)
		{
			num = LEAN_ANGLE;
		}
		else if (leanState == 2f)
		{
			num = 0f - LEAN_ANGLE;
		}
		Lean += (num - Lean) * lean_kP * (Time.deltaTime * 60f);
		if ((bool)leanLimb)
		{
			leanLimb.transform.localEulerAngles = new Vector3(leanLimb.transform.localEulerAngles.x, leanLimb.transform.localEulerAngles.y, Lean + leanLimb.transform.localEulerAngles.z);
		}
	}

	protected void UpdateMoveStateAnimationsValues()
	{
		if (ForwardInput != 0f)
		{
			if (Sprinting)
			{
				NetworkmoveAnimationState = 4;
			}
			else if (isSliding)
			{
				NetworkmoveAnimationState = 6;
			}
			else if (!crouch)
			{
				NetworkmoveAnimationState = 1;
			}
			else
			{
				NetworkmoveAnimationState = 6;
			}
		}
		else if (RightInput != 0f)
		{
			if (!crouch)
			{
				if (RightInput > 0f)
				{
					NetworkmoveAnimationState = 2;
				}
				else
				{
					NetworkmoveAnimationState = 3;
				}
			}
			else if (RightInput > 0f)
			{
				NetworkmoveAnimationState = 7;
			}
			else
			{
				NetworkmoveAnimationState = 8;
			}
		}
		else if (!crouch)
		{
			NetworkmoveAnimationState = 0;
		}
		else
		{
			NetworkmoveAnimationState = 5;
		}
	}

	protected void UpdateAimDownSightsState(bool aimedDownSights)
	{
		aimingDownSights = aimedDownSights;
		if (aimedDownSights)
		{
			NetworkarmAnimationState = 1;
		}
		else
		{
			NetworkarmAnimationState = 0;
		}
	}

	protected void UpdateUseAnimationState()
	{
		if (item.UseAnimationTrigger)
		{
			Animator.SetTrigger("Use");
			item.UseAnimationTrigger = false;
		}
	}

	protected virtual void Death()
	{
		callDeathFlag = true;
		NetworkServer.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
	}

	protected void PrepareRagdoll()
	{
		if ((bool)preparedRagdoll)
		{
			Object.Destroy(preparedRagdoll.gameObject);
		}
		preparedRagdoll = Object.Instantiate(Ragdoll, base.transform.position, base.transform.rotation);
		preparedRagdoll.GetComponent<Ragdoll>().SetEquipment(shirt, pants, headwear, vest, hood);
	}

	protected void SpawnRagdoll(Transform baseTransform)
	{
		PrepareRagdoll();
		int num = 0;
		GameObject[] ragdollPartsGet = preparedRagdoll.GetComponent<Ragdoll>().RagdollPartsGet;
		foreach (GameObject obj in ragdollPartsGet)
		{
			obj.transform.position = ragdollParts[num].transform.position;
			obj.transform.eulerAngles = ragdollParts[num].transform.eulerAngles;
			num++;
		}
		if ((bool)charController)
		{
			preparedRagdoll.GetComponent<Ragdoll>().ApplyVelocityEven(recordVelocity);
		}
		if (base.hasAuthority)
		{
			preparedRagdoll.GetComponent<Ragdoll>().SetCameraState(enabled: true);
		}
		if ((bool)GameObject.Find("Temp"))
		{
			preparedRagdoll.transform.parent = GameObject.Find("Temp").transform;
		}
	}

	protected void LoadItem(GameObject item, bool localPlayer)
	{
		if (item.GetComponent<Item>().ItemName != oldItem)
		{
			if ((bool)weaponParent.transform.GetChild(1))
			{
				Object.Destroy(weaponParent.transform.GetChild(1).gameObject);
			}
			GameObject gameObject = Object.Instantiate(item, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = weaponParent.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = gameObject.transform.lossyScale;
			this.item = gameObject.GetComponent<PlayerItem>();
			gameObject.GetComponent<PlayerItem>().User = this;
			Animator = GetComponent<Animator>();
			Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
			Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;
			UpdateAnimatorValues();
			if (localPlayer && (bool)gameObject.GetComponent<PlayerFirearm>())
			{
				gameObject.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
			}
		}
		oldItem = item.GetComponent<Item>().ItemName;
	}

	protected void LoadItemFromPickup(GameObject item, bool localPlayer)
	{
		if (item.GetComponent<Item>().ItemName != oldItem)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Weapon_" + item.GetComponent<PickupItem>().ItemName) as GameObject, base.transform.position, base.transform.rotation);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = gameObject.transform.lossyScale;
			gameObject.GetComponent<PlayerItem>().User = this;
			if (gameObject.GetComponent<PlayerItem>() is PlayerFirearm)
			{
				gameObject.GetComponent<PlayerFirearm>().SetSightAttachment(item.GetComponent<PickupItem>().SightAttachment);
				gameObject.GetComponent<PlayerFirearm>().SetBarrelAttachment(item.GetComponent<PickupItem>().BarrelAttachment);
				gameObject.GetComponent<PlayerFirearm>().SetGripAttachment(item.GetComponent<PickupItem>().GripAttachment);
			}
			if (localPlayer && (bool)gameObject.GetComponent<PlayerFirearm>())
			{
				gameObject.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
				gameObject.GetComponent<PlayerFirearm>().Chambered = true;
			}
			HumanInventory.AssignNewItem(gameObject.GetComponent<PlayerItem>(), fullAmmo: true);
			LoadItem(gameObject.GetComponent<PlayerItem>(), base.hasAuthority, init: true);
			Object.Destroy(gameObject);
		}
		else
		{
			if (this.item is PlayerFirearm)
			{
				(this.item as PlayerFirearm).SetSightAttachment(item.GetComponent<PickupItem>().SightAttachment);
				(this.item as PlayerFirearm).SetBarrelAttachment(item.GetComponent<PickupItem>().BarrelAttachment);
				(this.item as PlayerFirearm).SetGripAttachment(item.GetComponent<PickupItem>().GripAttachment);
				(this.item as PlayerFirearm).SetActiveAttachments();
				(this.item as PlayerFirearm).SetAsLocalPlayer();
			}
			HumanInventory.AssignNewItem(this.item.GetComponent<PlayerItem>(), base.hasAuthority);
			LoadItem(this.item.GetComponent<PlayerItem>(), base.hasAuthority, init: true);
		}
		oldItem = item.GetComponent<Item>().ItemName;
	}

	public void LoadItem(string item, bool localPlayer)
	{
		if (item != oldItem)
		{
			if ((bool)Item)
			{
				Object.Destroy(Item.gameObject);
			}
			if (item == "" || item == null)
			{
				item = "Unarmed";
			}
			GameObject gameObject = Object.Instantiate(Resources.Load("Weapon_" + item) as GameObject, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = weaponParent.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = gameObject.transform.lossyScale;
			if (gameObject.GetComponent<PlayerItem>() is PlayerFirearm)
			{
				(gameObject.GetComponent<PlayerItem>() as PlayerFirearm).SetSightAttachment(0);
				(gameObject.GetComponent<PlayerItem>() as PlayerFirearm).SetBarrelAttachment(0);
				(gameObject.GetComponent<PlayerItem>() as PlayerFirearm).SetGripAttachment(0);
			}
			Item = gameObject.GetComponent<PlayerItem>();
			gameObject.GetComponent<PlayerItem>().User = this;
			Animator = GetComponent<Animator>();
			Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
			Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;
			UpdateAnimatorValues();
			if (localPlayer && (bool)gameObject.GetComponent<PlayerFirearm>())
			{
				gameObject.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
			}
		}
		oldItem = item;
	}

	public void LoadItem(PlayerItem item, bool localPlayer, bool init)
	{
		if (item.ItemName != oldItem && Animator != null)
		{
			Object.Destroy(Item.gameObject);
			GameObject gameObject = Object.Instantiate(Resources.Load("Weapon_" + item.ItemName) as GameObject, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = weaponParent.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			if (localPlayer && (bool)gameObject.GetComponent<PlayerFirearm>())
			{
				gameObject.GetComponent<PlayerFirearm>().ReserveAmmo = (item as PlayerFirearm).ReserveAmmo;
				gameObject.GetComponent<PlayerFirearm>().Ammo = (item as PlayerFirearm).Ammo;
				gameObject.GetComponent<PlayerFirearm>().SetSightAttachment((item as PlayerFirearm).SightAttachment);
				gameObject.GetComponent<PlayerFirearm>().SetBarrelAttachment((item as PlayerFirearm).BarrelAttachment);
				gameObject.GetComponent<PlayerFirearm>().SetGripAttachment((item as PlayerFirearm).GripAttachment);
				gameObject.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
			}
			gameObject.transform.localScale = gameObject.transform.lossyScale;
			gameObject.GetComponent<PlayerItem>().User = this;
			if ((bool)gameObject.GetComponent<PlayerFirearm>())
			{
				gameObject.GetComponent<PlayerFirearm>().Chambered = (item as PlayerFirearm).Chambered;
			}
			Item = gameObject.GetComponent<PlayerItem>();
			Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
			Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;
			Animator.SetBool("Init", init);
			item.DrawSound(init);
			UpdateAnimatorValues();
		}
		aimingDownSights = false;
		oldItem = item.ItemName;
	}

	public void SelectFromInventory(int selection, bool equipForNewItem, bool init)
	{
		if (!(item is Consumable) || !(item as Consumable).UsingItem)
		{
			if (!equipForNewItem)
			{
				humanInventory.AssignNewItem(item, fullAmmo: false);
			}
			switch (selection)
			{
			case 1:
				LoadItem(HumanInventory.PrimaryItem, base.hasAuthority, init);
				break;
			case 2:
				LoadItem(HumanInventory.SecondaryItem, base.hasAuthority, init);
				break;
			case 3:
				LoadItem(HumanInventory.Equipment1, base.hasAuthority, init);
				break;
			case 4:
				LoadItem(HumanInventory.Equipment2, base.hasAuthority, init);
				break;
			case 5:
				LoadItem(HumanInventory.MeleeWeapon, base.hasAuthority, init);
				break;
			}
			HumanInventory.CurrentlySelected = selection;
			item.DrawSound(init);
		}
	}

	public void DamageAnotherPlayer(Human target, float damage, Vector3 hitPos, Vector3 hitRot)
	{
		if (networkManager is TestRangeNetworkManager)
		{
			(networkManager as TestRangeNetworkManager).DamageAnotherPlayer(target, damage, humanName, hitPos, hitRot);
		}
		else if ((object)networkManager != null)
		{
			networkManager.DamageAnotherPlayer(target, damage, humanName, hitPos, hitRot);
		}
		else
		{
			Debug.LogError("No Network Manager found");
		}
	}

	public void AppendToPlayersDamaged(Human target)
	{
		if (!PlayersDamaged.Contains(target))
		{
			PlayersDamaged.Add(target);
		}
	}

	public virtual void UpdateBillboard()
	{
		if (team == localPlayer.team && localPlayer != this && health > 0f)
		{
			playerNameTag.SetNameTagVisibility(visible: true);
			playerNameTag.SetNameTag(HumanName);
		}
		else
		{
			playerNameTag.SetNameTagVisibility(visible: false);
		}
		if ((bool)SpectatorCamera)
		{
			Human human2 = (SpectatorCamera.CurrentlySpectating = this);
			if ((bool)human2)
			{
				playerNameTag.SetNameTagVisibility(visible: false);
			}
		}
	}

	public void CheckFootsteps()
	{
		if (!active || !animator || sneaking)
		{
			return;
		}
		if (animator.GetInteger("MoveState") == 4 && stepSound)
		{
			if (!Physics.Raycast(base.transform.position + new Vector3(0f, 1f, 0f), base.transform.TransformDirection(Vector3.down), out var hitInfo, 1.5f))
			{
				return;
			}
			stepSound = false;
			if (hitInfo.transform.tag == "Terrain")
			{
				int num = Random.Range(0, FootstepGrassSprintSounds.Length - 1);
				if (num == lastStepSound)
				{
					num++;
					if (num > FootstepGrassSprintSounds.Length)
					{
						num = 0;
					}
				}
				lastStepSound = num;
				Object.Instantiate(FootstepGrassSprintSounds[num], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Stone")
			{
				int num2 = Random.Range(0, FootstepConcreteSprintSounds.Length - 1);
				if (num2 == lastStepSound)
				{
					num2++;
					if (num2 > FootstepConcreteSprintSounds.Length)
					{
						num2 = 0;
					}
				}
				lastStepSound = num2;
				Object.Instantiate(FootstepConcreteSprintSounds[num2], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Metal")
			{
				int num3 = Random.Range(0, FootstepMetalSprintSounds.Length - 1);
				if (num3 == lastStepSound)
				{
					num3++;
					if (num3 > FootstepMetalSprintSounds.Length)
					{
						num3 = 0;
					}
				}
				lastStepSound = num3;
				Object.Instantiate(FootstepMetalSprintSounds[num3], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo.transform.tag == "Wood")
			{
				int num4 = Random.Range(0, FootstepWoodSprintSounds.Length - 1);
				if (num4 == lastStepSound)
				{
					num4++;
					if (num4 > FootstepWoodSprintSounds.Length)
					{
						num4 = 0;
					}
				}
				lastStepSound = num4;
				Object.Instantiate(FootstepWoodSprintSounds[num4], base.transform.position, base.transform.rotation);
			}
			else
			{
				int num5 = Random.Range(0, FootstepConcreteSprintSounds.Length - 1);
				if (num5 == lastStepSound)
				{
					num5++;
					if (num5 > FootstepConcreteSprintSounds.Length)
					{
						num5 = 0;
					}
				}
				lastStepSound = num5;
				Object.Instantiate(FootstepConcreteSprintSounds[num5], base.transform.position, base.transform.rotation);
			}
			Invoke("StepSoundCoolDown", stepCoolTime / 1.3f);
		}
		else
		{
			if ((animator.GetInteger("MoveState") != 1 && moveAnimationState != 2 && moveAnimationState != 3 && animator.GetInteger("MoveState") != 6 && animator.GetInteger("MoveState") != 7 && animator.GetInteger("MoveState") != 8) || !stepSound || !Physics.Raycast(base.transform.position + new Vector3(0f, 1f, 0f), base.transform.TransformDirection(Vector3.down), out var hitInfo2, 1.5f))
			{
				return;
			}
			stepSound = false;
			if (hitInfo2.transform.tag == "Terrain")
			{
				int num6 = Random.Range(0, FootstepGrassSounds.Length - 1);
				if (num6 == lastStepSound)
				{
					num6++;
					if (num6 > FootstepGrassSounds.Length)
					{
						num6 = 0;
					}
				}
				lastStepSound = num6;
				Object.Instantiate(FootstepGrassSounds[num6], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo2.transform.tag == "Stone")
			{
				int num7 = Random.Range(0, FootstepConcreteSounds.Length - 1);
				if (num7 == lastStepSound)
				{
					num7++;
					if (num7 > FootstepConcreteSounds.Length)
					{
						num7 = 0;
					}
				}
				lastStepSound = num7;
				Object.Instantiate(FootstepConcreteSounds[num7], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo2.transform.tag == "Metal")
			{
				int num8 = Random.Range(0, FootstepMetalSounds.Length - 1);
				if (num8 == lastStepSound)
				{
					num8++;
					if (num8 > FootstepMetalSounds.Length)
					{
						num8 = 0;
					}
				}
				lastStepSound = num8;
				Object.Instantiate(FootstepMetalSounds[num8], base.transform.position, base.transform.rotation);
			}
			else if (hitInfo2.transform.tag == "Wood")
			{
				int num9 = Random.Range(0, FootstepWoodSounds.Length - 1);
				if (num9 == lastStepSound)
				{
					num9++;
					if (num9 > FootstepWoodSounds.Length)
					{
						num9 = 0;
					}
				}
				lastStepSound = num9;
				Object.Instantiate(FootstepWoodSounds[num9], base.transform.position, base.transform.rotation);
			}
			else
			{
				int num10 = Random.Range(0, FootstepConcreteSounds.Length - 1);
				if (num10 == lastStepSound)
				{
					num10++;
					if (num10 > FootstepConcreteSounds.Length)
					{
						num10 = 0;
					}
				}
				lastStepSound = num10;
				Object.Instantiate(FootstepConcreteSounds[num10], base.transform.position, base.transform.rotation);
			}
			Invoke("StepSoundCoolDown", stepCoolTime);
		}
	}

	public void StepSoundCoolDown()
	{
		stepSound = true;
	}

	public virtual void Suppress(float intensity)
	{
	}

	public void FleshImpact()
	{
		if (fleshImpactSounds.Length >= 1 && !fleshImpactCool)
		{
			fleshImpactCool = true;
			Invoke("FleshCool", fleshImpactCooldown);
			Object.Instantiate(fleshImpactSounds[Random.Range(0, fleshImpactSounds.Length)], base.transform.position, base.transform.rotation);
		}
	}

	public void CheckHeadClearance()
	{
		if (Physics.SphereCast(base.transform.position + headClearanceCheckPos, 0.2f, Vector3.up, out var _, headClearanceCheckRange, hitboxLayer))
		{
			crouchObstructed = true;
		}
		else
		{
			crouchObstructed = false;
		}
	}

	public void FleshCool()
	{
		fleshImpactCool = false;
	}

	public void RefreshJump()
	{
		jumpReady = true;
	}

	public void UpdateMovementPenalties()
	{
		if (jumpCooldownRemaining <= 0f)
		{
			jumpReady = true;
		}
		else
		{
			jumpCooldownRemaining -= Time.deltaTime;
			if (jumpCooldownRemaining - Time.deltaTime < jumpCooldownRemaining / jumpRound_coeff)
			{
				jumpCooldownRemaining = 0f;
			}
		}
		if (fallPenaltyCooldownRemaining <= 0f)
		{
			fallRecovered = true;
		}
		else
		{
			fallPenaltyCooldownRemaining -= Time.deltaTime;
			if (fallPenaltyCooldownRemaining - Time.deltaTime < fallPenaltyCooldownRemaining / jumpRound_coeff)
			{
				fallPenaltyCooldownRemaining = 0f;
			}
		}
		if (slidePenaltyCooldownRemaining <= 0f)
		{
			slideRecovered = true;
		}
		else
		{
			slidePenaltyCooldownRemaining -= Time.deltaTime;
			if (slidePenaltyCooldownRemaining - Time.deltaTime < slidePenaltyCooldownRemaining / jumpRound_coeff)
			{
				slidePenaltyCooldownRemaining = 0f;
			}
		}
		if (currentSlideAddVelocity >= slideAddBaseVelocity)
		{
			currentSlideAddVelocity = slideAddBaseVelocity;
		}
		else
		{
			currentSlideAddVelocity += slideBoostRecovery * Time.deltaTime;
		}
	}

	protected void CheckSlide()
	{
		if (velocity.magnitude >= slideInitMinVelocity && !isSliding && charController.isGrounded && currentSlideAddVelocity >= slideBoostMinimum)
		{
			isSliding = true;
			crouch = true;
			Velocity += spine1.transform.forward * ForwardInput * currentSlideAddVelocity + spine1.transform.right * RightInput * currentSlideAddVelocity;
			int num = Random.Range(0, slideSounds.Length - 1);
			Object.Instantiate(slideSounds[num], base.transform.position, base.transform.rotation);
		}
	}

	protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "KillBox" && base.hasAuthority)
		{
			ReplicateDealDamage(killboxDamage, HumanName, base.transform.position, base.transform.eulerAngles);
		}
	}

	static Human()
	{
		score_kill = 100;
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdUpdateLeanState", InvokeUserCode_CmdUpdateLeanState, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdDealDamage", InvokeUserCode_CmdDealDamage, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetAimY", InvokeUserCode_CmdSetAimY, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetAimX", InvokeUserCode_CmdSetAimX, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetForwardInput", InvokeUserCode_CmdSetForwardInput, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetRightInput", InvokeUserCode_CmdSetRightInput, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdUpdateAnimationStates", InvokeUserCode_CmdUpdateAnimationStates, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetItemString", InvokeUserCode_CmdSetItemString, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdRunReloadAnimation", InvokeUserCode_CmdRunReloadAnimation, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetLeaveRepetitiveReload", InvokeUserCode_CmdSetLeaveRepetitiveReload, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdPrimaryUseItem", InvokeUserCode_CmdPrimaryUseItem, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetSprinting", InvokeUserCode_CmdSetSprinting, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetSneaking", InvokeUserCode_CmdSetSneaking, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetSliding", InvokeUserCode_CmdSetSliding, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetTeam", InvokeUserCode_CmdSetTeam, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetHumanName", InvokeUserCode_CmdSetHumanName, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetIsGrounded", InvokeUserCode_CmdSetIsGrounded, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetScore", InvokeUserCode_CmdSetScore, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetKills", InvokeUserCode_CmdSetKills, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetAssists", InvokeUserCode_CmdSetAssists, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSetDeaths", InvokeUserCode_CmdSetDeaths, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Human), "CmdSyncAttachments", InvokeUserCode_CmdSyncAttachments, requiresAuthority: true);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcDealDamage", InvokeUserCode_RpcDealDamage);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetAimY", InvokeUserCode_RpcSetAimY);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetForwardInput", InvokeUserCode_RpcSetForwardInput);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetRightInput", InvokeUserCode_RpcSetRightInput);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetAimX", InvokeUserCode_RpcSetAimX);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcUpdateAnimationStates", InvokeUserCode_RpcUpdateAnimationStates);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetItemString", InvokeUserCode_RpcSetItemString);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcRunReloadAnimation", InvokeUserCode_RpcRunReloadAnimation);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcPrimaryUseItem", InvokeUserCode_RpcPrimaryUseItem);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetSprinting", InvokeUserCode_RpcSetSprinting);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetSneaking", InvokeUserCode_RpcSetSneaking);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetSliding", InvokeUserCode_RpcSetSliding);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetTeam", InvokeUserCode_RpcSetTeam);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetHumanName", InvokeUserCode_RpcSetHumanName);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetIsGrounded", InvokeUserCode_RpcSetIsGrounded);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetScore", InvokeUserCode_RpcSetScore);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetKills", InvokeUserCode_RpcSetKills);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetAssists", InvokeUserCode_RpcSetAssists);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSetDeaths", InvokeUserCode_RpcSetDeaths);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Human), "RpcSyncAttachments", InvokeUserCode_RpcSyncAttachments);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdUpdateLeanState(int value)
	{
		SetLeanState(value);
	}

	protected static void InvokeUserCode_CmdUpdateLeanState(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUpdateLeanState called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdUpdateLeanState(reader.ReadInt());
		}
	}

	protected void UserCode_CmdDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		if (base.isServer)
		{
			ReduceHealth(value, causer, hitPos, hitRot);
			RpcDealDamage(value, causer, hitPos, hitRot);
		}
	}

	protected static void InvokeUserCode_CmdDealDamage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDealDamage called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdDealDamage(reader.ReadFloat(), reader.ReadString(), reader.ReadVector3(), reader.ReadVector3());
		}
	}

	protected void UserCode_RpcDealDamage(float value, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		if (!base.isServer)
		{
			ReduceHealth(value, causer, hitPos, hitRot);
		}
	}

	protected static void InvokeUserCode_RpcDealDamage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDealDamage called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcDealDamage(reader.ReadFloat(), reader.ReadString(), reader.ReadVector3(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdSetAimY(float aimY)
	{
		if (base.isServer && !base.hasAuthority)
		{
			SetAimY(aimY);
			RpcSetAimY(aimY);
		}
	}

	protected static void InvokeUserCode_CmdSetAimY(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetAimY called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetAimY(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetAimY(float aimY)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			SetAimY(aimY);
		}
	}

	protected static void InvokeUserCode_RpcSetAimY(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetAimY called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetAimY(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdSetAimX(float aimX)
	{
		if (base.isServer && !base.hasAuthority)
		{
			SetAimX(aimX);
			RpcSetAimX(aimX);
		}
	}

	protected static void InvokeUserCode_CmdSetAimX(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetAimX called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetAimX(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetForwardInput(float forward)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			ForwardInput = forward;
		}
	}

	protected static void InvokeUserCode_RpcSetForwardInput(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetForwardInput called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetForwardInput(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdSetForwardInput(float forward)
	{
		if (base.isServer && !base.hasAuthority)
		{
			ForwardInput = forward;
			RpcSetForwardInput(forward);
		}
	}

	protected static void InvokeUserCode_CmdSetForwardInput(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetForwardInput called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetForwardInput(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetRightInput(float right)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			RightInput = right;
		}
	}

	protected static void InvokeUserCode_RpcSetRightInput(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetRightInput called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetRightInput(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdSetRightInput(float right)
	{
		if (base.isServer && !base.hasAuthority)
		{
			RightInput = right;
			RpcSetRightInput(right);
		}
	}

	protected static void InvokeUserCode_CmdSetRightInput(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetRightInput called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetRightInput(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetAimX(float aimX)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			SetAimX(aimX);
		}
	}

	protected static void InvokeUserCode_RpcSetAimX(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetAimX called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetAimX(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdUpdateAnimationStates(int movement, int arms, int overrideMovement)
	{
		if (base.isServer)
		{
			UpdateAnimationStates(movement, arms, overrideMovement);
			RpcUpdateAnimationStates(movement, arms, overrideMovement);
		}
	}

	protected static void InvokeUserCode_CmdUpdateAnimationStates(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUpdateAnimationStates called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdUpdateAnimationStates(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
		}
	}

	protected void UserCode_RpcUpdateAnimationStates(int movement, int arms, int overrideMovement)
	{
		if (!base.isServer)
		{
			UpdateAnimationStates(movement, arms, overrideMovement);
		}
	}

	protected static void InvokeUserCode_RpcUpdateAnimationStates(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUpdateAnimationStates called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcUpdateAnimationStates(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
		}
	}

	protected void UserCode_CmdSetItemString(string item)
	{
		if (base.isServer && !base.hasAuthority)
		{
			SetItemString(item);
			RpcSetItemString(item);
		}
	}

	protected static void InvokeUserCode_CmdSetItemString(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetItemString called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetItemString(reader.ReadString());
		}
	}

	protected void UserCode_RpcSetItemString(string item)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			SetItemString(item);
		}
	}

	protected static void InvokeUserCode_RpcSetItemString(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetItemString called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetItemString(reader.ReadString());
		}
	}

	protected void UserCode_CmdRunReloadAnimation()
	{
		if (base.isServer && !base.hasAuthority)
		{
			RunReloadAnimation();
			RpcRunReloadAnimation();
		}
	}

	protected static void InvokeUserCode_CmdRunReloadAnimation(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRunReloadAnimation called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdRunReloadAnimation();
		}
	}

	protected void UserCode_RpcRunReloadAnimation()
	{
		if (!base.isServer && !base.hasAuthority)
		{
			RunReloadAnimation();
		}
	}

	protected static void InvokeUserCode_RpcRunReloadAnimation(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcRunReloadAnimation called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcRunReloadAnimation();
		}
	}

	protected void UserCode_CmdSetLeaveRepetitiveReload(bool enabled)
	{
		SetLeaveRepetitiveReload(enabled);
	}

	protected static void InvokeUserCode_CmdSetLeaveRepetitiveReload(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetLeaveRepetitiveReload called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetLeaveRepetitiveReload(reader.ReadBool());
		}
	}

	protected void UserCode_CmdPrimaryUseItem()
	{
		if (!(health <= 0f) && base.isServer)
		{
			PrimaryUseItem();
		}
	}

	protected static void InvokeUserCode_CmdPrimaryUseItem(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPrimaryUseItem called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdPrimaryUseItem();
		}
	}

	protected void UserCode_RpcPrimaryUseItem()
	{
		if (!(health <= 0f) && !base.isServer)
		{
			PrimaryUseItem();
		}
	}

	protected static void InvokeUserCode_RpcPrimaryUseItem(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPrimaryUseItem called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcPrimaryUseItem();
		}
	}

	protected void UserCode_CmdSetSprinting(bool sprinting)
	{
		if (base.isServer && !base.hasAuthority)
		{
			Networksprinting = sprinting;
		}
	}

	protected static void InvokeUserCode_CmdSetSprinting(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetSprinting called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetSprinting(reader.ReadBool());
		}
	}

	protected void UserCode_RpcSetSprinting(bool sprinting)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			Networksprinting = sprinting;
		}
	}

	protected static void InvokeUserCode_RpcSetSprinting(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetSprinting called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetSprinting(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSetSneaking(bool sneaking)
	{
		if (base.isServer && !base.hasAuthority)
		{
			Networksneaking = sneaking;
		}
	}

	protected static void InvokeUserCode_CmdSetSneaking(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetSneaking called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetSneaking(reader.ReadBool());
		}
	}

	protected void UserCode_RpcSetSneaking(bool sneaking)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			Networksneaking = sneaking;
		}
	}

	protected static void InvokeUserCode_RpcSetSneaking(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetSneaking called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetSneaking(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSetSliding(bool sliding)
	{
		if (base.isServer && !base.hasAuthority)
		{
			isSliding = sliding;
		}
	}

	protected static void InvokeUserCode_CmdSetSliding(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetSliding called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetSliding(reader.ReadBool());
		}
	}

	protected void UserCode_RpcSetSliding(bool sliding)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			isSliding = sliding;
		}
	}

	protected static void InvokeUserCode_RpcSetSliding(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetSliding called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetSliding(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSetTeam(int team)
	{
		if (base.isServer)
		{
			SetTeam(team);
		}
	}

	protected static void InvokeUserCode_CmdSetTeam(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetTeam called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetTeam(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetTeam(int team)
	{
		if (!base.isServer)
		{
			SetTeam(team);
		}
	}

	protected static void InvokeUserCode_RpcSetTeam(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetTeam called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetTeam(reader.ReadInt());
		}
	}

	protected void UserCode_CmdSetHumanName(string humanName)
	{
		if (base.isServer)
		{
			SetHumanName(humanName);
		}
	}

	protected static void InvokeUserCode_CmdSetHumanName(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetHumanName called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetHumanName(reader.ReadString());
		}
	}

	protected void UserCode_RpcSetHumanName(string humanName)
	{
		if (!base.isServer)
		{
			SetHumanName(humanName);
		}
	}

	protected static void InvokeUserCode_RpcSetHumanName(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetHumanName called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetHumanName(reader.ReadString());
		}
	}

	protected void UserCode_CmdSetIsGrounded(bool isGrounded)
	{
		if (base.isServer)
		{
			SetIsGrounded(isGrounded);
		}
	}

	protected static void InvokeUserCode_CmdSetIsGrounded(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetIsGrounded called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetIsGrounded(reader.ReadBool());
		}
	}

	protected void UserCode_RpcSetIsGrounded(bool isGrounded)
	{
		if (!base.isServer)
		{
			SetIsGrounded(isGrounded);
		}
	}

	protected static void InvokeUserCode_RpcSetIsGrounded(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetIsGrounded called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetIsGrounded(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSetScore(int score)
	{
		if (base.isServer && !base.hasAuthority)
		{
			this.score = score;
			RpcSetScore(score);
		}
	}

	protected static void InvokeUserCode_CmdSetScore(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetScore called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetScore(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetScore(int score)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			this.score = score;
		}
	}

	protected static void InvokeUserCode_RpcSetScore(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetScore called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetScore(reader.ReadInt());
		}
	}

	protected void UserCode_CmdSetKills(int kills)
	{
		if (base.isServer && !base.hasAuthority)
		{
			this.kills = kills;
			RpcSetKills(kills);
		}
	}

	protected static void InvokeUserCode_CmdSetKills(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetKills called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetKills(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetKills(int kills)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			this.kills = kills;
		}
	}

	protected static void InvokeUserCode_RpcSetKills(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetKills called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetKills(reader.ReadInt());
		}
	}

	protected void UserCode_CmdSetAssists(int assists)
	{
		if (base.isServer && !base.hasAuthority)
		{
			this.assists = assists;
			RpcSetAssists(assists);
		}
	}

	protected static void InvokeUserCode_CmdSetAssists(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetAssists called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetAssists(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetAssists(int assists)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			this.assists = assists;
		}
	}

	protected static void InvokeUserCode_RpcSetAssists(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetAssists called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetAssists(reader.ReadInt());
		}
	}

	protected void UserCode_CmdSetDeaths(int deaths)
	{
		if (base.isServer && !base.hasAuthority)
		{
			this.deaths = deaths;
			RpcSetDeaths(deaths);
		}
	}

	protected static void InvokeUserCode_CmdSetDeaths(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetDeaths called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSetDeaths(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetDeaths(int deaths)
	{
		if (!base.isServer && !base.hasAuthority)
		{
			this.deaths = deaths;
		}
	}

	protected static void InvokeUserCode_RpcSetDeaths(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetDeaths called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSetDeaths(reader.ReadInt());
		}
	}

	protected void UserCode_CmdSyncAttachments(int sight, int barrel, int grip)
	{
		if (base.isServer && !base.hasAuthority)
		{
			if (item is PlayerFirearm)
			{
				(item as PlayerFirearm).SetSightAttachment(sight);
				(item as PlayerFirearm).SetBarrelAttachment(barrel);
				(item as PlayerFirearm).SetGripAttachment(grip);
			}
			RpcSyncAttachments(sight, barrel, grip);
		}
	}

	protected static void InvokeUserCode_CmdSyncAttachments(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncAttachments called on client.");
		}
		else
		{
			((Human)obj).UserCode_CmdSyncAttachments(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
		}
	}

	protected void UserCode_RpcSyncAttachments(int sight, int barrel, int grip)
	{
		if (!base.isServer && !base.hasAuthority && item is PlayerFirearm)
		{
			(item as PlayerFirearm).SetSightAttachment(sight);
			(item as PlayerFirearm).SetBarrelAttachment(barrel);
			(item as PlayerFirearm).SetGripAttachment(grip);
		}
	}

	protected static void InvokeUserCode_RpcSyncAttachments(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSyncAttachments called on server.");
		}
		else
		{
			((Human)obj).UserCode_RpcSyncAttachments(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
		}
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(syncForwardInput);
			writer.WriteFloat(syncRightInput);
			writer.WriteFloat(syncedAimY);
			writer.WriteFloat(syncedAimX);
			writer.WriteBool(syncIsGrounded);
			writer.WriteFloat(health);
			writer.WriteBool(sprinting);
			writer.WriteBool(sneaking);
			writer.WriteString(itemString);
			writer.WriteInt(leanState);
			writer.WriteInt(moveAnimationState);
			writer.WriteInt(overrideMoveAnimationState);
			writer.WriteInt(armAnimationState);
			writer.WriteBool(leaveRepetitiveReload);
			writer.WriteString(humanName);
			writer.WriteInt(team);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(syncForwardInput);
			result = true;
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(syncRightInput);
			result = true;
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(syncedAimY);
			result = true;
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteFloat(syncedAimX);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(syncIsGrounded);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteFloat(health);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteBool(sprinting);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteBool(sneaking);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x100L) != 0L)
		{
			writer.WriteString(itemString);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x200L) != 0L)
		{
			writer.WriteInt(leanState);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x400L) != 0L)
		{
			writer.WriteInt(moveAnimationState);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x800L) != 0L)
		{
			writer.WriteInt(overrideMoveAnimationState);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x1000L) != 0L)
		{
			writer.WriteInt(armAnimationState);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x2000L) != 0L)
		{
			writer.WriteBool(leaveRepetitiveReload);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x4000L) != 0L)
		{
			writer.WriteString(humanName);
			result = true;
		}
		if ((base.syncVarDirtyBits & 0x8000L) != 0L)
		{
			writer.WriteInt(team);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			float num = syncForwardInput;
			NetworksyncForwardInput = reader.ReadFloat();
			float num2 = syncRightInput;
			NetworksyncRightInput = reader.ReadFloat();
			float num3 = syncedAimY;
			NetworksyncedAimY = reader.ReadFloat();
			float num4 = syncedAimX;
			NetworksyncedAimX = reader.ReadFloat();
			bool flag = syncIsGrounded;
			NetworksyncIsGrounded = reader.ReadBool();
			float num5 = health;
			Networkhealth = reader.ReadFloat();
			if (!NetworkBehaviour.SyncVarEqual(num5, ref health))
			{
				HealthChanged(num5, health);
			}
			bool flag2 = sprinting;
			Networksprinting = reader.ReadBool();
			bool flag3 = sneaking;
			Networksneaking = reader.ReadBool();
			string text = itemString;
			NetworkitemString = reader.ReadString();
			int num6 = leanState;
			NetworkleanState = reader.ReadInt();
			int num7 = moveAnimationState;
			NetworkmoveAnimationState = reader.ReadInt();
			int num8 = overrideMoveAnimationState;
			NetworkoverrideMoveAnimationState = reader.ReadInt();
			int num9 = armAnimationState;
			NetworkarmAnimationState = reader.ReadInt();
			bool flag4 = leaveRepetitiveReload;
			NetworkleaveRepetitiveReload = reader.ReadBool();
			string text2 = humanName;
			NetworkhumanName = reader.ReadString();
			int num10 = team;
			Networkteam = reader.ReadInt();
			return;
		}
		long num11 = (long)reader.ReadULong();
		if ((num11 & 1L) != 0L)
		{
			float num12 = syncForwardInput;
			NetworksyncForwardInput = reader.ReadFloat();
		}
		if ((num11 & 2L) != 0L)
		{
			float num13 = syncRightInput;
			NetworksyncRightInput = reader.ReadFloat();
		}
		if ((num11 & 4L) != 0L)
		{
			float num14 = syncedAimY;
			NetworksyncedAimY = reader.ReadFloat();
		}
		if ((num11 & 8L) != 0L)
		{
			float num15 = syncedAimX;
			NetworksyncedAimX = reader.ReadFloat();
		}
		if ((num11 & 0x10L) != 0L)
		{
			bool flag5 = syncIsGrounded;
			NetworksyncIsGrounded = reader.ReadBool();
		}
		if ((num11 & 0x20L) != 0L)
		{
			float num16 = health;
			Networkhealth = reader.ReadFloat();
			if (!NetworkBehaviour.SyncVarEqual(num16, ref health))
			{
				HealthChanged(num16, health);
			}
		}
		if ((num11 & 0x40L) != 0L)
		{
			bool flag6 = sprinting;
			Networksprinting = reader.ReadBool();
		}
		if ((num11 & 0x80L) != 0L)
		{
			bool flag7 = sneaking;
			Networksneaking = reader.ReadBool();
		}
		if ((num11 & 0x100L) != 0L)
		{
			string text3 = itemString;
			NetworkitemString = reader.ReadString();
		}
		if ((num11 & 0x200L) != 0L)
		{
			int num17 = leanState;
			NetworkleanState = reader.ReadInt();
		}
		if ((num11 & 0x400L) != 0L)
		{
			int num18 = moveAnimationState;
			NetworkmoveAnimationState = reader.ReadInt();
		}
		if ((num11 & 0x800L) != 0L)
		{
			int num19 = overrideMoveAnimationState;
			NetworkoverrideMoveAnimationState = reader.ReadInt();
		}
		if ((num11 & 0x1000L) != 0L)
		{
			int num20 = armAnimationState;
			NetworkarmAnimationState = reader.ReadInt();
		}
		if ((num11 & 0x2000L) != 0L)
		{
			bool flag8 = leaveRepetitiveReload;
			NetworkleaveRepetitiveReload = reader.ReadBool();
		}
		if ((num11 & 0x4000L) != 0L)
		{
			string text4 = humanName;
			NetworkhumanName = reader.ReadString();
		}
		if ((num11 & 0x8000L) != 0L)
		{
			int num21 = team;
			Networkteam = reader.ReadInt();
		}
	}
}
