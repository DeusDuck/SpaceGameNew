using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        pipe.SetPipe(myNodeManager,myNode,currentExits);
        pipe.RotateTillConnect();                 
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
}
