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
	public Transform player1CameraPos;
	public Transform player2CameraPos;
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
			for(int i = 0; i<spawningPointsLocal.Length; i++)
			{
				int rnd = Random.Range(0,spawningPointsLocal.Length);
				if(spawningPointsLocal[rnd].childCount==0)
				{
					GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Cube", spawningPointsLocal[rnd].position,spawningPointsLocal[rnd].rotation,0);
					obj.transform.SetParent(spawningPointsLocal[rnd]);
				}
				else
				{
					for(int j = 1; j<spawningPointsLocal.Length; j++)
					{
						int index = (rnd+j) % spawningPointsLocal.Length;
						if(spawningPointsLocal[index].childCount==0)
						{
							GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Cube", spawningPointsLocal[index].position,spawningPointsLocal[index].rotation,0);
							obj.transform.SetParent(spawningPointsLocal[index]);
						}							
					}
				}
                
			}		
		}
		else if(team == 1)
		{
			for(int i = 0; i<spawningPointsOther.Length; i++)
			{
				int rnd = Random.Range(0,spawningPointsOther.Length);
				if(spawningPointsOther[rnd].childCount==0)
				{
					GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Sphere", spawningPointsOther[rnd].position,spawningPointsOther[rnd].rotation,0);
					obj.transform.SetParent(spawningPointsOther[rnd]);
				}
				else
				{
					for(int j = 1; j<spawningPointsOther.Length; j++)
					{
						int index = (rnd+j) % spawningPointsOther.Length;
						if(spawningPointsOther[index].childCount==0)
						{
							GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Sphere", spawningPointsOther[index].position,spawningPointsOther[index].rotation,0);
							obj.transform.SetParent(spawningPointsOther[index]);
						}							
					}
				}
			}	
		}
	}
}
