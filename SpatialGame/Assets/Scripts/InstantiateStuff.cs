using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstantiateStuff : MonoBehaviourPun
{
    public GameObject obj;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && base.photonView.IsMine)
            Instantiate(obj,transform.position,transform.rotation);
    }
}
