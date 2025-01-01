using MeshMakerNamespace;
using UnityEngine;

public class RuntimeMeshDeform : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	[SerializeField]
	private GameObject brush;

	private void Start()
	{
		CSG.EPSILON = 1E-05f;
		Debug.Log(new CSG
		{
			Brush = brush,
			Target = target,
			OperationType = CSG.Operation.Subtract,
			customMaterial = new Material(Shader.Find("Standard")),
			useCustomMaterial = false,
			hideGameObjects = true,
			keepSubmeshes = true
		}.PerformCSG().name);
	}

	private void Update()
	{
	}
}
