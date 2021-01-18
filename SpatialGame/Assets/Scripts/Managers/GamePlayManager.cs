using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GamePlayManager : MonoBehaviour
{
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
		switch(currentState)
		{           
            case EGameState.SELECTING:
                currentTime-=Time.deltaTime;
                time.text = currentTime.ToString("F0");
				if(Input.touchCount>0)
				{
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
    [PunRPC]
    void RPC_SetCurrentPlayer(int playerId)
	{
        currentPlayer = players[playerId];
	}
}
