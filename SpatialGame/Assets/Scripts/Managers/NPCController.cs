using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
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
    [SerializeField]
    protected float maxHealth;
    protected float currentHealth;

	private void Start()
	{
		currentHealth = maxHealth;
	}
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
    public virtual void TakeDamage(float damage)
	{
        currentHealth-=damage;
	}
}
