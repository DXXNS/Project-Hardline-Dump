using UnityEngine;

public class PlayerFirearm : PlayerItem
{
	[SerializeField]
	private float recoilRecovery_kP;

	private static float recoilVelocity_kP = 0.3f;

	private static float aimShiftMultiplier = 0.08f;

	private static float aimSwayMultiplier = 0.4f;

	private static float reticleAppearDelay = 0.2f;

	private static float coolDownRound_coeff = 5f;

	private bool shotQueued;

	private float recoilRecovery_kI;

	private float recoilRecovery_kD;

	[SerializeField]
	private float recoilPosRecovery_kP;

	private float recoilPosRecovery_kI;

	private float recoilPosRecovery_kD;

	[SerializeField]
	protected float spreadAmountHip;

	[SerializeField]
	protected float spreadAmountADS;

	[SerializeField]
	protected float hipSpreadIncreasePerShot;

	[SerializeField]
	protected float hipSpreadRecovery_kP;

	[SerializeField]
	protected float hipSpreadMax;

	protected float accumulatedHipSpread;

	protected float currentSpread;

	[SerializeField]
	private int numberOfProjectiles;

	[SerializeField]
	private Vector3 hitmarkerPosition;

	private bool aimedDownSights;

	private bool reloading;

	private bool reloadingFlag;

	[Header("Instantiation and Usage")]
	[SerializeField]
	private Projectile projectile;

	[SerializeField]
	private GameObject useAudio;

	[SerializeField]
	private GameObject useEffect;

	[SerializeField]
	private GameObject casing;

	[SerializeField]
	protected Transform barrelPoint;

	[SerializeField]
	private Transform casingPoint;

	private int ammo;

	[SerializeField]
	private int maxAmmo;

	private int reserveAmmo;

	[SerializeField]
	private int startingReserveAmmo;

	[SerializeField]
	private int resupplyReserveAmmo;

	[SerializeField]
	private bool chambered;

	private bool repetitiveReloading;

	private bool stopRepetitiveReload;

	[Header("Weapon stats")]
	[SerializeField]
	private float adsMovementSpeedMultiplier = 1f;

	[SerializeField]
	protected Vector3 gunRecoil;

	[SerializeField]
	protected Vector3 aimRecoil;

	[SerializeField]
	private Vector3 positionCoefficients;

	[SerializeField]
	protected Vector3 gunRecoilOffsets;

	[SerializeField]
	private float crouchRecoilMultiplier = 1f;

	protected Vector3 affectedGunRecoil;

	protected Vector3 affectedAimRecoil;

	[SerializeField]
	private float itemRecovery_kP_ads;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private bool useRepetitiveReload;

	[SerializeField]
	private float reloadTime;

	[SerializeField]
	private bool automatic;

	protected float cooldownRemaining;

	private float timeOfLastShot;

	private bool fireFlag;

	private Vector3 gunRecoilVelocity;

	private Vector3 targetGunRecoilVelocity;

	private Vector3 aimRecoilVelocity;

	private Vector3 gunRecoilDisplacement;

	private PIDController recoilPID_x;

	private PIDController recoilPID_y;

	private PIDController recoilPID_z;

	[SerializeField]
	private AnimationCurve penetrationDropOff;

	[SerializeField]
	private AnimationCurve damageDropOff;

	[Header("Weapon Physical Aspects")]
	[SerializeField]
	private float weaponDrawLength;

	[SerializeField]
	private float weaponRaiseStart;

	[SerializeField]
	private float weaponRaiseCoefficient;

	[SerializeField]
	private float distanceMin;

	[SerializeField]
	private float weaponDrawRaiseDownCoeff;

	private float weaponDraw_kP = 0.13f;

	private float weaponDrawBackTarg;

	private float weaponDrawRaiseTarg;

	private float weaponDrawBack;

	private float weaponDrawRaise;

	private float weaponDrawRaiseDown;

	private bool weaponADSObstruction;

	private bool weaponFireObstruction;

	[Header("Projectile stats")]
	[SerializeField]
	private float projectileSpeed;

	[SerializeField]
	private float projectileDamage;

	[SerializeField]
	private float projectileGravity;

	[SerializeField]
	private float deformationSize;

	[SerializeField]
	private float deformationChance;

	[SerializeField]
	private float penetration;

	[SerializeField]
	protected bool useLimbMultipliers;

	[SerializeField]
	protected float suppressStrength;

	[SerializeField]
	private GameObject reloadSound;

	private GameObject reticle;

	private GameObject baseHitmarker;

	private GameObject killHitmarker;

	[SerializeField]
	private GameObject sightAttachmentParent;

	[SerializeField]
	private GameObject barrelAttachmentParent;

	[SerializeField]
	private GameObject gripAttachmentParent;

	[SerializeField]
	private int sightAttachment;

	[SerializeField]
	private int barrelAttachment;

	[SerializeField]
	private int gripAttachment;

	protected WeaponSight activeSight;

