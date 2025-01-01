using UnityEngine;

public class GeneratedMesh : MonoBehaviour
{
	public void AddDeformation(Vector3 position, float scale, float penetration)
	{
		base.transform.GetComponentInParent<DeformableWall>().AddDeformation(position, scale, penetration);
	}
}
