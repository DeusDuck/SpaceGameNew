using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class NPC : NPCController
{
    public float walkingSpeed;
    public float randomPositionRadius;
    public float timeToChat;
    public float timeToWait;
    
    public enum EState
    { 
        CHATTING, MOVING, WORKING, RANDOM_POS
    }
    public EState currentState;
    public enum EType 
    {
        FARMER,SCIENTIST,MERCHANT
    }
    public EType currentType;
    Vector3 currentTarget;
    Transform currentWorkingPosition;
    float currentTime;   
    NPCManager myManager;
    ResourcesRoom currentRoom;
    [SerializeField]
    List<Transform> attackingPositions;
    List<Transform> occupiedPosition = new List<Transform>();
    Transform currentAttackingPos;

    // Start is called before the first frame update
    void Start()
    {
        base.myAgent.updateRotation = false;
        base.myAgent.speed = walkingSpeed; 
        currentAttackingPos = attackingPositions[0];
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        { 
            case EState.MOVING:
                UpdateMoving();
                break;
            case EState.CHATTING:
                UpdateChatting();
                break;  
            case EState.WORKING:
                UpdateWorking();
                break; 
            case EState.RANDOM_POS:
                UpdateRandomPos();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "NPC")
            ChangeState(EState.CHATTING);
    }
    void ChangeState(EState nextState)
    {
        switch(currentState)
        { 
            case EState.MOVING:                              
                 break;
            case EState.CHATTING:
                 currentTime = 0.0f;
                 break;
            case EState.WORKING:
                currentTime = 0.0f;
                SetAnimationAsset(idle,true,1);
                break; 
            case EState.RANDOM_POS:
                
                break;
        }
        switch(nextState)
        { 
            case EState.MOVING: 
                SetAnimationAsset(walk,true,1);
                break;
            case EState.CHATTING:
                
                break;
            case EState.WORKING:
                SetAnimationAsset(idle,true,1);
                myManager.NPCInWorkingPosition(this);
                break;
            case EState.RANDOM_POS:
                currentTarget = Vector3.zero;
                currentTarget = RandomNavmeshLocation();
                SetAnimationAsset(walk,true,1);
                break;
        
        }
        currentState = nextState;
    }
	#region Update Current State
	void UpdateMoving()
    {
        myAgent.destination = currentTarget;
        TurnAroundCharacter();
        if(Vector3.Distance(currentTarget,transform.position) <= myAgent.stoppingDistance)
            ChangeState(EState.WORKING);        
    }
    void UpdateChatting()
    {
        currentTime+=Time.deltaTime;
        if(currentTime>=timeToChat)
            ChangeState(EState.MOVING);
    }

    void UpdateWorking()
    {
        /**/
    }
    void UpdateRandomPos()
    {
        if(Vector3.Distance(currentTarget,transform.position) <= myAgent.stoppingDistance)
        {
            currentTime+=Time.deltaTime;
            SetAnimationAsset(idle,true,1);
            if(currentTime>=timeToWait)
            {
                currentTarget = RandomNavmeshLocation();
                currentTime = 0.0f;
                SetAnimationAsset(walk,true,1);
            }   
        }
        myAgent.destination = currentTarget;
        TurnAroundCharacter();
    }
	#endregion
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
    
    public  override void TurnAroundCharacter(float scale = 0.08f)
    {
        base.TurnAroundCharacter(scale);
    }
    public override void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        base.SetAnimationAsset(anim,loop,timeScale);
    }
	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
        if(base.currentHealth<=0)
            Die();
	}
    void Die()
	{
        currentRoom.FreeWorkingPosition(currentWorkingPosition);
        currentRoom.RemoveWorker();
        myManager.RemoveNPC(this);
        Destroy(gameObject);
	}
	#region Setter and Getters
	public void SetManager(NPCManager manager){myManager = manager;}
    public void SetBuilding(ResourcesRoom room){ currentRoom = room;}
    public ResourcesRoom GetWorkingRoom(){return currentRoom;}
    public Transform GetAttackingPosition()
	{
        return currentAttackingPos;
	}
    public void SetAttackingPositions()
	{
        attackingPositions.Remove(currentAttackingPos);
        occupiedPosition.Add(currentAttackingPos);

        if(attackingPositions.Count==0)
            currentAttackingPos = null;

        foreach(Transform t in attackingPositions)
		{
            if(currentAttackingPos==t)
                continue;
            currentAttackingPos = t;
		}
	}
    public void SetDestination(Transform target)
    {
        if(target !=null)
        {
            currentTarget = target.position;
            currentWorkingPosition = target;
            ChangeState(EState.MOVING);
        }
        else
        {
            ChangeState(EState.RANDOM_POS); 
        }            
    }   
	#endregion
}
