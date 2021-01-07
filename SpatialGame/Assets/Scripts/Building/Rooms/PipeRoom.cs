﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PipeRoom : MonoBehaviour
{
    [SerializeField]
    BuildingType myType;
    [SerializeField]
    Transform anchorPointLeft;
    [SerializeField]
    Transform anchorPointRight;
    [SerializeField]
    Transform anchorPointTop;
    [SerializeField]
    Transform anchorPointBottom;
    List<BuildingType.ExitsPosition> myExits;
    NodeManager myNodeManager;
    Node myNode;
    [SerializeField]
    Transform linkLeft;
    [SerializeField]
    Transform linkRight;
    [SerializeField]
    Transform linkTop;
    [SerializeField]
    Transform linkBottom;
    Transform currentLink;
    [SerializeField]
    OffMeshLink offMeshLink;
    [SerializeField]
    Transform navMeshLongLeft;
    [SerializeField]
    Transform navMeshLongRight;
    [SerializeField]
    Transform navMeshShortTop;
    [SerializeField]
    Transform navMeshShortBottom;

    Transform initBottomTransform;
    Transform initTopTransform;
    Transform initLeftTranform;
    Transform initRightTransform;
    Quaternion rotation;
    public bool mustChangeNavMesh;


	private void Awake()
	{
		initBottomTransform = anchorPointBottom;
        initLeftTranform = anchorPointLeft;
        initRightTransform = anchorPointRight;
        initTopTransform = anchorPointTop;
        rotation = transform.rotation;
        if(offMeshLink!=null)
        currentLink = offMeshLink.startTransform;
	}
    public void RotatePipe(bool clockwise = false)
    {
        UpdateExits(clockwise);
    }
    bool CanConnect(List<BuildingType.ExitsPosition> updatedExits)
    {
        Node up = myNode.GetTopNode();
        Node down = myNode.GetBottomNode();
        Node left = myNode.GetLeftNode();
        Node right = myNode.GetRightNode();

        if(up!=null && up.GetIsBuilt())
            if( up.GetBuildingType().GetExitsType().Contains(BuildingType.ExitsPosition.BOTTOM) && updatedExits.Contains(BuildingType.ExitsPosition.TOP))
                return true;
        if(down!=null && down.GetIsBuilt())
            if( down.GetBuildingType().GetExitsType().Contains(BuildingType.ExitsPosition.TOP) && updatedExits.Contains(BuildingType.ExitsPosition.BOTTOM))
                return true;
        if(left!=null && left.GetIsBuilt())
            if( left.GetBuildingType().GetExitsType().Contains(BuildingType.ExitsPosition.RIGHT) && updatedExits.Contains(BuildingType.ExitsPosition.LEFT))
                return true;              
        if(right!=null && right.GetIsBuilt())
            if( right.GetBuildingType().GetExitsType().Contains(BuildingType.ExitsPosition.LEFT) && updatedExits.Contains(BuildingType.ExitsPosition.RIGHT))
                return true;

        return false;
    }
    public void RotateTillConnect()
    {
        float degrees = 0;
        List<BuildingType.ExitsPosition> updatedPos = new List<BuildingType.ExitsPosition>();
        for(int i = 0; i<4;i++)
        {
            if(CanConnect(myExits))
                break;
            foreach(BuildingType.ExitsPosition pos in myExits)
            {
                switch(pos)
                {
                    case BuildingType.ExitsPosition.LEFT:
                        updatedPos.Add(BuildingType.ExitsPosition.TOP);
                        break;
                    case BuildingType.ExitsPosition.RIGHT:
                        updatedPos.Add(BuildingType.ExitsPosition.BOTTOM);
                        break;
                    case BuildingType.ExitsPosition.TOP:
                        updatedPos.Add(BuildingType.ExitsPosition.RIGHT);
                        break;
                    case BuildingType.ExitsPosition.BOTTOM:                        
                        updatedPos.Add(BuildingType.ExitsPosition.LEFT);
                        break;
                }
            }            
            myExits.Clear();
            foreach(BuildingType.ExitsPosition pos in updatedPos)
            {
                if(!myExits.Contains(pos))
                {
                    myExits.Add(pos);
                }                    
            }                
            updatedPos.Clear();
            degrees = (degrees - 90) % 360;
            UpdateExitsTransforms(true);            
        }
        transform.Rotate(0,0,degrees);         
        myType.UpdateExits(myExits,anchorPointTop,anchorPointBottom,anchorPointLeft,anchorPointRight);
        SetActiveNavMesh();
        myNodeManager.UpdateNodeDistance(myNode);
    }
    void UpdateExitsTransforms(bool clockwise = false)
    {
        if(!clockwise)
        {
            Transform savedTrans = initBottomTransform;
            anchorPointRight = initBottomTransform;
            anchorPointLeft = initTopTransform;
            anchorPointTop = initRightTransform;
            anchorPointBottom = initLeftTranform;

            initBottomTransform = initLeftTranform;
            initLeftTranform = initTopTransform;
            initTopTransform = initRightTransform;
            initRightTransform = savedTrans; 
        }else
        {
            Transform savedTrans = initTopTransform;
            anchorPointRight = initTopTransform;
            anchorPointLeft = initBottomTransform;
            anchorPointTop = initLeftTranform;
            anchorPointBottom = initRightTransform;

            initTopTransform = initLeftTranform;
            initLeftTranform = initBottomTransform;
            initBottomTransform = initRightTransform;                       
            initRightTransform = savedTrans;
        }
            
    }
    void UpdateExits(bool clockwise = false)
    {
        if(!myType.myNode.GetIsBuilt() && mustChangeNavMesh)
        {
            List<BuildingType.ExitsPosition> updatedPos = new List<BuildingType.ExitsPosition>();
            float degrees;            
            if(clockwise)
            {
                degrees = -90;
                foreach(BuildingType.ExitsPosition pos in myExits)
                {
                    switch(pos)
                    {
                        case BuildingType.ExitsPosition.LEFT:
                            updatedPos.Add(BuildingType.ExitsPosition.TOP);
                            break;
                        case BuildingType.ExitsPosition.RIGHT:
                            updatedPos.Add(BuildingType.ExitsPosition.BOTTOM);
                            break;
                        case BuildingType.ExitsPosition.TOP:
                            updatedPos.Add(BuildingType.ExitsPosition.RIGHT);
                            break;
                        case BuildingType.ExitsPosition.BOTTOM:
                            updatedPos.Add(BuildingType.ExitsPosition.LEFT);
                            break;
                    }
                }            
            }else
            {
                degrees = 90;
                foreach(BuildingType.ExitsPosition pos in myExits)
                {
                    switch(pos)
                    {
                        case BuildingType.ExitsPosition.LEFT:                        
                            updatedPos.Add(BuildingType.ExitsPosition.BOTTOM);
                            break;
                        case BuildingType.ExitsPosition.RIGHT:                       
                            updatedPos.Add(BuildingType.ExitsPosition.TOP);
                            break;
                        case BuildingType.ExitsPosition.TOP:                                                
                            updatedPos.Add(BuildingType.ExitsPosition.LEFT);
                            break;
                        case BuildingType.ExitsPosition.BOTTOM:                      
                            updatedPos.Add(BuildingType.ExitsPosition.RIGHT);                       
                            break;
                    }
                }        
            }
            if(CanConnect(updatedPos))
            {
                transform.Rotate(0,0,degrees);
                UpdateExitsTransforms(clockwise);
                myType.UpdateExits(updatedPos, anchorPointTop, anchorPointBottom, anchorPointLeft,anchorPointRight);
                myExits = updatedPos;
                SetActiveNavMesh();
                myNodeManager.UpdateNodeDistance(myNode);
            }            
        }  
    }  
    public void SetPipe(NodeManager manager, Node node, List<BuildingType.ExitsPosition> exits){myNodeManager = manager; myNode = node; myExits = exits;} 
    public Transform GetLink(){return currentLink;}
    public void CreateLink()
    {
        if(myNode.GetTopNode()!=null)
        {
            PipeRoom neightBoor = myNode.GetTopNode().GetBuildingType().GetComponent<PipeRoom>();
            offMeshLink.endTransform = neightBoor.GetLink();
        }        
    }
    void SetActiveNavMesh()
    {     
        if(mustChangeNavMesh)
        {
            navMeshLongLeft.gameObject.SetActive(false);
            navMeshLongRight.gameObject.SetActive(false);
            navMeshShortTop.gameObject.SetActive(false);
            navMeshShortBottom.gameObject.SetActive(false);

            if(Quaternion.Angle(rotation,transform.rotation) == 0)
            {
                navMeshLongLeft.gameObject.SetActive(true);
                myType.SetNavMeshSurface(navMeshLongLeft.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkLeft;
                currentLink = linkLeft;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 180)
            {
                navMeshLongRight.gameObject.SetActive(true);
                myType.SetNavMeshSurface(navMeshLongRight.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkRight;
                currentLink = linkRight;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 90 && myExits.Contains(BuildingType.ExitsPosition.BOTTOM))
            {
                navMeshShortTop.gameObject.SetActive(true);
                myType.SetNavMeshSurface(navMeshShortTop.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkBottom;
                currentLink = linkBottom;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 90 && myExits.Contains(BuildingType.ExitsPosition.TOP))
            {
                navMeshShortBottom.gameObject.SetActive(true);
                myType.SetNavMeshSurface(navMeshShortBottom.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkTop;
                currentLink = linkTop;
            }
        }               
    }
}
    
