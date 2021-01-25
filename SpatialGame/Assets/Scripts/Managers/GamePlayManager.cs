using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    List<Transform> attackingDrones = new List<Transform>();
    public bool attacking;
    Transform currentAttackingDrone;
    public List<BigDrone> avatars = new List<BigDrone>();
    bool firstTime = true;
        
    
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
                
				if(Input.GetMouseButtonDown(0) && currentPlayer.GetPV().IsMine && !attacking)
				{                    
                    //Mira que jugador has selecionado para hacerle daño
                    //Touch touch = Input.GetTouch(0);
					Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);  
                   
                    if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					{                       
                        currentAttackingDrone = hit.transform.GetComponent<Transform>();

						if(currentAttackingDrone!=null && currentPlayer.GetMySoldiers().Contains(currentAttackingDrone) && !attackingDrones.Contains(currentAttackingDrone))
						{
                            attackingDrones.Add(currentAttackingDrone);
                            panel.gameObject.SetActive(true);
                            ActivateArrows(true);
						}
					}   
				}
				if(attacking && Input.GetMouseButtonDown(0))
				{                    
                    Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);  
                    
                    if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					{
                        Transform drone = hit.transform.GetComponent<Transform>();

                        if(drone!=null && !currentPlayer.GetMySoldiers().Contains(drone))
						{
                            currentAttackingDrone.GetComponent<BigDrone>().target = drone;
                            attacking = false;
                            ActivateArrows(attacking);
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
                    foreach(Transform drone in attackingDrones)
				    {
                        BigDrone current = drone.GetComponent<BigDrone>();
                        BigDrone target = current.target.GetComponent<BigDrone>();
                        current.Attack();
                        PV.RPC("RPC_ApplyDamage",RpcTarget.Others, current.damage,target.PV.ViewID);
				    }
                    firstTime = false;
				}
                
				if(PhotonNetwork.IsMasterClient && currentTime<=0)
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
                break;
            case EGameState.SELECTING:
                attacking = false;
                break;  
            case EGameState.ATTACKING:
                attackingDrones.Clear();
                firstTime = true;
                break;
		}
        currentState = nextState;
	}
    public void AddPlayer(PlayerOnlineController player){players.Add(player);}
    
    void SetCurrentPlayer(int team)
	{
        if(currentPlayer!=null)
            currentPlayer.transform.position = Vector3.up*10f;
        for(int i = 0; i<players.Count; i++)
		{
            if(players[i].myTeam == team)
			{                   
                currentPlayer = players[i];
			}
		}
        if(currentPlayer !=null)
            currentPlayer.transform.position = Vector3.zero;
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
    void ChangeTurn()
	{
        current++;
        if(current>2)
            current = 1;        
	}
    void ActivateArrows(bool must)
	{
        foreach(BigDrone drone in avatars)
		{
            if(currentPlayer.GetMySoldiers().Contains(drone.transform))
                continue;

            drone.ShowArrow(must);
		}
	}
}


