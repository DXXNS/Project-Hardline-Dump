using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
	private static float AIM_KP = 0.2f;

	private static float HOR_OFFSET = 0f;

	private Rigidbody rigidBody;

	private Collider collider;

	private bool flyMode;

	private float lookY;

	private float lookX;

	private int spectating;

	private Human player;

	private List<Human> spectatablePlayers = new List<Human>();

	[SerializeField]
	private Human currentlySpectating;

	[SerializeField]
	private float spectatorSpeed;

	private float baseSensitivity = 1.6f;

	private Vector3 followCameraOffset = new Vector3(0f, 1.9f, 0f);

	private Vector3 followHeadCamPosOffset = new Vector3(0f, 0.35f, 0.08f);

	private Vector3 followHeadCamRotOffset = new Vector3(0f, 0f, 0f);

	private Player localPlayer;

	private Human[] players;

	public Human CurrentlySpectating
	{
		get
		{
			return currentlySpectating;
		}
		set
		{
			currentlySpectating = value;
		}
	}

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		players = Object.FindObjectsOfType<Human>();
		Human[] array = players;
		foreach (Human human in array)
		{
			if (human.hasAuthority)
			{
				player = human;
			}
		}
		array = players;
		foreach (Human human2 in array)
		{
			if (human2.Team == player.Team && human2.Health > 0f)
			{
				spectatablePlayers.Add(human2);
			}
		}
		localPlayer = Object.FindObjectOfType<HardlineGameManager>().GetLocalPlayer();
	}

	private void Update()
	{
		if (!flyMode)
		{
			FollowPlayerMode();
			return;
		}
		lookY = Mathf.Clamp(lookY, -80f, 70f);
		base.transform.eulerAngles = new Vector3(lookY, lookX, 0f);
	}

	private void FixedUpdate()
	{
		rigidBody.isKinematic = false;
		if (flyMode)
		{
			FlyModeFixedUpdate();
		}
	}

	public void FlyModeFixedUpdate()
	{
		if (Input.GetKey(KeyCode.W))
		{
			rigidBody.AddForce(base.transform.forward * spectatorSpeed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			rigidBody.AddForce(base.transform.forward * (0f - spectatorSpeed));
		}
		if (Input.GetKey(KeyCode.A))
		{
			rigidBody.AddForce(base.transform.right * (0f - spectatorSpeed));
		}
		if (Input.GetKey(KeyCode.D))
		{
			rigidBody.AddForce(base.transform.right * spectatorSpeed);
		}
		if (Input.GetKey(KeyCode.Space))
		{
			rigidBody.AddForce(Vector3.up * spectatorSpeed);
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
		{
			rigidBody.AddForce(Vector3.up * (0f - spectatorSpeed));
		}
		rigidBody.velocity = new Vector3(0f, 0f, 0f);
	}

	private void OnDestroy()
	{
		if ((bool)Object.FindObjectOfType<UserInterface>() && (bool)CurrentlySpectating)
		{
			Object.FindObjectOfType<UserInterface>().UpdateSpectateText("");
		}
	}

	public void FollowPlayerMode()
	{
		foreach (Human spectatablePlayer in spectatablePlayers)
		{
			if (spectatablePlayer.Health <= 0f)
			{
				spectatablePlayers.Remove(spectatablePlayer);
				if (spectatablePlayer != CurrentlySpectating)
				{
					spectating = spectatablePlayers.IndexOf(CurrentlySpectating);
				}
			}
			spectatablePlayer.SpectatorCamera = this;
		}
		if (spectatablePlayers.Count >= 1)
		{
			if (Input.GetMouseButtonDown(0))
			{
				spectating++;
			}
			else if (Input.GetMouseButtonDown(1))
			{
				spectating--;
			}
			if (spectating > spectatablePlayers.Count - 1)
			{
				spectating = 0;
			}
			if (spectating < 0)
			{
				spectating = spectatablePlayers.Count - 1;
			}
			CurrentlySpectating = spectatablePlayers[spectating];
			if ((bool)currentlySpectating.GetComponent<Human>())
			{
				Human component = currentlySpectating.GetComponent<Human>();
				base.transform.position = component.Neck.transform.TransformPoint(followHeadCamPosOffset);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(new Vector3(component.Spine3.transform.eulerAngles.x, component.SyncedAimX, 0f)) * Quaternion.Euler(0f, HOR_OFFSET, 0f), AIM_KP);
			}
			else
			{
				base.transform.position = CurrentlySpectating.transform.position + followCameraOffset;
			}
			Object.FindObjectOfType<UserInterface>().UpdateSpectateText(CurrentlySpectating.HumanName);
		}
		else
		{
			Object.FindObjectOfType<UserInterface>().UpdateSpectateText("No one");
			rigidBody.velocity = new Vector3(0f, 0f, 0f);
		}
	}
}
