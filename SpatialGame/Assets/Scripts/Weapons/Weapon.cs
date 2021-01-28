using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float criticProb;
    public virtual float GetDamage(){return damage;}
    public virtual float GetSpeed(){return speed;}
    public virtual float GetCriticProb(){return criticProb;}
}
