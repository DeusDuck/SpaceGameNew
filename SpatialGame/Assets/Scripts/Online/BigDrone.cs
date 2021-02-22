﻿using Photon.Pun;
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
    public float movingSpeed;
    public float distanceToStop;
    int attack;
    int defense;
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
    [SerializeField]
    Transform currentPosition;
    [SerializeField]
    SpriteRenderer mySprite;
    bool move;
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
	private void Update()
	{
		if(move)
		{
            if(Vector3.Distance(transform.position,target.GetPosition().position)>distanceToStop && Vector3.Distance(target.GetPosition().position,transform.position)>distanceToStop)
                MoveToTargetPosition();
			else
			{
                Transform targetPos = target.GetPosition();
                target.SetPosition(currentPosition);
                currentPosition = targetPos;
                move = false;
			}
                
		}
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
        target.TakeDamage(damage * attack);
	}
    public void ShowArrow(bool must)
	{
        arrowImage.gameObject.SetActive(must);
	}
    public void SetDamage(float newDamage)
	{
        damage = newDamage;
	}
    public void SpendEnergy()
	{
		if(myPlayerController.GetCurrentEnergy()>0)
		{
            myPlayerController.SpendEnergy();
            gamePlayManager.SetAttack(true);
		}            
	}
    public void SetPlayerController(PlayerOnlineController player){myPlayerController = player;}
    public void SetGamePlayManager(GamePlayManager manager){ gamePlayManager = manager;}
    public PlayerOnlineController Getplayer(){return myPlayerController;}
    public void SetBullet(Bullets currentBullet)
    {
        myBullet = currentBullet;
        damage = myBullet.GetDamage();
    }
    public void SetPosition(Transform next){currentPosition = next;}
    public Transform GetPosition(){return currentPosition;}
    public void MoveToTargetPosition()
	{
        move = true;
        Transform nextPos = target.GetPosition();		            
        target.MoveToPosition(currentPosition);
        transform.position = Vector3.Lerp(transform.position, nextPos.position, movingSpeed*Time.deltaTime);		        
	}
    public void MoveToPosition(Transform next)
	{
        transform.position = Vector3.Lerp(transform.position, next.position, movingSpeed*Time.deltaTime);
	}
    public void FaceEnemies()
	{
        if(gamePlayManager.transform.position.x>transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
	}
    public SpriteRenderer GetMySprite()
	{
        return mySprite;
	}
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
        if(myPlayerController.GetCurrentEnergy()>0)
		{
            myPlayerController.SpendEnergy();
            defense++;
            GamePlayManager.instance.GetUIManager().UpdateAmounts();
        }
	}
    public void ResetParameters()
	{
        attack = 0;
        defense = 0;
        GamePlayManager.instance.GetUIManager().UpdateAmounts();
	}
    public int GetAttack(){return attack;}
    public int GetDefense(){return defense;}
}
