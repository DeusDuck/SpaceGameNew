using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GamePlayManager : MonoBehaviour
{
    //Esta clase se encarga de controlar el gameplay online
    [SerializeField]
    PhotonView PV;
    [SerializeField]
    float timeToPrepare;
    float currentTime;
    [SerializeField]
    Text time;
    [SerializeField]
    List<PlayerOnlineController> players = new List<PlayerOnlineController>();
    public PlayerOnlineController currentPlayer;
    [SerializeField]
    LayerMask layerToCollide;
    
    public enum EGameState
	{
        SELECTING, ATTACKING
	}
    public EGameState currentState;
    // Start is called before the first frame update
    void Start()
    {
        //Setea la partida, determina con un rnd el primer jugador en atacar
        currentTime = timeToPrepare;
		if(PhotonNetwork.IsMasterClient)
		{
            int rnd = Random.Range(0,players.Count);
            currentPlayer = players[rnd];
            PV.RPC("RPC_SetCurrentPlayer",RpcTarget.OthersBuffered,rnd);
		}
    }

    // Update is called once per frame
    void Update()
    {
        //En el estado selecting, 
		switch(currentState)
		{           
            case EGameState.SELECTING:
                //Resta el tiempo en hacer una acción
                currentTime-=Time.deltaTime;
                time.text = currentTime.ToString("F0");
				if(Input.touchCount>0)
				{
                    //Mira que jugador has selecionado para hacerle daño
                    Touch touch = Input.GetTouch(0);
					Ray rayo = Camera.main.ScreenPointToRay(touch.position);                
                    if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					{
                        PhotonView enemyPV = hit.transform.GetComponent<PhotonView>();
						if(enemyPV!=null)
						{
                            //enemyPV.Owner
						}
					}   
				}
                //Si el tiempo llega a 0 se canvia el estado a attacking
		        if(currentTime<0)
		        {
                    currentTime = timeToPrepare;
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
            
            case EGameState.SELECTING:
                break;
            case EGameState.ATTACKING:
                break;
		}
		switch(currentState)
		{            
            case EGameState.SELECTING:
                break;  
            case EGameState.ATTACKING:
                break;
		}
        currentState = nextState;
	}
    public void AddPlayer(PlayerOnlineController player){players.Add(player);}
    //Setea el turno del jugador
    [PunRPC]
    void RPC_SetCurrentPlayer(int playerId)
	{
        currentPlayer = players[playerId];
	}
}
