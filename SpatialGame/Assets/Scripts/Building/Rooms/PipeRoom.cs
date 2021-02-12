using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PipeRoom : BuildingType
{          
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
    bool CanConnect(List<ExitsPosition> updatedExits)
    {
        Node up = myNode.GetTopNode();
        Node down = myNode.GetBottomNode();
        Node left = myNode.GetLeftNode();
        Node right = myNode.GetRightNode();

        if(up!=null && up.GetIsBuilt())
            if( up.GetBuildingType().GetExitsType().Contains(ExitsPosition.BOTTOM) && updatedExits.Contains(ExitsPosition.TOP))
                return true;
        if(down!=null && down.GetIsBuilt())
            if( down.GetBuildingType().GetExitsType().Contains(ExitsPosition.TOP) && updatedExits.Contains(ExitsPosition.BOTTOM))
                return true;
        if(left!=null && left.GetIsBuilt())
            if( left.GetBuildingType().GetExitsType().Contains(ExitsPosition.RIGHT) && updatedExits.Contains(ExitsPosition.LEFT))
                return true;              
        if(right!=null && right.GetIsBuilt())
            if( right.GetBuildingType().GetExitsType().Contains(ExitsPosition.LEFT) && updatedExits.Contains(ExitsPosition.RIGHT))
                return true;

        return false;
    }
    public void RotateTillConnect()
    {
        float degrees = 0;
        List<ExitsPosition> updatedPos = new List<ExitsPosition>();
        for(int i = 0; i<4;i++)
        {
            if(CanConnect(currentExits))
                break;
            foreach(ExitsPosition pos in currentExits)
            {
                switch(pos)
                {
                    case ExitsPosition.LEFT:
                        updatedPos.Add(ExitsPosition.TOP);
                        break;
                    case ExitsPosition.RIGHT:
                        updatedPos.Add(ExitsPosition.BOTTOM);
                        break;
                    case ExitsPosition.TOP:
                        updatedPos.Add(ExitsPosition.RIGHT);
                        break;
                    case ExitsPosition.BOTTOM:                        
                        updatedPos.Add(ExitsPosition.LEFT);
                        break;
                }
            }            
            currentExits.Clear();
            foreach(ExitsPosition pos in updatedPos)
            {
                if(!currentExits.Contains(pos))
                {
                    currentExits.Add(pos);
                }                    
            }                
            updatedPos.Clear();
            degrees = (degrees - 90) % 360;
            UpdateExitsTransforms(true);            
        }
        transform.Rotate(0,0,degrees);         
        base.UpdateExits(currentExits,anchorPointTop,anchorPointBottom,anchorPointLeft,anchorPointRight);
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
        if(!myNode.GetIsBuilt() && mustChangeNavMesh)
        {
            List<ExitsPosition> updatedPos = new List<ExitsPosition>();
            float degrees;            
            if(clockwise)
            {
                degrees = -90;
                foreach(ExitsPosition pos in currentExits)
                {
                    switch(pos)
                    {
                        case ExitsPosition.LEFT:
                            updatedPos.Add(ExitsPosition.TOP);
                            break;
                        case ExitsPosition.RIGHT:
                            updatedPos.Add(ExitsPosition.BOTTOM);
                            break;
                        case ExitsPosition.TOP:
                            updatedPos.Add(ExitsPosition.RIGHT);
                            break;
                        case ExitsPosition.BOTTOM:
                            updatedPos.Add(ExitsPosition.LEFT);
                            break;
                    }
                }            
            }else
            {
                degrees = 90;
                foreach(ExitsPosition pos in currentExits)
                {
                    switch(pos)
                    {
                        case ExitsPosition.LEFT:                        
                            updatedPos.Add(ExitsPosition.BOTTOM);
                            break;
                        case ExitsPosition.RIGHT:                       
                            updatedPos.Add(ExitsPosition.TOP);
                            break;
                        case ExitsPosition.TOP:                                                
                            updatedPos.Add(ExitsPosition.LEFT);
                            break;
                        case ExitsPosition.BOTTOM:                      
                            updatedPos.Add(ExitsPosition.RIGHT);                       
                            break;
                    }
                }        
            }
            if(CanConnect(updatedPos))
            {
                transform.Rotate(0,0,degrees);
                UpdateExitsTransforms(clockwise);
                base.UpdateExits(updatedPos, anchorPointTop, anchorPointBottom, anchorPointLeft,anchorPointRight);
                currentExits = updatedPos;
                SetActiveNavMesh();
                myNodeManager.UpdateNodeDistance(myNode);
				if(CheckIfBuildingColliding())
				{
                    SetCanBeBuild(false);
                    HasToChangeMat();
				}
				else
				{
                    SetCanBeBuild(true);
                    HasToChangeMat();
				}
            }            
        }  
    }    
    public void SetPipe(NodeManager manager, Node node){myNodeManager = manager; myNode = node;} 
    public Transform GetLink(){return currentLink;}
    public void CreateLink()
    {
        if(myNode.GetTopNode()!=null && myNode.GetIsBuilt() && myNode.GetTopNode().GetIsBuilt())
        {            
            PipeRoom neightBoor = myNode.GetTopNode().GetBuildingType().GetComponent<PipeRoom>();
            if(neightBoor!=null)
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
                SetNavMeshSurface(navMeshLongLeft.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkLeft;
                currentLink = linkLeft;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 180)
            {
                navMeshLongRight.gameObject.SetActive(true);
                SetNavMeshSurface(navMeshLongRight.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkRight;
                currentLink = linkRight;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 90 && currentExits.Contains(BuildingType.ExitsPosition.BOTTOM))
            {
                navMeshShortTop.gameObject.SetActive(true);
                SetNavMeshSurface(navMeshShortTop.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkBottom;
                currentLink = linkBottom;
            }
            if(Quaternion.Angle(rotation,transform.rotation) == 90 && currentExits.Contains(BuildingType.ExitsPosition.TOP))
            {
                navMeshShortBottom.gameObject.SetActive(true);
                SetNavMeshSurface(navMeshShortBottom.GetComponent<NavMeshSurface>());
                offMeshLink.startTransform = linkTop;
                currentLink = linkTop;
            }
        }               
    }
}
    
