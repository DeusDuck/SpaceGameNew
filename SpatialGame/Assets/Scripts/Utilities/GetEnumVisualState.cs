using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEnumVisualState : MonoBehaviour
{
    public VisualManager.VisualState state;
    public void SetState(VisualManager.VisualState nextState){state = nextState; }
}
