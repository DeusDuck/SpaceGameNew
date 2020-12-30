using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingType : MonoBehaviour
{
    public enum EBuildingType
    {
        CLONING,DINNER,PIPE
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

	private void OnDrawGizmos()
	{
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(myCollider.transform.TransformPoint(myCollider.center), transform.rotation, myCollider.size * 0.45f);
        Gizmos.matrix = rotationMatrix;
		Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero,Vector3.one);
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
    public void SetManager(NodeManager manager, Node currentNode){myNodeManager = manager; myNode = currentNode;}
    public void HideBuilding(bool hide){meshObject.SetActive(!hide);}
    public void ConnectPipe()
    {        
        PipeRoom pipe = GetComponent<PipeRoom>();
        if(pipe!=null)
        {
            pipe.SetPipe(myNodeManager,myNode,currentExits);
            pipe.RotateTillConnect(); 
        }                        
    }
    public void ChangeMaterial(Material currentMaterial)    {
        
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
            Collider[] colliders = Physics.OverlapSphere(t.position, 0.5f);            
            foreach(Collider col in colliders)
            {
                if(col.transform == transform)
                    continue;
                
               if(col.transform.GetComponentInParent<Node>().GetIsBuilt())
                    trans.Add(t);                    
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
}
