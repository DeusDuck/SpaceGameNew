﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public Transform spawningPoint;
    public Material availablePositionMat;
    public Material unAvailablePositionMat;
    public GameObject currentBuilding;
    public MeshRenderer[] currentMats;
    [SerializeField]
    GameObject horizontalPipe;
    [SerializeField]
    GameObject verticalPipe;   
    [SerializeField]
    Node node;
    BuildingType currentBuildingType;
    [SerializeField]
    NPCManager npcManager;
    [SerializeField]
    ResourceManager resourceManager;
    List<Node> buildNodes;
    List<Node> neightboors;    
    public VisualManager visualManager;

    // Start is called before the first frame update
    void Start()
    {  
        buildNodes = new List<Node>();  
        neightboors = new List<Node>();        
        
        Node nodeToBuild =Instantiate(node,transform.position,transform.rotation);        
        GameObject building = Instantiate(currentBuilding,transform.position,currentBuilding.transform.rotation,nodeToBuild.transform);
        nodeToBuild.InitNode(this,building.GetComponent<BuildingType>()); 
        currentMats = currentBuilding.transform.GetComponentsInChildren<MeshRenderer>();
        nodeToBuild.SetAvailableBuilding(building,availablePositionMat);        
        nodeToBuild.GetBuildingType().builtTime = 0.01f;
        
        nodeToBuild.GetBuildingType().myNodeManager = this;
        nodeToBuild.BuildBuilding(currentMats,building.GetComponent<BuildingType>());        
        nodeToBuild.GetComponentInChildren<CloningRoom>().SetNPCManager(npcManager);        
        AddBuildNode(nodeToBuild);
        CreateNodesNeightboors();
        currentBuilding = null;
    }
    void Update()
    {
        if(Input.touchCount>0 && currentBuilding!=null)
        {
            Touch touch;
            if(Input.touchCount>0)
            {
                touch = Input.GetTouch(0);
                Vector3 touchPos = touch.position;
                touchPos.z = transform.position.z - Camera.main.transform.position.z;
                currentBuilding.transform.position = Camera.main.ScreenToWorldPoint(touchPos);
            }
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                BuildingType type = currentBuilding.GetComponent<BuildingType>();
                if(!type.CheckIfBuildingColliding())
                {
                    Destroy(currentBuilding);
                }
                currentBuilding = null;
                visualManager.DisplayBuildingMenu();
            }
        }        
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
                currentNode.InitNode(this,type);
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
    
    public void CreateBuilding(GameObject building)
    {
        currentBuilding = Instantiate(building,Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10f)),building.transform.rotation,spawningPoint);
        currentBuildingType = currentBuilding.GetComponent<BuildingType>();
        currentMats = building.transform.GetComponentsInChildren<MeshRenderer>(); 
        currentBuildingType.ChangeMaterial(unAvailablePositionMat);
        currentBuildingType.SetManager(this);
    }
	//Canvia el edificio a construir y el material a usar
	public void SetCurrentBuilding(GameObject _currentBuilding)
    { 
        currentBuilding = _currentBuilding;
        currentMats = _currentBuilding.transform.GetComponentsInChildren<MeshRenderer>();               
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
    public bool HasNeightboorsWithPipes(Node node)
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
    public void SpendResources(int[] cost)
    {
        resourceManager.SpendResources(cost);
    }
   public bool EnoughCurrency(Node node)
    {
        return resourceManager.EnoughResources(node.GetBuildingType().MyCost());
    }
    public void BuildNode(Node currentNode)
    {            
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

        buildNodes.Add(currentNode);
        if(neightboors.Contains(currentNode))
            neightboors.Remove(currentNode);
        CreateNodesNeightboors();
               
    }    
    public void BuildPipe(Transform building)
    {
        if(building!=null)
        {
            Node node = building.transform.GetComponent<BuildingType>().myNode;
     
            if(node.CanBeBuild())
            {
                resourceManager.SpendResources(node.GetBuildingType().MyCost());
                node.BuildBuilding(currentMats);
                buildNodes.Add(node);
                if(neightboors.Contains(node))
                    neightboors.Remove(node);
                visualManager.HidePipesMenu();
            }  
        }      
    }
    
    //Añade un nodo a la lista de los que están construidos
    public void AddBuildNode(Node node)
    {
        if(!buildNodes.Contains(node))
            buildNodes.Add(node);
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
