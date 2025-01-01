using UnityEngine;

public class ShatterableGlassInfo
{
	public Vector3 HitPoint;

	public Vector3 HitDirrection;

	public ShatterableGlassInfo(Vector3 HitPoint, Vector3 HitDirrection)
	{
		this.HitPoint = HitPoint;
		this.HitDirrection = HitDirrection;
	}
}
