using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameSetUp : MonoBehaviourPunCallbacks
{
    public static GameSetUp gameSetUp;
	public PhotonView PV;

    public Transform[] spawningPointsLocal;
    public Transform[] spawningPointsOther;
	// Start is called before the first frame update
	public override void OnEnable()
	{
		if(GameSetUp.gameSetUp==null)
			GameSetUp.gameSetUp = this;
	}
	private void Start()
	{
		Player[] players = PhotonNetwork.PlayerList;

		if(PhotonNetwork.IsMasterClient)
		{			
			for(int i = 0; i<players.Length;i++)
			{
				PV.RPC("RPC_SetSpawnPoints",players[i],i);
			}
		}
	}
	[PunRPC]
	void RPC_SetSpawnPoints(int team)
	{
		if(team == 0)
		{
			foreach(Transform pos in GameSetUp.gameSetUp.spawningPointsLocal)
			{
                PhotonNetwork.Instantiate("MultiplayerPrefabs/Cube", pos.position,pos.rotation,0);
			}
		}
		else if(team == 1)
		{
			foreach(Transform pos in GameSetUp.gameSetUp.spawningPointsOther)
			{
                PhotonNetwork.Instantiate("MultiplayerPrefabs/Sphere", pos.position,pos.rotation,0);
			}
		}
	}
}
