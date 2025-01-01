using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material Map", menuName = "ResonanceAudio/Material Map", order = 1000)]
public class ResonanceAudioMaterialMap : ScriptableObject
{
	public static readonly Color[] surfaceMaterialColors = new Color[23]
	{
		new Color(0.5f, 0.5f, 0.5f),
		new Color(0.545098f, 0.909804f, 0.678431f),
		new Color(0.184314f, 0.258824f, 0.521569f),
		new Color(0.552941f, 0.737255f, 0.976471f),
		new Color(0.035294f, 0.376471f, 0.07451f),
		new Color(0.952941f, 0.415686f, 0.835294f),
		new Color(0.105882f, 0.894118f, 0.427451f),
		new Color(0.541176f, 0.015686f, 0.345098f),
		new Color(0.631373f, 0.847059f, 0.196078f),
		new Color(0.513725f, 0.003922f, 0.741176f),
		new Color(0.94902f, 0.690196f, 0.964706f),
		new Color(0.082353f, 0.305882f, 0.337255f),
		new Color(0.152941f, 0.792157f, 0.901961f),
		new Color(0.921569f, 0.070588f, 0.254902f),
		new Color(0.27451f, 0.635294f, 0.423529f),
		new Color(0.556863f, 0.215686f, 0.066667f),
		new Color(0.960784f, 0.803922f, 0.686275f),
		new Color(0.305882f, 0.282353f, 0.035294f),
		new Color(0.917647f, 0.839216f, 0.141176f),
		new Color(0.521569f, 0.458824f, 0.858824f),
		new Color(0.937255f, 0.592157f, 0.176471f),
		new Color(0.980392f, 0.105882f, 0.988235f),
		new Color(0.72549f, 0.423529f, 0.552941f)
	};

	[SerializeField]
	private ResonanceAudioRoomManager.SurfaceMaterialDictionary surfaceMaterialFromGuid;

	private const ResonanceAudioRoomManager.SurfaceMaterial defaultSurfaceMaterial = ResonanceAudioRoomManager.SurfaceMaterial.Transparent;

	public List<string> GuidList()
	{
		return surfaceMaterialFromGuid.Keys.ToList();
	}

	public ResonanceAudioRoomManager.SurfaceMaterial GetMaterialFromGuid(string guid)
	{
		return surfaceMaterialFromGuid[guid];
	}

	public void AddDefaultMaterialIfGuidUnmapped(string guid)
	{
		if (!surfaceMaterialFromGuid.ContainsKey(guid))
		{
			surfaceMaterialFromGuid.Add(guid, ResonanceAudioRoomManager.SurfaceMaterial.Transparent);
		}
	}
}
