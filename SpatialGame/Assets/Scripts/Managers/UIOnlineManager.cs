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
    GameObject panel;

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
    public void SetEnergyCost(int energy)
	{
		if(energy<=currentDrone.Getplayer().GetCurrentEnergy())
            currentDrone.SetEnergyCost(energy);            
	}
    //Cambia el dron seleccionado
    public void SetCurrentDrone(BigDrone drone)
	{
        currentDrone = drone;
	}
    //Esconde el panel de ataque
    public void HidePanel()
	{
        panel.SetActive(false);
	}
    //Enseña las flechas sobre los drones
    public void ShowAvailableDrones()
	{
        foreach(BigDrone drone in gamePlayManager.avatars)
		{
            if(gamePlayManager.attackingDrones.Contains(drone) || gamePlayManager.movingDrones.Contains(drone)
                || !gamePlayManager.currentPlayer.GetMySoldiers().Contains(drone))
                continue;

            drone.ShowArrow(true);
		}
	}
}
