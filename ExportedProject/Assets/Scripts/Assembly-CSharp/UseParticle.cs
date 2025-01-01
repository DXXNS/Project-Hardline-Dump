using UnityEngine;

public class UseParticle : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] particleSystems;

	public void Play()
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}
}
