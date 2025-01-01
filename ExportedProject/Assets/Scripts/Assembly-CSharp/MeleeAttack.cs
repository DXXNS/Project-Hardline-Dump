using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
	private static int playerMask = 4096;

	private static int characterMask = 64;

	private static int barrierMask = 1024;

	private static int ignoreRaycast = 4;

	private static int interactable = 16384;

	private static int projectile = 524288;

	private float baseDamage;

	private float baseRange;

	[SerializeField]
	private GameObject bloodImpactParticle;

	[SerializeField]
	private GameObject stoneImpactParticles;

	private HardlineNetworkManager networkHandler;

	private HardlineGameManager gameManager;

	private MeleeWeapon item;

	public MeleeWeapon Item
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

	public void SetAttackStats(float damage, float range)
	{
		baseDamage = damage;
		baseRange = range;
	}

	public void CheckDamage()
	{
		gameManager = Object.FindObjectOfType<HardlineGameManager>();
		networkHandler = Object.FindObjectOfType<HardlineNetworkManager>();
		int layerMask = ~(playerMask | characterMask | barrierMask | interactable | ignoreRaycast | projectile);
		if (Physics.Raycast(base.transform.position, base.transform.forward, out var hitInfo, baseRange, layerMask))
		{
			if (hitInfo.transform.gameObject.layer == 7)
			{
				if (Item.User.hasAuthority)
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("BloodParticles", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), Item.User);
					Physics.RaycastAll(base.transform.position, base.transform.TransformDirection(Vector3.forward), baseRange * Time.deltaTime * HardlineGameManager.DeltaTimeFrameSpeedConstant, layerMask);
					float damage = hitInfo.transform.GetComponent<Hitbox>().getDamage(baseDamage);
					gameManager.CallHitAnotherPlayer(Item.User, hitInfo.transform.GetComponent<Hitbox>().Player, hitInfo.point, hitInfo.normal, damage);
				}
			}
			else if (hitInfo.transform.tag == "Stone")
			{
				if (Item.User.hasAuthority)
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("StoneImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), Item.User);
				}
			}
			else if (hitInfo.transform.tag == "RagdollParts")
			{
				if (Item.User.hasAuthority)
				{
					Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("BloodParticles", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), Item.User);
				}
			}
			else if (hitInfo.transform.tag == "ShatterableGlass")
			{
				hitInfo.transform.GetComponentInParent<MultiplayerGlassInstance>().ReplicateDestroy(hitInfo.point, base.transform.forward);
			}
			else if (Item.User.hasAuthority && hitInfo.transform.tag != "Deformable")
			{
				Object.FindObjectOfType<HardlineGameManager>().CallServerSpawnObject("StoneImpact", hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), Item.User);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
