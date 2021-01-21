using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	public class GamePlayManager : MonoBehaviour,IPunTurnManagerCallbacks
    {
        //Esta clase se encarga de controlar el gameplay online
        [SerializeField]
        PhotonView PV;
        [SerializeField]
        float timeToPrepare;
        [SerializeField]
        float timeToSelect;
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
        int current = 1;
        [SerializeField]
        PunTurnManager turnManager;
    
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
                        current++;
                        if(current>2)
                            current = 1;
                        SetCurrentPlayer(current);
                        PV.RPC("RPC_SetCurrentPlayer",RpcTarget.Others,current);
                        PV.RPC("RPC_ChangeState",RpcTarget.Others,1);
                        ChangeState(EGameState.SELECTING);                        
					}
                    break;
                case EGameState.SELECTING:
                    //Resta el tiempo en hacer una acción
                    currentTime-=Time.deltaTime;
                    time.text = currentTime.ToString("F0");
				    if(Input.GetMouseButtonDown(0) && currentPlayer.GetPV().IsMine)
				    {                    
                        //Mira que jugador has selecionado para hacerle daño
                        //Touch touch = Input.GetTouch(0);
					    Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);  
                        //Debug.Log(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide));
                        if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					    {                       
                            panel.gameObject.SetActive(true);
                            GameObject enemyPV = hit.transform.GetComponent<GameObject>();
						    if(enemyPV!=null && currentPlayer.GetMySoldiers().Contains(enemyPV))
						    {
                                
						    }
					    }   
				    }
                    //Si el tiempo llega a 0 se canvia el estado a attacking
		            if(currentTime<0)
		            {
                        ChangeState(EGameState.ATTACKING);
		            }
                    break;
                case EGameState.ATTACKING:
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
                    break;
		    }
		    switch(currentState)
		    {            
                case EGameState.PREPARE:
                    break;
                case EGameState.SELECTING:
                    break;  
                case EGameState.ATTACKING:
                    break;
		    }
            currentState = nextState;
	    }
        public void AddPlayer(PlayerOnlineController player){players.Add(player);}
        void SetPlayersSoldiers()
	    {
            //foreach(PhotonView controlable in GameSetUp.gameSetUp.soldiers)
		    {
            
		    }
	    }
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
	    public void OnTurnBegins(int turn)
	    {
		
	    }

	    public void OnTurnCompleted(int turn)
	    {
		
	    }

	    public void OnPlayerMove(Player player, int turn, object move)
	    {
		
	    }

	    public void OnPlayerFinished(Player player, int turn, object move)
	    {
		
	    }

	    public void OnTurnTimeEnds(int turn)
	    {
		
	    }
    }
}

