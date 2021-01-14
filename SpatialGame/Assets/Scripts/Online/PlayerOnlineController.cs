using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerOnlineController : MonoBehaviour
{
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
		if(PhotonNetwork.IsMasterClient)
		{
            foreach(Transform pos in GameSetUp.gameSetUp.spawningPointsLocal)
			{
                PhotonNetwork.Instantiate("MultiplayerPrefabs/Cube", pos.position,pos.rotation,0);
			}
		}
		else
		{
            foreach(Transform pos in GameSetUp.gameSetUp.spawningPointsOther)
			{
                PhotonNetwork.Instantiate("MultiplayerPrefabs/Sphere", pos.position,pos.rotation,0);
			}
		}
    }
}
