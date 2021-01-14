using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPositions : MonoBehaviour
{
    [SerializeField]
    Transform[] positions;

    public Transform[] Positions{get{return positions;}}
}
