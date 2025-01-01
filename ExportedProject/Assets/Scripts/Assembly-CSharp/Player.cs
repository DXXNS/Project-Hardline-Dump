using System.Collections.Generic;
using EZCameraShake;
using Mirror;
using Mirror.RemoteCalls;
using TrickShotAssets.CustomInputManager;
using UnityEngine;

public class Player : Human
{
	private InputManagerStorage inputManagerStorage;

	private static int playerLayerMask;

	private static int characterLayerMask;

	private static int projectileMask;

	private static int physicsParticleMask;

	private static int hitboxLayerMask;

	private static int ignoreRaycast;

	private static int interactableLayerMask;

	private static float swaySpeed_idle;

	private static float swaySpeed_walking;

	private static float swaySpeed_sprinting;

	private static float swaySpeed_focused;

	private static float swayMagnitude_idle;

	private static float swayMagnitude_walking;

	private static float swayMagnitude_sprinting;

	private static float swayMagnitude_focused;

	private static float itemSwayMagnitude_idle;

	private static float itemSwayMagnitude_walking;

	private static float itemSwayMagnitude_sprinting;

	private static float itemSwayMagnitude_focused;

	private static float ytransform_client_offset;

	private static float spectatorCamYOffset;

	private static float hitCameraShakeRoughness;

	private static float hitCameraShakeFadeInTime;

	private static float hitCameraShakeFadeOutTime;

	private static float suppressCameraShakeRoughness;

	private static float suppressCameraShakeFadeInTime;

	private static float suppressCameraShakeFadeOutTime;

	private static float fallingCameraShakeMultiplier;

	private static float fallingCameraShakeRoughness;

	private static float fallingCameraShakeFadeInTime;

	private static float fallingCameraShakeFadeOutTime;

	private static float sprintCooldown;

	private float interactionDistance = 2f;

	private static float damageCameraShakeMultiplier;

	private static float damageCameraShakeMin;

	private static float damageCameraShakeMax;

	private static float suppressCameraShakeMultiplier;

	private bool menuOpened;

	private bool loadoutSelectOpened;

	private bool scoreboardOpened;

	private bool inputEnabled = true;

	private bool lookEnabled = true;

	[SerializeField]
	private float baseSensitivity;

	private float playerSensitivity;

	private float sensitivity;

	private bool sprintReady;

	[SerializeField]
	private PlayerCamera playerCamera;

	private float aimDownSightSwingReduction = 3.5f;

	private bool focused;

	private bool toggleADS;

	private bool toggleLean;

	[SerializeField]
	private bool playerAuthority;

	[SerializeField]
	private GameObject spectatorModeCamera;

	private bool isSpectating;

	private bool crouchToggle;

	private float lastYVelocity;

	[SerializeField]
	private GameObject hitSoundEffect;

	[SerializeField]
	private GameObject killSoundEffect;

	[SerializeField]
	private GameObject playerReticleGroup;

	[SerializeField]
	private GameObject baseHitMarker;

	[SerializeField]
	private GameObject killHitMarker;

	private ChatHandler chatHandler;

	private float checkLatencyFrequency = 0.3f;

	private float pingSendTime;

	private bool showHeadgear;

	private UserInterface userInterface;

	private PostProcessingManager postProcessingManager;

	private KillText killText;

	public bool InputEnabled
	{
		get
		{
			return inputEnabled;
		}
		set
		{
			inputEnabled = value;
		}
	}

	public bool MenuOpened
	{
		get
		{
			return menuOpened;
		}
		set
		{
			menuOpened = value;
		}
	}

	public float PlayerSensitivity
	{
		get
		{
			return playerSensitivity;
		}
		set
		{
			playerSensitivity = value;
		}
	}

	public bool LoadoutSelectOpened
	{
		get
		{
			return loadoutSelectOpened;
		}
		set
		{
			loadoutSelectOpened = value;
		}
	}

	public bool LookEnabled
	{
		get
		{
			return lookEnabled;
		}
		set
		{
			lookEnabled = value;
		}
	}

	public bool IsSpectating
	{
		get
		{
			return isSpectating;
		}
		set
		{
			isSpectating = value;
		}
	}

	public GameObject BaseHitMarker
	{
		get
		{
			return baseHitMarker;
		}
		set
		{
			baseHitMarker = value;
		}
	}

	public GameObject KillHitMarker
	{
		get
		{
			return killHitMarker;
		}
		set
		{
			killHitMarker = value;
		}
	}

