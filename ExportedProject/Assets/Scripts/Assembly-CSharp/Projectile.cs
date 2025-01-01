using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
	private static float CRITICAL_HIT_DAMAGE_THRESHOLD = 2f;

	[SerializeField]
	private GameObject remainingTracer;

	private static int playerMask = 4096;

	private static int characterMask = 64;

	private static int barrierMask = 1024;

	private static int ignoreRaycast = 4;

	private static int interactable = 16384;

	private static int projectile = 524288;

	private static float limbPenDamageLoss = 0.3f;

	private bool serverInstance;

	private float baseSpeed;

	private float baseDamage;

	private float gravity;

	private bool active;

	private Vector3 velocity;

	private static float playerHeightOffset = 1.5f;

	private static float minRangeToSuppress = 7f;

	[SerializeField]
	private float projectileForce;

	private PlayerFirearm item;

	[SerializeField]
	private GameObject deformerBrush;

	private bool queueDestroy;

	private bool canInteract = true;

	private float deformationSize;

	private float penetration;

	private float deformationChance;

	private float distanceTravelled;

	[SerializeField]
	private float suppressionRange;

	[SerializeField]
	private float suppressionIntensity;

	[SerializeField]
	private GameObject[] flybySounds;

	[SerializeField]
	private GameObject[] cementImpactSounds;

	private bool hasSuppressed;

	private AnimationCurve penetrationDropOff;

	private AnimationCurve damageDropOff;

	private HardlineNetworkManager networkHandler;

	private bool useLimbMultipliers;

	private HardlineGameManager gameManager;

	public PlayerFirearm Item
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

	public void SetProjectileStats(float baseSpeed, float baseDamage, float gravity, float deformationSize, float penetration, float deformationChance, AnimationCurve damageDropOff, AnimationCurve penetrationDropOff, bool useLimbMultipliers, float suppressStrength)
	{
		this.baseSpeed = baseSpeed;
		this.baseDamage = baseDamage;
		this.gravity = gravity;
		this.deformationSize = deformationSize;
		this.penetration = penetration;
		this.deformationChance = deformationChance;
		this.penetrationDropOff = penetrationDropOff;
		this.damageDropOff = damageDropOff;
		this.useLimbMultipliers = useLimbMultipliers;
		suppressionIntensity = suppressStrength;
	}

	private void Start()
	{
		gameManager = Object.FindObjectOfType<HardlineGameManager>();
		networkHandler = Object.FindObjectOfType<HardlineNetworkManager>();
		Object.Destroy(base.gameObject, 3f);
		velocity = base.transform.forward * baseSpeed;
		GetComponent<SphereCollider>().radius = suppressionRange;
	}

	private void Update()
	{
		int layerMask = ~(playerMask | characterMask | barrierMask | interactable | ignoreRaycast | projectile);
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward), out var hitInfo, velocity.magnitude * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, layerMask))
		{
			if ((bool)remainingTracer)
			{
				remainingTracer.transform.position = hitInfo.point;
				remainingTracer.transform.parent = null;
			}
			distanceTravelled += Vector3.Distance(base.transform.position, hitInfo.point);
			if (hitInfo.transform.gameObject.layer == 7)
			{
				if (item.User.hasAuthority || (item.User is EnemyAI && gameManager.isServer))
				{
					RaycastHit[] array = Physics.RaycastAll(base.transform.position, base.transform.TransformDirection(Vector3.forward), velocity.magnitude * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, layerMask);
					bool flag = false;
					float num = 1f;
					float num2 = 0f;
					for (int i = 0; i < array.Length; i++)
					{
						RaycastHit raycastHit = array[i];
						if ((bool)raycastHit.transform.GetComponent<Hitbox>())
						{
							if (raycastHit.transform.GetComponent<Hitbox>().DamageMultiplier > CRITICAL_HIT_DAMAGE_THRESHOLD)
							{
								flag = true;
							}
							num2 = ((damageDropOff == null) ? (num2 + baseDamage * num) : ((!useLimbMultipliers) ? (num2 + baseDamage * damageDropOff.Evaluate(distanceTravelled)) : (num2 + raycastHit.transform.GetComponent<Hitbox>().getDamage(baseDamage * damageDropOff.Evaluate(distanceTravelled)) * num)));
							num *= limbPenDamageLoss;
						}
					}
					if (flag)
					{
						Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("HeadshotBloodParticles", hitInfo.point, -Vector3.right * 90f, item.User);
					}
					else
					{
						Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("BloodParticles", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
					}
					gameManager.CallHitAnotherPlayer(item.User, hitInfo.transform.GetComponent<Hitbox>().Player, hitInfo.point, hitInfo.normal, num2);
				}
				queueDestroy = true;
			}
			else if (hitInfo.transform.tag == "Stone")
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("StoneImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
				queueDestroy = true;
			}
			else if ((hitInfo.transform.tag == "Dirt") | (hitInfo.transform.tag == "Terrain"))
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("DirtImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
				queueDestroy = true;
			}
			else if (hitInfo.transform.tag == "Wood")
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("WoodImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
				queueDestroy = true;
			}
			else if (hitInfo.transform.tag == "Doors")
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("WoodImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
			}
			else if (hitInfo.transform.tag == "Metal")
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("MetalImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
				queueDestroy = true;
			}
			else if (hitInfo.transform.tag == "RagdollParts")
			{
				if (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer))
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("BloodParticles", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
					hitInfo.transform.GetComponent<Rigidbody>().AddForce(base.transform.forward * projectileForce);
				}
				queueDestroy = true;
			}
			else if (hitInfo.transform.tag == "ShatterableGlass")
			{
				MultiplayerGlassInstance componentInParent = hitInfo.transform.GetComponentInParent<MultiplayerGlassInstance>();
				if (!componentInParent.Broken)
				{
					componentInParent.ReplicateDestroy(hitInfo.point, base.transform.forward);
				}
			}
			else
			{
				if ((item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer)) && hitInfo.transform.tag != "Deformable")
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("StoneImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), item.User);
				}
				queueDestroy = true;
			}
			if ((bool)hitInfo.transform.GetComponent<GeneratedMesh>() && canInteract && (item.User.hasAuthority | (item.User is EnemyAI && gameManager.isServer)))
			{
				canInteract = false;
				if (deformationSize > 0f && Random.Range(0f, 1f) < deformationChance)
				{
					if (penetrationDropOff != null)
					{
						hitInfo.transform.GetComponent<GeneratedMesh>().AddDeformation(hitInfo.point, deformationSize, penetration * penetrationDropOff.Evaluate(distanceTravelled));
					}
					else
					{
						hitInfo.transform.GetComponent<GeneratedMesh>().AddDeformation(hitInfo.point, deformationSize, penetration);
					}
				}
			}
			if (queueDestroy)
			{
				DestroyProjectile();
			}
		}
		Vector3 position = base.transform.position;
		velocity += new Vector3(0f, 0f - gravity, 0f) * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant;
		base.transform.position += velocity * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant;
		distanceTravelled += Vector3.Distance(position, base.transform.position);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (distanceTravelled <= minRangeToSuppress || !other.GetComponent<Human>())
		{
			return;
		}
		Human component = other.GetComponent<Human>();
		if (!hasSuppressed)
		{
			int layerMask = ~(playerMask | characterMask);
			if ((component.hasAuthority || (component is EnemyAI && gameManager.isServer)) && component != item.User && Physics.Raycast(base.transform.position, (component.transform.position + new Vector3(0f, playerHeightOffset, 0f) - base.transform.position).normalized, out var hitInfo, suppressionRange, layerMask) && (bool)hitInfo.transform.GetComponent<Hitbox>())
			{
				component.Suppress(suppressionIntensity);
				hasSuppressed = true;
				Object.Instantiate(flybySounds[Random.Range(0, flybySounds.Length)], base.transform.position, base.transform.rotation);
			}
		}
	}

	public void CementImpactSound(Vector3 hitPoint)
	{
		Object.Instantiate(cementImpactSounds[Random.Range(0, cementImpactSounds.Length)], hitPoint, base.transform.rotation);
	}

	public void DestroyProjectile()
	{
		Object.Destroy(base.gameObject);
	}

	private void MirrorProcessed()
	{
	}
}
