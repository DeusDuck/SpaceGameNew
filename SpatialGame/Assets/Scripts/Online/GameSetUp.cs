using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetUp : MonoBehaviour
{
    public static GameSetUp gameSetUp;

    public Transform[] spawningPointsLocal;
    public Transform[] spawningPointsOther;
	// Start is called before the first frame update
	private void OnEnable()
	{
		if(GameSetUp.gameSetUp==null)
			GameSetUp.gameSetUp = this;
	}
}