	public void ReplicateReset()
	{
		if (base.hasAuthority)
		{
			if (!base.isServer)
			{
				CmdReset();
			}
			else
			{
				RpcReset();
			}
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdReset()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(Player), "CmdReset", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcReset()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(Player), "RpcReset", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void ResetSprintReady()
	{
		sprintReady = true;
	}

	protected override void ReduceHealth(float damage, string causer, Vector3 hitPos, Vector3 hitRot)
	{
		ApplyImpact(hitPos, hitRot, damage);
		if (damage >= 0f && base.hasAuthority)
		{
			float magnitude = Mathf.Clamp(damage * damageCameraShakeMultiplier, damageCameraShakeMin, damageCameraShakeMax);
			playerCamera.AddCameraShake(magnitude, hitCameraShakeRoughness, hitCameraShakeFadeInTime, hitCameraShakeFadeOutTime);
			postProcessingManager.Hit(Mathf.Abs(damage));
		}
		else if (base.hasAuthority)
		{
			postProcessingManager.Heal(Mathf.Abs(damage));
		}
		if (!(base.Health > 0f))
		{
			return;
		}
		if (base.Health - damage < 100f)
		{
			base.Health -= damage;
			if (!(base.Health < 0f))
			{
				return;
			}
			if (base.isServer)
			{
				if (base.hasAuthority)
				{
					userInterface.KilledByNotification(causer);
				}
				RpcDeathMessage(causer);
				if (!base.CallDeathFlag)
				{
					Death();
					base.CallDeathFlag = true;
				}
			}
			else
			{
				CmdDeathMessage(causer);
			}
		}
		else
		{
			base.Health = 100f;
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdDeathMessage(string causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(causer);
		SendCommandInternal(typeof(Player), "CmdDeathMessage", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcDeathMessage(string causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(causer);
		SendRPCInternal(typeof(Player), "RpcDeathMessage", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdPing(string causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(causer);
		SendCommandInternal(typeof(Player), "CmdPing", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcPing(string causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(causer);
		SendRPCInternal(typeof(Player), "RpcPing", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public override void HealthChanged(float oldValue, float newValue)
	{
		base.HealthChanged(oldValue, newValue);
	}

	private void OnEnable()
	{
		base.Active = false;
	}

	public override void Start()
	{
		base.Start();
		inputManagerStorage = InputManagerStorage.Instance;
		if ((bool)Object.FindObjectOfType<ChatHandler>())
		{
			chatHandler = Object.FindObjectOfType<ChatHandler>();
		}
		if (base.hasAuthority && (bool)chatHandler)
		{
			chatHandler.GetPlayerInfo();
		}
	}

	public void CheckLatency()
	{
		if (base.hasAuthority)
		{
			if (!base.isServer)
			{
				CmdPing(base.HumanName);
				pingSendTime = Time.time;
			}
			else
			{
				userInterface.UpdateLatency(0);
			}
		}
	}

	public override void Init()
	{
		gameManager = Object.FindObjectOfType<HardlineGameManager>();
		networkManager = Object.FindObjectOfType<HardlineNetworkManager>();
		userInterface = Object.FindObjectOfType<UserInterface>();
		postProcessingManager = Object.FindObjectOfType<PostProcessingManager>();
		killText = Object.FindObjectOfType<KillText>();
		base.Lean = 0f;
		base.NetworkleanState = 0;
		weaponParent.SetActive(value: true);
		if (!IsInvoking("CheckLatency") && base.hasAuthority)
		{
			InvokeRepeating("CheckLatency", checkLatencyFrequency, checkLatencyFrequency);
		}
		playerCamera.ResetCamera();
		base.PlayersKilled.Clear();
		base.PlayersDamaged.Clear();
		playerCamera.EnableAudioListening(base.hasAuthority);
		if (PlayerPrefs.GetInt("Input_ToggleLean") == 1)
		{
			toggleLean = true;
		}
		else
		{
			toggleLean = false;
		}
		MenuOpened = false;
		userInterface = Object.FindObjectOfType<UserInterface>();
		base.Init();
		InitializeClient();
		UpdateMenu();
		UpdateScoreboard();
		EnableHitboxes();
		charController.enabled = true;
		base.Active = true;
		base.CallDeathFlag = false;
		ClothingWithExtras[] componentsInChildren = GetComponentsInChildren<ClothingWithExtras>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ShowExtras();
		}
		if (base.hasAuthority)
		{
			UpdateSensitivity(PlayerPrefs.GetFloat("Input_Sensitivity", 0.5f));
			UpdateVolume(PlayerPrefs.GetFloat("Audio_Volume", 0.5f));
			base.UseThirdPersonAnimations = false;
			if (!playerCamera.GetComponent<CameraShaker>().enabled)
			{
				playerCamera.GetComponent<CameraShaker>().enabled = true;
				playerCamera.GetComponent<CameraShaker>().AddInstance();
			}
			playerCamera.gameObject.SetActive(value: true);
			playerCamera.GetComponent<Camera>().enabled = true;
			postProcessingManager.IsDead = false;
			CloseSpectatorMode();
			CancelInvoke("EnterSpectatorMode");
			GoToSpawn();
		}
		else
		{
			base.UseThirdPersonAnimations = true;
		}
		UpdateTeamAppearance();
	}

	public void GoToSpawn()
	{
		base.Velocity = new Vector3(0f, 0f, 0f);
		if ((bool)gameManager)
		{
			Transform spawnPositionForTeam = gameManager.GetSpawnPositionForTeam(base.Team);
			Vector3 vector = new Vector3(0f, 0f, 0f);
			if ((bool)spawnPositionForTeam.GetComponent<TeamSpawn>())
			{
				TeamSpawn component = spawnPositionForTeam.GetComponent<TeamSpawn>();
				vector = new Vector3(Random.Range(0f - component.SpawnRadius, component.SpawnRadius), 0f, Random.Range(0f - component.SpawnRadius, component.SpawnRadius));
			}
			charController.enabled = false;
			base.transform.position = spawnPositionForTeam.position + vector;
			base.transform.eulerAngles = spawnPositionForTeam.eulerAngles;
			charController.enabled = true;
		}
	}

	public void ResetCharacter()
	{
		base.NetworkleanState = 0;
		base.Lean = 0f;
		GameObject[] array = base.Meshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		GetComponent<CharacterController>().detectCollisions = true;
		Init();
	}

	public void InitializeClient()
	{
		if (base.hasAuthority)
		{
			LoadItem("Unarmed", base.hasAuthority);
		}
		else
		{
			playerCamera.gameObject.SetActive(value: false);
		}
	}

	public override void Update()
	{
		if (!base.Active)
		{
			return;
		}
		if (base.hasAuthority)
		{
			postProcessingManager.InMenu = menuOpened | loadoutSelectOpened | scoreboardOpened;
		}
		if (base.Health > 0f)
		{
			CheckHeadClearance();
			UpdateAnimatorValues();
			CheckFootsteps();
			UpdatePivot();
			if (base.ForwardInput != 0f || base.RightInput != 0f)
			{
				base.Walking = true;
			}
			else
			{
				base.Walking = false;
			}
			if (base.hasAuthority)
			{
				headwearParent.SetActive(value: false);
			}
			else if ((bool)base.SpectatorCamera)
			{
				if (base.SpectatorCamera.GetComponent<SpectatorCamera>().CurrentlySpectating == this)
				{
					headwearParent.SetActive(value: false);
				}
				else
				{
					headwearParent.SetActive(value: true);
				}
			}
			else
			{
				headwearParent.SetActive(value: true);
			}
			if (updateAppearanceFlag)
			{
				UpdateTeamAppearance();
				updateAppearanceFlag = false;
			}
		}
		if (base.hasAuthority)
		{
			ClientUpdate();
		}
		else
		{
			UpdateAimDownSightState();
		}
		if (localPlayer == null)
		{
			FindLocalPlayer();
		}
		if ((bool)playerNameTag)
		{
			UpdateBillboard();
		}
	}

	public void LateUpdate()
	{
		if (!base.CallDeathFlag && base.Health <= 0f)
		{
			Death();
			base.CallDeathFlag = true;
		}
		if (base.Animator.GetLayerIndex("Head") != -1)
		{
			if (base.UseThirdPersonAnimations)
			{
				UpdateFeetIK();
				base.Animator.SetLayerWeight(base.Animator.GetLayerIndex("Head"), 1f);
			}
			else
			{
				base.Animator.SetLayerWeight(base.Animator.GetLayerIndex("Head"), 0f);
			}
		}
		if (base.Active)
		{
			UpdateCharacterControllerDimensions();
			UpdateAnimations();
			if (base.hasAuthority)
			{
				CheckForAssists();
			}
		}
	}

	public void CheckForAssists()
	{
		if (!base.hasAuthority)
		{
			return;
		}
		List<Human> list = new List<Human>();
		foreach (Human item in base.PlayersDamaged)
		{
			if (base.PlayersKilled.Contains(item))
			{
				list.Add(item);
			}
			else if (item.Health <= 0f)
			{
				base.Assists++;
				AddScore(50);
				if ((bool)killText)
				{
					killText.CreateNewAssistText(item.HumanName);
				}
				list.Add(item);
			}
		}
		foreach (Human item2 in list)
		{
			base.PlayersDamaged.Remove(item2);
		}
	}

	[ClientCallback]
	protected override void ClientUpdate()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		base.ClientUpdate();
		UpdateUI();
		if (base.Health > 0f && base.hasAuthority)
		{
			UpdatePlayerReticle();
			CheckReloadAnimations();
			if (InputEnabled)
			{
				RegisterInputs();
			}
			else
			{
				CancelInputs();
			}
			if (lookEnabled)
			{
				PlayerMouseAxisInputs();
			}
			userInterface.UpdateItemDisplay(base.Item.GetDisplayName(), base.Item.ItemIcon);
			UpdateCamera();
		}
		if (base.hasAuthority)
		{
			PlayerMiscInputs();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateImpactForce();
		if (base.Active)
		{
			if (base.hasAuthority)
			{
				ClientFixedUpdate();
			}
			if (base.Health > 0f)
			{
				CastForwardSpaceRay();
				UpdatePControlIK();
				UpdateFixedAnimationValues();
				UpdateSway();
			}
		}
	}

	[Client]
	protected override void ClientFixedUpdate()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Player::ClientFixedUpdate()' called when client was not active");
			return;
		}
		base.ClientFixedUpdate();
		if (base.hasAuthority)
		{
			UpdateMovement();
			CastForwardSpaceRay();
			ImplementAimRecoil();
		}
	}

	public override void UpdateMovement()
	{
		base.UpdateMovement();
		base.Item.UpdateItemShift(base.RightInput, base.Velocity.y + base.FloorGravity);
		if (!charController.isGrounded)
		{
			crouchToggle = false;
		}
	}

	public void CancelInputs()
	{
		base.ForwardInput = 0f;
		base.RightInput = 0f;
	}

	public void RegisterInputs()
	{
		if (!sprintReady && !IsInvoking("ResetSprintReady"))
		{
			Invoke("ResetSprintReady", sprintCooldown);
		}
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Forward")].key))
		{
			base.ForwardInput = 1f;
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Sprint")].key) && charController.isGrounded && sprintReady && fallRecovered && !isSliding && slideRecovered)
			{
				base.Sprinting = true;
				base.Item.Sprinting = true;
			}
			else if (base.Sprinting)
			{
				base.Sprinting = false;
				base.Item.Sprinting = false;
				sprintReady = false;
			}
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Sprint")].key) && !isSliding)
			{
				crouchToggle = false;
			}
		}
		else if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Back")].key))
		{
			base.ForwardInput = -1f;
			base.Sprinting = false;
			base.Item.Sprinting = false;
		}
		else
		{
			base.ForwardInput = 0f;
			base.Sprinting = false;
			base.Item.Sprinting = false;
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Sprint")].key) && !Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Left")].key) && !Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Right")].key))
			{
				focused = true;
			}
			else
			{
				focused = false;
			}
		}
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Left")].key))
		{
			base.RightInput = -1f;
			base.ForwardInput /= 2f;
		}
		else if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Right")].key))
		{
			base.RightInput = 1f;
			base.ForwardInput /= 2f;
		}
		else
		{
			base.RightInput = 0f;
		}
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Jump")].key))
		{
			base.JumpQueued = true;
		}
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Reload")].key))
		{
			if (base.Item is PlayerManualFirearm)
			{
				if (!(base.Item as PlayerFirearm).Reloading && !(base.Item as PlayerManualFirearm).CurrentlyRecycling && (base.Item as PlayerFirearm).CanReload())
				{
					(base.Item as PlayerManualFirearm).CallReload();
				}
			}
			else if (base.Item is PlayerFirearm && !(base.Item as PlayerFirearm).Reloading && (base.Item as PlayerFirearm).CanReload())
			{
				(base.Item as PlayerFirearm).CallReload();
			}
		}
		if (!base.Sprinting)
		{
			LeanInputs();
		}
		else
		{
			base.NetworkleanState = 0;
		}
		if (Input.GetKeyDown(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Interact")].key))
		{
			CastInteractionRay(isInteract: true);
		}
		else
		{
			CastInteractionRay(isInteract: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1) && base.HumanInventory.CurrentlySelected != 1)
		{
			SelectFromInventory(1, equipForNewItem: false, init: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) && base.HumanInventory.CurrentlySelected != 2)
		{
			SelectFromInventory(2, equipForNewItem: false, init: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) && base.HumanInventory.CurrentlySelected != 3)
		{
			SelectFromInventory(3, equipForNewItem: false, init: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4) && base.HumanInventory.CurrentlySelected != 4)
		{
			SelectFromInventory(4, equipForNewItem: false, init: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5) && base.HumanInventory.CurrentlySelected != 5)
		{
			SelectFromInventory(5, equipForNewItem: false, init: false);
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			int num = base.HumanInventory.CurrentlySelected;
			if (axis > 0f && base.HumanInventory.CurrentlySelected < 5)
			{
				num++;
			}
			else if (axis < 0f && base.HumanInventory.CurrentlySelected > 1)
			{
				num--;
			}
			if (num != base.HumanInventory.CurrentlySelected)
			{
				SelectFromInventory(num, equipForNewItem: false, init: false);
			}
		}
		if (Input.GetKeyDown(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("CrouchToggle")].key))
		{
			crouchToggle = !crouchToggle;
		}
		if (!base.Sprinting)
		{
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Sneak")].key))
			{
				base.Networksneaking = true;
			}
			else
			{
				base.Networksneaking = false;
			}
		}
		else
		{
			base.Networksneaking = false;
		}
		if (!charController.isGrounded)
		{
			crouchToggle = false;
		}
		if (!crouchToggle)
		{
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("CrouchHold")].key) || crouchObstructed || isSliding)
			{
				if (base.Sprinting)
				{
					CheckSlide();
				}
				base.Crouch = true;
			}
			else if (!crouchObstructed)
			{
				base.Crouch = false;
			}
		}
		else
		{
			if (base.Sprinting)
			{
				CheckSlide();
			}
			base.Crouch = true;
		}
		if (!toggleADS)
		{
			base.Item.SecondaryUseItem(Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("Aim")].key));
		}
		else
		{
			base.Item.SecondaryUseItem(toggleADS);
		}
		if (base.Item.PrimaryUseItem(Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("UseItem")].key)))
		{
			playerCamera.AddCameraShake(base.Item.CameraShakeMagnitude, base.Item.CameraShakeRoughness, base.Item.CameraShakeFadeIn, base.Item.CameraShakeFadeOut);
		}
		Input.GetKeyDown(KeyCode.Y);
	}

