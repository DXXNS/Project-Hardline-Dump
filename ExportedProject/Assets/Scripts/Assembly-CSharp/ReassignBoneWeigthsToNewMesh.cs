using UnityEngine;

[ExecuteInEditMode]
public class ReassignBoneWeigthsToNewMesh : MonoBehaviour
{
	public Transform newArmature;

	public string rootBoneName = "Hips";

	public bool PressToReassign;

	private void Update()
	{
		if (PressToReassign)
		{
			Reassign();
		}
		PressToReassign = false;
	}

	public void Reassign()
	{
		if (newArmature == null)
		{
			Debug.Log("No new armature assigned");
			return;
		}
		if (newArmature.Find(rootBoneName) == null)
		{
			Debug.Log("Root bone not found");
			return;
		}
		Debug.Log("Reassingning bones");
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (!component)
		{
			return;
		}
		Transform[] bones = component.bones;
		component.rootBone = newArmature.Find(rootBoneName);
		Transform[] componentsInChildren = newArmature.GetComponentsInChildren<Transform>(includeInactive: true);
		MonoBehaviour.print("root bone " + rootBoneName);
		MonoBehaviour.print("Rend root bone " + component.rootBone);
		MonoBehaviour.print(bones);
		MonoBehaviour.print(componentsInChildren);
		for (int i = 0; i < bones.Length; i++)
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (bones[i].name == componentsInChildren[j].name)
				{
					bones[i] = componentsInChildren[j];
					break;
				}
			}
		}
		component.bones = bones;
	}

	public void ReassignClothing()
	{
		if (newArmature == null)
		{
			Debug.Log("No new armature assigned");
			return;
		}
		if (newArmature.Find(rootBoneName) == null)
		{
			Debug.Log("Root bone not found");
			return;
		}
		Debug.Log("Reassingning bones");
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (!component)
		{
			return;
		}
		Transform[] bones = component.bones;
		component.rootBone = newArmature.Find(rootBoneName);
		Transform[] componentsInChildren = newArmature.GetComponentsInChildren<Transform>(includeInactive: true);
		MonoBehaviour.print("root bone " + rootBoneName);
		MonoBehaviour.print("Rend root bone " + component.rootBone);
		MonoBehaviour.print(bones);
		MonoBehaviour.print(componentsInChildren);
		for (int i = 0; i < bones.Length; i++)
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (bones[i].name == componentsInChildren[j].name)
				{
					bones[i] = componentsInChildren[j];
					break;
				}
			}
		}
		component.bones = bones;
	}
}
