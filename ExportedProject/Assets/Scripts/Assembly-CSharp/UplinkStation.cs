using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

public class UplinkStation : NetworkBehaviour
{
	private static float pointsToWin;

	private float progressRate;

	[SerializeField]
	private ResonanceAudioSource alarmSound;

	[SerializeField]
	private GameObject lockedIcon;

	[SerializeField]
	private GameObject neutralIcon;

	[SerializeField]
	private GameObject blueTeamIcon;

	[SerializeField]
	private GameObject redTeamIcon;

	[SerializeField]
	private Slider blueProgressSlider;

	[SerializeField]
	private Slider redProgressSlider;

	[SerializeField]
	[SyncVar]
	private int controlState;

	[SyncVar]
	private float blueProgress;

	[SyncVar]
	private float redProgress;

	[SerializeField]
	private bool visible;

	[SerializeField]
	private bool interactable;

	[SyncVar]
	private bool activeUplink;

	[SerializeField]
	private MeshRenderer[] meshes;

	[SerializeField]
	private Collider[] colliders;

	private RoundsHardlineGameManager gameManager;

	private bool uplinkFlag = true;

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public bool Interactable
	{
		get
		{
			return interactable;
		}
		set
		{
			interactable = value;
		}
	}

	public int ControlState
	{
		get
		{
			return controlState;
		}
		set
		{
			NetworkcontrolState = value;
		}
	}

