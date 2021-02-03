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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            AttackShip();
    }
    void AttackShip()
	{
        Debug.Log("olaaa");
        DogAlien dog = Instantiate(dogBasic,RandomNavmeshLocation(),dogBasic.transform.rotation).GetComponent<DogAlien>();
        dog.Target = npcManager.GetNPC().transform;
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