	protected BarrelAttachment activeBarrel;

	private GripAttachment activeGrip;

	[Header("Item Body for ADS")]
	[SerializeField]
	private Vector3 adsPosition;

	[SerializeField]
	private Vector3 adsRotation;

	[SerializeField]
	private Vector3 adsPosition_firstPerson;

	[SerializeField]
	private Vector3 adsRotation_firstPerson;

	[SerializeField]
	private Vector3 crouchPosition_firstPerson;

	[SerializeField]
	private Vector3 crouchRotation_firstPerson;

	private float adsWeight;

	private float crouchWeight;

	[SerializeField]
	private float ads_kP;

	private static float crouch_kP = 0.1f;

	private UseParticle currentMuzzleFlash;

	private UseParticle currentCasingParticle;

	private PostProcessingManager postProcessingManager;

	private KillText killText;

	public bool AimedDownSights
	{
		get
		{
			return aimedDownSights;
		}
		set
		{
			aimedDownSights = value;
		}
	}

	public Projectile Projectile
	{
		get
		{
			return projectile;
		}
		set
		{
			projectile = value;
		}
	}

	public Transform BarrelPoint
	{
		get
		{
			return barrelPoint;
		}
		set
		{
			barrelPoint = value;
		}
	}

	public Vector3 AimRecoilVelocity
	{
		get
		{
			return aimRecoilVelocity;
		}
		set
		{
			aimRecoilVelocity = value;
		}
	}

	public bool Reloading
	{
		get
		{
			return reloading;
		}
		set
		{
			reloading = value;
		}
	}

	public bool Chambered
	{
		get
		{
			return chambered;
		}
		set
		{
			chambered = value;
		}
	}

	public float Cooldown
	{
		get
		{
			return cooldown;
		}
		set
		{
			cooldown = value;
		}
	}

	protected float ProjectileSpeed
	{
		get
		{
			return projectileSpeed;
		}
		set
		{
			projectileSpeed = value;
		}
	}

	protected float ProjectileDamage
	{
		get
		{
			return projectileDamage;
		}
		set
		{
			projectileDamage = value;
		}
	}

	protected float ProjectileGravity
	{
		get
		{
			return projectileGravity;
		}
		set
		{
			projectileGravity = value;
		}
	}

	protected Vector3 GunRecoilVelocity
	{
		get
		{
			return gunRecoilVelocity;
		}
		set
		{
			gunRecoilVelocity = value;
		}
	}

	protected Vector3 GunRecoil
	{
		get
		{
			return gunRecoil;
		}
		set
		{
			gunRecoil = value;
		}
	}

	public Vector3 AimRecoil
	{
		get
		{
			return aimRecoil;
		}
		set
		{
			aimRecoil = value;
		}
	}

	public GameObject UseAudio
	{
		get
		{
			return useAudio;
		}
		set
		{
			useAudio = value;
		}
	}

	public GameObject UseEffect
	{
		get
		{
			return useEffect;
		}
		set
		{
			useEffect = value;
		}
	}

	public int Ammo
	{
		get
		{
			return ammo;
		}
		set
		{
			ammo = value;
		}
	}

	public WeaponSight ActiveSight
	{
		get
		{
			return activeSight;
		}
		set
		{
			activeSight = value;
		}
	}

	public int MaxAmmo
	{
		get
		{
			return maxAmmo;
		}
		set
		{
			maxAmmo = value;
		}
	}

	public bool WeaponADSObstruction
	{
		get
		{
			return weaponADSObstruction;
		}
		set
		{
			weaponADSObstruction = value;
		}
	}

	public bool WeaponFireObstruction
	{
		get
		{
			return weaponFireObstruction;
		}
		set
		{
			weaponFireObstruction = value;
		}
	}

	public float SpreadAmountHip
	{
		get
		{
			return spreadAmountHip;
		}
		set
		{
			spreadAmountHip = value;
		}
	}

	public float SpreadAmountADS
	{
		get
		{
			return spreadAmountADS;
		}
		set
		{
			spreadAmountADS = value;
		}
	}

	public int NumberOfProjectiles
	{
		get
		{
			return numberOfProjectiles;
		}
		set
		{
			numberOfProjectiles = value;
		}
	}

	public bool RepetitiveReloading
	{
		get
		{
			return repetitiveReloading;
		}
		set
		{
			repetitiveReloading = value;
		}
	}

	public bool UseRepetitiveReload
	{
		get
		{
			return useRepetitiveReload;
		}
		set
		{
			useRepetitiveReload = value;
		}
	}

	public bool ReloadingFlag
	{
		get
		{
			return reloadingFlag;
		}
		set
		{
			reloadingFlag = value;
		}
	}

	public bool StopRepetitiveReload
	{
		get
		{
			return stopRepetitiveReload;
		}
		set
		{
			stopRepetitiveReload = value;
		}
	}

	public float DeformationSize
	{
		get
		{
			return deformationSize;
		}
		set
		{
			deformationSize = value;
		}
	}

