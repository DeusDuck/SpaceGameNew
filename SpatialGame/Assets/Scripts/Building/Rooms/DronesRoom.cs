using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronesRoom : MonoBehaviour
{
    [SerializeField]
    List<Drone> availableDrones;
    [SerializeField]
    List<Drone> travellingDrones;
    [SerializeField]
    Transform takeOffPosition;
    [SerializeField]
    Transform recollectionPosition;
    [SerializeField]
    Text droneTime;
    ResourceManager resourceManager;
    int rock;
    int wood;
    int iron;
    float seconds;
    float minutes;
    // Start is called before the first frame update
    void Start()
    {
        travellingDrones = new List<Drone>();
    }
	private void Update()
	{
		if(travellingDrones.Count>0)
        {
            droneTime.text = minutes.ToString("00") + " : " + seconds.ToString("00");            
        }            
	}
	public List<Drone> GetDrones()
    {
        return availableDrones;
    }
    public void SetWorkingDrone(Drone drone)
    {
        drone.SetTarget(takeOffPosition);
        drone.ChangeState(Drone.EState.FETCHING);
        availableDrones.Remove(drone);
        travellingDrones.Add(drone);
        droneTime.gameObject.SetActive(true);        
    }
    public void CalculateTime(float time)
    {
        minutes = time / 60;
        seconds = time % 60;
		if(time>0)
		{
            if(seconds<=0)
            {
                minutes--;
                seconds = 59;
            }
		}      
    }
    public void DroneReturned(Drone drone)
    {
        travellingDrones.Remove(drone);
        availableDrones.Add(drone);
        droneTime.gameObject.SetActive(false);
        int rnd = Random.Range(10,1000);
        rock = rnd;
        rnd = Random.Range(10,1000);
        iron = rnd;
        rnd = Random.Range(10,1000);        
        wood = rnd;
        resourceManager.AddResourceInventory(rock,iron,wood);
    }
    public Transform GetRecollectingPosition(){return recollectionPosition;}
    public void SetResourceManager(ResourceManager manager){resourceManager = manager;}
    public void ActivateDrones()
    {
        foreach(Drone d in availableDrones)
        {
            d.gameObject.SetActive(true);
        }
    }
}
