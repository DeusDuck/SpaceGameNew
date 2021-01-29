using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GamePlayManager : MonoBehaviour
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
    Transform player1FacingPoint;
    [SerializeField]
    Transform player2FacingPoint;
    float currentTime;
    [SerializeField]
    Text time;
    [SerializeField]
    List<PlayerOnlineController> players = new List<PlayerOnlineController>();
    public PlayerOnlineController currentPlayer;
    [SerializeField]
    LayerMask layerToCollide;
    [SerializeField]
    Transform panel;
    int current = 0;
    public List<BigDrone> attackingDrones = new List<BigDrone>();
    public List<BigDrone> movingDrones = new List<BigDrone>();
    public bool attacking;
    bool moving;
    BigDrone currentAttackingDrone;
    public List<BigDrone> avatars = new List<BigDrone>();
    bool firstTime = true;
    [SerializeField]
    UIOnlineManager UIManager;
        
    
    public enum EGameState
	{
        PREPARE, SELECTING, ATTACKING
	}
    public EGameState currentState;
    // Start is called before the first frame update
    void Start()
    {
        //Setea la partida, determina con un rnd el primer jugador en atacar
        currentTime = timeToPrepare;        
    }

    // Update is called once per frame
    void Update()
    {
        //En el estado selecting, 
		switch(currentState)
		{           
            case EGameState.PREPARE:
                currentTime-=Time.deltaTime;
				if(currentTime<=0 && PhotonNetwork.IsMasterClient)
                {                    
                    ChangeTurn();
                    SetCurrentPlayer(current);
                    PV.RPC("RPC_SetCurrentPlayer",RpcTarget.Others,current);
                    ChangeState(EGameState.SELECTING);
                    PV.RPC("RPC_ChangeState",RpcTarget.Others,1);
				}
                break;
            case EGameState.SELECTING:
                //Resta el tiempo en hacer una acción
                currentTime-=Time.deltaTime;
                time.text = currentTime.ToString("F0");
                //if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                if(!EventSystem.current.IsPointerOverGameObject())
                {
				    if(Input.GetMouseButtonDown(0) && currentPlayer.GetPV().IsMine && !attacking && !moving)
				    {                    
                        //Mira que jugador has selecionado para hacerle daño
                        //Touch touch = Input.GetTouch(0);
					    Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);  
                   
                        if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					    {                       
                            currentAttackingDrone = hit.transform.GetComponent<BigDrone>();

						    if(currentAttackingDrone!=null)
						    {
                                if(currentPlayer.GetMySoldiers().Contains(currentAttackingDrone) && !attackingDrones.Contains(currentAttackingDrone))
							    {
                                    panel.gameObject.SetActive(true);
                                    ActivateAlliesArrows(false);
                                    UIManager.SetCurrentDrone(currentAttackingDrone);
							    }                           
						    }


					    }   
				    }
				    if((moving || attacking) && Input.GetMouseButtonDown(0))
				    {                    
                        Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);  
                    
                        if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					    {
                            BigDrone drone = hit.transform.GetComponent<BigDrone>();
                            if(drone!=null) 
						    {
                                currentAttackingDrone.target = drone;
							    if(!currentPlayer.GetMySoldiers().Contains(drone))
							    {
                                    attacking = false;
                                    ActivateEnemyArrows(attacking);
                                    ActivateAlliesArrows(!attacking);
							    }
							    else
							    {                 
                                    currentAttackingDrone.MoveToTargetPosition();
                                    moving = false;
                                    ActivateEnemyArrows(false);
                                    ActivateAlliesArrows(true);
							    }
						    }
					    }
					
				    }
                }
                //Si el tiempo llega a 0 se canvia el estado a attacking
		        if(currentTime<0 && PhotonNetwork.IsMasterClient)
		        {
                    ChangeState(EGameState.ATTACKING);
                    PV.RPC("RPC_ChangeState",RpcTarget.Others,2);
		        }
                break;
            case EGameState.ATTACKING:
                currentTime-=Time.deltaTime;
				if(firstTime)
				{
                    foreach(BigDrone drone in attackingDrones)
                    {
                       if(drone.target == null)
                            continue;

                        drone.Attack();
                        PV.RPC("RPC_ApplyDamage",RpcTarget.Others, drone.damage,drone.target.PV.ViewID);
				    }
                    firstTime = false;
				}
                
				if(currentTime<=0 && PhotonNetwork.IsMasterClient)
				{
                    ChangeTurn();
                    SetCurrentPlayer(current);
                    PV.RPC("RPC_SetCurrentPlayer",RpcTarget.Others,current);
                    ChangeState(EGameState.SELECTING);
                    PV.RPC("RPC_ChangeState",RpcTarget.Others,1);                
				}
                break;
                
		}        
    }    
    void ChangeState(EGameState nextState)
	{
		switch(nextState)
		{
            case EGameState.PREPARE:
                break;
            
            case EGameState.SELECTING:
                currentTime = timeToSelect;
                ActivateAlliesArrows(true);
				if(PhotonNetwork.IsMasterClient)
				{
                    AddEnergy();
                    PV.RPC("RPC_AddMoreEnergy",RpcTarget.Others);
				}
                break;
            case EGameState.ATTACKING:
                currentTime = timeAttacking;
                break;
		}
		switch(currentState)
		{            
            case EGameState.PREPARE: 
                BigDrone[] drones = FindObjectsOfType<BigDrone>();
                for(int i = 0; i<drones.Length;i++)
                    avatars.Add(drones[i]); 
                foreach(PlayerOnlineController player in players)
				{
                    if(player.myTeam == 1)
                        player.facingPoint = player2FacingPoint;
                    else
                        player.facingPoint = player1FacingPoint;
                    player.SetEnergyImages();
                    player.AddMoreEnergy();
				}
                ActivateAlliesArrows(true);
                Camera.main.transform.LookAt(currentPlayer.facingPoint);
                break;
            case EGameState.SELECTING:
                attacking = false;
                foreach(BigDrone drone in currentPlayer.GetMySoldiers())
                    drone.ShowArrow(false);
                break;  
            case EGameState.ATTACKING:
                attackingDrones.Clear();
                movingDrones.Clear();
                firstTime = true;
                break;
		}
        currentState = nextState;
	}
    public void AddPlayer(PlayerOnlineController player){players.Add(player);}
    
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
    //Setea el turno del jugador
    [PunRPC]
    void RPC_SetCurrentPlayer(int team)
	{
        SetCurrentPlayer(team);
	}
    [PunRPC]
    void RPC_ChangeState(int index)
	{
        ChangeState((EGameState)index);
	}
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
    [PunRPC]
    void RPC_AddMoreEnergy()
	{
        AddEnergy();
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
	}
    public void ActivateEnemyArrows(bool must)
	{
        foreach(BigDrone drone in avatars)
		{
            if(currentPlayer.GetMySoldiers().Contains(drone))
                continue;

            drone.ShowArrow(must);
		}
	}
    public void ActivateAlliesArrows(bool must)
	{
        foreach(BigDrone drone in avatars)
		{
            if(!currentPlayer.GetMySoldiers().Contains(drone)) 
                continue;

			if(attackingDrones.Contains(drone) || movingDrones.Contains(drone))
			{
                drone.ShowArrow(false);
                continue;
			}


            drone.ShowArrow(must);
		}
	}
    public UIOnlineManager GetUIManager(){return UIManager;}
    public void SetMoving(bool must)
	{
        moving = must;
        movingDrones.Add(currentAttackingDrone);
	}
    public void SetAttack(bool must)
	{
        attacking = must;
        attackingDrones.Add(currentAttackingDrone);
	}
}


