using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigDrone : MonoBehaviour,IControlable
{
    float currentHealth;    
    [SerializeField]
    Image healthBar;
    [SerializeField]
    Image arrowImage;
    [Header("Drone Stats")]
    [Space(5)]
    public float maxHealth;
    public float damage;
    public float armor;
    public float velocity;
    public float criticalProb;
    public float avoidanceProb;
    public float resistance;
    [Space(5)]

    [Header ("Weapons")]
    [Space(5)]
    [SerializeField]
    List<Weapon> myWeapons;
    [SerializeField]
    Chasis myChasis;
    [Space(5)]

    public Transform target;
    public PhotonView PV;
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
		if(myChasis!=null)
		{
            armor+=myChasis.GetArmor();
            maxHealth+=myChasis.GetHealth();
		}
        
        currentHealth = maxHealth;
	}
    public void AddWeapon(Weapon weapon)
	{
        myWeapons.Add(weapon);
	}
    public void SetChasis(Chasis nextChasis)
    {
        myChasis = nextChasis;
    
    }
    public void Attack()
	{
        target.GetComponent<BigDrone>().TakeDamage(damage);
	}
    public void ShowArrow(bool must)
	{
        arrowImage.gameObject.SetActive(must);
	}
}
