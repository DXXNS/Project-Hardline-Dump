using UnityEngine;

public class WeaponTab : SelectableItemTab
{
	[SerializeField]
	private PlayerItem item;

	public void SetItem(PlayerItem item)
	{
		this.item = item;
		icon.texture = item.ItemIcon;
		nameText.text = item.GetDisplayName();
		costText.text = item.ItemCost.ToString();
	}

	public override void Select()
	{
		Object.FindObjectOfType<LoadoutCustomization>().SetItem(item);
	}
}
