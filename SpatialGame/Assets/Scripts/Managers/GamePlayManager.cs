using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

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
    [SerializeField]
    Transform panel;
    int current = 0;
    
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
            current = Random.Range(0,players.Count);
            Player[] _players = PhotonNetwork.PlayerList;
            for(int i = 0; i<_players.Length;i++)
			{
                PV.RPC("RPC_SetCurrentPlayer",_players[i],current);
			}
            
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
				if(Input.GetMouseButtonDown(0) && PV.IsMine)
				{
                    Player[] _players = PhotonNetwork.PlayerList;
                    for(int i = 0; i<_players.Length;i++)
			        {
                        PV.RPC("RPC_SetCurrentPlayer",_players[i],current);
			        }
				}
                    
				if(Input.touchCount>0 && currentPlayer.GetPV().IsMine)
				{
                    
                    //Mira que jugador has selecionado para hacerle daño
                    Touch touch = Input.GetTouch(0);
					Ray rayo = Camera.main.ScreenPointToRay(touch.position);  
                    //Debug.Log(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide));
                    if(Physics.Raycast(rayo,out RaycastHit hit, 1000, layerToCollide))
					{                       
                        GameObject enemyPV = hit.transform.GetComponent<GameObject>();
						if(enemyPV!=null && currentPlayer.GetMySoldiers().Contains(enemyPV))
						{
                            panel.gameObject.SetActive(true);
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
            
            case EGameState.SELECTING:
                currentTime = timeToPrepare;
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
    void SetPlayersSoldiers()
	{
        //foreach(PhotonView controlable in GameSetUp.gameSetUp.soldiers)
		{
            
		}
	}
    //Setea el turno del jugador
    [PunRPC]
    void RPC_SetCurrentPlayer(int playerId)
	{
        if(currentPlayer !=null)
            currentPlayer.transform.position = Vector3.up * 10;
        currentPlayer = players[playerId];
        currentPlayer.transform.position = Vector3.zero;
        current = (current++)%players.Count;
	}
    [PunRPC]
    void RPC_SetPlayers()
	{
        
	}
}
