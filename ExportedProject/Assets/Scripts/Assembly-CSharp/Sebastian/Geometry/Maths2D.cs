using UnityEngine;

namespace Sebastian.Geometry
{
	public static class Maths2D
	{
		public static float PseudoDistanceFromPointToLine(Vector2 a, Vector2 b, Vector2 c)
		{
			return Mathf.Abs((c.x - a.x) * (0f - b.y + a.y) + (c.y - a.y) * (b.x - a.x));
		}

		public static int SideOfLine(Vector2 a, Vector2 b, Vector2 c)
		{
			return (int)Mathf.Sign((c.x - a.x) * (0f - b.y + a.y) + (c.y - a.y) * (b.x - a.x));
		}

		public static int SideOfLine(float ax, float ay, float bx, float by, float cx, float cy)
		{
			return (int)Mathf.Sign((cx - ax) * (0f - by + ay) + (cy - ay) * (bx - ax));
		}

		public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
		{
			float num = 0.5f * ((0f - b.y) * c.x + a.y * (0f - b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
			float num2 = 1f / (2f * num) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
			float num3 = 1f / (2f * num) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
			if (num2 >= 0f && num3 >= 0f)
			{
				return num2 + num3 <= 1f;
			}
			return false;
		}

		public static bool LineSegmentsIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		{
			float num = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
			if (Mathf.Approximately(num, 0f))
			{
				return false;
			}
			float num2 = (a.y - c.y) * (d.x - c.x) - (a.x - c.x) * (d.y - c.y);
			float num3 = (a.y - c.y) * (b.x - a.x) - (a.x - c.x) * (b.y - a.y);
			if (Mathf.Approximately(num2, 0f) || Mathf.Approximately(num3, 0f))
			{
				return false;
			}
			float num4 = num2 / num;
			float num5 = num3 / num;
			if (num4 > 0f && num4 < 1f)
			{
				if (num5 > 0f)
				{
					return num5 < 1f;
				}
				return false;
			}
			return false;
		}
	}
}
