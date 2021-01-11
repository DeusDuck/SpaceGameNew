using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent myAgent;
    Transform currentTarget;
    [SerializeField]
    Transform homePosition;
    [SerializeField]
    DronesRoom myRoom;
    public enum EState
    {
        IDLE,RECOLLECTING,FETCHING
    }
    public EState currentState;
    public float timeTravelling;
    float currentTimeTravelling;
	private void Start()
	{
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
                currentTimeTravelling-=Time.deltaTime;
                myRoom.CalculateTime(currentTimeTravelling);
                if(currentTimeTravelling<=0)
                    ChangeState(EState.RECOLLECTING);
                break;
            case EState.RECOLLECTING:
                myAgent.SetDestination(currentTarget.position);
                if(Vector3.Distance(transform.position,currentTarget.position)<=5f)
                    ChangeState(EState.IDLE);
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
                break;
            case EState.FETCHING:
                currentTimeTravelling = timeTravelling;
                break;
            case EState.RECOLLECTING:
                currentTarget = myRoom.GetRecollectingPosition();
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
}
