using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    float damage;
    [SerializeField]
    float speed;
    [SerializeField]
    float criticProb;
    public float GetDamage(){return damage;}
    public float GetSpeed(){return speed;}
    public float GetCriticProb(){return criticProb;}
}
