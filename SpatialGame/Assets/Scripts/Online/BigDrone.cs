using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigDrone : MonoBehaviour,IControlable
{
    [SerializeField]
    float currentHealth;    
    [Header("Drone Stats")]
    [Space(5)]
    public float maxHealth;
    public float damage;
    public float armor;
    public float velocity;
    public float criticalProb;
    public float avoidanceProb;
    public float resistance;
    public float movingSpeed;
    public float distanceToStop;
    public int energyDefense;
    public int energySpecialAttack;
    public int energySpecialHability;
    int attack;
    int specialHability;
    int defense;
    int special;
    [SerializeField]
    float currentDamage;
    [Space(5)]

    [Header ("Weapons")]
    [Space(5)]
    [SerializeField]
    List<Weapon> myWeapons;
    [SerializeField]
    Chasis myChasis;
    [SerializeField]
    Bullets myBullet;
    [Space(5)]

    public BigDrone target;
    public PhotonView PV;
    PlayerOnlineController myPlayerController;
    [SerializeField]
    GameObject particle;
    [SerializeField]
    Animator animatorController;

	

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
        currentDamage = damage;
	}
    public void TakeDamage(float _damage)
	{
		currentHealth-=_damage;
        if(currentHealth<=0)
            Die();
    }
    void Die()
	{
        GamePlayManager.instance.IDied(myPlayerController);
        Destroy(this.gameObject);
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
        SetDamage();
        target.TakeDamage(currentDamage);
	}    
    public void SetDamage()
	{
        currentDamage = damage * attack;
	}
    public void ResetDamage()
	{
        currentDamage = damage;
	}
    public void SetPlayerController(PlayerOnlineController player){myPlayerController = player;}
    public PlayerOnlineController GetPlayerController(){return myPlayerController;} 
    public void AddAttack()
	{
		if(myPlayerController.GetCurrentEnergy()>0)
		{
            myPlayerController.SpendEnergy();
            attack++;
            GamePlayManager.instance.GetUIManager().UpdateAmounts();
		}        
	}
    public void AddDefense()
	{
        if(myPlayerController.GetCurrentEnergy()-energyDefense>=0)
		{
            myPlayerController.SpendEnergy(energyDefense);
            defense++;
            GamePlayManager.instance.GetUIManager().UpdateAmounts();
        }
	}
    public void AddSpecial()
	{
        if(myPlayerController.GetCurrentEnergy()-energySpecialAttack>=0)
		{
            myPlayerController.SpendEnergy(energySpecialAttack);
            special++;
            GamePlayManager.instance.GetUIManager().UpdateAmounts();
        }
	}
    public void AddSpecialHability()
	{
        if(myPlayerController.GetCurrentEnergy()-energySpecialHability>=0)
		{
            myPlayerController.SpendEnergy(energySpecialHability);
            specialHability++;
            GamePlayManager.instance.GetUIManager().UpdateAmounts();
        }
	}
    public void ResetParameters()
	{
        attack = 0;
        defense = 0;
        special = 0;
        specialHability = 0;
        GamePlayManager.instance.GetUIManager().UpdateAmounts();
	}
    public int GetAttack(){return attack;}
    public int GetDefense(){return defense;}
    public int GetSpecial(){return special;}
    public int GetSpecialAttack(){return specialHability;}
    public float GetCurrentHealth(){return currentHealth;}
    public float GetMaxHealth(){return maxHealth;}
    public float GetCurrentDamage(){return currentDamage;}
}
