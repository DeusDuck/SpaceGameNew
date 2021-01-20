using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasis : MonoBehaviour
{
    [SerializeField]
    float health;
    [SerializeField]
    float armor;

    public float GetHealth(){return health;}
    public float GetArmor(){return armor;}
}
