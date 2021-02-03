using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    //Se encarga de settear los NPC de la nave
    [SerializeField]
    List<NPC> currentNPC = new List<NPC>();
    List<ResourcesRoom> oxigenBuildings = new List<ResourcesRoom>();
    List<ResourcesRoom> foodBuildings = new List<ResourcesRoom>();
    List<ResourcesRoom> moneyBuildings = new List<ResourcesRoom>();

    //Asigna las posiciones a las que deben ir los npc dependiendo de su tipo
	void AssignJob(NPC npc)
    {
        
            switch(npc.currentType)
            {
                case NPC.EType.FARMER:
                    if(foodBuildings.Count == 0)
                    {
                        npc.SetDestination(null);
                        return;
                    }                    

                    foreach(ResourcesRoom room in foodBuildings)
                    {
                        if(!room.IsBuildingFull())
                        {
                            npc.SetDestination(room.GetWorkingPosition());
                            npc.SetBuilding(room);
                        }else
                            npc.SetDestination(null);
                    }
                    break;
                case NPC.EType.MERCHANT:
                    if(moneyBuildings.Count == 0)
                    {
                        npc.SetDestination(null);
                        return;
                    }                    

                    foreach(ResourcesRoom room in moneyBuildings)
                    {
                        if(!room.IsBuildingFull())
                        {
                            npc.SetDestination(room.GetWorkingPosition());
                            npc.SetBuilding(room);
                        }else
                            npc.SetDestination(null);
                    }
                    break;
                case NPC.EType.SCIENTIST:
                    if(oxigenBuildings.Count == 0)
                    {
                        npc.SetDestination(null);
                        return;
                    }                    

                    foreach(ResourcesRoom room in oxigenBuildings)
                    {
                        if(!room.IsBuildingFull())
                        {
                            npc.SetDestination(room.GetWorkingPosition());
                            npc.SetBuilding(room);
                        }else
                            npc.SetDestination(null);
                    }
                    break;
            }       
    }
    //Añade un NPC a la lista y le asigna un trabajo
    public void AddNPC(NPC npc)
    {
        currentNPC.Add(npc);
        AssignJob(npc);
    }
    //Añade un edificio a su lista dependiendo del tipo
    public void AddBuilding(ResourcesRoom building)
    {       
        switch(building.currentResource)
        {
            case ResourcesRoom.EResource.FOOD:
                foodBuildings.Add(building);
                break;
            case ResourcesRoom.EResource.MONEY:
                moneyBuildings.Add(building);
                break;
            case ResourcesRoom.EResource.OXIGEN:
                oxigenBuildings.Add(building);
                break;
        }
        GiveRandomNPCTarget();      
    }
    //Si hay un NPC sin rumbo, vuelve a mirar si hay alguna habitación disponible
    void GiveRandomNPCTarget()
    {
        foreach(NPC npc in currentNPC)
        {
            if(npc.currentState == NPC.EState.RANDOM_POS)
                AssignJob(npc);
        }
    }
    //Cuando el NPC llega a la posición de trabajo empieza el contador
    public void NPCInWorkingPosition(NPC current)
    {
        current.GetWorkingRoom().StartCounter(true);
        current.GetWorkingRoom().AddWorker();
    }
    public NPC GetNPC()
	{
        return currentNPC[0];
	}
}
