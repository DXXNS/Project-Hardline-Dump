using UnityEngine;

public class Inventory
{
	private PlayerItem primaryItem;

	private PlayerItem secondaryItem;

	private PlayerItem equipment1;

	private PlayerItem equipment2;

	private PlayerItem meleeWeapon;

	private int currentlySelected;

	public PlayerItem SecondaryItem
	{
		get
		{
			return secondaryItem;
		}
		set
		{
			secondaryItem = value;
		}
	}

	public PlayerItem PrimaryItem
	{
		get
		{
			return primaryItem;
		}
		set
		{
			primaryItem = value;
		}
	}

	public PlayerItem Equipment1
	{
		get
		{
			return equipment1;
		}
		set
		{
			equipment1 = value;
		}
	}

	public PlayerItem Equipment2
	{
		get
		{
			return equipment2;
		}
		set
		{
			equipment2 = value;
		}
	}

	public int CurrentlySelected
	{
		get
		{
			return currentlySelected;
		}
		set
		{
			currentlySelected = value;
		}
	}

	public PlayerItem MeleeWeapon
	{
		get
		{
			return meleeWeapon;
		}
		set
		{
			meleeWeapon = value;
		}
	}

	public void Init()
	{
		currentlySelected = 1;
		primaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
		secondaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
		equipment1 = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
		equipment2 = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
		meleeWeapon = (Resources.Load("Weapon_Knife") as GameObject).GetComponent<PlayerItem>();
	}

	public void SetInventory(PlayerItem primary, PlayerItem secondary, bool fullAmmo)
	{
		primaryItem = primary;
		secondaryItem = secondary;
		if (primaryItem is PlayerFirearm)
		{
			(primaryItem as PlayerFirearm).Chambered = true;
		}
		if (secondaryItem is PlayerFirearm)
		{
			(secondaryItem as PlayerFirearm).Chambered = true;
		}
		if (primaryItem is PlayerFirearm && fullAmmo)
		{
			(primaryItem as PlayerFirearm).Ammo = (primaryItem as PlayerFirearm).MaxAmmo;
			(primaryItem as PlayerFirearm).ReserveAmmo = (primaryItem as PlayerFirearm).StartingReserveAmmo;
		}
		if (secondaryItem is PlayerFirearm && fullAmmo)
		{
			(secondaryItem as PlayerFirearm).Ammo = (secondaryItem as PlayerFirearm).MaxAmmo;
			(secondaryItem as PlayerFirearm).ReserveAmmo = (secondaryItem as PlayerFirearm).StartingReserveAmmo;
		}
	}

	public void SetInventory(PlayerItem primary, PlayerItem secondary, PlayerItem equipment1, PlayerItem equipment2, PlayerItem meleeWeapon, bool fullAmmo)
	{
		primaryItem = primary;
		secondaryItem = secondary;
		this.equipment1 = equipment1;
		this.equipment2 = equipment2;
		MeleeWeapon = meleeWeapon;
		if (primaryItem is PlayerFirearm)
		{
			(primaryItem as PlayerFirearm).Chambered = true;
		}
		if (secondaryItem is PlayerFirearm)
		{
			(secondaryItem as PlayerFirearm).Chambered = true;
		}
		if (primaryItem is PlayerFirearm && fullAmmo)
		{
			(primaryItem as PlayerFirearm).Ammo = (primaryItem as PlayerFirearm).MaxAmmo;
			(primaryItem as PlayerFirearm).ReserveAmmo = (primaryItem as PlayerFirearm).StartingReserveAmmo;
		}
		if (secondaryItem is PlayerFirearm && fullAmmo)
		{
			(secondaryItem as PlayerFirearm).Ammo = (secondaryItem as PlayerFirearm).MaxAmmo;
			(secondaryItem as PlayerFirearm).ReserveAmmo = (secondaryItem as PlayerFirearm).StartingReserveAmmo;
		}
	}

	public void SetInventory(Inventory inventory)
	{
		primaryItem = inventory.PrimaryItem;
		secondaryItem = inventory.SecondaryItem;
		equipment1 = inventory.equipment1;
		equipment2 = inventory.equipment2;
	}

	public void GiveItemAsPrimary(PlayerItem item, bool fullAmmo)
	{
		primaryItem = item;
		if (item is PlayerFirearm && fullAmmo)
		{
			(item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
		}
	}

	public void GiveItemAsSecondary(PlayerItem item, bool fullAmmo)
	{
		secondaryItem = item;
		if (item is PlayerFirearm && fullAmmo)
		{
			(item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
		}
	}

	public void GiveItemAsEquipment1(PlayerItem item, bool fullAmmo)
	{
		equipment1 = item;
		if (item is PlayerFirearm && fullAmmo)
		{
			(item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
		}
	}

	public void GiveItemAsEquipment2(PlayerItem item, bool fullAmmo)
	{
		equipment2 = item;
		if (item is PlayerFirearm && fullAmmo)
		{
			(item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
		}
	}

	public void GiveItemAsMeleeWeapon(PlayerItem item)
	{
		MeleeWeapon = item;
	}

	public void AssignNewItem(PlayerItem item, bool fullAmmo)
	{
		if (CurrentlySelected == 1)
		{
			GiveItemAsPrimary(item, fullAmmo);
		}
		else if (CurrentlySelected == 2)
		{
			GiveItemAsSecondary(item, fullAmmo);
		}
		if (CurrentlySelected == 3)
		{
			GiveItemAsEquipment1(item, fullAmmo);
		}
		else if (CurrentlySelected == 4)
		{
			GiveItemAsEquipment2(item, fullAmmo);
		}
		else if (CurrentlySelected == 5)
		{
			GiveItemAsMeleeWeapon(item);
		}
	}
}
