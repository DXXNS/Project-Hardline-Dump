using UnityEngine;

public class Ragdoll : MonoBehaviour
{
	private static float RAGDOLL_VELOCITY_MULTIPLIER = 1.2f;

	private static float INIT_PHASE_LENGTH = 0.2f;

	private bool pastInitPhase;

	[SerializeField]
	private GameObject[] RagdollParts;

	[SerializeField]
	private GameObject camera;

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
	protected GameObject shirt;

	[SerializeField]
	protected GameObject pants;

	[SerializeField]
	protected GameObject headwear;

	[SerializeField]
	protected GameObject vest;

	[SerializeField]
	protected GameObject hood;

	private ClothesLibrary clothesLibrary;

	[SerializeField]
	private Transform armatureToAssign;

	[SerializeField]
	private GameObject clothesLibraryToSpawn;

	public GameObject[] RagdollPartsGet => RagdollParts;

	public bool PastInitPhase
	{
		get
		{
			return pastInitPhase;
		}
		set
		{
			pastInitPhase = value;
		}
	}

	public void SetCameraState(bool enabled)
	{
		camera.SetActive(enabled);
	}

	private void Start()
	{
		Invoke("EndInitPhase", INIT_PHASE_LENGTH);
	}

	public void EndInitPhase()
	{
		pastInitPhase = true;
	}

	public void SetEquipment(string shirtName, string pantsName, string headwearName, string vestName, string hoodName)
	{
		if ((bool)shirtParent && (bool)pantsParent && (bool)headwearParent && (bool)vestsParent && (bool)hoodsParent)
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
			SetAllActive(GetChildWithName(shirtParent, shirtName), enabled: true, self: true);
			SetAllActive(GetChildWithName(pantsParent, pantsName), enabled: true, self: true);
			SetAllActive(GetChildWithName(headwearParent, headwearName), enabled: true, self: true);
			SetAllActive(GetChildWithName(vestsParent, vestName), enabled: true, self: true);
			SetAllActive(GetChildWithName(hoodsParent, hoodName), enabled: true, self: true);
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

	public void ApplyVelocityEven(Vector3 velocity)
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].velocity = velocity * RAGDOLL_VELOCITY_MULTIPLIER;
		}
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
}
