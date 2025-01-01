using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class NetworkRoomPlayer : NetworkBehaviour
{
	[SyncVar]
	[SerializeField]
	private string playerName;

	[SyncVar]
	[SerializeField]
	private int team;

	private MultiplayerNetworkManager networkManager;

	private bool canSwapTeam;

	public int Team
	{
		get
		{
			return team;
		}
		set
		{
			Networkteam = value;
		}
	}

	public string PlayerName
	{
		get
		{
			return playerName;
		}
		set
		{
			NetworkplayerName = value;
		}
	}

	public string NetworkplayerName
	{
		get
		{
			return playerName;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref playerName))
			{
				string text = playerName;
				SetSyncVar(value, ref playerName, 1uL);
			}
		}
	}

	public int Networkteam
	{
		get
		{
			return team;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref team))
			{
				int num = team;
				SetSyncVar(value, ref team, 2uL);
			}
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		networkManager = Object.FindObjectOfType<MultiplayerNetworkManager>();
		if (base.hasAuthority)
		{
			if (SteamManager.Initialized)
			{
				NetworkplayerName = LobbyUIManager.GetSteamName();
			}
			else
			{
				NetworkplayerName = "player";
			}
			ReplicatePlayerName(PlayerName);
			if (networkManager.Team1.Count > networkManager.Team2.Count)
			{
				Networkteam = 2;
				ReplicateTeam(2);
			}
			else
			{
				Networkteam = 1;
				ReplicateTeam(1);
			}
		}
		networkManager.UpdateTeams();
		canSwapTeam = true;
		if ((bool)Object.FindObjectOfType<ChatHandler>())
		{
			Object.FindObjectOfType<ChatHandler>().GetPlayerInfo();
		}
	}

	public void SetPlayerName(string playerName)
	{
		NetworkplayerName = playerName;
	}

	public void SetTeam(int team)
	{
		Team = team;
		if ((bool)networkManager)
		{
			networkManager.UpdateTeams();
		}
	}

	[Command]
	public void CmdSetPlayerName(string playerName)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(playerName);
		SendCommandInternal(typeof(NetworkRoomPlayer), "CmdSetPlayerName", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void RpcSetPlayerName(string playerName)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(playerName);
		SendCommandInternal(typeof(NetworkRoomPlayer), "RpcSetPlayerName", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[Command]
	public void CmdSetTeam(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendCommandInternal(typeof(NetworkRoomPlayer), "CmdSetTeam", writer, 0);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetTeam(int team)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteInt(team);
		SendRPCInternal(typeof(NetworkRoomPlayer), "RpcSetTeam", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void ReplicatePlayerName(string playerName)
	{
		if (!base.isServer && base.hasAuthority)
		{
			CmdSetPlayerName(playerName);
		}
		else if (base.isServer && base.hasAuthority)
		{
			RpcSetPlayerName(playerName);
		}
	}

	public void ReplicateTeam(int team)
	{
		if (!base.isServer)
		{
			CmdSetTeam(team);
		}
		else if (base.isServer)
		{
			RpcSetTeam(team);
		}
	}

	public void SwapTeam()
	{
		if (canSwapTeam && base.hasAuthority)
		{
			if (team == 1 && networkManager.Team2.Count < 5)
			{
				Networkteam = 2;
				ReplicateTeam(2);
				canSwapTeam = false;
				Invoke("EnableSwapTeam", 1f);
			}
			else if (team == 2 && networkManager.Team1.Count < 5)
			{
				Networkteam = 1;
				ReplicateTeam(1);
				canSwapTeam = false;
				Invoke("EnableSwapTeam", 1f);
			}
		}
	}

	public void EnableSwapTeam()
	{
		canSwapTeam = true;
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSetPlayerName(string playerName)
	{
		if (base.isServer)
		{
			SetPlayerName(playerName);
		}
	}

	protected static void InvokeUserCode_CmdSetPlayerName(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetPlayerName called on client.");
		}
		else
		{
			((NetworkRoomPlayer)obj).UserCode_CmdSetPlayerName(reader.ReadString());
		}
	}

	protected void UserCode_RpcSetPlayerName(string playerName)
	{
		if (!base.isServer)
		{
			SetPlayerName(playerName);
		}
	}

	protected static void InvokeUserCode_RpcSetPlayerName(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command RpcSetPlayerName called on client.");
		}
		else
		{
			((NetworkRoomPlayer)obj).UserCode_RpcSetPlayerName(reader.ReadString());
		}
	}

	protected void UserCode_CmdSetTeam(int team)
	{
		if (base.isServer)
		{
			SetTeam(team);
		}
	}

	protected static void InvokeUserCode_CmdSetTeam(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetTeam called on client.");
		}
		else
		{
			((NetworkRoomPlayer)obj).UserCode_CmdSetTeam(reader.ReadInt());
		}
	}

	protected void UserCode_RpcSetTeam(int team)
	{
		if (!base.isServer)
		{
			SetTeam(team);
		}
	}

	protected static void InvokeUserCode_RpcSetTeam(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetTeam called on server.");
		}
		else
		{
			((NetworkRoomPlayer)obj).UserCode_RpcSetTeam(reader.ReadInt());
		}
	}

	static NetworkRoomPlayer()
	{
		RemoteCallHelper.RegisterCommandDelegate(typeof(NetworkRoomPlayer), "CmdSetPlayerName", InvokeUserCode_CmdSetPlayerName, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(NetworkRoomPlayer), "RpcSetPlayerName", InvokeUserCode_RpcSetPlayerName, requiresAuthority: true);
		RemoteCallHelper.RegisterCommandDelegate(typeof(NetworkRoomPlayer), "CmdSetTeam", InvokeUserCode_CmdSetTeam, requiresAuthority: true);
		RemoteCallHelper.RegisterRpcDelegate(typeof(NetworkRoomPlayer), "RpcSetTeam", InvokeUserCode_RpcSetTeam);
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(playerName);
			writer.WriteInt(team);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteString(playerName);
			result = true;
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(team);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			string text = playerName;
			NetworkplayerName = reader.ReadString();
			int num = team;
			Networkteam = reader.ReadInt();
			return;
		}
		long num2 = (long)reader.ReadULong();
		if ((num2 & 1L) != 0L)
		{
			string text2 = playerName;
			NetworkplayerName = reader.ReadString();
		}
		if ((num2 & 2L) != 0L)
		{
			int num3 = team;
			Networkteam = reader.ReadInt();
		}
	}
}