	public float Penetration
	{
		get
		{
			return penetration;
		}
		set
		{
			penetration = value;
		}
	}

	public float DeformationChance
	{
		get
		{
			return deformationChance;
		}
		set
		{
			deformationChance = value;
		}
	}

	public AnimationCurve DamageDropOff
	{
		get
		{
			return damageDropOff;
		}
		set
		{
			damageDropOff = value;
		}
	}

	public AnimationCurve PenetrationDropOff
	{
		get
		{
			return penetrationDropOff;
		}
		set
		{
			penetrationDropOff = value;
		}
	}

	public GameObject SightAttachmentParent
	{
		get
		{
			return sightAttachmentParent;
		}
		set
		{
			sightAttachmentParent = value;
		}
	}

	public int SightAttachment
	{
		get
		{
			return sightAttachment;
		}
		set
		{
			sightAttachment = value;
		}
	}

	public float AdsMovementSpeedMultiplier
	{
		get
		{
			return adsMovementSpeedMultiplier;
		}
		set
		{
			adsMovementSpeedMultiplier = value;
		}
	}

	public int BarrelAttachment
	{
		get
		{
			return barrelAttachment;
		}
		set
		{
			barrelAttachment = value;
		}
	}

	public int GripAttachment
	{
		get
		{
			return gripAttachment;
		}
		set
		{
			gripAttachment = value;
		}
	}

	protected GripAttachment ActiveGrip
	{
		get
		{
			return activeGrip;
		}
		set
		{
			activeGrip = value;
		}
	}

	public GameObject GripAttachmentParent
	{
		get
		{
			return gripAttachmentParent;
		}
		set
		{
			gripAttachmentParent = value;
		}
	}

	public GameObject BarrelAttachmentParent
	{
		get
		{
			return barrelAttachmentParent;
		}
		set
		{
			barrelAttachmentParent = value;
		}
	}

	public int ReserveAmmo
	{
		get
		{
			return reserveAmmo;
		}
		set
		{
			reserveAmmo = value;
		}
	}

	public int StartingReserveAmmo
	{
		get
		{
			return startingReserveAmmo;
		}
		set
		{
			startingReserveAmmo = value;
		}
	}

	public float CurrentSpread
	{
		get
		{
			return currentSpread;
		}
		set
		{
			currentSpread = value;
		}
	}

	public override void Start()
	{
		base.Start();
		recoilPID_x = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
		recoilPID_y = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
		recoilPID_z = new PIDController(recoilPosRecovery_kP, recoilPosRecovery_kI, recoilPosRecovery_kD);
		recoilPID_x.Init();
		recoilPID_y.Init();
		recoilPID_z.Init();
		InitHitmarkers();
		InitReticle();
		postProcessingManager = Object.FindObjectOfType<PostProcessingManager>();
		killText = Object.FindObjectOfType<KillText>();
		adsWeight = 0f;
		aimedDownSights = false;
	}

	public void OnDestroy()
	{
		DestroyEffects();
		if ((bool)postProcessingManager)
		{
			postProcessingManager.FocusStrength = 0f;
		}
	}

