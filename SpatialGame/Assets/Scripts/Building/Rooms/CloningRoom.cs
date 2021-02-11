using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloningRoom : BuildingType
{
    GameObject NPC;
    float currentTimeToSpawn;
    [SerializeField]
    float timeToSpawn;
    [SerializeField]
    Transform spawnPos;
    bool hasNPCToSpawn;
    [SerializeField]
    Image progressBarBack;
    [SerializeField]
    Image progressBar;
    [SerializeField]
    Canvas canvas;
    NPCManager npcManager;

    // Update is called once per frame
    void Update()
    {
        if(hasNPCToSpawn)
        {
            currentTimeToSpawn += Time.deltaTime;
            progressBar.fillAmount = currentTimeToSpawn/timeToSpawn;            
        }
        if(currentTimeToSpawn>=timeToSpawn)
        {
            NPC currentNPC = Instantiate(NPC,spawnPos.position,NPC.gameObject.transform.rotation).GetComponent<NPC>();
            currentNPC.SetManager(npcManager);
            currentTimeToSpawn = 0.0f;
            progressBarBack.gameObject.SetActive(false);
            hasNPCToSpawn = false;
            npcManager.AddNPC(currentNPC);
        }
    }
# region Setters and Getters
	public void SetNPC(GameObject currentNPC)
    {
        NPC = currentNPC;
        hasNPCToSpawn = true;
        progressBarBack.gameObject.SetActive(true);
    }
    public void DisplayProgessBar(bool show)
    {
        canvas.gameObject.SetActive(show);
    }
    public void SetNPCManager(NPCManager currentManager){npcManager = currentManager; }

	#endregion
}
