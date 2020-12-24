using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    List<NPC> currentNPC = new List<NPC>();
    List<ResourcesRoom> oxigenBuildings = new List<ResourcesRoom>();
    List<ResourcesRoom> foodBuildings = new List<ResourcesRoom>();
    List<ResourcesRoom> moneyBuildings = new List<ResourcesRoom>();

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
    public void AddNPC(NPC npc)
    {
        currentNPC.Add(npc);
        AssignJob(npc);
    }
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
    void GiveRandomNPCTarget()
    {
        foreach(NPC npc in currentNPC)
        {
            if(npc.currentState == NPC.EState.RANDOM_POS)
                AssignJob(npc);
        }
    }
    public void NPCInWorkingPosition(NPC current)
    {
        current.GetWorkingRoom().StartCounter(true);
        current.GetWorkingRoom().AddWorker();
    }
}