	public void DestroyEffects()
	{
		if ((bool)currentMuzzleFlash)
		{
			Object.Destroy(currentMuzzleFlash.gameObject);
		}
		if ((bool)currentCasingParticle)
		{
			Object.Destroy(currentCasingParticle.gameObject);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateReticle();
		UpdatePostProcessBlur();
	}

	public void FixedUpdate()
	{
		UpdateRecoil();
		UpdateBulletSpread();
	}

	private void UpdatePostProcessBlur()
	{
		if ((bool)base.User && base.User.hasAuthority)
		{
			if (aimedDownSights && base.User.Health > 0f)
			{
				postProcessingManager.FocusStrength = GetSightAttachment().FocusWeight;
			}
			else
			{
				postProcessingManager.FocusStrength = 0f;
			}
		}
	}

	public void UpdateChamberStatus()
	{
		if (cooldownRemaining <= 0f)
		{
			if (!chambered && !reloading && ammo >= 1)
			{
				ChamberWeapon();
			}
			return;
		}
		cooldownRemaining -= Time.deltaTime;
		if (cooldownRemaining - Time.deltaTime < cooldownRemaining / coolDownRound_coeff)
		{
			cooldownRemaining = 0f;
		}
	}

	private void UpdateBulletSpread()
	{
		accumulatedHipSpread += (0f - accumulatedHipSpread) * hipSpreadRecovery_kP;
		accumulatedHipSpread = Mathf.Clamp(accumulatedHipSpread, 0f, hipSpreadMax);
		if (aimedDownSights)
		{
			CurrentSpread += (spreadAmountADS - CurrentSpread) * ads_kP;
		}
		else
		{
			CurrentSpread = spreadAmountHip + accumulatedHipSpread;
		}
	}

	private void InitReticle()
	{
		if ((bool)base.User && base.User.hasAuthority)
		{
			reticle = Object.Instantiate(Resources.Load("Reticle"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
			reticle.transform.SetParent(BarrelPoint.transform);
			reticle.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z);
		}
	}

	private void UpdateReticle()
	{
		if (!base.User || !base.User.hasAuthority || !reticle)
		{
			return;
		}
		if (!base.Sprinting && !aimedDownSights && !reloading)
		{
			if (!IsInvoking("ShowReticle"))
			{
				Invoke("ShowReticle", reticleAppearDelay);
			}
		}
		else
		{
			CancelInvoke("ShowReticle");
			reticle.SetActive(value: false);
		}
	}

	private void ShowReticle()
	{
		reticle.SetActive(value: true);
	}

	private void InitHitmarkers()
	{
		baseHitmarker = Object.Instantiate(Resources.Load("Hitmarker Base"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
		baseHitmarker.transform.SetParent(BarrelPoint.transform);
		baseHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z);
		killHitmarker = Object.Instantiate(Resources.Load("Hitmarker Kill"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
		killHitmarker.transform.SetParent(BarrelPoint.transform);
		killHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
	}

	private void UpdateHitmarkers()
	{
		RectTransform component = baseHitmarker.GetComponent<RectTransform>();
		component.eulerAngles = new Vector3(component.eulerAngles.x, component.eulerAngles.y, 0f);
		RectTransform component2 = killHitmarker.GetComponent<RectTransform>();
		component2.eulerAngles = new Vector3(component2.eulerAngles.x, component2.eulerAngles.y, 0f);
	}

	public override void UpdateTransforms()
	{
		if ((bool)GetComponentInParent<Human>())
		{
			base.User = GetComponentInParent<Human>();
		}
		if ((bool)base.User)
		{
			Vector3 vector;
			Vector3 vector2;
			if (base.User.UseThirdPersonAnimations)
			{
				vector = adsPosition;
				vector2 = adsRotation;
			}
			else
			{
				vector = adsPosition_firstPerson;
				vector2 = adsRotation_firstPerson;
			}
			if (aimedDownSights)
			{
				base.ItemSway *= aimSwayMultiplier;
				adsWeight += (1f - adsWeight) * Mathf.Min(ads_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
				crouchWeight += (0f - crouchWeight) * Mathf.Min(ads_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
			}
			else
			{
				if (base.User.Crouch)
				{
					crouchWeight += (1f - crouchWeight) * Mathf.Min(crouch_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
				}
				else
				{
					crouchWeight += (0f - crouchWeight) * Mathf.Min(crouch_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
				}
				adsWeight += (0f - adsWeight) * Mathf.Min(ads_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
			}
			if ((bool)ActiveSight)
			{
				base.PositionOffset = new Vector3(gunRecoilDisplacement.x * positionCoefficients.x + ActiveSight.PositionalOffset.x + base.ItemShift.x + adsWeight * vector.x + crouchWeight * crouchPosition_firstPerson.x, gunRecoilDisplacement.y * positionCoefficients.y + ActiveSight.PositionalOffset.y + base.ItemShift.y - weaponDrawRaise * weaponDrawRaiseDownCoeff + adsWeight * vector.y + crouchWeight * crouchPosition_firstPerson.y, gunRecoilDisplacement.z + ActiveSight.PositionalOffset.z + weaponDrawBack + adsWeight * vector.z + crouchWeight * crouchPosition_firstPerson.z);
			}
			else
			{
				base.PositionOffset = new Vector3(gunRecoilDisplacement.x * positionCoefficients.x + base.ItemShift.x + (adsWeight * vector.x + crouchWeight * crouchPosition_firstPerson.x), gunRecoilDisplacement.y * positionCoefficients.y + base.ItemShift.y - weaponDrawRaise * weaponDrawRaiseDownCoeff + adsWeight * vector.y + crouchWeight * crouchPosition_firstPerson.y, gunRecoilDisplacement.z + weaponDrawBack + adsWeight * vector.z + crouchWeight * crouchPosition_firstPerson.z);
			}
			base.RotationOffset = new Vector3(base.ItemSwing_y + gunRecoilDisplacement.y + base.ItemSway.x + weaponDrawRaise + adsWeight * vector2.x + crouchWeight * crouchRotation_firstPerson.x, adsWeight * vector2.y + crouchWeight * crouchRotation_firstPerson.y, base.ItemSwing_x + gunRecoilDisplacement.x + base.ItemSway.y + adsWeight * vector2.z + crouchWeight * crouchRotation_firstPerson.z);
			if (!aimedDownSights)
			{
				base.ItemSwing_x += (0f - base.ItemSwing_x) * Mathf.Min(base.ItemRecovery_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
				base.ItemSwing_y += (0f - base.ItemSwing_y) * Mathf.Min(base.ItemRecovery_kP * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
			}
			else
			{
				base.ItemSwing_x += (0f - base.ItemSwing_x) * Mathf.Min(itemRecovery_kP_ads * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
				base.ItemSwing_y += (0f - base.ItemSwing_y) * Mathf.Min(itemRecovery_kP_ads * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
			}
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			base.transform.Translate(base.PositionOffset, base.ForwardTransform);
			base.transform.localEulerAngles = ConvertVectorDirections(base.RotationOffset);
		}
		UpdateHitmarkers();
		UpdateRecoilDisplacement();
	}

	public void UpdateRecoil()
	{
		targetGunRecoilVelocity = new Vector3(recoilPID_x.GetOutput(0f - gunRecoilDisplacement.x), recoilPID_y.GetOutput(0f - gunRecoilDisplacement.y), recoilPID_z.GetOutput(0f - gunRecoilDisplacement.z));
		GunRecoilVelocity += new Vector3((targetGunRecoilVelocity.x - GunRecoilVelocity.x) * recoilVelocity_kP, (targetGunRecoilVelocity.y - GunRecoilVelocity.y) * recoilVelocity_kP, (targetGunRecoilVelocity.z - GunRecoilVelocity.z) * recoilVelocity_kP);
		AimRecoilVelocity += new Vector3((0f - AimRecoilVelocity.x) * recoilVelocity_kP, (0f - AimRecoilVelocity.y) * recoilVelocity_kP, (0f - AimRecoilVelocity.z) * recoilVelocity_kP);
	}

	private void UpdateRecoilDisplacement()
	{
		gunRecoilDisplacement += GunRecoilVelocity * Mathf.Min(Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, 1f);
	}

	public override bool PrimaryUseItem(bool input)
	{
		if (input && !automatic && fireFlag)
		{
			return false;
		}
		if (input)
		{
			fireFlag = true;
		}
		else
		{
			fireFlag = false;
		}
		if (input && !reloading && itemEnabled && !base.Sprinting && fireFlag && !automatic && !weaponFireObstruction)
		{
			shotQueued = true;
		}
		UpdateChamberStatus();
		if (input)
		{
			StopRepetitiveReload = true;
		}
		if (((shotQueued && !automatic) | (input && automatic)) && Chambered && !WeaponFireObstruction && !base.Sprinting && itemEnabled)
		{
			timeOfLastShot = Time.time;
			shotQueued = false;
			Shoot(CurrentSpread);
			cooldownRemaining = cooldown;
			Chambered = false;
			base.UseAnimationTrigger = true;
			return true;
		}
		return false;
	}

	public override void SecondaryUseItem(bool input)
	{
		if (!WeaponADSObstruction && !base.Sprinting)
		{
			AimedDownSights = input;
		}
		else
		{
			AimedDownSights = false;
		}
	}

	protected Vector3 GetActiveBarrelPos()
	{
		if ((bool)activeBarrel.ReplacementBarrelPoint)
		{
			return activeBarrel.ReplacementBarrelPoint.transform.position;
		}
		return barrelPoint.transform.position;
	}

	protected Quaternion GetActiveBarrelRot()
	{
		if ((bool)activeBarrel.ReplacementBarrelPoint)
		{
			return activeBarrel.ReplacementBarrelPoint.transform.rotation;
		}
		return barrelPoint.transform.rotation;
	}

	public void ApplyRecoil()
	{
		affectedGunRecoil = new Vector3(activeBarrel.GunRecoilMultiplier.x * activeSight.GunRecoilMultiplier.x * activeGrip.GunRecoilMultiplier.x * gunRecoil.x, activeBarrel.GunRecoilMultiplier.y * activeSight.GunRecoilMultiplier.y * activeGrip.GunRecoilMultiplier.y * gunRecoil.y, activeBarrel.GunRecoilMultiplier.z * activeSight.GunRecoilMultiplier.z * activeGrip.GunRecoilMultiplier.z * gunRecoil.z);
		affectedAimRecoil = new Vector3(activeBarrel.AimRecoilMultiplier.x * activeSight.AimRecoilMultiplier.x * activeGrip.AimRecoilMultiplier.x * aimRecoil.x, activeBarrel.AimRecoilMultiplier.y * activeSight.AimRecoilMultiplier.y * activeGrip.AimRecoilMultiplier.y * aimRecoil.y, activeBarrel.AimRecoilMultiplier.z * activeSight.AimRecoilMultiplier.z * activeGrip.AimRecoilMultiplier.z * aimRecoil.z);
		float num = 1f;
		if (base.User.Crouch)
		{
			num = crouchRecoilMultiplier;
		}
		GunRecoilVelocity += gunRecoilOffsets + new Vector3(Random.Range(0f - affectedGunRecoil.x, affectedGunRecoil.x) * num, affectedGunRecoil.y * num, affectedGunRecoil.z);
		AimRecoilVelocity += new Vector3(Random.Range(0f - affectedAimRecoil.x, affectedAimRecoil.x) * num, affectedAimRecoil.y * num, Random.Range(0f - affectedAimRecoil.z, affectedAimRecoil.z) * num);
	}

	public virtual void Shoot(float spreadAmount)
	{
		accumulatedHipSpread += hipSpreadIncreasePerShot;
		Vector3 activeBarrelPos = GetActiveBarrelPos();
		Quaternion activeBarrelRot = GetActiveBarrelRot();
		base.User.ReplicatePrimaryUseItem();
		for (int i = 0; i < NumberOfProjectiles; i++)
		{
			Projectile projectile = (projectile = Object.Instantiate(Projectile, activeBarrelPos, activeBarrelRot));
			projectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers, suppressStrength);
			projectile.Item = this;
			projectile.transform.Rotate(new Vector3(Random.Range((0f - spreadAmount) / 2f, spreadAmount / 2f), Random.Range((0f - spreadAmount) / 2f, spreadAmount / 2f), 0f));
			if ((bool)GameObject.Find("Temp"))
			{
				projectile.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		ApplyRecoil();
		if ((bool)UseAudio)
		{
			GameObject gameObject = ((!activeBarrel.ReplacementUseAudio) ? Object.Instantiate(UseAudio, activeBarrelPos, activeBarrelRot) : Object.Instantiate(activeBarrel.ReplacementUseAudio, activeBarrelPos, activeBarrelRot));
			if ((bool)GameObject.Find("Temp"))
			{
				gameObject.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		if ((bool)UseEffect)
		{
			ProduceMuzzleFlash();
		}
		DepleteAmmo(1);
		ProduceCasing();
	}

	public void ProduceMuzzleFlash()
	{
		if (currentMuzzleFlash == null)
		{
			Vector3 activeBarrelPos = GetActiveBarrelPos();
			Quaternion activeBarrelRot = GetActiveBarrelRot();
			GameObject gameObject = ((!activeBarrel.ReplacementMuzzleFlash) ? Object.Instantiate(UseEffect, activeBarrelPos, activeBarrelRot) : Object.Instantiate(activeBarrel.ReplacementMuzzleFlash, activeBarrelPos, activeBarrelRot));
			gameObject.transform.parent = GameObject.Find("Temp").transform;
			if ((bool)gameObject.GetComponent<UseParticle>())
			{
				currentMuzzleFlash = gameObject.GetComponent<UseParticle>();
			}
		}
		else
		{
			Vector3 activeBarrelPos2 = GetActiveBarrelPos();
			Quaternion activeBarrelRot2 = GetActiveBarrelRot();
			currentMuzzleFlash.transform.position = activeBarrelPos2;
			currentMuzzleFlash.transform.rotation = activeBarrelRot2;
			currentMuzzleFlash.Play();
		}
	}

	public virtual void Shoot()
	{
		Vector3 activeBarrelPos = GetActiveBarrelPos();
		Quaternion activeBarrelRot = GetActiveBarrelRot();
		base.User.ReplicatePrimaryUseItem();
		for (int i = 0; i < NumberOfProjectiles; i++)
		{
			Projectile projectile = Object.Instantiate(Projectile, activeBarrelPos, activeBarrelRot);
			projectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers, suppressStrength);
			projectile.Item = this;
			if ((bool)GameObject.Find("Temp"))
			{
				projectile.transform.parent = GameObject.Find("Temp").transform;
			}
		}
		ApplyRecoil();
		if ((bool)UseAudio)
		{
			GameObject gameObject = ((!activeBarrel.ReplacementUseAudio) ? Object.Instantiate(UseAudio, activeBarrelPos, activeBarrelRot) : Object.Instantiate(activeBarrel.ReplacementUseAudio, activeBarrelPos, activeBarrelRot));
			gameObject.transform.parent = BarrelPoint.transform;
		}
		if ((bool)UseEffect)
		{
			ProduceMuzzleFlash();
		}
		DepleteAmmo(1);
		ProduceCasing();
	}

	protected void ProduceCasing()
	{
		if (!casing || !(Vector3.Distance(casingPoint.position, GetActiveCamera().transform.position) < Casing.maxRenderDistance))
		{
			return;
		}
		if (currentCasingParticle == null)
		{
			GameObject gameObject = Object.Instantiate(casing, casingPoint.transform.position, casingPoint.transform.rotation);
			gameObject.transform.parent = GameObject.Find("Temp").transform;
			if ((bool)gameObject.GetComponent<UseParticle>())
			{
				currentCasingParticle = gameObject.GetComponent<UseParticle>();
			}
		}
		else
		{
			Vector3 position = casingPoint.transform.position;
			Quaternion rotation = casingPoint.transform.rotation;
			currentCasingParticle.transform.position = position;
			currentCasingParticle.transform.rotation = rotation;
			currentCasingParticle.Play();
		}
	}

	private Camera GetActiveCamera()
	{
		return Object.FindObjectOfType<Camera>();
	}

	public virtual void ChamberWeapon()
	{
		if (Ammo > 0 && !Reloading)
		{
			Chambered = true;
		}
	}

	public void DepleteAmmo(int amount)
	{
		Ammo -= amount;
	}

	public virtual void CallReload()
	{
		shotQueued = false;
		if (ReserveAmmo <= 0)
		{
			return;
		}
		if (!UseRepetitiveReload)
		{
			if (!Reloading)
			{
				StopRepetitiveReload = false;
				reloadingFlag = false;
				Chambered = false;
				Reloading = true;
				Invoke("CompleteReload", reloadTime);
				if ((bool)reloadSound)
				{
					Object.Instantiate(reloadSound, barrelPoint.transform.position, base.transform.rotation).transform.SetParent(base.transform);
				}
			}
		}
		else if (!repetitiveReloading)
		{
			StopRepetitiveReload = false;
			Reloading = true;
			Chambered = false;
			RepetitiveReloading = true;
			Invoke("RepetitiveReload", reloadTime);
			if ((bool)reloadSound)
			{
				Object.Instantiate(reloadSound, barrelPoint.transform.position, base.transform.rotation).transform.SetParent(base.transform);
			}
		}
	}

	public bool CanReload()
	{
		return ammo < maxAmmo;
	}

	public void CompleteReload()
	{
		if (ReserveAmmo >= maxAmmo - ammo)
		{
			int num = maxAmmo - ammo;
			ReserveAmmo -= num;
			ammo = MaxAmmo;
		}
		else
		{
			ammo = ReserveAmmo + ammo;
			ReserveAmmo = 0;
		}
		Reloading = false;
		Chambered = true;
	}

	public void RepetitiveReload()
	{
		ammo++;
		reserveAmmo--;
		if ((ammo >= maxAmmo) | StopRepetitiveReload | (reserveAmmo <= 0))
		{
			RepetitiveReloading = false;
			Reloading = false;
			chambered = true;
			StopRepetitiveReload = false;
			return;
		}
		RepetitiveReloading = true;
		Invoke("RepetitiveReload", reloadTime);
		if ((bool)reloadSound)
		{
			Object.Instantiate(reloadSound, base.transform.position, base.transform.rotation).transform.SetParent(base.transform);
		}
	}

	public override void HitAnotherTarget(bool killShot, Human target)
	{
		if (base.User.hasAuthority && base.User is Player && !base.User.PlayersKilled.Contains(target))
		{
			(base.User as Player).HitMarkerSound(killShot);
		}
		if (killShot)
		{
			if (target.Team != base.User.Team)
			{
				if (!base.User.PlayersKilled.Contains(target))
				{
					if ((bool)killText)
					{
						killText.CreateNewKillText(target.HumanName);
					}
					base.User.AddKill();
					base.User.AddScore(100);
					base.User.PlayersKilled.Add(target);
				}
				killHitmarker.GetComponent<Hitmarker>().HitAnimation();
			}
			else
			{
				base.User.AddScore(-100);
			}
		}
		else if (target.Team != base.User.Team)
		{
			baseHitmarker.GetComponent<Hitmarker>().HitAnimation();
		}
	}

	public void SetActiveAttachments()
	{
		DestroyEffects();
		if ((bool)SightAttachmentParent)
		{
			WeaponSight[] componentsInChildren = SightAttachmentParent.GetComponentsInChildren<WeaponSight>();
			foreach (WeaponSight weaponSight in componentsInChildren)
			{
				if (weaponSight.gameObject.activeSelf)
				{
					ActiveSight = weaponSight;
				}
			}
		}
		if ((bool)BarrelAttachmentParent)
		{
			BarrelAttachment[] componentsInChildren2 = BarrelAttachmentParent.GetComponentsInChildren<BarrelAttachment>();
			foreach (BarrelAttachment barrelAttachment in componentsInChildren2)
			{
				if (barrelAttachment.gameObject.activeSelf)
				{
					activeBarrel = barrelAttachment;
				}
			}
		}
		if (!GripAttachmentParent)
		{
			return;
		}
		GripAttachment[] componentsInChildren3 = GripAttachmentParent.GetComponentsInChildren<GripAttachment>();
		foreach (GripAttachment gripAttachment in componentsInChildren3)
		{
			if (gripAttachment.gameObject.activeSelf)
			{
				activeGrip = gripAttachment;
			}
		}
	}

	public void SetAsLocalPlayer()
	{
		SetActiveAttachments();
		if ((bool)activeSight)
		{
			ActiveSight.EnableCamera();
		}
		else
		{
			ActiveSight.DisableCamera();
		}
	}

	public override void UpdateItemShift(float x, float y)
	{
		if (!aimedDownSights)
		{
			base.TargItemShift = new Vector3(Mathf.Clamp(x * PlayerItem.ItemShiftAmount_x, 0f - PlayerItem.ItemShiftCap, PlayerItem.ItemShiftCap), Mathf.Clamp(y * PlayerItem.ItemShiftAmount_y, 0f - PlayerItem.ItemShiftCap, PlayerItem.ItemShiftCap), 0f);
		}
		else
		{
			base.TargItemShift = new Vector3(Mathf.Clamp(x * PlayerItem.ItemShiftAmount_x * aimShiftMultiplier, 0f - PlayerItem.ItemShiftCap, PlayerItem.ItemShiftCap), Mathf.Clamp(y * PlayerItem.ItemShiftAmount_y * aimShiftMultiplier, 0f - PlayerItem.ItemShiftCap, PlayerItem.ItemShiftCap), 0f);
		}
		base.ItemShift += new Vector3((base.TargItemShift.x - base.ItemShift.x) * PlayerItem.ItemShift_kP, (base.TargItemShift.y - base.ItemShift.y) * PlayerItem.ItemShift_kP, 0f);
	}

	public void SetSightAttachment(int sight)
	{
		sightAttachment = sight;
		if ((bool)SightAttachmentParent)
		{
			for (int i = 0; i < SightAttachmentParent.transform.childCount; i++)
			{
				WeaponSight component = SightAttachmentParent.transform.GetChild(i).GetComponent<WeaponSight>();
				if (i == sightAttachment)
				{
					component.gameObject.SetActive(value: true);
				}
				else
				{
					component.gameObject.SetActive(value: false);
				}
			}
		}
		SetActiveAttachments();
	}

	public WeaponSight GetSightAttachment()
	{
		if ((bool)SightAttachmentParent)
		{
			for (int i = 0; i < SightAttachmentParent.transform.childCount; i++)
			{
				WeaponSight component = SightAttachmentParent.transform.GetChild(i).GetComponent<WeaponSight>();
				if (i == sightAttachment)
				{
					return component;
				}
			}
		}
		return null;
	}

	public void SetBarrelAttachment(int barrel)
	{
		BarrelAttachment = barrel;
		if ((bool)BarrelAttachmentParent)
		{
			for (int i = 0; i < BarrelAttachmentParent.transform.childCount; i++)
			{
				BarrelAttachment component = BarrelAttachmentParent.transform.GetChild(i).GetComponent<BarrelAttachment>();
				if (i == BarrelAttachment)
				{
					component.gameObject.SetActive(value: true);
				}
				else
				{
					component.gameObject.SetActive(value: false);
				}
			}
		}
		SetActiveAttachments();
	}

	public BarrelAttachment GetBarrelAttachment()
	{
		if ((bool)BarrelAttachmentParent)
		{
			for (int i = 0; i < BarrelAttachmentParent.transform.childCount; i++)
			{
				BarrelAttachment component = BarrelAttachmentParent.transform.GetChild(i).GetComponent<BarrelAttachment>();
				if (i == barrelAttachment)
				{
					return component;
				}
			}
		}
		return null;
	}

	public void SetGripAttachment(int grip)
	{
		gripAttachment = grip;
		if ((bool)BarrelAttachmentParent)
		{
			for (int i = 0; i < GripAttachmentParent.transform.childCount; i++)
			{
				GripAttachment component = GripAttachmentParent.transform.GetChild(i).GetComponent<GripAttachment>();
				if (i == gripAttachment)
				{
					component.gameObject.SetActive(value: true);
				}
				else
				{
					component.gameObject.SetActive(value: false);
				}
			}
		}
		SetActiveAttachments();
	}

	public GripAttachment GetGripAttachment()
	{
		if ((bool)GripAttachmentParent)
		{
			for (int i = 0; i < GripAttachmentParent.transform.childCount; i++)
			{
				GripAttachment component = GripAttachmentParent.transform.GetChild(i).GetComponent<GripAttachment>();
				if (i == gripAttachment)
				{
					return component;
				}
			}
		}
		return null;
	}

	public void SetWeaponDraw(float distance)
	{
		float num = weaponDrawLength;
		if ((bool)activeBarrel)
		{
			num += activeBarrel.Length;
		}
		float num2 = distance;
		if (num2 < distanceMin)
		{
			num2 = distanceMin;
		}
		if (num2 < num && !base.Sprinting)
		{
			WeaponADSObstruction = true;
			weaponDrawBackTarg = 0f - (num - num2);
			if (num2 < weaponRaiseStart)
			{
				weaponDrawRaiseTarg = (0f - (weaponRaiseStart - num2)) * weaponRaiseCoefficient;
				WeaponFireObstruction = true;
			}
			else
			{
				WeaponFireObstruction = false;
			}
		}
		else
		{
			weaponDrawBackTarg = 0f;
			weaponDrawRaiseTarg = 0f;
			WeaponADSObstruction = false;
			WeaponFireObstruction = false;
		}
		weaponDrawBack += (weaponDrawBackTarg - weaponDrawBack) * weaponDraw_kP;
		weaponDrawRaise += (weaponDrawRaiseTarg - weaponDrawRaise) * weaponDraw_kP;
	}

	public void ResupplyWeapon()
	{
		if (reserveAmmo <= resupplyReserveAmmo)
		{
			reserveAmmo = resupplyReserveAmmo;
		}
	}
}
