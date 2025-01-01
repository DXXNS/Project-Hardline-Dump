using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using UnityEngine;

public class HardlineGameManager : NetworkBehaviour
{
	private static float deltaTimeFrameSpeedConstant;

	public static CSteamID currentLobby;

	[SerializeField]
	protected List<GameObject> team1Spawns = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> team2Spawns = new List<GameObject>();

	private Player localPlayer;

	[SerializeField]
	private int playerSupplyPoints;

	[SerializeField]
	[SyncVar]
	private int playersLoaded;

	protected bool loadedAllPlayersFlag;

	protected List<Human> humans = new List<Human>();

	public List<GameObject> Team1Spawns
	{
		get
		{
			return team1Spawns;
		}
		set
		{
			team1Spawns = value;
		}
	}

	public List<GameObject> Team2Spawns
	{
		get
		{
			return team2Spawns;
		}
		set
		{
			team2Spawns = value;
		}
	}

	public static float DeltaTimeFrameSpeedConstant
	{
		get
		{
			return deltaTimeFrameSpeedConstant;
		}
		set
		{
			deltaTimeFrameSpeedConstant = value;
		}
	}

	public int PlayerSupplyPoints
	{
		get
		{
			return playerSupplyPoints;
		}
		set
		{
			playerSupplyPoints = value;
		}
	}

	public int PlayersLoaded
	{
		get
		{
			return playersLoaded;
		}
		set
		{
			NetworkplayersLoaded = value;
		}
	}

	public Player LocalPlayer
	{
		get
		{
			return localPlayer;
		}
		set
		{
			localPlayer = value;
		}
	}

