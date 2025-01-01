using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectable : MonoBehaviour
{
	[SerializeField]
	private Text title;

	[SerializeField]
	private Text primaryText;

	[SerializeField]
	private Text secondaryText;

	[SerializeField]
	private Text equipment1Text;

	[SerializeField]
	private Text equipment2Text;

	[SerializeField]
	private Text meleeWeaponText;

	[SerializeField]
	private Text primaryCost;

	[SerializeField]
	private Text secondaryCost;

	[SerializeField]
	private Text equipment1Cost;

	[SerializeField]
	private Text equipment2Cost;

	[SerializeField]
	private Text meleeWeaponCost;

	[SerializeField]
	private Text totalCostText;

	[SerializeField]
	private RawImage primaryItemImage;

	[SerializeField]
	private RawImage secondaryItemImage;

	[SerializeField]
	private RawImage primaryItemSightImage;

	[SerializeField]
	private RawImage secondaryItemSightImage;

	[SerializeField]
	private RawImage primaryItemGripImage;

	[SerializeField]
	private RawImage secondaryItemGripImage;

	[SerializeField]
	private RawImage primaryItemBarrelImage;

	[SerializeField]
	private RawImage secondaryItemBarrelImage;

	[SerializeField]
	private RawImage equipment1Image;

	[SerializeField]
	private RawImage equipment2Image;

	[SerializeField]
	private RawImage meleeWeaponImage;

	[SerializeField]
	private string loadoutName;

	[SerializeField]
	private PlayerItem primaryItem;

	[SerializeField]
	private PlayerItem secondaryItem;

	[SerializeField]
	private PlayerItem equipment1;

	[SerializeField]
	private PlayerItem equipment2;

	[SerializeField]
	private PlayerItem meleeWeapon;

	[SerializeField]
	private bool supplyCost;

	[SerializeField]
	private GameObject inaccessiblePanel;

	private HardlineGameManager gameManager;

	private int totalCost;

	private void Awake()
	{
		gameManager = Object.FindObjectOfType<HardlineGameManager>();
	}

	private void Update()
	{
		if (supplyCost)
		{
			CheckAffordable();
		}
	}

	public void CheckAffordable()
	{
		if (gameManager is PracticeModeGameManager)
		{
			inaccessiblePanel.SetActive(value: false);
		}
		else if (gameManager is RoundsHardlineGameManager)
		{
			if (gameManager.PlayerSupplyPoints >= totalCost)
			{
				inaccessiblePanel.SetActive(value: false);
			}
			else
			{
				inaccessiblePanel.SetActive(value: true);
			}
		}
	}

	public void SetLoadout(string loadoutName, PlayerItem primary, PlayerItem secondary)
	{
		this.loadoutName = loadoutName;
		primaryItem = primary;
		secondaryItem = secondary;
		totalCost = CalculateTotalCost();
		UpdateUI();
	}

	public void SetLoadout(string loadoutName, PlayerItem primary, PlayerItem secondary, PlayerItem equipment1, PlayerItem equipment2, PlayerItem meleeWeapon)
	{
		this.loadoutName = loadoutName;
		primaryItem = primary;
		secondaryItem = secondary;
		this.equipment1 = equipment1;
		this.equipment2 = equipment2;
		this.meleeWeapon = meleeWeapon;
		totalCost = CalculateTotalCost();
		UpdateUI();
	}

	public void SetLoadout(string loadoutName, Inventory inventory)
	{
		this.loadoutName = loadoutName;
		primaryItem = inventory.PrimaryItem;
		secondaryItem = inventory.SecondaryItem;
		equipment1 = inventory.Equipment1;
		equipment2 = inventory.Equipment2;
		meleeWeapon = inventory.MeleeWeapon;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if ((bool)title)
		{
			title.text = loadoutName;
		}
		primaryText.text = primaryItem.GetDisplayName();
		secondaryText.text = secondaryItem.GetDisplayName();
		equipment1Text.text = equipment1.GetDisplayName();
		equipment2Text.text = equipment2.GetDisplayName();
		meleeWeaponText.text = meleeWeapon.GetDisplayName();
		primaryItemSightImage.color = new Color(1f, 1f, 1f, 0f);
		secondaryItemSightImage.color = new Color(1f, 1f, 1f, 0f);
		primaryItemGripImage.color = new Color(1f, 1f, 1f, 0f);
		secondaryItemGripImage.color = new Color(1f, 1f, 1f, 0f);
		primaryItemBarrelImage.color = new Color(1f, 1f, 1f, 0f);
		secondaryItemBarrelImage.color = new Color(1f, 1f, 1f, 0f);
		if ((bool)primaryItem.ItemIcon)
		{
			primaryItemImage.texture = primaryItem.ItemIcon;
			primaryItemImage.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			primaryItemImage.color = new Color(1f, 1f, 1f, 0f);
		}
		if ((bool)secondaryItem.ItemIcon)
		{
			secondaryItemImage.texture = secondaryItem.ItemIcon;
			secondaryItemImage.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			secondaryItemImage.color = new Color(1f, 1f, 1f, 0f);
		}
		if ((bool)equipment1.ItemIcon)
		{
			equipment1Image.texture = equipment1.ItemIcon;
			equipment1Image.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			equipment1Image.color = new Color(1f, 1f, 1f, 0f);
		}
		if ((bool)equipment2.ItemIcon)
		{
			equipment2Image.texture = equipment2.ItemIcon;
			equipment2Image.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			equipment2Image.color = new Color(1f, 1f, 1f, 0f);
		}
		if ((bool)meleeWeapon.ItemIcon)
		{
			meleeWeaponImage.texture = meleeWeapon.ItemIcon;
			meleeWeaponImage.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			meleeWeaponImage.color = new Color(1f, 1f, 1f, 0f);
		}
		if (primaryItem is PlayerFirearm)
		{
			if ((bool)(primaryItem as PlayerFirearm).GetSightAttachment().ItemIcon)
			{
				primaryItemSightImage.texture = (primaryItem as PlayerFirearm).GetSightAttachment().ItemIcon;
				primaryItemSightImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				primaryItemSightImage.color = new Color(1f, 1f, 1f, 0f);
			}
			if ((bool)(primaryItem as PlayerFirearm).GetBarrelAttachment().ItemIcon)
			{
				primaryItemBarrelImage.texture = (primaryItem as PlayerFirearm).GetBarrelAttachment().ItemIcon;
				primaryItemBarrelImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				primaryItemBarrelImage.color = new Color(1f, 1f, 1f, 0f);
			}
			if ((bool)(primaryItem as PlayerFirearm).GetGripAttachment().ItemIcon)
			{
				primaryItemGripImage.texture = (primaryItem as PlayerFirearm).GetGripAttachment().ItemIcon;
				primaryItemGripImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				primaryItemGripImage.color = new Color(1f, 1f, 1f, 0f);
			}
		}
		if (secondaryItem is PlayerFirearm)
		{
			if ((bool)(secondaryItem as PlayerFirearm).GetSightAttachment().ItemIcon)
			{
				secondaryItemSightImage.texture = (secondaryItem as PlayerFirearm).GetSightAttachment().ItemIcon;
				secondaryItemSightImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				secondaryItemSightImage.color = new Color(1f, 1f, 1f, 0f);
			}
			if ((bool)(secondaryItem as PlayerFirearm).GetBarrelAttachment().ItemIcon)
			{
				secondaryItemBarrelImage.texture = (secondaryItem as PlayerFirearm).GetBarrelAttachment().ItemIcon;
				secondaryItemBarrelImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				secondaryItemBarrelImage.color = new Color(1f, 1f, 1f, 0f);
			}
			if ((bool)(secondaryItem as PlayerFirearm).GetGripAttachment().ItemIcon)
			{
				secondaryItemGripImage.texture = (secondaryItem as PlayerFirearm).GetGripAttachment().ItemIcon;
				secondaryItemGripImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				secondaryItemGripImage.color = new Color(1f, 1f, 1f, 0f);
			}
		}
		if ((bool)primaryCost && (bool)secondaryCost && (bool)equipment1Cost && (bool)equipment2Cost && (bool)meleeWeaponCost)
		{
			if (primaryItem is PlayerFirearm)
			{
				PlayerFirearm playerFirearm = primaryItem as PlayerFirearm;
				primaryCost.text = (primaryItem.ItemCost + playerFirearm.GetSightAttachment().Cost + playerFirearm.GetBarrelAttachment().Cost + playerFirearm.GetGripAttachment().Cost).ToString();
			}
			else
			{
				primaryCost.text = primaryItem.ItemCost.ToString();
			}
			if (secondaryItem is PlayerFirearm)
			{
				PlayerFirearm playerFirearm2 = secondaryItem as PlayerFirearm;
				secondaryCost.text = (secondaryItem.ItemCost + playerFirearm2.GetSightAttachment().Cost + playerFirearm2.GetBarrelAttachment().Cost + playerFirearm2.GetGripAttachment().Cost).ToString();
			}
			else
			{
				secondaryCost.text = secondaryItem.ItemCost.ToString();
			}
			equipment1Cost.text = equipment1.ItemCost.ToString();
			equipment2Cost.text = equipment2.ItemCost.ToString();
			meleeWeaponCost.text = meleeWeapon.ItemCost.ToString();
		}
		if ((bool)totalCostText)
		{
			totalCostText.text = totalCost.ToString();
		}
	}

	public int CalculateTotalCost()
	{
		int num = 0;
		if (primaryItem is PlayerFirearm)
		{
			PlayerFirearm playerFirearm = primaryItem as PlayerFirearm;
			num += primaryItem.ItemCost + playerFirearm.GetSightAttachment().Cost + playerFirearm.GetBarrelAttachment().Cost + playerFirearm.GetGripAttachment().Cost;
		}
		else
		{
			num += primaryItem.ItemCost;
		}
		if (secondaryItem is PlayerFirearm)
		{
			PlayerFirearm playerFirearm2 = secondaryItem as PlayerFirearm;
			num += secondaryItem.ItemCost + playerFirearm2.GetSightAttachment().Cost + playerFirearm2.GetBarrelAttachment().Cost + playerFirearm2.GetGripAttachment().Cost;
		}
		else
		{
			num += secondaryItem.ItemCost;
		}
		num += equipment1.ItemCost;
		return num + equipment2.ItemCost;
	}

	public void SelectLoadout()
	{
		if (gameManager is RoundsHardlineGameManager)
		{
			if (supplyCost)
			{
				if (gameManager.PlayerSupplyPoints - totalCost >= 0)
				{
					gameManager.SpendSupplyPoints(totalCost);
					gameManager.GivePlayerLoadout(primaryItem, secondaryItem, equipment1, equipment2, meleeWeapon);
					(gameManager as RoundsHardlineGameManager).CloseLoadOutSelect();
				}
			}
			else
			{
				gameManager.GivePlayerLoadout(primaryItem, secondaryItem, equipment1, equipment2, meleeWeapon);
				(gameManager as RoundsHardlineGameManager).CloseLoadOutSelect();
			}
		}
		else if (gameManager is PracticeModeGameManager)
		{
			gameManager.GivePlayerLoadout(primaryItem, secondaryItem, equipment1, equipment2, meleeWeapon);
			(gameManager as PracticeModeGameManager).CloseLoadOutSelect();
			(gameManager as PracticeModeGameManager).PlayerSelectedLoadout();
		}
	}
}
