using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameSetUp : MonoBehaviourPunCallbacks
{
    public static GameSetUp gameSetUp;
	public PhotonView PV;
	public int nextTeam = 1;

    public Transform[] spawningPointsLocal;
    public Transform[] spawningPointsOther;	
	public List<GameObject> soldiers1  = new List<GameObject>();
	public List<GameObject> soldiers2  = new List<GameObject>();
	public List<PlayerOnlineController> players = new List<PlayerOnlineController>();
	// Start is called before the first frame update
	public override void OnEnable()
	{
		if(GameSetUp.gameSetUp==null)
			GameSetUp.gameSetUp = this;
	}
	private void Start()
	{
		//Busca la lista de jugadores
		/*Player[] players = PhotonNetwork.PlayerList;

		//Mira si es el cliente maestro, si lo es utiliza la lista de jugadores para llamar la funcion RPC_SetSpawnPoints
		if(PhotonNetwork.IsMasterClient)
		{			
			for(int i = 0; i<players.Length;i++)
			{
				//PV.RPC("RPC_SetSpawnPoints",players[i],i);
			}
		}*/
	}
	public void UpdateTeam()
	{
		if(nextTeam == 1)
		{
			nextTeam = 2;
		}
		else
		{
			nextTeam = 1;
		}
	}
	//Instancia los avatares en las posiciones adecuadas dependiendo del equipo que le haya tocado
	/*[PunRPC]
	void RPC_SetSpawnPoints(int team)
	{
		if(team == 0)
		{
			for(int i = 0; i<spawningPointsLocal.Length; i++)
			{
				GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Cube", spawningPointsLocal[i].position,spawningPointsLocal[i].rotation,0);
				obj.transform.SetParent(spawningPointsLocal[i]);
				soldiers1.Add(obj);
			}		
		}
		else if(team == 1)
		{
			for(int i = 0; i<spawningPointsOther.Length; i++)
			{
				GameObject obj = PhotonNetwork.Instantiate("MultiplayerPrefabs/Sphere", spawningPointsOther[i].position,spawningPointsOther[i].rotation,0);
				obj.transform.SetParent(spawningPointsOther[i]);
				soldiers2.Add(obj);
			}	
		}
		foreach(GameObject obj in soldiers1)
		{
			players[0].AddSoldier(obj);
		}
		foreach(GameObject obj in soldiers2)
		{
			players[1].AddSoldier(obj);
		}
	}*/
	
}
