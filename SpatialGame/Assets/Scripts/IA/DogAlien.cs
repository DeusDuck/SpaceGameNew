using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class DogAlien : NPCController
{
   
    [SerializeField]
    float walkingSpeed;
    [SerializeField]
    Transform attackingPosition;
    NPC target;
    [SerializeField]
    float attackingdistance;
    [SerializeField]
    AnimationReferenceAsset attack;
    [SerializeField]
    float damage;
    [SerializeField]
    float timeToDamage;
    float currentTime;
    
    public enum EState
	{
        SEARCHING,ATTACKING,DYING,WAIT
	}
    public EState currentState;
	private void Start()
	{
        base.myAgent.speed = walkingSpeed;
        base.myAgent.updateRotation = false;
        currentTime = timeToDamage;
		ChangeState(EState.SEARCHING);
	}

	// Update is called once per frame
	void Update()
    {
		switch(currentState)
		{
            case EState.SEARCHING:
                if(attackingPosition != null)
				{
                    base.myAgent.SetDestination(attackingPosition.position);
                    TurnAroundCharacter();
                    if(Vector3.Distance(transform.position, attackingPosition.position)<=attackingdistance)
                        ChangeState(EState.ATTACKING);
				}               
                else
                    ChangeState(EState.WAIT);
                break;
            case EState.ATTACKING:
				if(target!=null)
				{
                    currentTime-=Time.deltaTime;
				    if(currentTime<=0)
				    {
                        currentTime=timeToDamage;
                        target.TakeDamage(damage);                    
				    }
                    if(Vector3.Distance(transform.position, attackingPosition.position)>attackingdistance)
                        ChangeState(EState.SEARCHING);
				}
                else
                    ChangeState(EState.WAIT);
                
                break;
            case EState.WAIT:

                break;
            case EState.DYING:
                break;
		}
    }
    void ChangeState(EState nextState)
	{
		switch(currentState)
		{
            case EState.SEARCHING:
                myAgent.isStopped = true;
                break;
            case EState.ATTACKING:
                break;
            case EState.WAIT:
                break;
            case EState.DYING:
                break;
		}
        switch(nextState)
		{
            case EState.SEARCHING:
                myAgent.isStopped = false;
                SetAnimationAsset(base.walk,true,1);
                break;
            case EState.ATTACKING:
                SetAnimationAsset(attack,true,1);
				if(target.transform.position.x>=transform.position.x)
                    transform.localScale = new Vector3(-0.08f,0.08f,0);
				else
                    transform.localScale = new Vector3(0.08f,0.08f,0);
                break;
            case EState.WAIT:
                SetAnimationAsset(base.idle,true,1);
                break;
            case EState.DYING:
                break;
		}
        currentState = nextState;
	}
	public override void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
	{
		base.SetAnimationAsset(anim, loop, timeScale);
	}

	public override void TurnAroundCharacter(float scale = 0.08f)
	{
		if(myAgent.velocity.x>=0)
            transform.localScale = new Vector3(-scale,scale,1);
		else
            transform.localScale = new Vector3(scale,scale,1);
	}
	#region Setters and Getters

    public Transform AttackingPosition{get{ return attackingPosition;}set {attackingPosition = value;}}
    public NPC Target{set{target = value;}}

	#endregion
}
