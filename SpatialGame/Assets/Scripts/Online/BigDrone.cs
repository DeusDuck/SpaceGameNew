using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigDrone : MonoBehaviour,IControlable
{
    float currentHealth;
    public float maxHealth;
    [SerializeField]
    Image healthBar;
    public float damage;
    public float armor;
    public float velocity;
    public float criticalProb;
    [SerializeField]
    List<Weapon> myWeapons;
    [SerializeField]
    Chasis myChasis;
	public void TakeDamage(float _damage)
	{
		currentHealth-=_damage;
        healthBar.fillAmount = currentHealth/maxHealth;
	}

	// Start is called before the first frame update
	void Start()
    {
        SetUpStats();
    }

    void SetUpStats()
	{
        foreach(Weapon weapon in myWeapons)
		{
            damage+=weapon.GetDamage();
            velocity+=weapon.GetSpeed();
            criticalProb+=weapon.GetCriticProb();            
		}
        armor+=myChasis.GetArmor();
        maxHealth+=myChasis.GetHealth();
	}
    public void AddWeapon(Weapon weapon)
	{
        myWeapons.Add(weapon);
	}
    public void SetChasis(Chasis nextChasis){myChasis = nextChasis;}
}
