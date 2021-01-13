using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField]
    string gameVersion = "0.0.0";
    public string GameVersion{get{return gameVersion;}}
    [SerializeField]
    string nickName = "yellowBeatle";
    public string NickName
	{
		get
		{
            int rnd = Random.Range(1,100);
            return nickName + rnd.ToString();
		}
	}
}
