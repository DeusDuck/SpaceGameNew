using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class DogAlien : AnimationManager
{
   
    [SerializeField]
    float walkingSpeed;
    [SerializeField]
    Transform target;
    [SerializeField]
    float maxHealth;
    float currentHealth;
    [SerializeField]
    float attackingdistance;
    
    public enum EState
	{
        SEARCHING,ATTACKING,DYING
	}
    public EState currentState;
	private void Start()
	{
        base.myAgent.speed = walkingSpeed;
        base.myAgent.updateRotation = false;
		ChangeState(EState.SEARCHING);
	}

	// Update is called once per frame
	void Update()
    {
		switch(currentState)
		{
            case EState.SEARCHING:
                base.myAgent.SetDestination(target.position);
                TurnAroundCharacter();
                if(Vector3.Distance(transform.position, target.position)<=attackingdistance)
                    ChangeState(EState.ATTACKING);
                break;
            case EState.ATTACKING:
                if(Vector3.Distance(transform.position, target.position)>=attackingdistance)
                    ChangeState(EState.SEARCHING);
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
                break;
            case EState.ATTACKING:
                break;
            case EState.DYING:
                break;
		}
        switch(nextState)
		{
            case EState.SEARCHING:
                SetAnimationAsset(base.walk,true,1);
                break;
            case EState.ATTACKING:
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
		{
            Debug.Log("Buenas");
            transform.localScale = new Vector3(-scale,scale,1);
		}
		else
		{
            Debug.Log("Deuu");
            transform.localScale = new Vector3(scale,scale,1);
		}
            

	}
	#region Setters and Getters

    public Transform Target{get{ return target;}set {target = value;}}

	#endregion
}
