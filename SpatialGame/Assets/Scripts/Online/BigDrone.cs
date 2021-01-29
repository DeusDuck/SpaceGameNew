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
    public float movingSpeed;
    public float distanceToStop;
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
        FaceEnemies();
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
            myPlayerController.SpendEnergy(_energyCost);
            gamePlayManager.GetUIManager().HidePanel();
            gamePlayManager.SetAttack(true);
            gamePlayManager.ActivateEnemyArrows(true);
		}            
	}
    public void SetPlayerController(PlayerOnlineController player, GamePlayManager manager){myPlayerController = player; gamePlayManager = manager;}
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
    void FaceEnemies()
	{
        if(gamePlayManager.transform.position.x>transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
	}
}
