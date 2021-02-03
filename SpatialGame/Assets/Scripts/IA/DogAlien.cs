using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class DogAlien : AnimationManager
{
   
    [SerializeField]
    float walkingSpeed;
    Transform target;
    [SerializeField]
    float maxHealth;
    float currentHealth;
    public enum EState
	{
        SEARCHING,ATTACKING,DYING
	}
    public EState currentState;

    // Update is called once per frame
    void Update()
    {
		switch(currentState)
		{
            case EState.SEARCHING:
                base.myAgent.SetDestination(target.position);
                if(Vector3.Distance(transform.position, target.position)<=3.5f)
                    ChangeState(EState.ATTACKING);
                break;
            case EState.ATTACKING:
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

	public override void TurnAroundCharacter()
	{
		base.TurnAroundCharacter();
	}
	#region Setters and Getters

    public Transform Target{get;set;}

	#endregion
}
