using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public Transform spawningPoint;
    public Material availablePositionMat;
    public Material unAvailablePositionMat;
    public GameObject currentBuilding;
    [SerializeField]
    GameObject horizontalPipe;
    [SerializeField]
    GameObject verticalPipe;   
    [SerializeField]
    Node node;
    MeshRenderer[] currentMats;
    [SerializeField]
    NPCManager npcManager;
    [SerializeField]
    ResourceManager resourceManager;
    List<Node> buildNodes;
    List<Node> neightboors;

    // Start is called before the first frame update
    void Start()
    {  
        buildNodes = new List<Node>();  
        neightboors = new List<Node>();
        
        Node nodeToBuild =Instantiate(node,transform.position,transform.rotation);
        nodeToBuild.SetAvailableBuilding(currentBuilding,availablePositionMat);
        currentMats = currentBuilding.transform.GetComponentsInChildren<MeshRenderer>();
        nodeToBuild.BuildBuilding(currentMats);
        nodeToBuild.GetComponentInChildren<CloningRoom>().SetNPCManager(npcManager);        
        AddBuildNode(nodeToBuild);
        CreateNodesNeightboors();      
    }
	//Crea los vecinos de los nodos, para hacerlo, coje las posiciones de construccion de los edificios y las usa para instanciarlos
    void CreateNodesNeightboors()
    {
        BuildingType type;
        foreach(Node build in buildNodes)
        {
            type = build.GetBuildingType();

            if(type.GetCurrentBuildingPositions().Count == 0)
                continue;

            type.CheckBuildingConnected();
            List<Transform> trans = new List<Transform>();
            foreach(Transform t in type.GetCurrentBuildingPositions())
            {                   
                Node currentNode = Instantiate(node,t.position,node.transform.rotation);
                neightboors.Add(currentNode);
                currentNode.InitNode(this);
                currentNode.AddNeightboors(build);
                build.AddNeightboors(currentNode);
                trans.Add(t);
            }
            foreach(Transform t in trans)
            {
                type.RemoveBuildingPosition(t);
            }
        }
    }
    //Da a los nodos el edificio a construir
    public void SetUpNodes()
    {        
        foreach(Node node in neightboors)
        {
            if(node.GetIsBuilt())
                continue;
            //Mira si tiene un edificio ya construido y se lo destruye 
            if(node.HasAvailableBuilding() && node.GetAvailableBuilding()!= currentBuilding)
                node.DestroyAvailableBuilding();
            

            //Comprueba si tiene vecinos con tuberia y actualiza la distancia entre nodos
            if(HasNeightboorsWithPipes(node))
            {                
                //Le da el nuevo edificio
                node.SetAvailableBuilding(currentBuilding, availablePositionMat);
                UpdateNodeDistance(node);
            }
            else
            {
                //Destruye el edificio actual del nodo y mira si el edificio que tiene que construir es una tuberia
                node.DestroyAvailableBuilding();
                if(currentBuilding.GetComponent<BuildingType>().GetBuildingType() == BuildingType.EBuildingType.PIPE)
                {                    
                    Node up = node.GetTopNode();
                    Node down = node.GetBottomNode();
                    Node left = node.GetLeftNode();
                    Node right = node.GetRightNode();
                    List<BuildingType.ExitsPosition> exits = currentBuilding.GetComponent<BuildingType>().GetExitsType();                    
                                               
                    if(up != null && up.GetIsBuilt() || down != null && down.GetIsBuilt())
                    {
                        if(!exits.Contains(BuildingType.ExitsPosition.TOP) && !exits.Contains(BuildingType.ExitsPosition.BOTTOM))
                            currentBuilding = verticalPipe;
                    }                            
                    if(left!=null && left.GetIsBuilt() || right!=null && right.GetIsBuilt())
                    {
                        if(!exits.Contains(BuildingType.ExitsPosition.LEFT) && !exits.Contains(BuildingType.ExitsPosition.RIGHT))
                            currentBuilding = horizontalPipe;
                    }  
                    node.SetAvailableBuilding(currentBuilding,availablePositionMat);
                }               
            }            
        }
        //Miramos si los edificios colisionan, si es así se destruyen
        foreach(Node node in neightboors)
        {
            if(node.HasAvailableBuilding())
                node.CheckIfBuildingColliding();
        }  
    }
	//Canvia el edificio a construir y el material a usar
	public void SetCurrentBuilding(GameObject _currentBuilding)
    { 
        currentBuilding = _currentBuilding;
        currentMats = _currentBuilding.transform.GetComponentsInChildren<MeshRenderer>();        
        SetUpNodes();        
    } 
    //Actualiza la distancia entre los nodos dependiendo del edificio que se quiera construir
    public void UpdateNodeDistance(Node node)
    {
        var nodeType = node.GetBuildingType();
        Node up = node.GetTopNode();
        Node down = node.GetBottomNode();
        Node left = node.GetLeftNode();
        Node right = node.GetRightNode();

        if(up != null && up.GetIsBuilt())
        {
            if(nodeType.GetExitsType().Contains(BuildingType.ExitsPosition.TOP))
            {
                 node.transform.Translate(up.GetBuildingType().GetBottomPos().position - nodeType.GetTopPos().position);
            }
        }
        if(down != null && down.GetIsBuilt())
        {
            if(nodeType.GetExitsType().Contains(BuildingType.ExitsPosition.BOTTOM))
            {
                 node.transform.Translate(down.GetBuildingType().GetTopPos().position - nodeType.GetBottomPos().position);
            }
        }
        if(left != null && left.GetIsBuilt())
        {
            if(nodeType.GetExitsType().Contains(BuildingType.ExitsPosition.LEFT))
            {
                node.transform.Translate(left.GetBuildingType().GetRightPos().position - nodeType.GetLeftPos().position);
            }
        }
        if(right != null && right.GetIsBuilt())
        {
            if(nodeType.GetExitsType().Contains(BuildingType.ExitsPosition.RIGHT))
            {
                 node.transform.Translate(right.GetBuildingType().GetLeftPos().position - nodeType.GetRightPos().position);
            }
        }
    }
    //Comprueba si el nodo tiene vecinos con tuberias construidas
    bool HasNeightboorsWithPipes(Node node)
    {        
        if(node.GetBuildingType() !=null)
        {
            List<BuildingType.ExitsPosition> exitsPosEnum = currentBuilding.GetComponent<BuildingType>().GetExitsType();
            List<BuildingType.ExitsPosition> currentExits = new List<BuildingType.ExitsPosition>();
            
            //Mira si las conexiones de los nodos encajan(Si el nodo tiene la conexion TOP y el vecino de arriba la conexion BOTTOM, devuelve true)
            if(exitsPosEnum.Contains(BuildingType.ExitsPosition.TOP))
            {
                Node up = node.GetTopNode();
                if(up!=null && up.GetIsBuilt())
                {
                    currentExits = up.GetBuildingType().GetExitsType();
                    if(up.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.PIPE && currentExits.Contains(BuildingType.ExitsPosition.BOTTOM))
                       return true;
                }
            }
            //(Si el nodo tiene la conexion BOTTOM y el vecino de arriba la conexion TOP, devuelve true)
            if(exitsPosEnum.Contains(BuildingType.ExitsPosition.BOTTOM))
            {
                Node down = node.GetBottomNode();
                if(down!=null && down.GetIsBuilt())
                {
                    currentExits.Clear();
                    currentExits = down.GetBuildingType().GetExitsType();
                    if(down.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.PIPE && currentExits.Contains(BuildingType.ExitsPosition.TOP))
                        return true;
                }
            }
            //(Si el nodo tiene la conexion LEFT y el vecino de arriba la conexion RIGHT, devuelve true)
            if(exitsPosEnum.Contains(BuildingType.ExitsPosition.LEFT))
            {
                Node left = node.GetLeftNode();
                if(left!=null && left.GetIsBuilt())
                {
                    currentExits.Clear();
                    currentExits = left.GetBuildingType().GetExitsType();
                    if(left.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.PIPE && currentExits.Contains(BuildingType.ExitsPosition.RIGHT))
                        return true;
                }
            }
            //(Si el nodo tiene la conexion RIGHT y el vecino de arriba la conexion LEFT, devuelve true)
            if(exitsPosEnum.Contains(BuildingType.ExitsPosition.RIGHT))
            {
                Node right = node.GetRightNode();
                if(right!=null && right.GetIsBuilt())
                {
                    currentExits.Clear();
                    currentExits = right.GetBuildingType().GetExitsType();
                    if(right.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.PIPE && currentExits.Contains(BuildingType.ExitsPosition.LEFT))
                        return true;
                }
            }  
        }        
        return false;
    }
    //Recibe un nodo y construye el edificio actual, después calcula los nodos edificados y sus vecinos
    public void GetClickedNode(Node currentNode)
    {
        if(!currentNode.GetIsBuilt() && currentNode.CanBeBuild())
        {         
            if(currentNode.GetBuildingType().GetBuildingType()!=BuildingType.EBuildingType.PIPE)
            {            
                currentNode.BuildBuilding(currentMats);
                buildNodes.Add(currentNode);
                if(neightboors.Contains(currentNode))
                    neightboors.Remove(currentNode);
            
                if(currentNode.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.CLONING)
                    currentNode.GetComponentInChildren<CloningRoom>().SetNPCManager(npcManager);
                else if(currentNode.GetBuildingType().GetBuildingType() == BuildingType.EBuildingType.DINNER)
                {
                    ResourcesRoom room = currentNode.GetBuildingType().transform.GetComponent<ResourcesRoom>();
                    if(room!=null)
                    {
                        npcManager.AddBuilding(room);
                        room.SetResourceManager(resourceManager);
                    }
                }               

                CreateNodesNeightboors();
                SetUpNodes();
            }
        }
    }
    //Esconde a los vecinos(Se usa cuando dejas de construir)
    public void HideBuildings(bool hide)
    {
        foreach(Node node in neightboors)
        {
            node.HideBuilding(hide);
        }  
    }
    //Añade un nodo a la lista de los que están construidos
    public void AddBuildNode(Node node)
    {
        if(!buildNodes.Contains(node))
            buildNodes.Add(node);
    }  
    public void BuildPipe(Transform building)
    {
        if(building!=null)
        {
            Node node = building.transform.GetComponent<BuildingType>().myNode;
     
            if(node.CanBeBuild())
            {
                node.BuildBuilding(currentMats);
                buildNodes.Add(node);
                if(neightboors.Contains(node))
                    neightboors.Remove(node);
                CreateNodesNeightboors();
                SetUpNodes();
            }  
        }      
    }
    public void EraseBuilding(Node node)
    {
        Node up = node.GetTopNode();
        Node down = node.GetBottomNode();
        Node left = node.GetLeftNode();
        Node right = node.GetRightNode();
        if(up!=null && !up.GetIsBuilt())
        {
            neightboors.Remove(up);
            Destroy(up.gameObject);
        }            
        if(down!=null && !down.GetIsBuilt())
        {
            neightboors.Remove(down);
            Destroy(down.gameObject);
        }            
        if(left!=null && !left.GetIsBuilt())
        {
            neightboors.Remove(left);
            Destroy(left.gameObject);
        }            
        if(right!=null && !right.GetIsBuilt())
        {
            neightboors.Remove(right);
            Destroy(right.gameObject);
        }
        node.DestroyAvailableBuilding();
        buildNodes.Remove(node);
        if(!neightboors.Contains(node))
            neightboors.Add(node);

        node.SetAvailableBuilding(currentBuilding,availablePositionMat);       
    }
}
