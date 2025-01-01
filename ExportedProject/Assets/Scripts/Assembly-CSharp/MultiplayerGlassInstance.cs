using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class MultiplayerGlassInstance : NetworkBehaviour
{
	[SerializeField]
	private GameObject glass;

	[SerializeField]
	private GameObject currentGlass;

	private Vector3 glassScale;

	private bool broken;

	public bool Broken
	{
		get
		{
			return broken;
		}
		set
		{
			broken = value;
		}
	}

	public void ReplicateDestroy(Vector3 hitPos, Vector3 hitRot)
	{
		if (!base.isServer)
		{
			CmdDestroyGlass(hitPos, hitRot);
			return;
		}
		DestroyGlass(hitPos, hitRot);
		RpcDestroyGlass(hitPos, hitRot);
	}

	[Command(requiresAuthority = false)]
	public void CmdDestroyGlass(Vector3 hitPos, Vector3 hitRot)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		SendCommandInternal(typeof(MultiplayerGlassInstance), "CmdDestroyGlass", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcDestroyGlass(Vector3 hitPos, Vector3 hitRot)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteVector3(hitPos);
		writer.WriteVector3(hitRot);
		SendRPCInternal(typeof(MultiplayerGlassInstance), "RpcDestroyGlass", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void DestroyGlass(Vector3 hitPos, Vector3 hitRot)
	{
		if ((bool)currentGlass)
		{
			Broken = true;
			if ((bool)currentGlass.GetComponent<ShatterableGlass>())
			{
				currentGlass.GetComponent<ShatterableGlass>().Shatter3D(new ShatterableGlassInfo(hitPos, hitRot));
			}
		}
	}

	public void ReplicateCreateGlass()
	{
		if (!base.isServer)
		{
			CmdCreateGlass();
			return;
		}
		CreateGlass();
		RpcCreateGlass();
	}

	[Command(requiresAuthority = false)]
	public void CmdCreateGlass()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendCommandInternal(typeof(MultiplayerGlassInstance), "CmdCreateGlass", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcCreateGlass()
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		SendRPCInternal(typeof(MultiplayerGlassInstance), "RpcCreateGlass", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	public void CreateGlass()
	{
		if ((bool)glass && currentGlass == null)
		{
			Broken = false;
			GameObject gameObject = Object.Instantiate(glass, base.transform.position, base.transform.rotation);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = glassScale;
			currentGlass = gameObject;
		}
	}

	private void Awake()
	{
		GetGlassDimensions();
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	private void GetGlassDimensions()
	{
		glassScale = base.transform.GetChild(0).transform.localScale;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdDestroyGlass(Vector3 hitPos, Vector3 hitRot)
	{
		DestroyGlass(hitPos, hitRot);
		RpcDestroyGlass(hitPos, hitRot);
	}

	protected static void InvokeUserCode_CmdDestroyGlass(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDestroyGlass called on client.");
		}
		else
		{
			((MultiplayerGlassInstance)obj).UserCode_CmdDestroyGlass(reader.ReadVector3(), reader.ReadVector3());
		}
	}

	protected void UserCode_RpcDestroyGlass(Vector3 hitPos, Vector3 hitRot)
	{
		DestroyGlass(hitPos, hitRot);
	}

	protected static void InvokeUserCode_RpcDestroyGlass(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDestroyGlass called on server.");
		}
		else
		{
			((MultiplayerGlassInstance)obj).UserCode_RpcDestroyGlass(reader.ReadVector3(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdCreateGlass()
	{
		CreateGlass();
		RpcCreateGlass();
	}

	protected static void InvokeUserCode_CmdCreateGlass(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCreateGlass called on client.");
		}
		else
		{
			((MultiplayerGlassInstance)obj).UserCode_CmdCreateGlass();
		}
	}

	protected void UserCode_RpcCreateGlass()
	{
		CreateGlass();
	}

	protected static void InvokeUserCode_RpcCreateGlass(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcCreateGlass called on server.");
		}
		else
		{
			((MultiplayerGlassInstance)obj).UserCode_RpcCreateGlass();
		}
	}

	static MultiplayerGlassInstance()
	{
		RemoteCallHelper.RegisterCommandDelegate(typeof(MultiplayerGlassInstance), "CmdDestroyGlass", InvokeUserCode_CmdDestroyGlass, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(MultiplayerGlassInstance), "CmdCreateGlass", InvokeUserCode_CmdCreateGlass, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(MultiplayerGlassInstance), "RpcDestroyGlass", InvokeUserCode_RpcDestroyGlass);
		RemoteCallHelper.RegisterRpcDelegate(typeof(MultiplayerGlassInstance), "RpcCreateGlass", InvokeUserCode_RpcCreateGlass);
	}
}
