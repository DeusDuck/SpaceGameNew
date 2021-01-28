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
    [SerializeField]
    Bullets myBullet;
    [Space(5)]

    public BigDrone target;
    public PhotonView PV;
    PlayerOnlineController myPlayerController;
    GamePlayManager gamePlayManager;
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
        if(target!=null)
            target.TakeDamage(damage);
	}
    public void ShowArrow(bool must)
	{
        arrowImage.gameObject.SetActive(must);
	}
    public void SetDamage(float newDamage)
	{
        damage = newDamage;
	}
    public void SetEnergyCost(int _energyCost)
	{
		if(_energyCost<=myPlayerController.GetCurrentEnergy())
		{
            Debug.Log(_energyCost);
            myPlayerController.SpendEnergy(_energyCost);
            gamePlayManager.GetUIManager().HidePanel();
            gamePlayManager.attacking = true;
		}            
	}
    public void SetPlayerController(PlayerOnlineController player, GamePlayManager manager){myPlayerController = player; gamePlayManager = manager;}
    public PlayerOnlineController Getplayer(){return myPlayerController;}
    public void SetBullet(Bullets currentBullet)
    {
        myBullet = currentBullet;
        damage = myBullet.GetDamage();
    }
}
