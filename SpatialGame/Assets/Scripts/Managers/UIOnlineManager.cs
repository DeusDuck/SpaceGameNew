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

    public void BasicAttack()
	{
        gamePlayManager.attacking = true;
	}
}
