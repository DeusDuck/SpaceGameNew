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
    BigDrone currentDrone;
    public List<Image> energyPlayer1;
    public List<Image> energyPlayer2;
    [SerializeField]
    Button attackButton;
    [SerializeField]
    Button defenseButton;
    [SerializeField]
    Button specialButton;
    [SerializeField]
    Text attackText;
    [SerializeField]
    Text defenseText;

    //Cambia el daño del dron sobre el que has  pulsado
    public void ChangeAttackDamage(float newDamage)
	{
        currentDrone.SetDamage(newDamage);        
	}
    //Cambia la bala del dron seleccionado
    public void ChangeBullet(Bullets currentBullet)
	{
        currentDrone.SetBullet(currentBullet);
	}
    //Cambia el coste de energia del dron seleccionado
    public void SetEnergyCost()
	{
        currentDrone.SpendEnergy();            
	}
    //Cambia el dron seleccionado
    public void SetCurrentDrone(BigDrone drone)
	{
        currentDrone = drone;
	}
    public void SetButtons()
	{
        BigDrone drone = GamePlayManager.instance.GetLocalPlayer().GetMySoldiers()[0];

        attackButton.onClick.AddListener(delegate{drone.AddAttack();});
        defenseButton.onClick.AddListener(delegate{drone.AddDefense();});
	}
    public void UpdateAmounts()
	{
        BigDrone drone = GamePlayManager.instance.GetLocalPlayer().GetMySoldiers()[0];
        attackText.text = drone.GetAttack().ToString();
        defenseText.text = drone.GetDefense().ToString();
	}
    public void SetInteractable(bool must)
	{
        attackButton.interactable = must;
        defenseButton.interactable = must;
        specialButton.interactable = must;
	}
}
