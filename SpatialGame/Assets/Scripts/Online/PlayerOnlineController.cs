using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerOnlineController : MonoBehaviour
{
    [SerializeField]
    PhotonView PV;
    public GamePlayManager gamePlayManager;
    [SerializeField]
    Color notMyTurn;
    [SerializeField]
    List<BigDrone> mySoldiers;
    public Transform facingPoint;
    public string path;
    public int myTeam;
    [SerializeField]
    int currentEnergy;
    [SerializeField]
    int maxEnergy;
    List<Image> energyImages = new List<Image>();
    PlayerOnlineController enemy;
    

    // Start is called before the first frame update
    void Awake()
    {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        gamePlayManager.AddPlayer(this);
        currentEnergy = maxEnergy;
    }
	private void Start()
	{
        Recharge();
		if(PV.IsMine)
		{
            gamePlayManager.localPlayer = this;
            PV.RPC("RPC_GetTeam",RpcTarget.MasterClient);
            if(myTeam == 1)
			{
                for(int i = 0; i<GamePlayManager.instance.spawningPointsLocal.Length; i++)
			    {
                    BigDrone current = PhotonNetwork.Instantiate(path, gamePlayManager.spawningPointsLocal[i].position,gamePlayManager.spawningPointsLocal[i].rotation,0).GetComponent<BigDrone>();
				    mySoldiers.Add(current);
                    current.SetPosition(gamePlayManager.spawningPointsLocal[i]);
			    }
			}
			else
			{
                for(int i = 0; i<GamePlayManager.instance.spawningPointsOther.Length; i++)
			    {
                    BigDrone current = PhotonNetwork.Instantiate(path, gamePlayManager.spawningPointsOther[i].position,gamePlayManager.spawningPointsOther[i].rotation,0).GetComponent<BigDrone>();
				    mySoldiers.Add(current);
                    current.SetPosition(gamePlayManager.spawningPointsOther[i]);
			    }	
			}
		}

        foreach(BigDrone drone in mySoldiers)
		{
            drone.SetPlayerController(this);
		}
	}
    [PunRPC]
    void RPC_GetTeam()
	{
        myTeam = gamePlayManager.nextTeam;
       gamePlayManager.UpdateTeam();
        PV.RPC("RPC_SetPlayerTeam",RpcTarget.OthersBuffered,myTeam);
	}
    [PunRPC]
    void RPC_SetPlayerTeam(int team)
	{
        myTeam = team;
	}
    public PhotonView GetPV(){return PV;}
    public List<BigDrone> GetMySoldiers(){return mySoldiers;}
    public void AddSoldier(BigDrone drone){mySoldiers.Add(drone);}
    public void DamageSoldier(float damage, int id)
	{
        foreach(BigDrone current in mySoldiers)
		{
            if(current.PV.ViewID == id)
			{
                current.TakeDamage(damage);
                break;
			}
		}
	}
    public void SpendEnergy()
	{
		if(currentEnergy>=0)
		{
            energyImages[currentEnergy - 1].color = Color.white;
            currentEnergy--;
		}            
	}
    public void Recharge()
	{
        currentEnergy = maxEnergy;

        for(int i = 0; i<energyImages.Count; i++)
		{
            energyImages[i].color = Color.red;
		}
	}
    public void AddMoreEnergy()
	{
        if(maxEnergy<energyImages.Count)
            maxEnergy++;

        UpdateMaxEnergyImage();
        Recharge();
	}
    public void SetEnergyImages()
	{
        if(myTeam == 1)
		{
            foreach(Image image in gamePlayManager.GetUIManager().energyPlayer2)
                energyImages.Add(image);
		}
        else
           foreach(Image image in gamePlayManager.GetUIManager().energyPlayer1)
                energyImages.Add(image);
	}
    //Retorna una lista de int, el primero siempre sera la energia que tiene y el segundo la energia maxima
    public int GetCurrentEnergy()
	{        
        return currentEnergy;
	}
    void UpdateMaxEnergyImage()
	{
        for(int i = 0; i<maxEnergy; i++)
		{
            energyImages[i].gameObject.SetActive(true);           
		}
	}
    public PlayerOnlineController Enemy{get{return Enemy; } set {enemy = value; mySoldiers[0].target = enemy.GetMySoldiers()[0];} }
}
