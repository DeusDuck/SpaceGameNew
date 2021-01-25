using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerOnlineController : MonoBehaviour
{
    [SerializeField]
    PhotonView PV;
    GamePlayManager gamePlayManager;
    [SerializeField]
    List<Transform> mySoldiers;
    public string path;
    public int myTeam;

    // Start is called before the first frame update
    void Awake()
    {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        gamePlayManager.AddPlayer(this);
        GameSetUp.gameSetUp.players.Add(this);
    }
	private void Start()
	{
		if(PV.IsMine)
		{
            PV.RPC("RPC_GetTeam",RpcTarget.MasterClient);
            if(myTeam == 1)
			{
                for(int i = 0; i<GameSetUp.gameSetUp.spawningPointsLocal.Length; i++)
			    {
				    GameObject obj = PhotonNetwork.Instantiate(path, GameSetUp.gameSetUp.spawningPointsLocal[i].position,GameSetUp.gameSetUp.spawningPointsLocal[i].rotation,0);
				    mySoldiers.Add(obj.transform);
			    }
			}
			else
			{
                for(int i = 0; i<GameSetUp.gameSetUp.spawningPointsOther.Length; i++)
			    {
				    GameObject obj = PhotonNetwork.Instantiate(path, GameSetUp.gameSetUp.spawningPointsOther[i].position,GameSetUp.gameSetUp.spawningPointsOther[i].rotation,0);
				    mySoldiers.Add(obj.transform);
			    }	
			}
		}
	}
	public void AddSoldier(Transform soldier){mySoldiers.Add(soldier);}
    [PunRPC]
    void RPC_GetTeam()
	{
        myTeam = GameSetUp.gameSetUp.nextTeam;
        GameSetUp.gameSetUp.UpdateTeam();
        PV.RPC("RPC_SetPlayerTeam",RpcTarget.OthersBuffered,myTeam);
	}
    [PunRPC]
    void RPC_SetPlayerTeam(int team)
	{
        myTeam = team;
	}
    public PhotonView GetPV(){return PV;}
    public List<Transform> GetMySoldiers(){return mySoldiers;}
    public void DamageSoldier(float damage, int id)
	{
        foreach(Transform t in mySoldiers)
		{
            BigDrone current = t.GetComponent<BigDrone>();
            if(current.PV.ViewID == id)
			{
                current.TakeDamage(damage);
                break;
			}
		}
	}
}
