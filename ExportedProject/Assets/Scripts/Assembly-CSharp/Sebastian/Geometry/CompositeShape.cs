using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sebastian.Geometry
{
	public class CompositeShape
	{
		public class CompositeShapeData
		{
			public struct LineSegment
			{
				public readonly Vector2 a;

				public readonly Vector2 b;

				public LineSegment(Vector2 a, Vector2 b)
				{
					this.a = a;
					this.b = b;
				}
			}

			public readonly Vector2[] points;

			public readonly Polygon polygon;

			public readonly int[] triangles;

			public List<CompositeShapeData> parents = new List<CompositeShapeData>();

			public List<CompositeShapeData> holes = new List<CompositeShapeData>();

			public bool IsValidShape { get; private set; }

			public CompositeShapeData(Vector3[] points)
			{
				this.points = points.Select((Vector3 v) => new Vector2(v.x, v.z)).ToArray();
				IsValidShape = points.Length >= 3 && !IntersectsWithSelf();
				if (IsValidShape)
				{
					polygon = new Polygon(this.points);
					Triangulator triangulator = new Triangulator(polygon);
					triangles = triangulator.Triangulate();
				}
			}

			public void ValidateHoles()
			{
				for (int i = 0; i < holes.Count; i++)
				{
					for (int j = i + 1; j < holes.Count; j++)
					{
						if (holes[i].OverlapsPartially(holes[j]))
						{
							holes[i].IsValidShape = false;
							break;
						}
					}
				}
				for (int num = holes.Count - 1; num >= 0; num--)
				{
					if (!holes[num].IsValidShape)
					{
						holes.RemoveAt(num);
					}
				}
			}

			public bool IsParentOf(CompositeShapeData otherShape)
			{
				if (otherShape.parents.Contains(this))
				{
					return true;
				}
				if (parents.Contains(otherShape))
				{
					return false;
				}
				bool flag = false;
				for (int i = 0; i < triangles.Length; i += 3)
				{
					if (Maths2D.PointInTriangle(polygon.points[triangles[i]], polygon.points[triangles[i + 1]], polygon.points[triangles[i + 2]], otherShape.points[0]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
				for (int j = 0; j < points.Length; j++)
				{
					LineSegment lineSegment = new LineSegment(points[j], points[(j + 1) % points.Length]);
					for (int k = 0; k < otherShape.points.Length; k++)
					{
						LineSegment lineSegment2 = new LineSegment(otherShape.points[k], otherShape.points[(k + 1) % otherShape.points.Length]);
						if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
						{
							return false;
						}
					}
				}
				return true;
			}

			public bool OverlapsPartially(CompositeShapeData otherShape)
			{
				for (int i = 0; i < points.Length; i++)
				{
					LineSegment lineSegment = new LineSegment(points[i], points[(i + 1) % points.Length]);
					for (int j = 0; j < otherShape.points.Length; j++)
					{
						LineSegment lineSegment2 = new LineSegment(otherShape.points[j], otherShape.points[(j + 1) % otherShape.points.Length]);
						if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
						{
							return true;
						}
					}
				}
				return false;
			}

			public bool IntersectsWithSelf()
			{
				for (int i = 0; i < points.Length; i++)
				{
					LineSegment lineSegment = new LineSegment(points[i], points[(i + 1) % points.Length]);
					for (int j = i + 2; j < points.Length; j++)
					{
						if ((j + 1) % points.Length != i)
						{
							LineSegment lineSegment2 = new LineSegment(points[j], points[(j + 1) % points.Length]);
							if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		public Vector3[] vertices;

		public int[] triangles;

		private Shape[] shapes;

		private float height;

		public CompositeShape(IEnumerable<Shape> shapes)
		{
			this.shapes = shapes.ToArray();
		}

		public Mesh GetMesh()
		{
			Process();
			return new Mesh
			{
				vertices = vertices,
				triangles = triangles,
				normals = vertices.Select((Vector3 x) => Vector3.up).ToArray()
			};
		}

		public void Process()
		{
			CompositeShapeData[] array = (from x in shapes
				select new CompositeShapeData(x.points.ToArray()) into x
				where x.IsValidShape
				select x).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (i != j && array[i].IsParentOf(array[j]))
					{
						array[j].parents.Add(array[i]);
					}
				}
			}
			CompositeShapeData[] array2 = array.Where((CompositeShapeData x) => x.parents.Count % 2 != 0).ToArray();
			foreach (CompositeShapeData compositeShapeData in array2)
			{
				compositeShapeData.parents.OrderByDescending((CompositeShapeData x) => x.parents.Count).First().holes.Add(compositeShapeData);
			}
			CompositeShapeData[] array3 = array.Where((CompositeShapeData x) => x.parents.Count % 2 == 0).ToArray();
			array2 = array3;
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k].ValidateHoles();
			}
			Polygon[] array4 = array3.Select((CompositeShapeData x) => new Polygon(x.polygon.points, x.holes.Select((CompositeShapeData h) => h.polygon.points).ToArray())).ToArray();
			vertices = array4.SelectMany((Polygon x) => x.points.Select((Vector2 v2) => new Vector3(v2.x, height, v2.y))).ToArray();
			List<int> list = new List<int>();
			int num = 0;
			for (int l = 0; l < array4.Length; l++)
			{
				int[] array5 = new Triangulator(array4[l]).Triangulate();
				for (int m = 0; m < array5.Length; m++)
				{
					list.Add(array5[m] + num);
				}
				num += array4[l].numPoints;
			}
			triangles = list.ToArray();
		}
	}
}