	private void LeanInputs()
	{
		if (!toggleLean)
		{
			if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("LeanLeft")].key))
			{
				base.NetworkleanState = 1;
			}
			else if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("LeanRight")].key))
			{
				base.NetworkleanState = 2;
			}
			else
			{
				base.NetworkleanState = 0;
			}
		}
		else if (Input.GetKeyDown(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("LeanLeft")].key))
		{
			if (leanState != 1)
			{
				base.NetworkleanState = 1;
			}
			else
			{
				base.NetworkleanState = 0;
			}
		}
		else if (Input.GetKeyDown(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("LeanRight")].key))
		{
			if (leanState != 2)
			{
				base.NetworkleanState = 2;
			}
			else
			{
				base.NetworkleanState = 0;
			}
		}
	}

	public void PlayerMiscInputs()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if ((bool)chatHandler)
			{
				if (!chatHandler.InputField.isFocused)
				{
					MenuOpened = !MenuOpened;
					UpdateMenu();
				}
			}
			else
			{
				MenuOpened = !MenuOpened;
				UpdateMenu();
			}
		}
		if (Input.GetKey(KeyCode.Tab))
		{
			scoreboardOpened = true;
			UpdateScoreboard();
		}
		else
		{
			scoreboardOpened = false;
			UpdateScoreboard();
		}
	}

	public void PlayerMouseAxisInputs()
	{
		float num = Input.GetAxis("Mouse X") * sensitivity;
		float num2 = Input.GetAxis("Mouse Y") * sensitivity;
		if (!base.AimingDownSights)
		{
			base.Item.AddItemSwing(0f - num, num2);
		}
		else
		{
			base.Item.AddItemSwing((0f - num) / aimDownSightSwingReduction, num2 / aimDownSightSwingReduction);
		}
		UpdatePlayerRotations(num, num2);
	}

	private void ImplementAimRecoil()
	{
		float num = 0f;
		float num2 = 0f;
		if (base.Item is PlayerFirearm)
		{
			num += (base.Item as PlayerFirearm).AimRecoilVelocity.x * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant;
			num2 += (base.Item as PlayerFirearm).AimRecoilVelocity.y * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant;
		}
		UpdatePlayerRotations(num, num2);
	}

	private void UpdateMenu()
	{
		if (MenuOpened)
		{
			userInterface.ToggleSettings(opened: true);
			ResetInputs();
		}
		else
		{
			userInterface.ToggleSettings(opened: false);
		}
	}

	private void UpdateScoreboard()
	{
		if (scoreboardOpened)
		{
			userInterface.SetScoreBoardState(state: true);
		}
		else
		{
			userInterface.SetScoreBoardState(state: false);
		}
	}

	protected override void Death()
	{
		if (base.hasAuthority)
		{
			base.Deaths++;
		}
		ClothingWithExtras[] componentsInChildren = GetComponentsInChildren<ClothingWithExtras>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].HideExtras();
		}
		GameObject[] array = base.Meshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		headwearParent.SetActive(value: false);
		SetClothingVisiblity(visible: false);
		GetComponent<CharacterController>().detectCollisions = false;
		DisableHitboxes();
		SpawnRagdoll(base.transform);
		ForceSetItemString("Unarmed");
		weaponParent.SetActive(value: false);
		charController.enabled = false;
		if (base.hasAuthority)
		{
			postProcessingManager.IsDead = true;
			Invoke("EnterSpectatorMode", 4f);
			playerCamera.GetComponent<Camera>().enabled = false;
		}
	}

	public void DisableHitboxes()
	{
		Hitbox[] componentsInChildren = GetComponentsInChildren<Hitbox>();
		foreach (Hitbox hitbox in componentsInChildren)
		{
			if (hitbox.Player == this)
			{
				hitbox.GetComponent<Collider>().enabled = false;
			}
		}
	}

	public void UpdatePlayerReticle()
	{
		if (base.Item is PlayerFirearm)
		{
			playerReticleGroup.SetActive(value: false);
		}
		else
		{
			playerReticleGroup.SetActive(value: true);
		}
	}

	public void EnableHitboxes()
	{
		Hitbox[] componentsInChildren = GetComponentsInChildren<Hitbox>();
		foreach (Hitbox hitbox in componentsInChildren)
		{
			if (hitbox.Player == this)
			{
				hitbox.GetComponent<Collider>().enabled = true;
			}
		}
	}

	private void UpdateCamera()
	{
		if (base.Item is PlayerFirearm)
		{
			if ((bool)(base.Item as PlayerFirearm).ActiveSight)
			{
				playerCamera.SetTargFOV((base.Item as PlayerFirearm).ActiveSight.UseFOV, base.AimingDownSights);
				if (base.AimingDownSights)
				{
					sensitivity = baseSensitivity * (base.Item as PlayerFirearm).ActiveSight.SensitivityMultiplier * playerSensitivity;
				}
				else
				{
					sensitivity = baseSensitivity * playerSensitivity;
				}
			}
		}
		else
		{
			playerCamera.SetTargFOV(playerCamera.BaseFov, base.AimingDownSights);
			sensitivity = baseSensitivity * playerSensitivity;
		}
	}

	private void UpdateSway()
	{
		if (base.Walking)
		{
			if (base.Sprinting)
			{
				playerCamera.SetCameraSwayValues(swaySpeed_sprinting, swayMagnitude_sprinting);
				base.Item.SetItemSwayValues(swaySpeed_sprinting, itemSwayMagnitude_sprinting);
			}
			else if (sneaking)
			{
				playerCamera.SetCameraSwayValues(swaySpeed_walking * sneakSpeedMultiplier, swayMagnitude_walking);
				base.Item.SetItemSwayValues(swaySpeed_walking * sneakSpeedMultiplier, itemSwayMagnitude_walking);
			}
			else
			{
				playerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_walking);
				base.Item.SetItemSwayValues(swaySpeed_walking, itemSwayMagnitude_walking);
			}
		}
		else if (focused)
		{
			playerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_focused);
			base.Item.SetItemSwayValues(swaySpeed_focused, itemSwayMagnitude_focused);
		}
		else
		{
			playerCamera.SetCameraSwayValues(swaySpeed_idle, swayMagnitude_idle);
			base.Item.SetItemSwayValues(swaySpeed_idle, itemSwayMagnitude_idle);
		}
	}

	private void CastInteractionRay(bool isInteract)
	{
		if (!Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var hitInfo))
		{
			return;
		}
		if (hitInfo.distance <= interactionDistance)
		{
			if ((bool)hitInfo.transform.GetComponent<PickupItem>())
			{
				userInterface.UpdateInteractPrompt(enabled: true, "Equip");
			}
			else if ((bool)hitInfo.transform.GetComponent<DoorInteractive>())
			{
				userInterface.UpdateInteractPrompt(enabled: true, "Open");
			}
			else if ((bool)hitInfo.transform.GetComponent<UplinkStation>() && hitInfo.transform.GetComponent<UplinkStation>().Interactable && hitInfo.transform.GetComponent<UplinkStation>().ControlState != base.Team)
			{
				userInterface.UpdateInteractPrompt(enabled: true, "Capture");
			}
			else if (hitInfo.transform.tag == "AmmoCrate")
			{
				userInterface.UpdateInteractPrompt(enabled: true, "Resupply");
			}
			else if (hitInfo.transform.tag == "WorldButton")
			{
				userInterface.UpdateInteractPrompt(enabled: true, "Select");
			}
			else
			{
				userInterface.UpdateInteractPrompt(enabled: false, "");
			}
			if (!isInteract)
			{
				return;
			}
			if ((bool)hitInfo.transform.GetComponent<PickupItem>())
			{
				LoadItemFromPickup(hitInfo.transform.gameObject, base.hasAuthority);
			}
			if ((bool)hitInfo.transform.GetComponent<DoorInteractive>())
			{
				hitInfo.transform.GetComponent<DoorInteractive>().Interact(base.transform.position);
			}
			if ((bool)hitInfo.transform.GetComponent<UplinkStation>())
			{
				hitInfo.transform.GetComponent<UplinkStation>().ReplicateInteract(base.Team);
			}
			if (hitInfo.transform.tag == "AmmoCrate")
			{
				Resupply();
			}
			if (hitInfo.transform.tag == "WorldButton")
			{
				if (hitInfo.transform.gameObject.name == "Loadout1 Select")
				{
					base.HumanInventory.SetInventory(CreateSpecialistInventory(1));
				}
				if (hitInfo.transform.gameObject.name == "Loadout2 Select")
				{
					base.HumanInventory.SetInventory(CreateSpecialistInventory(2));
				}
				if (hitInfo.transform.gameObject.name == "Loadout3 Select")
				{
					base.HumanInventory.SetInventory(CreateSpecialistInventory(3));
				}
				SelectFromInventory(1, equipForNewItem: true, init: true);
			}
		}
		else
		{
			userInterface.UpdateInteractPrompt(enabled: false, "");
		}
	}

	public Inventory CreateSpecialistInventory(int loadout)
	{
		PlayerItem primary = gameManager.GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + loadout + "_primary", "M416"), PlayerPrefs.GetInt("loadout_" + loadout + "_primary_sight", 0), PlayerPrefs.GetInt("loadout_" + loadout + "_primary_barrel", 0), PlayerPrefs.GetInt("loadout_" + loadout + "_primary_grip", 0));
		PlayerItem secondary = gameManager.GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + loadout + "_secondary", "Glock"), PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_sight", 0), PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_barrel", 0), PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_grip", 0));
		PlayerItem equipment = gameManager.GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + loadout + "_equipment1", "FragGrenade"));
		PlayerItem equipment2 = gameManager.GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + loadout + "_equipment2", "StimPen"));
		PlayerItem meleeWeapon = gameManager.GenerateItem("Weapon_" + PlayerPrefs.GetString("loadout_" + loadout + "_meleeWeapon", "Knife"));
		Inventory inventory = new Inventory();
		inventory.SetInventory(primary, secondary, equipment, equipment2, meleeWeapon, fullAmmo: true);
		gameManager.ClearGeneratedWeapons();
		return inventory;
	}

	public void Resupply()
	{
		if (base.Item is PlayerFirearm)
		{
			(base.Item as PlayerFirearm).ResupplyWeapon();
		}
		if (base.HumanInventory.PrimaryItem is PlayerFirearm)
		{
			(base.HumanInventory.PrimaryItem as PlayerFirearm).ResupplyWeapon();
		}
		if (base.HumanInventory.SecondaryItem is PlayerFirearm)
		{
			(base.HumanInventory.SecondaryItem as PlayerFirearm).ResupplyWeapon();
		}
	}

	private void CastForwardSpaceRay()
	{
		if (Physics.Raycast(base.ItemAlignment.transform.position, playerCamera.transform.forward, out var hitInfo, 20f, ~(playerLayerMask | hitboxLayerMask | interactableLayerMask | ignoreRaycast | characterLayerMask | projectileMask | physicsParticleMask)))
		{
			if (base.Item is PlayerFirearm)
			{
				(base.Item as PlayerFirearm).SetWeaponDraw(hitInfo.distance);
			}
		}
		else if (base.Item is PlayerFirearm)
		{
			(base.Item as PlayerFirearm).SetWeaponDraw(20f);
		}
	}

	private void UpdateUI()
	{
		if ((bool)userInterface && base.hasAuthority)
		{
			userInterface.UpdateHealthDisplay(base.Health);
			if (base.Item is PlayerFirearm)
			{
				userInterface.SetAmmoTexts((base.Item as PlayerFirearm).Ammo, (base.Item as PlayerFirearm).ReserveAmmo);
			}
			else
			{
				userInterface.SetAmmoTexts(0, 0);
			}
		}
	}

	public void CheckReloadAnimations()
	{
		if (!(base.Item is PlayerFirearm))
		{
			return;
		}
		if ((base.Item as PlayerFirearm).UseRepetitiveReload)
		{
			if ((base.Item as PlayerFirearm).RepetitiveReloading)
			{
				base.LeaveRepetitiveReload = false;
			}
			else
			{
				base.LeaveRepetitiveReload = true;
			}
			CmdSetLeaveRepetitiveReload(base.LeaveRepetitiveReload);
		}
		else if ((base.Item as PlayerFirearm).Reloading && !(base.Item as PlayerFirearm).ReloadingFlag)
		{
			RunReloadAnimation();
			ReplicateReloadAnimation();
			(base.Item as PlayerFirearm).ReloadingFlag = true;
		}
	}

	public void UpdateSensitivity(float sensitivity)
	{
		PlayerSensitivity = sensitivity;
	}

	public void UpdateVolume(float volume)
	{
		AudioListener.volume = volume;
	}

	public void UpdateFOV(float fov)
	{
		playerCamera.SetBaseFOV(fov);
	}

	public void UpdateToggleLean(bool toggleLean)
	{
		this.toggleLean = toggleLean;
	}

	public void EnterSpectatorMode()
	{
		isSpectating = true;
		postProcessingManager.IsDead = false;
		Object.Instantiate(spectatorModeCamera, base.transform.position + new Vector3(0f, spectatorCamYOffset, 0f), base.transform.rotation);
	}

	public void CloseSpectatorMode()
	{
		isSpectating = false;
		if ((bool)base.SpectatorCamera)
		{
			Object.Destroy(base.SpectatorCamera.gameObject);
		}
	}

	public override void Suppress(float intensity)
	{
		playerCamera.AddCameraShake(intensity * suppressCameraShakeMultiplier, suppressCameraShakeRoughness, suppressCameraShakeFadeInTime, suppressCameraShakeFadeOutTime);
		postProcessingManager.Suppress(intensity);
	}

	public void FleshImpactSound(Vector3 hitPoint)
	{
		Object.Instantiate(fleshImpactSounds[Random.Range(0, fleshImpactSounds.Length)], base.transform.position, base.transform.rotation);
	}

	public void HitMarkerSound(bool killShot)
	{
		if ((bool)hitSoundEffect && !killShot)
		{
			Object.Instantiate(hitSoundEffect, base.transform.position, base.transform.rotation);
		}
		else if ((bool)killSoundEffect && killShot)
		{
			Object.Instantiate(killSoundEffect, base.transform.position, base.transform.rotation);
		}
	}

	protected override void FallLand(float velocity)
	{
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("CrouchHold")].key))
		{
			CheckSlide();
			base.Crouch = true;
		}
		if (Input.GetKey(inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName("CrouchToggle")].key))
		{
			CheckSlide();
			crouchToggle = true;
			base.Crouch = true;
		}
		base.FallLand(velocity);
		playerCamera.AddCameraShake(velocity * fallingCameraShakeMultiplier, fallingCameraShakeRoughness, fallingCameraShakeFadeInTime, fallingCameraShakeFadeOutTime);
	}

	static Player()
	{
		playerLayerMask = 4096;
		characterLayerMask = 64;
		projectileMask = 524288;
		physicsParticleMask = 256;
		hitboxLayerMask = 128;
		ignoreRaycast = 4;
		interactableLayerMask = 16384;
		swaySpeed_idle = 0.02f;
		swaySpeed_walking = 0.11f;
		swaySpeed_sprinting = 0.16f;
		swaySpeed_focused = 0.05f;
		swayMagnitude_idle = 0.001f;
		swayMagnitude_walking = 0.01f;
		swayMagnitude_sprinting = 0.03f;
		swayMagnitude_focused = 0f;
		itemSwayMagnitude_idle = 3f;
		itemSwayMagnitude_walking = 2f;
		itemSwayMagnitude_sprinting = 0f;
		itemSwayMagnitude_focused = 0.04f;
		ytransform_client_offset = 0.05f;
		spectatorCamYOffset = 0.5f;
		hitCameraShakeRoughness = 3f;
		hitCameraShakeFadeInTime = 0.1f;
		hitCameraShakeFadeOutTime = 1.4f;
		suppressCameraShakeRoughness = 6f;
		suppressCameraShakeFadeInTime = 0.1f;
		suppressCameraShakeFadeOutTime = 0.6f;
		fallingCameraShakeMultiplier = 6f;
		fallingCameraShakeRoughness = 1.3f;
		fallingCameraShakeFadeInTime = 0.1f;
		fallingCameraShakeFadeOutTime = 1f;
		sprintCooldown = 0.2f;
		damageCameraShakeMultiplier = 0.05f;
		damageCameraShakeMin = 1.5f;
		damageCameraShakeMax = 3f;
		suppressCameraShakeMultiplier = 0.4f;
		RemoteCallHelper.RegisterCommandDelegate(typeof(Player), "CmdReset", InvokeUserCode_CmdReset, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Player), "CmdDeathMessage", InvokeUserCode_CmdDeathMessage, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Player), "CmdPing", InvokeUserCode_CmdPing, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Player), "RpcReset", InvokeUserCode_RpcReset);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Player), "RpcDeathMessage", InvokeUserCode_RpcDeathMessage);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Player), "RpcPing", InvokeUserCode_RpcPing);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdReset()
	{
		if (base.isServer && !base.hasAuthority)
		{
			ResetCharacter();
		}
		RpcReset();
	}

	protected static void InvokeUserCode_CmdReset(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdReset called on client.");
		}
		else
		{
			((Player)obj).UserCode_CmdReset();
		}
	}

	protected void UserCode_RpcReset()
	{
		if (!base.isServer && !base.hasAuthority)
		{
			ResetCharacter();
		}
	}

	protected static void InvokeUserCode_RpcReset(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcReset called on server.");
		}
		else
		{
			((Player)obj).UserCode_RpcReset();
		}
	}

	protected void UserCode_CmdDeathMessage(string causer)
	{
		if (!base.CallDeathFlag)
		{
			base.Health = 0f;
			Death();
			base.CallDeathFlag = true;
		}
		if (base.hasAuthority)
		{
			userInterface.KilledByNotification(causer);
		}
		RpcDeathMessage(causer);
	}

	protected static void InvokeUserCode_CmdDeathMessage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDeathMessage called on client.");
		}
		else
		{
			((Player)obj).UserCode_CmdDeathMessage(reader.ReadString());
		}
	}

	protected void UserCode_RpcDeathMessage(string causer)
	{
		if (!base.CallDeathFlag)
		{
			base.Health = 0f;
			Death();
			base.CallDeathFlag = true;
		}
		if (base.hasAuthority)
		{
			userInterface.KilledByNotification(causer);
		}
	}

	protected static void InvokeUserCode_RpcDeathMessage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDeathMessage called on server.");
		}
		else
		{
			((Player)obj).UserCode_RpcDeathMessage(reader.ReadString());
		}
	}

	protected void UserCode_CmdPing(string causer)
	{
		if (base.isServer)
		{
			RpcPing(causer);
		}
	}

	protected static void InvokeUserCode_CmdPing(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPing called on client.");
		}
		else
		{
			((Player)obj).UserCode_CmdPing(reader.ReadString());
		}
	}

	protected void UserCode_RpcPing(string causer)
	{
		if (causer == base.HumanName && base.hasAuthority)
		{
			int latency = Mathf.RoundToInt((Time.time - pingSendTime) * 1000f);
			userInterface.UpdateLatency(latency);
		}
	}

	protected static void InvokeUserCode_RpcPing(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPing called on server.");
		}
		else
		{
			((Player)obj).UserCode_RpcPing(reader.ReadString());
		}
	}
}
