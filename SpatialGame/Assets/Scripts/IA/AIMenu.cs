using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIMenu : MonoBehaviour
{
    public int numberOfNPC;
	[SerializeField]
    Button myImage;

	private void Start()
	{
		for(int i = 1; i< numberOfNPC; i++)
		{
			Instantiate(myImage.gameObject,transform);
		}
	}
}
