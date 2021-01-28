using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
	[SerializeField]
	float damage;
	[SerializeField]
	float attackSpeed;
	[SerializeField]
	float criticalProb;
	[SerializeField]
	float armorPenetration;

	public float GetDamage(){return damage;}
}
