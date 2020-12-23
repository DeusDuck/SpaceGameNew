using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesRoom : MonoBehaviour
{
    int numberOfWorkers = 0; 
    bool startCounting;
    float currentTimeWorking = 0.0f;
    public enum EResource
    {
        FOOD,OXIGEN,MONEY
    }
    public EResource currentResource;
    [SerializeField]
    int amountOfResourcesToGive;
    int currentResources;
    [SerializeField]
    float timeToCollectResources;    
    [SerializeField]
    List<Transform> availableWorkingPositions;
    [SerializeField]
    List<Transform> workingPositions;
    ResourceManager myResourceManager;

	private void Update()
	{
		if(startCounting)
        {
            currentTimeWorking+=Time.deltaTime;
            if(currentTimeWorking>=timeToCollectResources/numberOfWorkers)
            {
                currentTimeWorking = 0;
                myResourceManager.AddResource(currentResource,amountOfResourcesToGive);
            }
        }
	}
	public Transform GetWorkingPosition()
    { 
        if(availableWorkingPositions.Count>0)
        {
            Transform pos = availableWorkingPositions[0];
            availableWorkingPositions.Remove(pos);
            return pos;
        }
        return null;
    }
    public bool IsBuildingFull(){return availableWorkingPositions.Count == 0;}
    public void AddWorker(){numberOfWorkers++;}
    public void RemoveWorker(){numberOfWorkers--;}
    public void StartCounter(bool must){startCounting = must;}
    public void SetResourceManager(ResourceManager manager){myResourceManager = manager;}
}
