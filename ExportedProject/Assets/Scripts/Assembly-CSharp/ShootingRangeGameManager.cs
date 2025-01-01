using UnityEngine;

public class ShootingRangeGameManager : HardlineGameManager
{
	private bool lowFrameMode;

	protected override void Start()
	{
		Object.FindObjectOfType<MultiplayerNetworkManager>().AllPlayersLoaded();
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	private void MirrorProcessed()
	{
	}
}
