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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            AttackShip();
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
