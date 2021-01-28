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

    public void ChangeAttackDamage(float newDamage)
	{
        currentDrone.SetDamage(newDamage);        
	}
    public void ChangeBullet(Bullets currentBullet)
	{
        currentDrone.SetBullet(currentBullet);
	}
    public void SetEnergyCost(int energy)
	{
		if(energy<=currentDrone.Getplayer().GetCurrentEnergy())
            currentDrone.SetEnergyCost(energy);            
	}

    public void SetCurrentDrone(BigDrone drone)
	{
        currentDrone = drone;
	}
    public void HidePanel()
	{
        panel.SetActive(false);
	}
}
