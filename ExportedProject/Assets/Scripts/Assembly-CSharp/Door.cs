using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Door : NetworkBehaviour
{
	private float angle;

	[SyncVar]
	[SerializeField]
	private float targAngle;

	private float angle_kP = 0.15f;

	[SerializeField]
	private GameObject doorPivot;

	[SerializeField]
	private float openAngle;

	[SerializeField]
	private float closeAngle;

	[SerializeField]
	private GameObject doorOpenSound;

	[SerializeField]
	private GameObject doorCloseSound;

	public float NetworktargAngle
	{
		get
		{
			return targAngle;
		}
		[param: In]
		set
		{
			if (!NetworkBehaviour.SyncVarEqual(value, ref targAngle))
			{
				float num = targAngle;
				SetSyncVar(value, ref targAngle, 1uL);
			}
		}
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		UpdateDoorAngle();
	}

	private void UpdateDoorAngle()
	{
		angle += (targAngle - angle) * angle_kP;
		doorPivot.transform.localEulerAngles = new Vector3(0f, angle, 0f);
	}

	public void Interact(float dot)
	{
		if (targAngle != closeAngle)
		{
			ReplicateTargAngle(closeAngle);
		}
		else if (dot >= 0f)
		{
			ReplicateTargAngle(closeAngle + openAngle);
		}
		else
		{
			ReplicateTargAngle(closeAngle - openAngle);
		}
	}

	public void Open(float dot)
	{
		if (dot >= 0f)
		{
			ReplicateTargAngle(closeAngle + openAngle);
		}
		else
		{
			ReplicateTargAngle(closeAngle - openAngle);
		}
	}

	public bool CheckOpenedCorrectly(float dot)
	{
		if (dot >= 0f)
		{
			if (targAngle == closeAngle + openAngle)
			{
				return true;
			}
			return false;
		}
		if (targAngle == closeAngle - openAngle)
		{
			return true;
		}
		return false;
	}

	public void SetClose()
	{
		NetworktargAngle = closeAngle;
		angle = closeAngle;
		ReplicateSetClose();
	}

	public bool IsOpen()
	{
		if (targAngle == closeAngle)
		{
			return false;
		}
		return true;
	}

	public void ReplicateTargAngle(float angle)
	{
		if (!base.isServer)
		{
			NetworktargAngle = angle;
			if (targAngle == openAngle)
			{
				Object.Instantiate(doorOpenSound, base.transform.position, base.transform.rotation);
			}
			else
			{
				Object.Instantiate(doorCloseSound, base.transform.position, base.transform.rotation);
			}
			CmdSetTargAngle(angle);
		}
		else if (base.isServer)
		{
			NetworktargAngle = angle;
			RpcSetTargAngle(angle);
		}
	}

	public void ReplicateSetClose()
	{
		if (!base.isServer)
		{
			CmdSetClose(angle);
		}
		else if (base.isServer)
		{
			NetworktargAngle = closeAngle;
			angle = closeAngle;
			RpcSetClose(angle);
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdSetTargAngle(float angle)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(angle);
		SendCommandInternal(typeof(Door), "CmdSetTargAngle", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetTargAngle(float angle)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(angle);
		SendRPCInternal(typeof(Door), "RpcSetTargAngle", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdSetClose(float angle)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(angle);
		SendCommandInternal(typeof(Door), "CmdSetClose", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSetClose(float angle)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteFloat(angle);
		SendRPCInternal(typeof(Door), "RpcSetClose", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSetTargAngle(float angle)
	{
		if (base.isServer)
		{
			NetworktargAngle = angle;
		}
		if (targAngle == openAngle)
		{
			Object.Instantiate(doorOpenSound, base.transform.position, base.transform.rotation);
		}
		else
		{
			Object.Instantiate(doorCloseSound, base.transform.position, base.transform.rotation);
		}
	}

	protected static void InvokeUserCode_CmdSetTargAngle(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetTargAngle called on client.");
		}
		else
		{
			((Door)obj).UserCode_CmdSetTargAngle(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetTargAngle(float angle)
	{
		if (!base.isServer)
		{
			NetworktargAngle = angle;
		}
		if (targAngle == openAngle)
		{
			Object.Instantiate(doorOpenSound, base.transform.position, base.transform.rotation);
		}
		else
		{
			Object.Instantiate(doorCloseSound, base.transform.position, base.transform.rotation);
		}
	}

	protected static void InvokeUserCode_RpcSetTargAngle(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetTargAngle called on server.");
		}
		else
		{
			((Door)obj).UserCode_RpcSetTargAngle(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdSetClose(float angle)
	{
		if (base.isServer)
		{
			NetworktargAngle = closeAngle;
			angle = closeAngle;
		}
	}

	protected static void InvokeUserCode_CmdSetClose(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetClose called on client.");
		}
		else
		{
			((Door)obj).UserCode_CmdSetClose(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcSetClose(float angle)
	{
		if (!base.isServer)
		{
			NetworktargAngle = closeAngle;
			angle = closeAngle;
		}
	}

	protected static void InvokeUserCode_RpcSetClose(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetClose called on server.");
		}
		else
		{
			((Door)obj).UserCode_RpcSetClose(reader.ReadFloat());
		}
	}

	static Door()
	{
		RemoteCallHelper.RegisterCommandDelegate(typeof(Door), "CmdSetTargAngle", InvokeUserCode_CmdSetTargAngle, requiresAuthority: false);
		RemoteCallHelper.RegisterCommandDelegate(typeof(Door), "CmdSetClose", InvokeUserCode_CmdSetClose, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Door), "RpcSetTargAngle", InvokeUserCode_RpcSetTargAngle);
		RemoteCallHelper.RegisterRpcDelegate(typeof(Door), "RpcSetClose", InvokeUserCode_RpcSetClose);
	}

	public override bool SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		bool result = base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(targAngle);
			return true;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(targAngle);
			result = true;
		}
		return result;
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			float num = targAngle;
			NetworktargAngle = reader.ReadFloat();
			return;
		}
		long num2 = (long)reader.ReadULong();
		if ((num2 & 1L) != 0L)
		{
			float num3 = targAngle;
			NetworktargAngle = reader.ReadFloat();
		}
	}
}
