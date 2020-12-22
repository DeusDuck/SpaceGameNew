using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField]
    List<NPC> currentNPC;
    [SerializeField]
    List<BuildingType> currentBuildings;

	void AssignJob(NPC npc)
    {
        if(currentBuildings.Count == 0)
            npc.SetDestination(null);
        foreach(BuildingType building in currentBuildings)
        {
            if(building.IsBuildingFull())
                continue;
            npc.SetDestination(building.GetWorkingPosition());
            break;
        }
    }
    public void AddNPC(NPC npc)
    {
        currentNPC.Add(npc);
        AssignJob(npc);
    }
    public void AddBuilding(BuildingType building)
    {
        if(building.GetBuildingType() == BuildingType.EBuildingType.DINNER)
        {
            currentBuildings.Add(building);
            GiveRandomNPCTarget();
        }
    }
    void GiveRandomNPCTarget()
    {
        foreach(NPC npc in currentNPC)
        {
            if(npc.currentState == NPC.EState.RANDOM_POS)
                AssignJob(npc);
        }
    }
}
