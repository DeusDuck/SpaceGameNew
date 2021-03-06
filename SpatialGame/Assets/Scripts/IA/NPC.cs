﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class NPC : MonoBehaviour
{
    public float walkingSpeed;
    public float randomPositionRadius;
    public float timeToChat;
    public float timeToWait;
    public NavMeshAgent myAgent;
    
    public enum EState
    { 
        CHATTING, MOVING, WORKING, RANDOM_POS
    }
    public EState currentState;

    Vector3 currentTarget;
    float currentTime;
    [SerializeField]
    SkeletonAnimation mySkeleton;
    [SerializeField]
    AnimationReferenceAsset idle;
    [SerializeField]
    AnimationReferenceAsset walk;
    string currentAnimationName;

    // Start is called before the first frame update
    void Start()
    {
        myAgent.updateRotation = false;
        myAgent.speed = walkingSpeed; 
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
                break;
            case EState.RANDOM_POS:
                currentTarget = Vector3.zero;
                currentTarget = RandomNavmeshLocation();
                SetAnimationAsset(walk,true,1);
                break;
        
        }
        currentState = nextState;
    }
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
    public void SetDestination(Transform target)
    {
        if(target !=null)
        {
            currentTarget = target.position;
            ChangeState(EState.MOVING);
        }
        else
        {
            ChangeState(EState.RANDOM_POS); 
        }            
    }
    void TurnAroundCharacter()
    {
        if(myAgent.velocity.x<0)
            transform.localScale = new Vector3(-0.08f,0.08f,1);
        else
            transform.localScale = new Vector3(0.08f,0.08f,1);
    }
    void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        if(currentAnimationName == anim.name)
            return;
        mySkeleton.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
        currentAnimationName = anim.name;
    }
}