	public int NetworkcontrolState
	{
		get
		{
			return controlState;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref controlState))
			{
				int num = controlState;
				SetSyncVar(value, ref controlState, 1uL);
			}
		}
	}

	public float NetworkblueProgress
	{
		get
		{
			return blueProgress;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref blueProgress))
			{
				float num = blueProgress;
				SetSyncVar(value, ref blueProgress, 2uL);
			}
		}
	}

	public float NetworkredProgress
	{
		get
		{
			return redProgress;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref redProgress))
			{
				float num = redProgress;
				SetSyncVar(value, ref redProgress, 4uL);
			}
		}
	}

	public bool NetworkactiveUplink
	{
		get
		{
			return activeUplink;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref activeUplink))
			{
				bool flag = activeUplink;
				SetSyncVar(value, ref activeUplink, 8uL);
			}
		}
	}

	public void ReplicateActiveUplink(bool active)
	{
		NetworkactiveUplink = active;
		if (!base.isServer)
		{
			CmdActiveUplink(active);
		}
		else if (base.isServer)
		{
			RpcActiveUplink(active);
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdActiveUplink(bool active)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(active);
		SendCommandInternal(typeof(UplinkStation), "CmdActiveUplink", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcActiveUplink(bool active)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteBool(active);
		SendRPCInternal(typeof(UplinkStation), "RpcActiveUplink", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void EnableUplink()
	{
		MeshRenderer[] array = meshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		Collider[] array2 = colliders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
	}

	public void DisableUplink()
	{
		MeshRenderer[] array = meshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		Collider[] array2 = colliders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
	}

	public void ReplicateResetUplink()
	{
		if (!base.isServer)
		{
			CmdResetUpLink();
		}
		else if (base.isServer)
		{
			ResetValues();
			RpcResetUpLink();
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdResetUpLink()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(UplinkStation), "CmdResetUpLink", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcResetUpLink()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(UplinkStation), "RpcResetUpLink", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void ReplicateInteract(int team)
	{
		Interact(team);
		if (!base.isServer)
		{
			CmdInteract(team);
		}
		else if (base.isServer)
		{
			Interact(team);
			RpcInteract(team);
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdInteract(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendCommandInternal(typeof(UplinkStation), "CmdInteract", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcInteract(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendRPCInternal(typeof(UplinkStation), "RpcInteract", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void ReplicateProgress(float blueProgress, float redProgress)
	{
		if (base.isServer)
		{
			RpcProgress(blueProgress, redProgress);
		}
	}

	[ClientRpc]
	public void RpcProgress(float blueProgress, float redProgress)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(blueProgress);
		writer.WriteFloat(redProgress);
		SendRPCInternal(typeof(UplinkStation), "RpcProgress", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	private void Awake()
	{
		NetworkactiveUplink = false;
		DisableUplink();
	}

	private void Start()
	{
		gameManager = Object.FindObjectOfType<RoundsHardlineGameManager>();
		Visible = false;
		Interactable = false;
		if (base.isServer)
		{
			InvokeRepeating("CheckPossession", 1f, 1f);
		}
		if ((bool)gameManager)
		{
			progressRate = gameManager.UplinkProgressRate;
		}
	}

	private void Update()
	{
		if (!gameManager)
		{
			gameManager = Object.FindObjectOfType<RoundsHardlineGameManager>();
		}
		UpdateIconVisibilities();
		blueProgressSlider.value = blueProgress;
		redProgressSlider.value = redProgress;
		UpdateAudio();
		CheckWins();
		if (activeUplink)
		{
			if (!uplinkFlag)
			{
				uplinkFlag = true;
				EnableUplink();
			}
		}
		else if (uplinkFlag)
		{
			uplinkFlag = false;
			DisableUplink();
		}
	}

	public void UpdateAudio()
	{
		if (ControlState == 0)
		{
			alarmSound.audioSource.Stop();
		}
		else if (ControlState == 1 && !alarmSound.audioSource.isPlaying)
		{
			alarmSound.audioSource.Play();
		}
		else if (ControlState == 2 && !alarmSound.audioSource.isPlaying)
		{
			alarmSound.audioSource.Play();
		}
	}

	public void UpdateIconVisibilities()
	{
		if (Visible && activeUplink)
		{
			if (Interactable)
			{
				if (ControlState == 0)
				{
					lockedIcon.SetActive(value: false);
					neutralIcon.SetActive(value: true);
					blueTeamIcon.SetActive(value: false);
					redTeamIcon.SetActive(value: false);
				}
				else if (ControlState == 1)
				{
					lockedIcon.SetActive(value: false);
					neutralIcon.SetActive(value: false);
					blueTeamIcon.SetActive(value: true);
					redTeamIcon.SetActive(value: false);
				}
				else if (ControlState == 2)
				{
					lockedIcon.SetActive(value: false);
					neutralIcon.SetActive(value: false);
					blueTeamIcon.SetActive(value: false);
					redTeamIcon.SetActive(value: true);
				}
			}
			else
			{
				lockedIcon.SetActive(value: true);
				neutralIcon.SetActive(value: false);
				blueTeamIcon.SetActive(value: false);
				redTeamIcon.SetActive(value: false);
			}
		}
		else
		{
			lockedIcon.SetActive(value: false);
			neutralIcon.SetActive(value: false);
			blueTeamIcon.SetActive(value: false);
			redTeamIcon.SetActive(value: false);
		}
	}

	public void CheckWins()
	{
		if ((bool)gameManager && base.isServer)
		{
			if (blueProgress >= pointsToWin && !gameManager.RoundEndFlag && base.isServer)
			{
				gameManager.ReplicateSubGameWin(1);
			}
			else if (redProgress >= pointsToWin && !gameManager.RoundEndFlag && base.isServer)
			{
				gameManager.ReplicateSubGameWin(2);
			}
		}
	}

	public void CheckPossession()
	{
		if (ControlState == 1)
		{
			NetworkblueProgress = blueProgress + progressRate;
		}
		else if (ControlState == 2)
		{
			NetworkredProgress = redProgress + progressRate;
		}
		if (base.isServer)
		{
			ReplicateProgress(blueProgress, redProgress);
		}
	}

	public void Interact(int team)
	{
		if (Interactable && ControlState != team)
		{
			ControlState = team;
			gameManager.UpdateUplinkInteract(ControlState);
		}
	}

	public void ResetUplink()
	{
		ResetValues();
		ReplicateResetUplink();
	}

	public void ResetValues()
	{
		Visible = false;
		Interactable = false;
		NetworkblueProgress = 0f;
		NetworkredProgress = 0f;
		ControlState = 0;
	}

	static UplinkStation()
	{
		pointsToWin = 100f;
		RemoteCallHelper.RegisterCommandDelegate(typeof(UplinkStation), "CmdActiveUplink", InvokeUserCode_CmdActiveUplink, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(UplinkStation), "CmdResetUpLink", InvokeUserCode_CmdResetUpLink, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(UplinkStation), "CmdInteract", InvokeUserCode_CmdInteract, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(UplinkStation), "RpcActiveUplink", InvokeUserCode_RpcActiveUplink);
		RemoteCallHelper.RegisterRpcDelegate(typeof(UplinkStation), "RpcResetUpLink", InvokeUserCode_RpcResetUpLink);
		RemoteCallHelper.RegisterRpcDelegate(typeof(UplinkStation), "RpcInteract", InvokeUserCode_RpcInteract);
		RemoteCallHelper.RegisterRpcDelegate(typeof(UplinkStation), "RpcProgress", InvokeUserCode_RpcProgress);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdActiveUplink(bool active)
	{
		if (base.isServer)
		{
			NetworkactiveUplink = active;
		}
		if (active)
		{
			EnableUplink();
		}
		else
		{
			DisableUplink();
		}
	}

	protected static void InvokeUserCode_CmdActiveUplink(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdActiveUplink called on client.");
		}
		else
		{
			((UplinkStation)obj).UserCode_CmdActiveUplink(reader.ReadBool());
		}
	}

	protected void UserCode_RpcActiveUplink(bool active)
	{
		if (!base.isServer)
		{
			NetworkactiveUplink = active;
		}
	}

	protected static void InvokeUserCode_RpcActiveUplink(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcActiveUplink called on server.");
		}
		else
		{
			((UplinkStation)obj).UserCode_RpcActiveUplink(reader.ReadBool());
		}
	}

	protected void UserCode_CmdResetUpLink()
	{
		if (base.isServer)
		{
			ResetValues();
		}
	}

	protected static void InvokeUserCode_CmdResetUpLink(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdResetUpLink called on client.");
		}
		else
		{
			((UplinkStation)obj).UserCode_CmdResetUpLink();
		}
	}

	protected void UserCode_RpcResetUpLink()
	{
		if (!base.isServer)
		{
			ResetValues();
		}
	}

	protected static void InvokeUserCode_RpcResetUpLink(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcResetUpLink called on server.");
		}
		else
		{
			((UplinkStation)obj).UserCode_RpcResetUpLink();
		}
	}

	protected void UserCode_CmdInteract(int team)
	{
		if (base.isServer)
		{
			Interact(team);
		}
	}

	protected static void InvokeUserCode_CmdInteract(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInteract called on client.");
		}
		else
		{
			((UplinkStation)obj).UserCode_CmdInteract(reader.ReadInt());
		}
	}

	protected void UserCode_RpcInteract(int team)
	{
		if (!base.isServer)
		{
			Interact(team);
		}
	}

	protected static void InvokeUserCode_RpcInteract(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInteract called on server.");
		}
		else
		{
			((UplinkStation)obj).UserCode_RpcInteract(reader.ReadInt());
		}
	}

	protected void UserCode_RpcProgress(float blueProgress, float redProgress)
	{
		NetworkblueProgress = blueProgress;
		NetworkredProgress = redProgress;
	}

	protected static void InvokeUserCode_RpcProgress(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcProgress called on server.");
		}
		else
		{
			((UplinkStation)obj).UserCode_RpcProgress(reader.ReadFloat(), reader.ReadFloat());
		}
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(controlState);
			writer.WriteFloat(blueProgress);
			writer.WriteFloat(redProgress);
			writer.WriteBool(activeUplink);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(controlState);
			result = true;
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(blueProgress);
			result = true;
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(redProgress);
			result = true;
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(activeUplink);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			int num = controlState;
			NetworkcontrolState = reader.ReadInt();
			float num2 = blueProgress;
			NetworkblueProgress = reader.ReadFloat();
			float num3 = redProgress;
			NetworkredProgress = reader.ReadFloat();
			bool flag = activeUplink;
			NetworkactiveUplink = reader.ReadBool();
			return;
		}
		long num4 = (long)reader.ReadULong();
		if ((num4 & 1L) != 0L)
		{
			int num5 = controlState;
			NetworkcontrolState = reader.ReadInt();
		}
		if ((num4 & 2L) != 0L)
		{
			float num6 = blueProgress;
			NetworkblueProgress = reader.ReadFloat();
		}
		if ((num4 & 4L) != 0L)
		{
			float num7 = redProgress;
			NetworkredProgress = reader.ReadFloat();
		}
		if ((num4 & 8L) != 0L)
		{
			bool flag2 = activeUplink;
			NetworkactiveUplink = reader.ReadBool();
		}
	}
}
