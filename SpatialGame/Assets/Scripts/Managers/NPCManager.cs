using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    List<NPC> currentNPC;
    [SerializeField]
    List<ResourcesRoom> currentBuildings;

	void AssignJob(NPC npc)
    {
        if(currentBuildings.Count == 0)
            npc.SetDestination(null);
        foreach(ResourcesRoom building in currentBuildings)
        {
            if(building.IsBuildingFull())
                continue;
            npc.SetDestination(building.GetWorkingPosition());
            npc.SetBuilding(building);
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
        currentBuildings.Add(building);
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
