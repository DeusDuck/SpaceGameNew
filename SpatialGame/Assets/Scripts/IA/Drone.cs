using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class Drone : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent myAgent;
    Transform currentTarget;
    [SerializeField]
    Transform homePosition;
    [SerializeField]
    DronesRoom myRoom;
    [SerializeField]
    SkeletonAnimation mySkeleton;
    [SerializeField]
    AnimationReferenceAsset idle;
    [SerializeField]
    AnimationReferenceAsset recollecting;
    string currentAnimName;
    float currentTime = 0.0f;
    public enum EState
    {
        IDLE,RECOLLECTING,FETCHING
    }
    public EState currentState;
    public float timeTravelling;
    float currentTimeTravelling;
	private void Start()
	{
        myAgent.updateRotation = false;
        currentTarget = homePosition;
	}
	private void Update()
	{		
        switch(currentState)
        {
            case EState.IDLE:
                myAgent.SetDestination(currentTarget.position);
                break;
            case EState.FETCHING:
                myAgent.SetDestination(currentTarget.position);
                TurnAroundCharacter();
                currentTimeTravelling-=Time.deltaTime;
                myRoom.CalculateTime(currentTimeTravelling);
                if(currentTimeTravelling<=0)
                    ChangeState(EState.RECOLLECTING);
                break;
            case EState.RECOLLECTING:
                myAgent.SetDestination(currentTarget.position);
				if(Vector3.Distance(transform.position, currentTarget.position)<=4.0f)
				{
                    if(currentTime == 0)
                        SetAnimationAsset(recollecting,false,1);
                    currentTime+=Time.deltaTime;
                    if(currentTime>=2.0f)
                        ChangeState(EState.IDLE);
				}
                   
                break;
        }
	}
    public void ChangeState(EState nextState)
    {
        switch(nextState)
        {
            case EState.IDLE:
                currentTarget = homePosition;
                myRoom.DroneReturned(this);
                SetAnimationAsset(idle,true,1);
                break;
            case EState.FETCHING:
                currentTimeTravelling = timeTravelling;
                break;
            case EState.RECOLLECTING:               
                currentTarget = myRoom.GetRecollectingPosition();
                TurnAroundCharacter();
                break;
        }
        switch(currentState)
        {
            case EState.IDLE:
               
                break;
            case EState.FETCHING:
                break;
            case EState.RECOLLECTING:
                break;
        }
        currentState = nextState;
    }
	public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
    public float GetTimeTravelling(){return timeTravelling;}
    public Transform GetHomePosition(){return homePosition;}

    void TurnAroundCharacter()
    {
        if(myAgent.destination.x<=transform.position.x)
            transform.localScale = new Vector3(-0.05f,0.05f,1);
        else
            transform.localScale = new Vector3(0.05f,0.05f,1);
    }
    void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        if(currentAnimName == anim.name)
            return;
        mySkeleton.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
        currentAnimName = anim.name;
    }
}
