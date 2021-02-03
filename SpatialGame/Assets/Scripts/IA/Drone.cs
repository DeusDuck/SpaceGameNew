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
            //Hace que se quede en su posicion inicial
            case EState.IDLE:
                if(Vector3.Distance(transform.position, currentTarget.position)>=2.0f)
                    myAgent.SetDestination(currentTarget.position);
                break;
            //El dron se va a recolectar durante un tiempo establecido
            case EState.FETCHING:
                myAgent.SetDestination(currentTarget.position);
                TurnAroundCharacter();
                currentTimeTravelling-=Time.deltaTime;
                myRoom.CalculateTime(currentTimeTravelling);
                if(currentTimeTravelling<=0)
                    ChangeState(EState.RECOLLECTING);
                break;
            //El dron va ala posicion de recolectar y cambia su animacion
            case EState.RECOLLECTING:
                myAgent.SetDestination(currentTarget.position);
               
				if(Vector3.Distance(transform.position, currentTarget.position)<=3.5f)
				{
                    //Cuando entra la primera vez hace la animacion de recollectar
                    if(currentTime == 0)
                        SetAnimationAsset(recollecting,false,1);

                    transform.localScale = new Vector3(0.05f,0.05f,1);
                    currentTime+=Time.deltaTime;
                    if(currentTime>=2.0f)
                        ChangeState(EState.IDLE);
				}
                   
                break;
        }
	}
    //Cambia el estado de el dron
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
    //Le da un nuevo target
	public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
    public float GetTimeTravelling(){return timeTravelling;}
    public Transform GetHomePosition(){return homePosition;}

    //Da la vuelta al sprite dependiendo de donde se encuentra  el target
    void TurnAroundCharacter()
    {
        if(myAgent.destination.x<=transform.position.x)
            transform.localScale = new Vector3(-0.05f,0.05f,1);
        else
            transform.localScale = new Vector3(0.05f,0.05f,1);
    }
    //Cambia la animacion
    void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        if(currentAnimName == anim.name)
            return;
        mySkeleton.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
        currentAnimName = anim.name;
    }
}
