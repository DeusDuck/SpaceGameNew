﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingType : MonoBehaviour
{
    public enum EBuildingType
    {
        CLONING,RESOURCES,PIPE,DRONES
    }
    public EBuildingType currentType;
    
    public enum ExitsPosition
    {
       RIGHT,LEFT,TOP,BOTTOM
    }
    public List<ExitsPosition> currentExits;
    
    [Header("Building Attributes")]
    [Space(5)]
    [SerializeField]
    GameObject meshObject;
    [SerializeField]
    BoxCollider myCollider;
    [SerializeField]
    NavMeshSurface myNavMesh;
    [SerializeField]
    Transform zoomObjective;   
    public NodeManager myNodeManager;
    public ResourceManager myResourceManager;
    public Node myNode;
    [SerializeField]
    LayerMask buildingLayer;
    [SerializeField]
    LayerMask collisionLayer;
    bool canBeBuilt = false;
    [SerializeField]
    int numOfNodes;
    [Space(5)]

    [Header("Anchors")]
    [Space(5)]
    [SerializeField]
    protected Transform anchorPointLeft;
    [SerializeField]
    protected Transform anchorPointRight;
    [SerializeField]
    protected Transform anchorPointTop;
    [SerializeField]
    protected Transform anchorPointBottom;
    [SerializeField]
    List<Transform> buildingPositions; 
    [SerializeField]
    List<Transform> checkers;
    [Space(5)]
    
    [Header("Building Costs")]
    [Space(5)]
    [SerializeField]
    int oxigenCost = 0;
    [SerializeField]
    int moneyCost = 0;
    [SerializeField]
    int foodCost = 0;
    [SerializeField]
    int upgrateCostOxigen = 0;
    [SerializeField]
    int upgrateCostMoney = 0;
    [SerializeField]
    int upgrateCostFood = 0;
    public float builtTime;    
    

    private void OnDrawGizmos()
	{
        Gizmos.color = Color.blue;
        foreach(Transform t in buildingPositions)
        {
            Gizmos.DrawWireSphere(t.position,1.0f);
        }
	}
    
    public void UpdateExits(List<ExitsPosition> updated, Transform top, Transform bottom, Transform left, Transform right)
    {
        currentExits = updated;
        anchorPointTop = top;
        anchorPointBottom = bottom;
        anchorPointLeft = left;
        anchorPointRight = right;
    }
    public void RemoveBuildingPosition(Transform pos){buildingPositions.Remove(pos);}
    
    public void ConnectPipe()
    {        
        PipeRoom pipe = GetComponent<PipeRoom>();
        if(pipe!=null)
        {
            pipe.SetPipe(myNodeManager,myNode);
            pipe.RotateTillConnect(); 
        }                        
    }
    public void ChangeMaterial(Material currentMaterial)
    { 
        MeshRenderer[] childRenderer = meshObject.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer rnd in childRenderer)
        {
            rnd.material = currentMaterial;                
        }
    }
    public void RemoveBuiltPositions()
    {
        List<Transform> trans = new List<Transform>();
        foreach(Transform t in buildingPositions)
        {
            Collider[] colliders = Physics.OverlapSphere(t.position, 1.0f,collisionLayer);            
			if(colliders.Length>=1)
			{
                foreach(Collider col in colliders)
                {
                    if(col.transform == transform)
                        continue;

                   if(col.transform.GetComponentInParent<Node>().GetIsBuilt())
                   {
                        trans.Add(t); 
                   }                                      
                }
			}            
        }
        foreach(Transform t in trans)
        {
            buildingPositions.Remove(t);
        }
    }  
    public bool CheckIfCanBeBuild()
	{
        foreach(Transform t in checkers)
        {
            if(t==null)
                continue;

            Collider[] colliders = Physics.OverlapSphere(t.position, 0.3f,collisionLayer);
			if(colliders.Length>=1)
			{               
                foreach(Collider col in colliders)
                {
                    if(col.transform == transform)
                        continue;  
                    
                    return false;
                }
			}
        }
        return true;
	}    
    
    public bool CheckIfPipeCollides()
    {        
        foreach(Transform t in GetAnchorPoints())
        {
            if(t==null)
                continue;

            Collider[] colliders = Physics.OverlapSphere(t.position, 0.3f,collisionLayer);
			if(colliders.Length>=1)
			{               
                foreach(Collider col in colliders)
                {
                    if(col.transform == transform || myNode.IsNeightboor(col.GetComponentInParent<Node>()))
                        continue;  
                    
                    return false;
                }
			}
        }
        return true;
               
    } 
    public virtual void BuildBuilding()
	{
        if(canBeBuilt && myNodeManager.EnoughCurrency(myNode) && !myNode.GetIsBuilt())
        {
            myNodeManager.SpendResources(MyCost());
            myNode.transform.position = transform.position;
            transform.SetParent(myNode.transform);                            
            myNode.BuildBuilding(myNodeManager.currentMats, this);
            myNode.SetAvailableBuilding(this.gameObject);                            
        }
	}
    
    public void AddNode()
    {
        numOfNodes++;        
    }
    public void RemoveNode()
    {
        if(numOfNodes>0)
            numOfNodes--;        
    }
    
    public void HasToChangeMat()
    {
        if(canBeBuilt)
            ChangeMaterial(myNodeManager.availablePositionMat); 
        else
            ChangeMaterial(myNodeManager.unAvailablePositionMat);         
    }
    public void UpdateThreshold(float threshold)
	{   
        float current = 1-threshold/builtTime;
        MeshRenderer[] childRenderer = meshObject.GetComponentsInChildren<MeshRenderer>(); 
        foreach(MeshRenderer rnd in childRenderer)
        {
            rnd.material.SetFloat("_TimeThreshold",current);          
        }
	}
    public void UpgrateRoom()
	{
        int[] currency = {upgrateCostOxigen,upgrateCostMoney,upgrateCostFood};
        if(CanBeUpgrated())
		{
            myResourceManager.SpendResources(currency);
		}
	}
    public bool CanBeUpgrated()
	{
        List<int> costs = myResourceManager.GetAllResources();
        return (costs[0]>=upgrateCostFood && costs[1]>= upgrateCostMoney && costs[2]>= upgrateCostOxigen);
	}
	#region Getters and Setters
    public void SetCanBeBuild(bool can){canBeBuilt = can;}
	public bool GetCanBeBuilt(){return canBeBuilt;}
    public int GetNumOfNodes(){return numOfNodes;}
    public NavMeshSurface GetNavMeshSurface(){return myNavMesh;}
    public void SetNavMeshSurface(NavMeshSurface nav){myNavMesh = nav;}
    public List<Transform> GetCurrentBuildingPositions(){return buildingPositions;}
    public Transform GetLeftPos(){return anchorPointLeft;}
    public Transform GetRightPos(){return anchorPointRight;}
    public Transform GetTopPos(){return anchorPointTop;}
    public Transform GetBottomPos(){return anchorPointBottom;}
    public Transform[] GetAnchorPoints()
    {
        Transform[] anchors = {anchorPointBottom,anchorPointTop,anchorPointLeft,anchorPointRight};
        return anchors;
    }
    public void SetManager(NodeManager manager){myNodeManager = manager;}
    public List<ExitsPosition> GetExitsType(){return currentExits;}
    public EBuildingType GetBuildingType(){return currentType;}
    public Transform GetZoomObjective(){ return zoomObjective;}
    public int[] MyCost()
    {
        int[] costs = {oxigenCost,moneyCost,foodCost };
        return costs;
    }
	#endregion
}
