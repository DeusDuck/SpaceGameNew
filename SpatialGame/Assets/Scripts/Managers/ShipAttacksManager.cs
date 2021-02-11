using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipAttacksManager : MonoBehaviour
{
    [SerializeField]
    NPCManager npcManager;
    [SerializeField]
    GameObject dogBasic;
    [SerializeField]
    float randomPositionRadius;
    [SerializeField]
    int numberOfenemies;
    [SerializeField]
    float assaultTime;
    float currentTime;

    // Update is called once per frame
    void Update()
    {
        //Esto esta hecho a modo de Debug 
        if(assaultTime == 0)
            return;
        currentTime+=Time.deltaTime;
		if(currentTime>=assaultTime)
		{
            AttackShip();
            currentTime = 0;
		}           
    }
    void AttackShip()
	{   
        for(int i = 1; i<=numberOfenemies; i++)
		{
            DogAlien dog = Instantiate(dogBasic,RandomNavmeshLocation(),dogBasic.transform.rotation).GetComponent<DogAlien>();
            NPC current = npcManager.GetNPC();
            dog.AttackingPosition = current.GetAttackingPosition();
            dog.Target = current;
            current.SetAttackingPositions();
		}        
	}
    Vector3 RandomNavmeshLocation() 
    {
         Vector3 randomDirection = Random.insideUnitSphere * randomPositionRadius;
         randomDirection += transform.position;
         NavMeshHit hit;
         Vector3 finalPosition = Vector3.zero;
         if (NavMesh.SamplePosition(randomDirection, out hit, randomPositionRadius, 1))
         {
             finalPosition = hit.position;            
         }
         return finalPosition;
    }
}
