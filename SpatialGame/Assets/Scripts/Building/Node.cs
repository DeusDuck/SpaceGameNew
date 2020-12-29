using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    [SerializeField]
    GameObject availableBuilding;
    [SerializeField]
    LayerMask buildingLayer;
    NodeManager nodeManager;
    BuildingType myBuildingType;
    [SerializeField]
    Node topNeightboor;
    [SerializeField]
    Node bottomNeightboor;
    [SerializeField]
    Node leftNeightboor;
    [SerializeField]
    Node rightNeightboor;
    [SerializeField]
    bool canBeBuilt = true;
    [SerializeField]
    bool isBuilt = false;
    float builtTime;
    float currentBuildTime;
    MeshRenderer[] mat;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
	}
	private void Update()
	{
		if(builtTime!=0)
        {
            currentBuildTime+=Time.deltaTime;
            if(currentBuildTime>=builtTime)
            {
                Renderer[] childRenderer = availableBuilding.transform.GetComponentsInChildren<MeshRenderer>();
                for(int i = 0; i<childRenderer.Length; i++)
                    childRenderer[i].material = mat[i].sharedMaterial;

                SetIsBuilt(true);               
                availableBuilding.transform.tag = "Building";
                var navMeshSurface = availableBuilding.GetComponent<NavMeshSurface>();
                if(navMeshSurface != null)
                    NavMeshManager.CalculateNavMesh(navMeshSurface);
                if(myBuildingType.GetBuildingType() == BuildingType.EBuildingType.PIPE)
                {
                    NavMeshManager.AddPipe(myBuildingType.GetComponent<PipeRoom>());
                    NavMeshManager.CalculateOffMeshLinks();
                }
                currentBuildTime = 0;
                builtTime = 0;
                myBuildingType.myNodeManager.BuildNode(this);
            }
        }
	}
	//Recibe un edificio y un material y lo instancia en la escena
	public void SetAvailableBuilding(GameObject _building, Material availableMat)
    {
        canBeBuilt = true;
        availableBuilding = Instantiate(_building,transform.position,_building.transform.rotation,transform);            
        myBuildingType = availableBuilding.transform.GetComponent<BuildingType>();
        
        myBuildingType.SetManager(nodeManager,this);
        if(myBuildingType.GetBuildingType() == BuildingType.EBuildingType.PIPE)               
            myBuildingType.ConnectPipe();               

       myBuildingType.ChangeMaterial(availableMat);                        
        
    }
    //Canvia el material del edificio y setea el nodo a construido
    public void BuildBuilding(MeshRenderer[] actualMat)
    {
        builtTime = myBuildingType.builtTime;
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
    public void HideBuilding(bool hide)
    {
        if(availableBuilding!=null)
            availableBuilding.GetComponent<BuildingType>().HideBuilding(hide);
    }

    //Crea una caja fisica en su posición, si colisiona con un edificio accede al nodo que lo contiene y comprueba si este está construido.
    //Si lo está, quiere decir que él también, se setea como contruido. Si no lo está y this si lo está le destruye su edificio porque no es una posición válida
    public void CheckIfBuildingColliding()
    {        
        BoxCollider col = myBuildingType.GetCollider();
        if(col!=null)
        {
            Vector3 worldCenter = col.transform.TransformPoint(col.center);
            Collider[] colliders = Physics.OverlapBox(worldCenter, col.size * 0.4f, myBuildingType.transform.rotation, buildingLayer);

            if(colliders.Length != 0)
            {            
                foreach(Collider collider in colliders)
                {
                    Node parent = collider.transform.GetComponentInParent<Node>();  
                    if(parent!=this)
                    {       
                        if(parent.GetIsBuilt() && !IsNeightboor(parent))
                        {                            
                            canBeBuilt = false;
                            myBuildingType.ChangeMaterial(nodeManager.unAvailablePositionMat);
                        }                                          
                    }
                    if(colliders.Length == 1 && !canBeBuilt)
                    {
                        canBeBuilt = true;
                        myBuildingType.ChangeMaterial(nodeManager.availablePositionMat);
                    }
                }
            }
        }        
    }    

# region Setters and Getters

    //Seta si el nodo está construido
	void SetIsBuilt(bool built){ isBuilt = built;}
    //Retorna si el nodo está construido
    public bool GetIsBuilt(){ return isBuilt;} 
    //Inicia las principales variables del nodo
    public void InitNode(NodeManager _nodeManager){nodeManager= _nodeManager;}
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

    bool IsNeightboor(Node node)
    {
        return(node == topNeightboor) || (node == bottomNeightboor) || (node==leftNeightboor) || (node == rightNeightboor);
    }
	#endregion

}
