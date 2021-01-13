using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{    
    [Header("Transforms")]
    [Space(5)]
    [SerializeField]
    Transform buildButton;
    [SerializeField]
    Transform buildingPanel;    
    [SerializeField]
    Transform AISelection;
    [SerializeField]
    Transform pipesPanel;
    [SerializeField]
    Transform droneSelectionMenu;
    [Space(5)]

    [Header("Buttons")]
    [Space(5)]
    [SerializeField]
    Button rotateLeft;
    [SerializeField]
    Button rotateRight;    
    [SerializeField]
    Button eraseButton;
    [SerializeField]
    Button buildPipe;
    [SerializeField]
    Button clonningMenuButton;
    [SerializeField]
    Button sendDroneButton;   
    [SerializeField]
    Button inventoryButton;
    GameObject currentRoom;
    [Space(5)]

    [Header("Inventory Text")]
    [Space(5)]
    [SerializeField]
    Text money;
    [SerializeField]
    Text water;
    [SerializeField]
    Text oxigen;
    [SerializeField]
    Text rockText;
    [SerializeField]
    Text ironText;
    [SerializeField]
    Text woodText;
    [Space(5)]

    [Header("Other Scripts")]
    [Space(5)]
    [SerializeField]
    CameraMovement cameraMovement;
    [SerializeField]
    ClickHandler clickHandler;
    [SerializeField]
    GetEnumVisualState getEnumVisualState;
    
    public enum VisualState
    {
        BUILDING,ON_ROOM,MOVING_AROUND
    }
    public VisualState currentState = VisualState.MOVING_AROUND;

	//Activa el menu de construcción
    public void DisplayBuildingMenu()
    {
        buildButton.gameObject.SetActive(false);
        inventoryButton.gameObject.SetActive(false);
        buildingPanel.gameObject.SetActive(true);
    }
    //Desactiva el menu de construcción
    public void HideBuildingMenu()
    {
        buildButton.gameObject.SetActive(true);
        inventoryButton.gameObject.SetActive(true);
        buildingPanel.gameObject.SetActive(false);
        pipesPanel.gameObject.SetActive(false);
        rotateLeft.gameObject.SetActive(false);
        rotateRight.gameObject.SetActive(false);
        buildPipe.gameObject.SetActive(false);        
    }
    public void HidePipesMenu()
    {
        rotateLeft.gameObject.SetActive(false);
        rotateRight.gameObject.SetActive(false);
        buildPipe.gameObject.SetActive(false);
    }
    public void ShowBuildingsMenu(Transform building)
    {
        //Dependiendo del tipo de edificio que selecciones, enseñará un menú u otro.
        var type = building.GetComponent<BuildingType>();
        if(type.myNode != null && type.myNode.GetIsBuilt())
        {
            ShowGameObject(eraseButton.gameObject);
            eraseButton.onClick.RemoveAllListeners();
            eraseButton.onClick.AddListener(delegate{clickHandler.nodeManager.EraseBuilding(type.myNode);});
        }        
        switch(type.GetBuildingType())
        {
            case BuildingType.EBuildingType.CLONING:
                Button[] buttons = AISelection.GetComponentsInChildren<Button>();
                foreach(Button b in buttons)
                {
                    if(b.transform.tag == "CloseButton")
                        continue;
                    GameObject npc = b.GetComponent<ButtonNPC>().GetCurrentNPC();
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(delegate{type.transform.GetComponent<CloningRoom>().SetNPC(npc);});
                }
                break;
                
                
            case BuildingType.EBuildingType.DRONES:                
                Button[] dronesButton = droneSelectionMenu.GetComponentsInChildren<Button>();
                List<Drone> drones = type.transform.GetComponent<DronesRoom>().GetDrones();
                for(int i = 0; i<dronesButton.Length; i++)
                {                    
                    Debug.Log(i);
                    Debug.Log(drones[i]);
                    Debug.Log(dronesButton[i]);
                    Drone current = drones[i];
                    dronesButton[i].onClick.RemoveAllListeners();
                    dronesButton[i].onClick.AddListener(delegate{type.transform.GetComponent<DronesRoom>().SetWorkingDrone(current);});
                }
                break; 
            case BuildingType.EBuildingType.PIPE:
                rotateLeft.gameObject.SetActive(true);
                rotateRight.gameObject.SetActive(true);
                buildPipe.gameObject.SetActive(true);
                var pipe = building.GetComponent<PipeRoom>();
                if(pipe!=null)
                {
                    rotateLeft.onClick.RemoveAllListeners();
                    rotateRight.onClick.RemoveAllListeners();
                    rotateLeft.onClick.AddListener(delegate {pipe.RotatePipe();});
                    rotateRight.onClick.AddListener(delegate{pipe.RotatePipe(true);});
                    buildPipe.onClick.AddListener(delegate{clickHandler.nodeManager.BuildPipe(building);});
                }
                break;
            case BuildingType.EBuildingType.RESOURCES:
                break;
        }
    }
    //Recibe un gameObject y lo desactiva (Se usa en la UI)
    public void HideGameObject(GameObject objecToHide)
    {
        objecToHide.SetActive(false);
    }
    public void ShowGameObject(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
    }

    //Recibe el siguiente estado y si es diferente del actual lo actualiza. Aparte ejecuta las acciones pertinentes del estado.
    public void ChangeState(GetEnumVisualState nextState)
    {
        if(currentState != nextState.state)
        {
            switch(nextState.state)
            {
                case VisualState.BUILDING:
                    DisplayBuildingMenu();
                    break;
                case VisualState.ON_ROOM:
                    StopCameraMovement(false);
                    if(currentRoom.GetComponent<CloningRoom>())
                    {
                        currentRoom.GetComponent<CloningRoom>().DisplayProgessBar(true);
                        ShowGameObject(clonningMenuButton.gameObject);
                    }else if(currentRoom.GetComponent<DronesRoom>())
                    {
                        ShowGameObject(sendDroneButton.gameObject);
                    }
                    break;
                case VisualState.MOVING_AROUND:
                    break;        
            }
        
            switch(currentState)
            {
                case VisualState.BUILDING:
                    HideBuildingMenu();
                    break;
                case VisualState.ON_ROOM:
                    if(currentRoom.GetComponent<CloningRoom>())
                    {
                        currentRoom.GetComponent<CloningRoom>().DisplayProgessBar(false);
                        HideGameObject(clonningMenuButton.gameObject);
                    }else if(currentRoom.GetComponent<DronesRoom>())
                    {
                        HideGameObject(sendDroneButton.gameObject);
                    }
                    break;
                case VisualState.MOVING_AROUND:
                    break; 
            }
        }
        currentState = nextState.state;
    }

    public void SetCurrentRoom(GameObject room){currentRoom = room; }
    public void UpdateResources(ResourcesRoom.EResource type, int amount)
    {       
        switch(type)
        {
            case ResourcesRoom.EResource.OXIGEN:
                oxigen.text = amount.ToString();
                break;
            case ResourcesRoom.EResource.MONEY:
                money.text= amount.ToString();
                break;
            case ResourcesRoom.EResource.FOOD:
                water.text=amount.ToString();
                break;
        }    
    }
    public void UpdateInventory(int rock, int iron, int wood)
    {
        rockText.text = rock.ToString();
        ironText.text = iron.ToString();
        woodText.text = wood.ToString();
    }
    public void StopCameraMovement(bool stop){cameraMovement.StopCameraMovement(stop);}
}
