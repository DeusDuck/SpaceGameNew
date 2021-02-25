using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GamePlayManager : MonoBehaviourPunCallbacks
{
    //Esta clase se encarga de controlar el gameplay online
    [SerializeField]
    PhotonView PV;
    [SerializeField]
    float timeToPrepare;
    [SerializeField]
    float timeToSelect;
    [SerializeField]
    float timeAttacking;
    [SerializeField]
    float timeFinishing;
    [SerializeField]
    Transform player1FacingPoint;
    [SerializeField]
    Transform player2FacingPoint;
    float currentTime;
    [SerializeField]
    Text time;
    [SerializeField]
    List<PlayerOnlineController> players = new List<PlayerOnlineController>();
    public PlayerOnlineController currentPlayer;
    public PlayerOnlineController localPlayer;
    [SerializeField]
    LayerMask layerToCollide;
    [SerializeField]
    Transform panel;
    int current = 0;
    public List<BigDrone> attackingDrones = new List<BigDrone>();
    public List<BigDrone> movingDrones = new List<BigDrone>();
    BigDrone currentAttackingDrone;
    public List<BigDrone> avatars = new List<BigDrone>();
    bool firstTime = true;
    [SerializeField]
    UIOnlineManager UIManager;
    [SerializeField]
    float facingSpeed;
    public static GamePlayManager instance;
    public int nextTeam = 1;

    public Transform[] spawningPointsLocal;
    public Transform[] spawningPointsOther;
    [SerializeField]
    CameraManager cameraManager;    
        
    
    public enum EGameState
	{
        PREPARE, SELECTING, ATTACKING, FINISHING
	}
    public EGameState currentState;
    // Start is called before the first frame update
    void Start()
    {
        //Setea la partida, determina con un rnd el primer jugador en atacar
        currentTime = timeToPrepare;        
    }
    public override void OnEnable()
	{
        if(GamePlayManager.instance==null)
			GamePlayManager.instance = this;
	}
    // Update is called once per frame
    void Update()
    { 
		switch(currentState)
		{           
            //Deja un  tiempo de preparacion a los ajustes del juego
            case EGameState.PREPARE:
                currentTime-=Time.deltaTime;
				if(currentTime<=0 && PhotonNetwork.IsMasterClient)
                {                    
                    ChangeTurn();                    
				}
                break;
            case EGameState.SELECTING:
                //Resta el tiempo en hacer una acción
                currentTime-=Time.deltaTime;
                time.text = currentTime.ToString("F0");
                
		        if(currentTime<0)
                {                
					if(PhotonNetwork.IsMasterClient)
					{
                        ChangeState(EGameState.ATTACKING);
                        PV.RPC("RPC_ChangeState",RpcTarget.Others,2);
					}                    
		        }
                break;
            case EGameState.ATTACKING:
                //Daña a todos los drones de la lista attackingDrones
                currentTime-=Time.deltaTime;
				if(firstTime)
				{
                    foreach(BigDrone drone in currentPlayer.GetMySoldiers())
                    {
                       if(drone.target == null)
                            continue;
                        
                        cameraManager.LookAtAttackingDrone(drone.transform);
                        PV.RPC("RPC_SetCameraAttackingDrone",RpcTarget.Others,drone.PV.ViewID);
                        drone.Attack();
                        PV.RPC("RPC_ApplyDamage",RpcTarget.Others, drone.GetCurrentDamage(),drone.target.PV.ViewID);                        
                        UIManager.UpdateEnemyHealth(drone.target.GetCurrentHealth(),drone.target.GetMaxHealth());

				    }
                    firstTime = false;
				}
				if(currentTime<=0) 
				{
                    currentPlayer.GetMySoldiers()[0].ResetParameters();
                    currentPlayer.GetMySoldiers()[0].ResetDamage();
                    cameraManager.ReturnToCameraPosition();                    
					
                    if(PhotonNetwork.IsMasterClient)
                        ChangeTurn();                                   
				}
                break;
                case EGameState.FINISHING:
                currentTime-=Time.deltaTime;
				if(currentTime<=0)
				{
                    OnlineManager.instance.ReturnToMainScene();
				}                    
                    break;
                
		}        
    }    
    //Cambia el estado del gameManager
    void ChangeState(EGameState nextState)
	{
		switch(nextState)
		{
            case EGameState.PREPARE:
                break;
            //Setea el current Time
            case EGameState.SELECTING:
                currentTime = timeToSelect;
                //Si eres el master client añades uno de energia a todos los jugadores
				if(PhotonNetwork.IsMasterClient)
				{
                    AddEnergy();
                    PV.RPC("RPC_AddMoreEnergy",RpcTarget.Others);
				}
                UIManager.SetInteractable(currentPlayer.GetPV().IsMine);
                break;
            case EGameState.ATTACKING:
                currentTime = timeAttacking;

                break;
            case EGameState.FINISHING:
                currentTime = timeFinishing;    
                break;
		}
		switch(currentState)
		{            
            //Busca a todos los avatares y los setea
            case EGameState.PREPARE:               
                BigDrone[] drones = FindObjectsOfType<BigDrone>();
                for(int i = 0; i<drones.Length; i++)
			    {
                    avatars.Add(drones[i]);
			    }
                SetOnlinePlayerAvatars();               
                UIManager.SetButtons();
                SetEnemy();
                break;
                //Desactiva el modo ataque y las flechas de los aliados del currentPlayer
            case EGameState.SELECTING:
                UIManager.SetInteractable(false);
                break;  
                //Borra toda la lista de atacantes y moving
            case EGameState.ATTACKING:
                attackingDrones.Clear();
                movingDrones.Clear();
                firstTime = true;
                break;
		}
        currentState = nextState;
	}
    public void AddPlayer(PlayerOnlineController player)
    {
        players.Add(player);
    }
    
    //Setea el jugador al que le toca el siguiente turno
    void SetCurrentPlayer(int team)
	{
        for(int i = 0; i<players.Count; i++)
		{
            if(players[i].myTeam == team)
			{                   
                currentPlayer = players[i];                
			}			
		}
	}
    public void IDied(PlayerOnlineController player)
	{
        ChangeState(EGameState.FINISHING);
		if(player.GetPV().IsMine)
		{
            DisplayLoseImage();
		}
		else
		{
            DisplayWinningImage();
		}
	}
    //Setea el turno del jugador
    [PunRPC]
    void RPC_SetCurrentPlayer(int team)
	{
        SetCurrentPlayer(team);
	}
    //Cambia el estado del gameManager
    [PunRPC]
    void RPC_ChangeState(int index)
	{
        ChangeState((EGameState)index);
	}
    //Le hace daño al dron que le toca
    [PunRPC]
    void RPC_ApplyDamage(float damage,int id)
	{
        for(int i = 0; i<players.Count; i++)
		{
            if(currentPlayer == players[i])
                continue;
            players[i].DamageSoldier(damage,id);
		}
	}
    //Añade uno de energia al jugador
    [PunRPC]
    void RPC_AddMoreEnergy()
	{
        AddEnergy();
	}
    [PunRPC]
    void RPC_SetCameraAttackingDrone(int id)
	{
        foreach(BigDrone drone in currentPlayer.GetMySoldiers())
        {
            if(drone.PV.ViewID == id)
                cameraManager.LookAtAttackingDrone(drone.transform);
        }
	}
    [PunRPC]
    void RPC_ResetCameraPosition()
	{
        cameraManager.ReturnToCameraPosition();
	}
    void AddEnergy()
	{
        foreach(PlayerOnlineController player in players)
		{
            player.AddMoreEnergy();
		}
	}
    void ChangeTurn()
	{
        current++;
        if(current>2)
            current = 1;   
        SetCurrentPlayer(current);
        PV.RPC("RPC_SetCurrentPlayer",RpcTarget.Others,current);
        ChangeState(EGameState.SELECTING);
        PV.RPC("RPC_ChangeState",RpcTarget.Others,1);
	}   
   
    public UIOnlineManager GetUIManager(){return UIManager;}
    public CameraManager GetCameraManager(){return cameraManager;}
    public void SkipTurn()
	{
		if(currentPlayer.GetPV().IsMine)
		{
            ChangeState(EGameState.ATTACKING);
            PV.RPC("RPC_ChangeState",RpcTarget.Others,2);
		}        
	}
    void DisplayWinningImage()
	{
        UIManager.ActivateVictoryImage();
	}
    void DisplayLoseImage()
	{
        UIManager.ActivateLoseImage();
	}
    void SetOnlinePlayerAvatars()
	{
        List<BigDrone> drones = new List<BigDrone>();
        foreach(PlayerOnlineController player in players)
		{
            if(player.GetMySoldiers().Count> 0)
			{
                foreach(BigDrone drone in player.GetMySoldiers())
				{
                    drones.Add(drone);
				}
			}
			else
			{
                foreach(BigDrone drone in avatars)
			    {
				    if(drones.Contains(drone))
				        continue;

                    player.AddSoldier(drone);
                    drone.SetPlayerController(player);
                    
			    }
			}
            
		}
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
    public PlayerOnlineController GetLocalPlayer()
	{
        foreach(PlayerOnlineController player in players)
		{
            if(player.GetPV().IsMine)
                return player;
		}
        return null;
	}
    public BigDrone SetEnemy(BigDrone drone)
	{
        foreach(BigDrone current in avatars)
		{
            if(drone == current)
                continue;

            return current;
		}
        return null;
	}
    void SetEnemy()
	{
        foreach(PlayerOnlineController player in players)
		{
            if(player == localPlayer)
                continue;

            localPlayer.Enemy = player;
		}
	}
}


