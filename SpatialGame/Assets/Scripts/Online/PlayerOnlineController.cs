using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerOnlineController : MonoBehaviour
{
    [SerializeField]
    PhotonView PV;
    GamePlayManager gamePlayManager;
    [SerializeField]
    List<BigDrone> mySoldiers;
    public string path;
    public int myTeam;
    [SerializeField]
    int currentEnergy;
    [SerializeField]
    int maxEnergy;
    List<Image> energyImages = new List<Image>();

    // Start is called before the first frame update
    void Awake()
    {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        gamePlayManager.AddPlayer(this);
        GameSetUp.gameSetUp.players.Add(this);
    }
	private void Start()
	{
        Recharge();
		if(PV.IsMine)
		{
            PV.RPC("RPC_GetTeam",RpcTarget.MasterClient);
            if(myTeam == 1)
			{
                for(int i = 0; i<GameSetUp.gameSetUp.spawningPointsLocal.Length; i++)
			    {
				    mySoldiers.Add(PhotonNetwork.Instantiate(path, GameSetUp.gameSetUp.spawningPointsLocal[i].position,GameSetUp.gameSetUp.spawningPointsLocal[i].rotation,0).GetComponent<BigDrone>());
			    }
			}
			else
			{
                for(int i = 0; i<GameSetUp.gameSetUp.spawningPointsOther.Length; i++)
			    {
				    mySoldiers.Add(PhotonNetwork.Instantiate(path, GameSetUp.gameSetUp.spawningPointsOther[i].position,GameSetUp.gameSetUp.spawningPointsOther[i].rotation,0).GetComponent<BigDrone>());
			    }	
			}
		}

        foreach(BigDrone drone in mySoldiers)
		{
            drone.SetPlayerController(this, gamePlayManager);
		}
	}
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
    public List<BigDrone> GetMySoldiers(){return mySoldiers;}
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
    public void SpendEnergy(int energy)
	{
        if(currentEnergy-energy>=0)
            currentEnergy-=energy;

        for(int i = maxEnergy-1; i>=0; i--)
		{
            energyImages[i].color = Color.white;
		}
	}
    public void Recharge()
	{
        currentEnergy = maxEnergy;

        for(int i = 0; i<energyImages.Count; i++)
		{
            energyImages[i].color = Color.cyan;
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
}
