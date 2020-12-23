using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    int oxigen;
    int money;
    int food;
    [SerializeField]
    VisualManager visualManager;
    public void AddResource(ResourcesRoom.EResource type, int amount)
    {
        switch(type)
        {
            case ResourcesRoom.EResource.OXIGEN:
                oxigen+=amount;
                visualManager.UpdateResources(type,oxigen);
                break;
            case ResourcesRoom.EResource.MONEY:
                money+=amount;
                visualManager.UpdateResources(type,money);
                break;
            case ResourcesRoom.EResource.FOOD:
                food+=amount;
                visualManager.UpdateResources(type,food);
                break;
        }        
    }
}
