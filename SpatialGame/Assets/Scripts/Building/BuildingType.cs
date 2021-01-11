using System.Collections;
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
    [SerializeField]
    Transform zoomObjective;    
    [SerializeField]
    List<Transform> buildingPositions;
    public NodeManager myNodeManager;
    public Node myNode;
    public float builtTime;
    [SerializeField]
    Transform anchorPointLeft;
    [SerializeField]
    Transform anchorPointRight;
    [SerializeField]
    Transform anchorPointTop;
    [SerializeField]
    Transform anchorPointBottom;
    [SerializeField]
    GameObject meshObject;
    [SerializeField]
    BoxCollider myCollider;
    [SerializeField]
    int oxigenCost = 0;
    [SerializeField]
    int moneyCost = 0;
    [SerializeField]
    int foodCost = 0;
    [SerializeField]
    NavMeshSurface myNavMesh;
    [SerializeField]
    LayerMask buildingLayer;
    bool canBeBuilt = false;
    [SerializeField]
    int numOfNodes;

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.blue;
        foreach(Transform t in buildingPositions)
        {
            Gizmos.DrawWireSphere(t.position,1.0f);
        }
	}	
	
	public List<ExitsPosition> GetExitsType(){return currentExits;}
    public EBuildingType GetBuildingType(){return currentType; }
    public Transform GetZoomObjective(){ return zoomObjective;}
    
    public void UpdateExits(List<ExitsPosition> updated, Transform top, Transform bottom, Transform left, Transform right)
    {
        currentExits = updated;
        anchorPointTop = top;
        anchorPointBottom = bottom;
        anchorPointLeft = left;
        anchorPointRight = right;
    }
    public void RemoveBuildingPosition(Transform pos){buildingPositions.Remove(pos);}
    public List<Transform> GetCurrentBuildingPositions(){return buildingPositions;}
    public Transform GetLeftPos(){return anchorPointLeft;}
    public Transform GetRightPos(){return anchorPointRight;}
    public Transform GetTopPos(){return anchorPointTop;}
    public Transform GetBottomPos(){return anchorPointBottom;}
    public void SetManager(NodeManager manager){myNodeManager = manager;}
    public void ConnectPipe()
    {        
        PipeRoom pipe = GetComponent<PipeRoom>();
        if(pipe!=null)
        {
            pipe.SetPipe(myNodeManager,myNode,currentExits);
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
    public void CheckBuildingConnected()
    {
        List<Transform> trans = new List<Transform>();
        foreach(Transform t in buildingPositions)
        {
            Collider[] colliders = Physics.OverlapSphere(t.position, 1.0f,buildingLayer);
            
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
        foreach(Transform t in trans)
        {
            buildingPositions.Remove(t);
        }
    }  
    
    public BoxCollider GetCollider(){return myCollider;}
    public int[] MyCost()
    {
        int[] costs = new int[3];
        costs[0] = oxigenCost;
        costs[1] = moneyCost;
        costs[2] = foodCost;
        return costs;
    }
    public NavMeshSurface GetNavMeshSurface(){return myNavMesh;}
    public void SetNavMeshSurface(NavMeshSurface nav){myNavMesh = nav;}
    public bool CheckIfBuildingColliding()
    {        
        Vector3 worldCenter = myCollider.transform.TransformPoint(myCollider.center);
        Collider[] colliders = Physics.OverlapBox(worldCenter, myCollider.size * 0.5f, transform.rotation, buildingLayer);

        if(colliders.Length != 0)
        {            
            foreach(Collider col in colliders)
            {
                if(col.tag == "Node")
                {
                    Node node = col.GetComponent<Node>();
                    if(canBeBuilt && myNodeManager.EnoughCurrency(node) && !node.GetIsBuilt() )
                    {
                        if(currentType != EBuildingType.PIPE)
						{
                            myNodeManager.SpendResources(MyCost());
                            col.transform.position = transform.position;
                            transform.SetParent(col.transform);                            
                            myNode = node;
                            node.BuildBuilding(myNodeManager.currentMats, this);
                            node.SetAvailableBuilding(this.gameObject,myNodeManager.availablePositionMat);                            
						}
						else
						{
                            col.transform.position = transform.position;
                            transform.SetParent(col.transform);
                            myNode = node;
                            node.SetAvailableBuilding(this.gameObject,myNodeManager.availablePositionMat);
                            myNodeManager.visualManager.ShowBuildingsMenu(transform);
                            
						} 
                        return true;
                    }                    
                }
            }
        }
        return false;
               
    }    
    public void SetCanBeBuild(bool can){canBeBuilt = can;}
    public void AddNode()
    {
        numOfNodes++;        
    }
    public void RemoveNode()
    {
        if(numOfNodes>0)
            numOfNodes--;        
    }
    public int GetNumOfNodes(){return numOfNodes;}
    public void HasToChangeMat()
    {
        if(canBeBuilt)
            ChangeMaterial(myNodeManager.availablePositionMat); 
        else
            ChangeMaterial(myNodeManager.unAvailablePositionMat);         
    }
}
