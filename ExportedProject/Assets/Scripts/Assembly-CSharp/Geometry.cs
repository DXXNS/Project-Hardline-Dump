using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
	private static Vector3 noPosition;

	public static Vector2 GetIntersectingPoint(Vector2 pointA1, Vector2 pointA2, Vector2 pointB1, Vector2 pointB2, bool checkMode)
	{
		Round(pointA1, 1);
		Vector2 result = default(Vector2);
		if (pointA1.x != pointA2.x && pointB1.x != pointB2.x)
		{
			float num = (pointA1.y - pointA2.y) / (pointA1.x - pointA2.x);
			float num2 = (pointB1.y - pointB2.y) / (pointB1.x - pointB2.x);
			float num3 = pointA1.y - num * pointA1.x;
			float num4 = pointB1.y - num2 * pointB1.x;
			if (num2 == num)
			{
				return noPosition;
			}
			result.x = (num3 - num4) / (num2 - num);
			result.y = num * result.x + num3;
			if (IsBetween(result.x, pointA1.x, pointA2.x) && IsBetween(result.y, pointA1.y, pointA2.y) && IsBetween(result.x, pointB1.x, pointB2.x) && IsBetween(result.y, pointB1.y, pointB2.y))
			{
				return result;
			}
			return noPosition;
		}
		if (pointA1.x == pointA2.x && pointB1.x != pointB2.x)
		{
			result.x = pointA1.x;
			float num2 = (pointB1.y - pointB2.y) / (pointB1.x - pointB2.x);
			float num4 = 0f - num2 * pointB1.x + pointB1.y;
			result.y = num2 * result.x + num4;
			if (IsBetween(result.y, pointA1.y, pointA2.y) && IsBetween(result.x, pointB1.x, pointB2.x))
			{
				return result;
			}
			return noPosition;
		}
		if (pointB1.x == pointB2.x && pointA1.x != pointA2.x)
		{
			result.x = pointB1.x;
			float num = (pointA1.y - pointA2.y) / (pointA1.x - pointA2.x);
			float num3 = 0f - num * pointA1.x + pointA1.y;
			result.y = num * result.x + num3;
			if (IsBetween(result.y, pointB1.y, pointB2.y) && IsBetween(result.x, pointA1.x, pointA2.x))
			{
				return result;
			}
			return noPosition;
		}
		return noPosition;
	}

	public static bool IsEntering(Vector2 pointA1, Vector2 pointA2, Vector2 pointB1, Vector2 pointB2, bool checkMode)
	{
		if (pointA1.x != pointA2.x)
		{
			float num = (pointA1.y - pointA2.y) / (pointA1.x - pointA2.x);
			float num2 = pointA1.y - num * pointA1.x;
			if (num >= 0f)
			{
				if (pointA2.x > pointA1.x)
				{
					if (pointB2.y >= num * pointB2.x + num2)
					{
						return pointB1.y <= num * pointB1.x + num2;
					}
					return false;
				}
				if (pointB2.y <= num * pointB2.x + num2)
				{
					return pointB1.y >= num * pointB1.x + num2;
				}
				return false;
			}
			if (pointA2.x > pointA1.x)
			{
				if (pointB2.y >= num * pointB2.x + num2)
				{
					return pointB1.y <= num * pointB1.x + num2;
				}
				return false;
			}
			if (pointB2.y <= num * pointB2.x + num2)
			{
				return pointB1.y >= num * pointB1.x + num2;
			}
			return false;
		}
		if (pointA2.y > pointA1.y)
		{
			if (pointB2.x <= pointA1.x)
			{
				return pointB1.x >= pointA2.x;
			}
			return false;
		}
		if (pointB2.x >= pointA1.x)
		{
			return pointB1.x <= pointA2.x;
		}
		return false;
	}

	public static Vector2 Round(Vector2 vector, int decimalPlaces)
	{
		float num = 1f;
		for (int i = 0; i < decimalPlaces; i++)
		{
			num *= 10f;
		}
		return new Vector2(Mathf.Round(vector.x * num) / num, Mathf.Round(vector.y * num) / num);
	}

	public bool IsOnLine(Vector2 point, Vector2 pointA1, Vector2 pointA2)
	{
		if (pointA1 != pointA2)
		{
			float num = (pointA1.y - pointA2.y) / (pointA1.x - pointA2.x);
			float num2 = num * pointA1.x + pointA1.y;
			return point.y == num * point.x + num2;
		}
		return point.x == pointA1.x;
	}

	public static bool IsBetween(double givenValue, double bound1, double bound2)
	{
		if (bound1 > bound2)
		{
			if (givenValue >= bound2)
			{
				return givenValue <= bound1;
			}
			return false;
		}
		if (givenValue >= bound1)
		{
			return givenValue <= bound2;
		}
		return false;
	}

	public static bool? IsPointInPolygon(Vector2 point, Vector2[] polygon)
	{
		int num = polygon.Length;
		int num2 = 0;
		bool flag = false;
		float x = point.x;
		float y = point.y;
		if (num - 1 < 0)
		{
			return null;
		}
		Vector2 vector = polygon[num - 1];
		float x2 = vector.x;
		float y2 = vector.y;
		while (num2 < num)
		{
			float num3 = x2;
			float num4 = y2;
			Vector2 vector2 = polygon[num2++];
			x2 = vector2.x;
			y2 = vector2.y;
			flag ^= ((y2 > y) ^ (num4 > y)) && x - x2 < (y - y2) * (num3 - x2) / (num4 - y2);
		}
		return flag;
	}

	public static bool IsClockwise(List<Vector2> vertices)
	{
		double num = 0.0;
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 vector = vertices[i];
			Vector2 vector2 = vertices[(i + 1) % vertices.Count];
			num += (double)((vector2.x - vector.x) * (vector2.y + vector.y));
		}
		return num > 0.0;
	}
}
