using System.Collections.Generic;
using UnityEngine;

public class ClipSegment
{
	private Vector2 enterPoint;

	private Vector2 exitPoint;

	private List<Vector2> segmentVertices = new List<Vector2>();

	private List<Vector2> hole;

	private int exitClipperVertice;

	private int enterHoleVertice;

	public Vector2 EnterPoint
	{
		get
		{
			return enterPoint;
		}
		set
		{
			enterPoint = value;
		}
	}

	public List<Vector2> SegmentVertices
	{
		get
		{
			return segmentVertices;
		}
		set
		{
			segmentVertices = value;
		}
	}

	public int ExitClipperVertice
	{
		get
		{
			return exitClipperVertice;
		}
		set
		{
			exitClipperVertice = value;
		}
	}

	public Vector2 ExitPoint
	{
		get
		{
			return exitPoint;
		}
		set
		{
			exitPoint = value;
		}
	}

	public int EnterHoleVertice
	{
		get
		{
			return enterHoleVertice;
		}
		set
		{
			enterHoleVertice = value;
		}
	}

	public List<Vector2> Hole
	{
		get
		{
			return hole;
		}
		set
		{
			hole = value;
		}
	}

	public ClipSegment()
	{
	}

	public ClipSegment(Vector2 enterPoint, Vector2 exitPoint, List<Vector2> segmentVertices, int exitClipperVertice, int enterHoleVertice)
	{
		this.enterPoint = enterPoint;
		this.exitPoint = exitPoint;
		this.segmentVertices = segmentVertices;
		this.exitClipperVertice = exitClipperVertice;
		this.enterHoleVertice = enterHoleVertice;
	}

	public void Set(ClipSegment clipSegment)
	{
		enterPoint = clipSegment.EnterPoint;
		exitPoint = clipSegment.ExitPoint;
		segmentVertices = clipSegment.SegmentVertices;
		exitClipperVertice = clipSegment.ExitClipperVertice;
		enterHoleVertice = clipSegment.EnterHoleVertice;
	}
}
