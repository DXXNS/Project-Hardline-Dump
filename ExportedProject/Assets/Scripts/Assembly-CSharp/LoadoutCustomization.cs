using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutCustomization : MonoBehaviour
{
	private float attachment_initialGap = 150f;

	private float attachment_contentScalePerItem = 150f;

	private float attachment_selectTabPadding = 150f;

	private float item_initialGap = 20f;

	private float item_contentScalePerItem = 110f;

	private float item_selectTabPadding = -110f;

	[SerializeField]
	private Vector3 entryBasePosition;

	[SerializeField]
	private Animator loadoutAnimator;

	[SerializeField]
	private Animator canvasAnimator;

	private int itemSelected;

	private PlayerItem primary;

	private PlayerItem secondary;

	private PlayerItem equipment1;

	private PlayerItem equipment2;

	private PlayerItem meleeWeapon;

	[SerializeField]
	private PlayerItem[] allowedPrimaries;

	[SerializeField]
	private PlayerItem[] allowedSecondaries;

	[SerializeField]
	private PlayerItem[] allowedEquipment1;

	[SerializeField]
	private PlayerItem[] allowedEquipment2;

	[SerializeField]
	private PlayerItem[] allowedMeleeWeapons;

	[SerializeField]
	private GameObject itemParent;

	private GameObject customizeShowcase;

	private GameObject primaryShowcase;

	private GameObject secondaryShowcase;

	private GameObject equipment1Showcase;

	private GameObject equipment2Showcase;

	private GameObject meleeWeaponShowcase;

	[SerializeField]
	private GameObject customizationShowcaseParent;

	[SerializeField]
	private GameObject primaryShowcaseParent;

	[SerializeField]
	private GameObject secondaryShowcaseParent;

	[SerializeField]
	private GameObject equipment1ShowcaseParent;

	[SerializeField]
	private GameObject equipment2ShowcaseParent;

	[SerializeField]
	private GameObject meleeWeaponShowcaseParent;

	[SerializeField]
	private GameObject primarySelectionScroll;

	[SerializeField]
	private GameObject secondarySelectionScroll;

	[SerializeField]
	private GameObject equipment1SelectionScroll;

	[SerializeField]
	private GameObject equipment2SelectionScroll;

	[SerializeField]
	private GameObject attachmentScroll;

	[SerializeField]
	private GameObject attachmentEntrySample;

	[SerializeField]
	private List<GameObject> attachments = new List<GameObject>();

	[SerializeField]
	private GameObject itemEntrySample;

	[SerializeField]
	private List<GameObject> items = new List<GameObject>();

	[SerializeField]
	private GameObject attachmentContent;

	[SerializeField]
	private GameObject itemContent;

	private LoadoutSelectable loadoutUI;

	private bool attachmentCustomizationOpened;

	private int selectedLoadout;

	[SerializeField]
	private GameObject itemDescriptionPanel;

	[SerializeField]
	private Text itemDescriptionTitle;

	[SerializeField]
	private Text itemDescription;

	private void Start()
	{
		loadoutUI = Object.FindObjectOfType<LoadoutSelectable>();
		LoadLoadout(1);
	}

	public void LoadLoadout(int loadout)
	{
		itemSelected = 0;
		selectedLoadout = loadout;
		FirearmEntry firearmEntry = GetFirearmEntry(loadout, 1);
		FirearmEntry firearmEntry2 = GetFirearmEntry(loadout, 2);
		string @string = PlayerPrefs.GetString("loadout_" + loadout + "_equipment1", "FragGrenade");
		string string2 = PlayerPrefs.GetString("loadout_" + loadout + "_equipment2", "StimPen");
		string string3 = PlayerPrefs.GetString("loadout_" + loadout + "_meleeWeapon", "Knife");
		SpawnFirearm(1, firearmEntry.ItemName, firearmEntry.Sight, firearmEntry.Barrel, firearmEntry.Grip);
		SpawnFirearm(2, firearmEntry2.ItemName, firearmEntry2.Sight, firearmEntry2.Barrel, firearmEntry2.Grip);
		SpawnItem(3, @string);
		SpawnItem(4, string2);
		SpawnItem(5, string3);
		itemDescriptionPanel.SetActive(value: false);
	}

	public FirearmEntry GetFirearmEntry(int loadout, int slot)
	{
		FirearmEntry firearmEntry = new FirearmEntry("Unarmed");
		switch (slot)
		{
		case 1:
			firearmEntry = new FirearmEntry(PlayerPrefs.GetString("loadout_" + loadout + "_primary", "M416"));
			firearmEntry.Sight = PlayerPrefs.GetInt("loadout_" + loadout + "_primary_sight", 0);
			firearmEntry.Barrel = PlayerPrefs.GetInt("loadout_" + loadout + "_primary_barrel", 0);
			firearmEntry.Grip = PlayerPrefs.GetInt("loadout_" + loadout + "_primary_grip", 0);
			break;
		case 2:
			firearmEntry = new FirearmEntry(PlayerPrefs.GetString("loadout_" + loadout + "_secondary", "Glock"));
			firearmEntry.Sight = PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_sight", 0);
			firearmEntry.Barrel = PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_barrel", 0);
			firearmEntry.Grip = PlayerPrefs.GetInt("loadout_" + loadout + "_secondary_grip", 0);
			break;
		}
		return firearmEntry;
	}

	public void SaveLoadouts()
	{
		PlayerPrefs.SetString("loadout_" + selectedLoadout + "_primary", primary.ItemName);
		PlayerPrefs.SetString("loadout_" + selectedLoadout + "_secondary", secondary.ItemName);
		PlayerPrefs.SetString("loadout_" + selectedLoadout + "_equipment1", equipment1.ItemName);
		PlayerPrefs.SetString("loadout_" + selectedLoadout + "_equipment2", equipment2.ItemName);
		PlayerPrefs.SetString("loadout_" + selectedLoadout + "_meleeWeapon", meleeWeapon.ItemName);
		if (primary is PlayerFirearm)
		{
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_primary_sight", (primary as PlayerFirearm).SightAttachment);
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_primary_barrel", (primary as PlayerFirearm).BarrelAttachment);
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_primary_grip", (primary as PlayerFirearm).GripAttachment);
		}
		if (secondary is PlayerFirearm)
		{
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_secondary_sight", (secondary as PlayerFirearm).SightAttachment);
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_secondary_barrel", (secondary as PlayerFirearm).BarrelAttachment);
			PlayerPrefs.SetInt("loadout_" + selectedLoadout + "_secondary_grip", (secondary as PlayerFirearm).GripAttachment);
		}
	}

	private void Update()
	{
		UpdateUI();
		UpdateShowcaseModels();
		SaveLoadouts();
	}

	private void UpdateShowcaseModels()
	{
		if ((bool)primaryShowcase && (bool)primary && (bool)primary.GetComponent<PlayerFirearm>())
		{
			if (itemSelected == 1)
			{
				customizeShowcase.GetComponent<PickupItem>().SightAttachment = primary.GetComponent<PlayerFirearm>().SightAttachment;
				customizeShowcase.GetComponent<PickupItem>().GripAttachment = primary.GetComponent<PlayerFirearm>().GripAttachment;
				customizeShowcase.GetComponent<PickupItem>().BarrelAttachment = primary.GetComponent<PlayerFirearm>().BarrelAttachment;
				customizeShowcase.GetComponent<PickupItem>().ResetAttachments();
			}
			primaryShowcase.GetComponent<PickupItem>().SightAttachment = primary.GetComponent<PlayerFirearm>().SightAttachment;
			primaryShowcase.GetComponent<PickupItem>().GripAttachment = primary.GetComponent<PlayerFirearm>().GripAttachment;
			primaryShowcase.GetComponent<PickupItem>().BarrelAttachment = primary.GetComponent<PlayerFirearm>().BarrelAttachment;
			primaryShowcase.GetComponent<PickupItem>().ResetAttachments();
		}
		if ((bool)secondaryShowcase && (bool)secondary && (bool)secondary.GetComponent<PlayerFirearm>())
		{
			if (itemSelected == 2)
			{
				customizeShowcase.GetComponent<PickupItem>().SightAttachment = secondary.GetComponent<PlayerFirearm>().SightAttachment;
				customizeShowcase.GetComponent<PickupItem>().GripAttachment = secondary.GetComponent<PlayerFirearm>().GripAttachment;
				customizeShowcase.GetComponent<PickupItem>().BarrelAttachment = secondary.GetComponent<PlayerFirearm>().BarrelAttachment;
				customizeShowcase.GetComponent<PickupItem>().ResetAttachments();
			}
			secondaryShowcase.GetComponent<PickupItem>().SightAttachment = secondary.GetComponent<PlayerFirearm>().SightAttachment;
			secondaryShowcase.GetComponent<PickupItem>().GripAttachment = secondary.GetComponent<PlayerFirearm>().GripAttachment;
			secondaryShowcase.GetComponent<PickupItem>().BarrelAttachment = secondary.GetComponent<PlayerFirearm>().BarrelAttachment;
			secondaryShowcase.GetComponent<PickupItem>().ResetAttachments();
		}
	}

	private void UpdateUI()
	{
		if ((bool)primary && (bool)secondary && (bool)equipment1 && (bool)equipment2)
		{
			loadoutUI.SetLoadout("loadout", primary, secondary, equipment1, equipment2, meleeWeapon);
		}
		_ = attachmentCustomizationOpened;
	}

	public void CloseAttachmentSelectScroll()
	{
		attachmentCustomizationOpened = false;
	}

	public void SelectItem(int item)
	{
		itemSelected = item;
		SetUIState(1);
		switch (item)
		{
		case 1:
			ShowDescriptionPanel(primary.GetComponent<PlayerItem>());
			SpawnShowCaseModels(1, primary.ItemName.Replace("Weapon_", ""));
			CreateItemsList(allowedPrimaries);
			break;
		case 2:
			ShowDescriptionPanel(secondary.GetComponent<PlayerItem>());
			SpawnShowCaseModels(2, secondary.ItemName.Replace("Weapon_", ""));
			CreateItemsList(allowedSecondaries);
			break;
		case 3:
			ShowDescriptionPanel(equipment1.GetComponent<PlayerItem>());
			SpawnShowCaseModels(3, equipment1.ItemName.Replace("Weapon_", ""));
			CreateItemsList(allowedEquipment1);
			break;
		case 4:
			ShowDescriptionPanel(equipment2.GetComponent<PlayerItem>());
			SpawnShowCaseModels(4, equipment2.ItemName.Replace("Weapon_", ""));
			CreateItemsList(allowedEquipment2);
			break;
		case 5:
			ShowDescriptionPanel(meleeWeapon.GetComponent<PlayerItem>());
			SpawnShowCaseModels(5, meleeWeapon.ItemName.Replace("Weapon_", ""));
			CreateItemsList(allowedMeleeWeapons);
			break;
		}
	}

	public void SetUIState(int state)
	{
		loadoutAnimator.SetInteger("Camera State", state);
		canvasAnimator.SetInteger("UI State", state);
		if (state == 2)
		{
			OpenAttachmentSelectScroll(1);
			UpdateShowcaseModels();
		}
	}

	public void OpenAttachmentSelectScroll(int slot)
	{
		attachmentCustomizationOpened = true;
		ClearAttachmentList();
		switch (itemSelected)
		{
		case 1:
			switch (slot)
			{
			case 1:
				CreateAttachmentsList(GetSightAttachmentsInChildren((primary as PlayerFirearm).SightAttachmentParent));
				break;
			case 2:
				CreateAttachmentsList(GetSightAttachmentsInChildren((primary as PlayerFirearm).GripAttachmentParent));
				break;
			case 3:
				CreateAttachmentsList(GetSightAttachmentsInChildren((primary as PlayerFirearm).BarrelAttachmentParent));
				break;
			}
			break;
		case 2:
			switch (slot)
			{
			case 1:
				CreateAttachmentsList(GetSightAttachmentsInChildren((secondary as PlayerFirearm).SightAttachmentParent));
				break;
			case 2:
				CreateAttachmentsList(GetSightAttachmentsInChildren((secondary as PlayerFirearm).GripAttachmentParent));
				break;
			case 3:
				CreateAttachmentsList(GetSightAttachmentsInChildren((secondary as PlayerFirearm).BarrelAttachmentParent));
				break;
			}
			break;
		}
	}

	public WeaponAttachment[] GetSightAttachmentsInChildren(GameObject parent)
	{
		List<WeaponAttachment> list = new List<WeaponAttachment>();
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			if ((bool)parent.transform.GetChild(i).GetComponent<WeaponAttachment>())
			{
				list.Add(parent.transform.GetChild(i).GetComponent<WeaponAttachment>());
			}
		}
		return list.ToArray();
	}

	public void ClearAttachmentList()
	{
		foreach (GameObject attachment in attachments)
		{
			Object.Destroy(attachment);
		}
		attachments = new List<GameObject>();
	}

	public void CreateItemsList(PlayerItem[] newItems)
	{
		ClearItemsList();
		int num = 0;
		itemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemContent.GetComponent<RectTransform>().sizeDelta.x, item_initialGap + (float)newItems.Length * item_contentScalePerItem);
		foreach (PlayerItem item in newItems)
		{
			GameObject gameObject = Object.Instantiate(itemEntrySample, base.transform.position, base.transform.rotation);
			gameObject.transform.gameObject.SetActive(value: true);
			gameObject.transform.parent = itemEntrySample.transform.parent;
			gameObject.transform.localPosition = itemEntrySample.transform.localPosition + new Vector3(0f, (float)num * item_selectTabPadding, 0f);
			gameObject.transform.localScale = itemEntrySample.transform.localScale;
			gameObject.GetComponent<WeaponTab>().SetItem(item);
			items.Add(gameObject);
			num++;
		}
	}

	private void ClearItemsList()
	{
		foreach (GameObject item in items)
		{
			Object.Destroy(item);
		}
	}

	public void CreateAttachmentsList(WeaponAttachment[] newAttachments)
	{
		int num = 0;
		attachmentContent.GetComponent<RectTransform>().sizeDelta = new Vector2(attachment_initialGap + (float)newAttachments.Length * attachment_contentScalePerItem, attachmentContent.GetComponent<RectTransform>().sizeDelta.y);
		foreach (WeaponAttachment attachment in newAttachments)
		{
			GameObject gameObject = Object.Instantiate(attachmentEntrySample, base.transform.position, base.transform.rotation);
			gameObject.transform.gameObject.SetActive(value: true);
			gameObject.transform.parent = attachmentEntrySample.transform.parent;
			gameObject.transform.localPosition = attachmentEntrySample.transform.localPosition + new Vector3((float)num * attachment_selectTabPadding, 0f, 0f);
			gameObject.transform.localScale = attachmentEntrySample.transform.localScale;
			gameObject.GetComponent<AttachmentTab>().SetAttachment(attachment);
			attachments.Add(gameObject);
			num++;
		}
	}

	public void SetAttachment(int slot, string attachment)
	{
		switch (itemSelected)
		{
		case 1:
			if (!(primary is PlayerFirearm))
			{
				break;
			}
			switch (slot)
			{
			case 1:
			{
				int sightAttachment2 = 0;
				for (int n = 0; n < (primary as PlayerFirearm).SightAttachmentParent.transform.childCount; n++)
				{
					if ((primary as PlayerFirearm).SightAttachmentParent.transform.GetChild(n).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						sightAttachment2 = n;
					}
				}
				(primary as PlayerFirearm).SetSightAttachment(sightAttachment2);
				break;
			}
			case 2:
			{
				int gripAttachment2 = 0;
				for (int m = 0; m < (primary as PlayerFirearm).GripAttachmentParent.transform.childCount; m++)
				{
					if ((primary as PlayerFirearm).GripAttachmentParent.transform.GetChild(m).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						gripAttachment2 = m;
					}
				}
				(primary as PlayerFirearm).SetGripAttachment(gripAttachment2);
				break;
			}
			case 3:
			{
				int barrelAttachment2 = 0;
				for (int l = 0; l < (primary as PlayerFirearm).BarrelAttachmentParent.transform.childCount; l++)
				{
					if ((primary as PlayerFirearm).BarrelAttachmentParent.transform.GetChild(l).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						barrelAttachment2 = l;
					}
				}
				(primary as PlayerFirearm).SetBarrelAttachment(barrelAttachment2);
				break;
			}
			}
			break;
		case 2:
			if (!(secondary is PlayerFirearm))
			{
				break;
			}
			switch (slot)
			{
			case 1:
			{
				int sightAttachment = 0;
				for (int k = 0; k < (secondary as PlayerFirearm).SightAttachmentParent.transform.childCount; k++)
				{
					if ((secondary as PlayerFirearm).SightAttachmentParent.transform.GetChild(k).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						sightAttachment = k;
					}
				}
				(secondary as PlayerFirearm).SetSightAttachment(sightAttachment);
				break;
			}
			case 2:
			{
				int gripAttachment = 0;
				for (int j = 0; j < (secondary as PlayerFirearm).GripAttachmentParent.transform.childCount; j++)
				{
					if ((secondary as PlayerFirearm).GripAttachmentParent.transform.GetChild(j).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						gripAttachment = j;
					}
				}
				(secondary as PlayerFirearm).SetGripAttachment(gripAttachment);
				break;
			}
			case 3:
			{
				int barrelAttachment = 0;
				for (int i = 0; i < (secondary as PlayerFirearm).BarrelAttachmentParent.transform.childCount; i++)
				{
					if ((secondary as PlayerFirearm).BarrelAttachmentParent.transform.GetChild(i).GetComponent<WeaponAttachment>().AttachmentName == attachment)
					{
						barrelAttachment = i;
					}
				}
				(secondary as PlayerFirearm).SetBarrelAttachment(barrelAttachment);
				break;
			}
			}
			break;
		case 3:
		case 4:
		case 5:
			break;
		}
	}

	public void SetItem(PlayerItem item)
	{
		SpawnItem(itemSelected, item.transform.name.Replace("Weapon_", ""));
	}

	public void SpawnFirearm(int slot, string name, int sight, int barrel, int grip)
	{
		SpawnItem(slot, name);
		if (slot == 1 && primary is PlayerFirearm)
		{
			primary.GetComponent<PlayerFirearm>().SightAttachment = sight;
			primary.GetComponent<PlayerFirearm>().BarrelAttachment = barrel;
			primary.GetComponent<PlayerFirearm>().GripAttachment = grip;
		}
		if (slot == 2 && secondary is PlayerFirearm)
		{
			secondary.GetComponent<PlayerFirearm>().SightAttachment = sight;
			secondary.GetComponent<PlayerFirearm>().BarrelAttachment = barrel;
			secondary.GetComponent<PlayerFirearm>().GripAttachment = grip;
		}
	}

	public void SpawnPrimary(string item)
	{
		SpawnItem(1, item);
	}

	public void SpawnSecondary(string item)
	{
		SpawnItem(2, item);
	}

	public void SpawnEquipment1(string item)
	{
		SpawnItem(3, item);
	}

	public void SpawnEquipment2(string item)
	{
		SpawnItem(4, item);
	}

	public void SpawnMeleeWeapon(string item)
	{
		SpawnItem(5, item);
	}

	public void ShowDescriptionPanel(PlayerItem item)
	{
		itemDescriptionPanel.SetActive(value: true);
		itemDescriptionTitle.text = item.GetDisplayName();
		itemDescription.text = item.ItemDescription;
	}

	public void SpawnShowcaseItem(string item)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("ItemPickup/ItemPickup_" + item) as GameObject, base.transform.position, base.transform.rotation);
		if (customizeShowcase != null)
		{
			Object.Destroy(customizeShowcase.gameObject);
		}
		customizeShowcase = gameObject;
		customizeShowcase.transform.parent = customizationShowcaseParent.transform;
		customizeShowcase.transform.localPosition = new Vector3(0f, 0f, 0f);
		customizeShowcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		customizeShowcase.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void SpawnItem(int slot, string item)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Weapon_" + item) as GameObject, base.transform.position, base.transform.rotation);
		ShowDescriptionPanel(gameObject.GetComponent<PlayerItem>());
		SpawnShowCaseModels(slot, item);
		switch (slot)
		{
		case 1:
			if (primary != null)
			{
				Object.Destroy(primary.gameObject);
			}
			primary = gameObject.GetComponent<PlayerItem>();
			gameObject.transform.parent = itemParent.transform;
			break;
		case 2:
			if (secondary != null)
			{
				Object.Destroy(secondary.gameObject);
			}
			secondary = gameObject.GetComponent<PlayerItem>();
			gameObject.transform.parent = itemParent.transform;
			break;
		case 3:
			if (equipment1 != null)
			{
				Object.Destroy(equipment1.gameObject);
			}
			equipment1 = gameObject.GetComponent<PlayerItem>();
			gameObject.transform.parent = itemParent.transform;
			break;
		case 4:
			if (equipment2 != null)
			{
				Object.Destroy(equipment2.gameObject);
			}
			equipment2 = gameObject.GetComponent<PlayerItem>();
			gameObject.transform.parent = itemParent.transform;
			break;
		case 5:
			if (meleeWeapon != null)
			{
				Object.Destroy(meleeWeapon.gameObject);
			}
			meleeWeapon = gameObject.GetComponent<PlayerItem>();
			gameObject.transform.parent = itemParent.transform;
			break;
		}
	}

	public void SpawnShowCaseModels(int slot, string item)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("ItemPickup/ItemPickup_" + item) as GameObject, base.transform.position, base.transform.rotation);
		SpawnShowcaseItem(item);
		switch (slot)
		{
		case 1:
			if (primaryShowcase != null)
			{
				Object.Destroy(primaryShowcase.gameObject);
			}
			primaryShowcase = gameObject;
			primaryShowcase.transform.parent = primaryShowcaseParent.transform;
			primaryShowcase.transform.localPosition = new Vector3(0f, 0f, 0f);
			primaryShowcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			primaryShowcase.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case 2:
			if (secondaryShowcase != null)
			{
				Object.Destroy(secondaryShowcase.gameObject);
			}
			secondaryShowcase = gameObject;
			secondaryShowcase.transform.parent = secondaryShowcaseParent.transform;
			secondaryShowcase.transform.localPosition = new Vector3(0f, 0f, 0f);
			secondaryShowcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			secondaryShowcase.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case 3:
			if (equipment1Showcase != null)
			{
				Object.Destroy(equipment1Showcase.gameObject);
			}
			equipment1Showcase = gameObject;
			equipment1Showcase.transform.parent = equipment1ShowcaseParent.transform;
			equipment1Showcase.transform.localPosition = new Vector3(0f, 0f, 0f);
			equipment1Showcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			equipment1Showcase.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case 4:
			if (equipment2Showcase != null)
			{
				Object.Destroy(equipment2Showcase.gameObject);
			}
			equipment2Showcase = gameObject;
			equipment2Showcase.transform.parent = equipment2ShowcaseParent.transform;
			equipment2Showcase.transform.localPosition = new Vector3(0f, 0f, 0f);
			equipment2Showcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			equipment2Showcase.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case 5:
			if (meleeWeaponShowcase != null)
			{
				Object.Destroy(meleeWeaponShowcase.gameObject);
			}
			meleeWeaponShowcase = gameObject;
			meleeWeaponShowcase.transform.parent = meleeWeaponShowcaseParent.transform;
			meleeWeaponShowcase.transform.localPosition = new Vector3(0f, 0f, 0f);
			meleeWeaponShowcase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			meleeWeaponShowcase.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		}
	}
}
