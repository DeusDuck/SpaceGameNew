using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOnlineManager : MonoBehaviour
{
    [SerializeField]
    Text currentPlayerTurnText;
    [SerializeField]
    GamePlayManager gamePlayManager;    
    [SerializeField]
    Button attackButton;
    [SerializeField]
    Button defenseButton;
    [SerializeField]
    Button specialButton;
    [SerializeField]
    Button specialAttackButton;
    [SerializeField]
    Text attackText;
    [SerializeField]
    Text defenseText;
    [SerializeField]
    Text specialText;
    [SerializeField]
    Text specialHabilityText;
    [SerializeField]
    Image healthBarLocal;
    [SerializeField]
    Image healthBarOther;
    [SerializeField]
    Image victoryImage;
    [SerializeField]
    Image loseImage;
    [SerializeField]
    Text currentEnergyText;
    [SerializeField]
    Text maxEnergyText;
    
    public void SetButtons()
	{
        BigDrone drone = GamePlayManager.instance.GetLocalPlayer().GetMySoldiers()[0];

        attackButton.onClick.AddListener(delegate{drone.AddAttack();});
        defenseButton.onClick.AddListener(delegate{drone.AddDefense();});
        specialButton.onClick.AddListener(delegate{drone.AddSpecial();});
        specialAttackButton.onClick.AddListener(delegate{drone.AddSpecialHability();});
	}
    public void UpdateAmounts()
	{
        BigDrone drone = GamePlayManager.instance.GetLocalPlayer().GetMySoldiers()[0];
        attackText.text = drone.GetAttack().ToString();
        defenseText.text = drone.GetDefense().ToString();
        specialText.text = drone.GetSpecial().ToString();
        currentEnergyText.text = drone.GetPlayerController().GetCurrentEnergy().ToString();
        maxEnergyText.text = drone.GetPlayerController().GetMaxEnergy().ToString();
        specialHabilityText.text = drone.GetSpecialAttack().ToString();
	}
    public void SetInteractable(bool must)
	{
        attackButton.interactable = must;
        defenseButton.interactable = must;
        specialButton.interactable = must;
        specialAttackButton.interactable = must;
	}    
    public void UpdateMyHealthBar(float localHealth, float maxHealth)
	{
        healthBarLocal.fillAmount = localHealth/maxHealth;
	}
    public void UpdateEnemyHealth(float enemyHealth, float maxHealthOther)
	{
        healthBarOther.fillAmount = enemyHealth/maxHealthOther;
	}
    public void ActivateVictoryImage()
	{
        victoryImage.gameObject.SetActive(true);
	}
    public void ActivateLoseImage()
	{
        loseImage.gameObject.SetActive(true);
	}
}
