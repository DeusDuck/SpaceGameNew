using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerOnlineController : MonoBehaviour
{
    [SerializeField]
    PhotonView PV;
    GamePlayManager gamePlayManager;
    List<IControlable> mySoldiers;

    // Start is called before the first frame update
    void Awake()
    {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        gamePlayManager.AddPlayer(this);
    }
    public void AddSoldier(IControlable soldier){mySoldiers.Add(soldier);}
}