	public int NetworkplayersLoaded
	{
		get
		{
			return playersLoaded;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref playersLoaded))
			{
				int num = playersLoaded;
				SetSyncVar(value, ref playersLoaded, 1uL);
			}
		}
	}

	protected virtual void Start()
	{
		AddLoadedPlayer();
	}

	[ClientRpc]
	public void RpcServerClose()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(HardlineGameManager), "RpcServerClose", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void InitSceneObjects()
	{
		Door[] array = Object.FindObjectsOfType<Door>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetClose();
		}
		MultiplayerGlassInstance[] array2 = Object.FindObjectsOfType<MultiplayerGlassInstance>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].CreateGlass();
		}
		Explosive[] array3 = Object.FindObjectsOfType<Explosive>();
		foreach (Explosive explosive in array3)
		{
			if (base.isServer)
			{
				NetworkServer.Destroy(explosive.gameObject);
			}
		}
	}

	public void SpendSupplyPoints(int value)
	{
		playerSupplyPoints -= value;
	}

	protected virtual void Update()
	{
		if (LocalPlayer == null)
		{
			InitPlayersAndFindLocalPlayer();
		}
		UpdateCursorState();
		UpdatePlayerLockState();
	}

	public void InitPlayersAndFindLocalPlayer()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			player.Init();
			if (player.isLocalPlayer)
			{
				LocalPlayer = player;
			}
		}
	}

	public void FindPlayers()
	{
		humans.Clear();
		Human[] array = Object.FindObjectsOfType<Human>();
		foreach (Human item in array)
		{
			humans.Add(item);
		}
	}

	public virtual void UpdateCursorState()
	{
		if ((bool)LocalPlayer)
		{
			if (LocalPlayer.MenuOpened)
			{
				Object.FindObjectOfType<UserInterface>().EnableCursor();
			}
			else
			{
				Object.FindObjectOfType<UserInterface>().DisableCursor();
			}
		}
	}

	public virtual void AllPlayersLoaded()
	{
	}

	public PlayerItem GenerateItem(string itemName)
	{
		GameObject gameObject = GameObject.Find("LoadedWeapons");
		PlayerItem playerItem = Object.Instantiate((Resources.Load(itemName) as GameObject).GetComponent<PlayerItem>());
		if ((bool)GameObject.Find("LoadedWeapons"))
		{
			playerItem.transform.parent = gameObject.transform;
		}
		playerItem.gameObject.SetActive(value: false);
		return playerItem;
	}

	public PlayerItem GenerateItem(string itemName, int sight, int muzzle, int grip)
	{
		GameObject gameObject = GameObject.Find("LoadedWeapons");
		PlayerItem playerItem = Object.Instantiate((Resources.Load(itemName) as GameObject).GetComponent<PlayerItem>());
		if (playerItem is PlayerFirearm)
		{
			(playerItem as PlayerFirearm).SetSightAttachment(sight);
			(playerItem as PlayerFirearm).SetBarrelAttachment(muzzle);
			(playerItem as PlayerFirearm).SetGripAttachment(grip);
		}
		if ((bool)GameObject.Find("LoadedWeapons"))
		{
			playerItem.transform.parent = gameObject.transform;
		}
		playerItem.gameObject.SetActive(value: false);
		return playerItem;
	}

	public void ClearGeneratedWeapons()
	{
		_ = (bool)GameObject.Find("LoadedWeapons");
		PlayerItem[] componentsInChildren = GameObject.Find("LoadedWeapons").GetComponentsInChildren<PlayerItem>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	public virtual void UpdatePlayerLockState()
	{
		if ((bool)LocalPlayer)
		{
			if (LocalPlayer.MenuOpened)
			{
				LocalPlayer.InputEnabled = false;
				LocalPlayer.LookEnabled = false;
			}
			else
			{
				LocalPlayer.InputEnabled = true;
				LocalPlayer.LookEnabled = true;
			}
		}
	}

	public Transform GetSpawnPositionForTeam(int team)
	{
		return team switch
		{
			1 => team1Spawns[Random.Range(0, team1Spawns.Count)].transform, 
			2 => team2Spawns[Random.Range(0, team1Spawns.Count)].transform, 
			_ => team1Spawns[Random.Range(0, team1Spawns.Count)].transform, 
		};
	}

	public virtual void RestartGameCharacter()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.hasAuthority)
			{
				player.ResetCharacter();
				player.ReplicateReset();
			}
		}
	}

	public void GivePlayerLoadout(PlayerItem primary, PlayerItem secondary)
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.hasAuthority)
			{
				player.HumanInventory.SetInventory(primary, secondary, fullAmmo: true);
				if (player.HumanInventory.PrimaryItem.ItemName == "Unarmed" && player.HumanInventory.SecondaryItem.ItemName != "Unarmed")
				{
					player.SelectFromInventory(2, equipForNewItem: true, init: true);
				}
				else
				{
					player.SelectFromInventory(1, equipForNewItem: true, init: true);
				}
			}
		}
	}

	public void GivePlayerLoadout(PlayerItem primary, PlayerItem secondary, PlayerItem equipment1, PlayerItem equipment2, PlayerItem meleeWeapon)
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.hasAuthority)
			{
				player.HumanInventory.SetInventory(primary, secondary, equipment1, equipment2, meleeWeapon, fullAmmo: true);
				if (player.HumanInventory.PrimaryItem.ItemName == "Unarmed" && player.HumanInventory.SecondaryItem.ItemName != "Unarmed")
				{
					player.SelectFromInventory(2, equipForNewItem: true, init: true);
				}
				else
				{
					player.SelectFromInventory(1, equipForNewItem: true, init: true);
				}
			}
		}
	}

	public void CallServerSpawnObject(string networkObject, Vector3 position, Vector3 rotation, Human causer)
	{
		if (base.isServer)
		{
			ServerSpawnObject(networkObject, position, rotation, causer);
		}
		else
		{
			CmdCallServerSpawnObject(networkObject, position, rotation, causer);
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdCallServerSpawnObject(string networkObject, Vector3 position, Vector3 rotation, Human causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(networkObject);
		writer.WriteVector3(position);
		writer.WriteVector3(rotation);
		writer.WriteNetworkBehaviour(causer);
		SendCommandInternal(typeof(HardlineGameManager), "CmdCallServerSpawnObject", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	public void ServerSpawnObject(string networkObject, Vector3 position, Vector3 rotation, Human causer)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("SpawnablePrefabs/" + networkObject) as GameObject, position, Quaternion.Euler(rotation));
		NetworkServer.Spawn(gameObject);
		if ((bool)gameObject.GetComponent<Explosive>())
		{
			gameObject.GetComponent<Explosive>().MyCauser = causer;
		}
		if ((bool)gameObject.GetComponent<Explosion>())
		{
			gameObject.GetComponent<Explosion>().MyCauser = causer;
		}
	}

	public void ServerSpawnObject(string networkObject, Vector3 position, Quaternion rotation, Human causer)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("SpawnablePrefabs/" + networkObject) as GameObject, position, rotation);
		NetworkServer.Spawn(gameObject);
		if ((bool)gameObject.GetComponent<Explosive>())
		{
			gameObject.GetComponent<Explosive>().MyCauser = causer;
		}
		if ((bool)gameObject.GetComponent<Explosion>())
		{
			gameObject.GetComponent<Explosion>().MyCauser = causer;
		}
	}

	public void CallServerSpawnObjectForward(string networkObject, Vector3 position, Vector3 rotation, float force, Human causer)
	{
		if (base.isServer)
		{
			ServerSpawnObjectForward(networkObject, position, rotation, force, causer);
		}
		else
		{
			CmdServerSpawnObjectForward(networkObject, position, rotation, force, causer);
		}
	}

	public void CallServerSpawnObject(string networkObject, Vector3 position, Quaternion rotation, Human causer)
	{
		if (base.isServer)
		{
			ServerSpawnObject(networkObject, position, rotation, causer);
		}
		else
		{
			CmdServerSpawnObject(networkObject, position, rotation, causer);
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdServerSpawnObjectForward(string networkObject, Vector3 position, Vector3 rotation, float force, Human causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(networkObject);
		writer.WriteVector3(position);
		writer.WriteVector3(rotation);
		writer.WriteFloat(force);
		writer.WriteNetworkBehaviour(causer);
		SendCommandInternal(typeof(HardlineGameManager), "CmdServerSpawnObjectForward", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdServerSpawnObject(string networkObject, Vector3 position, Quaternion rotation, Human causer)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(networkObject);
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteNetworkBehaviour(causer);
		SendCommandInternal(typeof(HardlineGameManager), "CmdServerSpawnObject", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	public void ServerSpawnObjectForward(string networkObject, Vector3 position, Vector3 rotation, float force, Human causer)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("SpawnablePrefabs/" + networkObject) as GameObject, position, Quaternion.Euler(rotation));
		NetworkServer.Spawn(gameObject);
		gameObject.GetComponent<Rigidbody>().AddForce(force * gameObject.transform.forward);
		if ((bool)gameObject.GetComponent<Explosion>())
		{
			gameObject.GetComponent<Explosion>().MyCauser = causer;
		}
		if ((bool)gameObject.GetComponent<Explosive>())
		{
			gameObject.GetComponent<Explosive>().MyCauser = causer;
		}
	}

	public void CallHitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, float damage)
	{
		if (!(target.Health < 0f))
		{
			if (base.isServer)
			{
				HitAnotherPlayer(causer, target, hitPos, hitRot, target.Health - damage <= 0f);
				RpcCallHitAnotherPlayer(causer, target, hitPos, hitRot, target.Health - damage <= 0f);
				causer.Item.User.DamageAnotherPlayer(target, damage, hitPos, hitRot);
			}
			else
			{
				CmdCallHitAnotherPlayer(causer, target, hitPos, hitRot, damage);
			}
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdCallHitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, float damage)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteNetworkBehaviour(causer);
		writer.WriteNetworkBehaviour(target);
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		writer.WriteFloat(damage);
		SendCommandInternal(typeof(HardlineGameManager), "CmdCallHitAnotherPlayer", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcCallHitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, bool killShot)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteNetworkBehaviour(causer);
		writer.WriteNetworkBehaviour(target);
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		writer.WriteBool(killShot);
		SendRPCInternal(typeof(HardlineGameManager), "RpcCallHitAnotherPlayer", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void HitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, bool killShot)
	{
		if (causer == target)
		{
			return;
		}
		Human[] array = Object.FindObjectsOfType<Human>();
		foreach (Human human in array)
		{
			if (human.hasAuthority && human == causer)
			{
				causer.Item.HitAnotherTarget(killShot, target);
				if (human.Team != target.Team)
				{
					human.AppendToPlayersDamaged(target);
				}
			}
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdAddPlayerLoaded()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(HardlineGameManager), "CmdAddPlayerLoaded", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcAddPlayerLoaded()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(HardlineGameManager), "RpcAddPlayerLoaded", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void AddLoadedPlayer()
	{
		if (base.isServer)
		{
			PlayersLoaded++;
			RpcAddPlayerLoaded();
		}
		else
		{
			CmdAddPlayerLoaded();
		}
	}

	public void DescreasePlayersLoaded()
	{
		NetworkplayersLoaded = playersLoaded - 1;
	}

	public Player GetLocalPlayer()
	{
		Player[] array = Object.FindObjectsOfType<Player>();
		foreach (Player player in array)
		{
			if (player.isLocalPlayer)
			{
				return player;
			}
		}
		return null;
	}

	static HardlineGameManager()
	{
		deltaTimeFrameSpeedConstant = 60f;
		RemoteCallHelper.RegisterCommandDelegate(typeof(HardlineGameManager), "CmdCallServerSpawnObject", InvokeUserCode_CmdCallServerSpawnObject, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(HardlineGameManager), "CmdServerSpawnObjectForward", InvokeUserCode_CmdServerSpawnObjectForward, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(HardlineGameManager), "CmdServerSpawnObject", InvokeUserCode_CmdServerSpawnObject, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(HardlineGameManager), "CmdCallHitAnotherPlayer", InvokeUserCode_CmdCallHitAnotherPlayer, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(HardlineGameManager), "CmdAddPlayerLoaded", InvokeUserCode_CmdAddPlayerLoaded, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(HardlineGameManager), "RpcServerClose", InvokeUserCode_RpcServerClose);
		RemoteCallHelper.RegisterRpcDelegate(typeof(HardlineGameManager), "RpcCallHitAnotherPlayer", InvokeUserCode_RpcCallHitAnotherPlayer);
		RemoteCallHelper.RegisterRpcDelegate(typeof(HardlineGameManager), "RpcAddPlayerLoaded", InvokeUserCode_RpcAddPlayerLoaded);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_RpcServerClose()
	{
		if (!base.isServer && (bool)Object.FindObjectOfType<HardlineNetworkManager>())
		{
			Object.FindObjectOfType<HardlineNetworkManager>().LeaveToLobbyAndDisconnect();
		}
	}

	protected static void InvokeUserCode_RpcServerClose(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcServerClose called on server.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_RpcServerClose();
		}
	}

	protected void UserCode_CmdCallServerSpawnObject(string networkObject, Vector3 position, Vector3 rotation, Human causer)
	{
		ServerSpawnObject(networkObject, position, rotation, causer);
	}

	protected static void InvokeUserCode_CmdCallServerSpawnObject(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCallServerSpawnObject called on client.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_CmdCallServerSpawnObject(reader.ReadString(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadNetworkBehaviour<Human>());
		}
	}

	protected void UserCode_CmdServerSpawnObjectForward(string networkObject, Vector3 position, Vector3 rotation, float force, Human causer)
	{
		ServerSpawnObjectForward(networkObject, position, rotation, force, causer);
	}

	protected static void InvokeUserCode_CmdServerSpawnObjectForward(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdServerSpawnObjectForward called on client.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_CmdServerSpawnObjectForward(reader.ReadString(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadNetworkBehaviour<Human>());
		}
	}

	protected void UserCode_CmdServerSpawnObject(string networkObject, Vector3 position, Quaternion rotation, Human causer)
	{
		ServerSpawnObject(networkObject, position, rotation, causer);
	}

	protected static void InvokeUserCode_CmdServerSpawnObject(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdServerSpawnObject called on client.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_CmdServerSpawnObject(reader.ReadString(), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadNetworkBehaviour<Human>());
		}
	}

	protected void UserCode_CmdCallHitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, float damage)
	{
		if (!(target.Health < 0f))
		{
			HitAnotherPlayer(causer, target, hitPos, hitRot, target.Health - damage <= 0f);
			RpcCallHitAnotherPlayer(causer, target, hitPos, hitRot, target.Health - damage <= 0f);
			causer.Item.User.DamageAnotherPlayer(target, damage, hitPos, hitRot);
		}
	}

	protected static void InvokeUserCode_CmdCallHitAnotherPlayer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCallHitAnotherPlayer called on client.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_CmdCallHitAnotherPlayer(reader.ReadNetworkBehaviour<Human>(), reader.ReadNetworkBehaviour<Human>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat());
		}
	}

	protected void UserCode_RpcCallHitAnotherPlayer(Human causer, Human target, Vector3 hitPos, Vector3 hitRot, bool killShot)
	{
		if ((bool)target && (bool)causer && !(target.Health < 0f))
		{
			HitAnotherPlayer(causer, target, hitPos, hitRot, killShot);
		}
	}

	protected static void InvokeUserCode_RpcCallHitAnotherPlayer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcCallHitAnotherPlayer called on server.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_RpcCallHitAnotherPlayer(reader.ReadNetworkBehaviour<Human>(), reader.ReadNetworkBehaviour<Human>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdAddPlayerLoaded()
	{
		if (base.isServer)
		{
			PlayersLoaded++;
			RpcAddPlayerLoaded();
		}
	}

	protected static void InvokeUserCode_CmdAddPlayerLoaded(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdAddPlayerLoaded called on client.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_CmdAddPlayerLoaded();
		}
	}

	protected void UserCode_RpcAddPlayerLoaded()
	{
		if (!base.isServer)
		{
			PlayersLoaded++;
		}
	}

	protected static void InvokeUserCode_RpcAddPlayerLoaded(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAddPlayerLoaded called on server.");
		}
		else
		{
			((HardlineGameManager)obj).UserCode_RpcAddPlayerLoaded();
		}
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(playersLoaded);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(playersLoaded);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			int num = playersLoaded;
			NetworkplayersLoaded = reader.ReadInt();
			return;
		}
		long num2 = (long)reader.ReadULong();
		if ((num2 & 1L) != 0L)
		{
			int num3 = playersLoaded;
			NetworkplayersLoaded = reader.ReadInt();
		}
	}
}
