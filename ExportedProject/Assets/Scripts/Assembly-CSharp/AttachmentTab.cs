using UnityEngine;

public class AttachmentTab : SelectableItemTab
{
	[SerializeField]
	private WeaponAttachment attachment;

	public WeaponAttachment Attachment
	{
		get
		{
			return attachment;
		}
		set
		{
			attachment = value;
		}
	}

	public void SetAttachment(WeaponAttachment attachment)
	{
		this.attachment = attachment;
		icon.texture = attachment.ItemIcon;
		nameText.text = attachment.AttachmentName;
		costText.text = attachment.Cost.ToString();
	}

	public override void Select()
	{
		int num = 0;
		if (attachment is WeaponSight)
		{
			num = 1;
		}
		else if (attachment is GripAttachment)
		{
			num = 2;
		}
		else if (attachment is BarrelAttachment)
		{
			num = 3;
		}
		if (num != 0)
		{
			MonoBehaviour.print("select " + num);
			Object.FindObjectOfType<LoadoutCustomization>().SetAttachment(num, attachment.AttachmentName);
		}
	}
}
