using UnityEngine;
using UnityEngine.UI;

public class SelectableItemTab : MonoBehaviour
{
	[SerializeField]
	protected RawImage icon;

	[SerializeField]
	protected Text nameText;

	[SerializeField]
	protected Text costText;

	public virtual void Select()
	{
	}
}
