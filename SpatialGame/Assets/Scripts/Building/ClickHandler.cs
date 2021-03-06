﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour
{
    public NodeManager nodeManager;
    public LayerMask hitableLayer;

    public VisualManager visualManager;

    // Update is called once per frame
    void Update()
    {
        //Al pulsar el botón derecho del ratón lanza un rayo que si colisiona con un nodo se lo pasa al nodeManager
        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(0).phase != TouchPhase.Moved)
        {
            if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Ray rayo = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if(Physics.Raycast(rayo, out RaycastHit hitInfo,hitableLayer))
                {                 
                    if(hitInfo.collider.transform.tag == "Node")
                    {                        
                        Node node = hitInfo.collider.transform.GetComponentInParent<Node>();
                        nodeManager.GetClickedNode(node);                        
                        visualManager.ShowBuildingsMenu(Input.GetTouch(0).position,node.GetAvailableBuildingTransform());
                    }
                    if(hitInfo.collider.transform.tag == "Building")
                    {                
                       visualManager.ShowBuildingsMenu(Input.GetTouch(0).position,hitInfo.collider.transform);
                    }
                }
            }
        }
        
    }
}
