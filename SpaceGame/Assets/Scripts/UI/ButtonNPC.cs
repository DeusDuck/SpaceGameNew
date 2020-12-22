using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNPC : MonoBehaviour
{
    [SerializeField]
    GameObject currentNPC;

    public GameObject GetCurrentNPC(){return currentNPC; }
}
