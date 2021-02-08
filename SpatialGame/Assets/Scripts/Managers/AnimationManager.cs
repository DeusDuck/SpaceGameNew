using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.AI;

public class AnimationManager : MonoBehaviour
{
    string currentAnimationName;
    [SerializeField]
    protected SkeletonAnimation mySkeleton;
    [SerializeField]
    protected AnimationReferenceAsset idle;
    [SerializeField]
    protected AnimationReferenceAsset walk;
    [SerializeField]
    protected NavMeshAgent myAgent;

    public virtual void TurnAroundCharacter(float scale)
    {
        if(myAgent.velocity.x<=0)
            transform.localScale = new Vector3(-scale,scale,1);
        else
            transform.localScale = new Vector3(scale,scale,1);
    }
    public virtual void SetAnimationAsset(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        if(currentAnimationName == anim.name)
            return;

        mySkeleton.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
        currentAnimationName = anim.name;
    }
}
