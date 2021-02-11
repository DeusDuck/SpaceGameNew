using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    [SerializeField]
    GameObject availableBuilding;
    [SerializeField]
    NodeManager myNodeManager;
    [SerializeField]
    BuildingType myBuildingType;
    BuildingType myCreator;
    [SerializeField]
    Node topNeightboor;
    [SerializeField]
    Node bottomNeightboor;
    [SerializeField]
    Node leftNeightboor;
    [SerializeField]
    Node rightNeightboor;
    bool canBeBuilt = true;
    [SerializeField]
    bool isBuilt = false;
    float builtTime;
    MeshRenderer[] mat;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);        
	}
	private void Update()
	{
		if(builtTime>0)
        {            
            builtTime-=Time.deltaTime;
            myBuildingType.UpdateThreshold(builtTime);
            if(builtTime<=0)
            {
                Renderer[] childRenderer = availableBuilding.transform.GetComponentsInChildren<MeshRenderer>();
                for(int i = 0; i<childRenderer.Length; i++)
                    childRenderer[i].material = mat[i].sharedMaterial;
                SetIsBuilt(true); 
                var navMeshSurface = myBuildingType.GetNavMeshSurface();
                if(navMeshSurface != null)
                    NavMeshManager.CalculateNavMesh(navMeshSurface);
                if(myBuildingType.GetBuildingType() == BuildingType.EBuildingType.PIPE)
                {
                    NavMeshManager.AddPipe(myBuildingType.GetComponent<PipeRoom>());
                    NavMeshManager.CalculateOffMeshLinks();
                }
                myNodeManager.BuildNode(this);    
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if(isBuilt)
            return;
        if(col.tag == "Building")
        {
            myBuildingType = col.GetComponent<BuildingType>();
            if(myCreator!=myBuildingType)
            {
                myBuildingType.AddNode();
                if(myBuildingType.GetBuildingType() == BuildingType.EBuildingType.PIPE )
                {
                    if(myBuildingType.GetNumOfNodes() <= 1)
                    {
                        myBuildingType.SetCanBeBuild(true);
                        myBuildingType.HasToChangeMat();
                        availableBuilding = myBuildingType.gameObject;
                    }                    
                }
                else
                {
                    if(myNodeManager.HasNeightboorsWithPipes(this) && myBuildingType.GetNumOfNodes() <= 1)
                    {                         
                        myBuildingType.SetCanBeBuild(true);
                        myBuildingType.HasToChangeMat();
                        availableBuilding = myBuildingType.gameObject;
                    }else
                    {
                        myBuildingType.SetCanBeBuild(false);
                        myBuildingType.HasToChangeMat();
                        myBuildingType = null;
                        availableBuilding = null;
                    }
                }
            }            
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(isBuilt)
            return;
        if(col.tag == "Building")
        {
            Node parent = col.GetComponentInParent<Node>();
            BuildingType type = col.GetComponent<BuildingType>();
            if(parent==null || !parent.GetIsBuilt())
            {
                type.RemoveNode();
                if(type!=null)
                {                    
                    if(type && builtTime==0)
                    {
                        type.SetCanBeBuild(false);
                        type.HasToChangeMat();
                        availableBuilding = null;
                    }   
                }                    
            }                     
        }
    }
	//Recibe un edificio y un material y lo instancia en la escena
	public void SetAvailableBuilding(GameObject _building, Material availableMat)
    {
        availableBuilding = _building;            
        myBuildingType = availableBuilding.transform.GetComponent<BuildingType>();
        //myBuildingType.myNode = this;
        myBuildingType.SetManager(myNodeManager);
        myNodeManager.UpdateNodeDistance(this);
        if(myBuildingType.GetBuildingType() == BuildingType.EBuildingType.PIPE)
		{
            myBuildingType.ConnectPipe(); 
		}               
                          

       myBuildingType.ChangeMaterial(myNodeManager.buildingMaterial);       
    }
    //Canvia el material del edificio y setea el nodo a construido
    public void BuildBuilding(MeshRenderer[] actualMat, BuildingType type)
    {
        builtTime = type.builtTime;
        mat = actualMat;
        canBeBuilt = false; 
    }
    //Destruye el edificio actual
    public void DestroyAvailableBuilding()
    {
        Destroy(availableBuilding);
        if(isBuilt)
            SetIsBuilt(false);
    }
    public void BuildBuilding(MeshRenderer[] actualMat)
    {
        builtTime = myBuildingType.builtTime;
        mat = actualMat;
        canBeBuilt = false;
    }
# region Setters and Getters

    //Seta si el nodo está construido
	void SetIsBuilt(bool built){ isBuilt = built;}
    //Retorna si el nodo está construido
    public bool GetIsBuilt(){ return isBuilt;} 
    //Inicia las principales variables del nodo
    public void InitNode(NodeManager _nodeManager, BuildingType creator){myNodeManager= _nodeManager;myCreator = creator;}
    public BuildingType GetBuildingType(){return myBuildingType;}
    public bool HasAvailableBuilding(){return availableBuilding!=null;}
    public GameObject GetAvailableBuilding(){return availableBuilding;}
    public void AddNeightboors(Node node)
    {
        float xDiference = Mathf.Abs(node.transform.position.x-transform.position.x);
        float yDiference = Mathf.Abs(node.transform.position.y-transform.position.y);
        
        if(xDiference>=yDiference)
        {
			if(node.transform.position.x>transform.position.x)
                    rightNeightboor = node;                
            else
                    leftNeightboor = node;
        }else
        {
            if(node.transform.position.y>transform.position.y)
                    topNeightboor = node;
            else
                    bottomNeightboor = node;
        }
    }
    public Node GetTopNode(){return topNeightboor;}
    public Node GetBottomNode(){return bottomNeightboor;}
    public Node GetLeftNode(){return leftNeightboor;}
    public Node GetRightNode(){return rightNeightboor;}
    public bool CanBeBuild(){return canBeBuilt;}
    public void SetCanBeBuild(bool can){canBeBuilt = can;}
    public Transform GetAvailableBuildingTransform(){return availableBuilding.transform;}
    public float GetBuiltTime(){return builtTime;}
    bool IsNeightboor(Node node)
    {
        return(node == topNeightboor) || (node == bottomNeightboor) || (node==leftNeightboor) || (node == rightNeightboor);
    }
	#endregion

}
