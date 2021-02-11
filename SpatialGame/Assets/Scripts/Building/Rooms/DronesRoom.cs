using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronesRoom : BuildingType
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
        //Updates el tiempo en el canvas
		if(travellingDrones.Count>0)
        {
            droneTime.text = minutes.ToString("00") + " : " + seconds.ToString("00");
		}          
	}
	public List<Drone> GetDrones()
    {
        return availableDrones;
    }
    //Activa el drone que has pulsado
    public void SetWorkingDrone(Drone drone)
    {
        drone.SetTarget(takeOffPosition);//Le da el punto de salida
        drone.ChangeState(Drone.EState.FETCHING);//Cambia el estado del dron
        availableDrones.Remove(drone);//Quita el dron de la lista de availableDrones
        travellingDrones.Add(drone);//lo añade a la lista de drones viajando
        droneTime.gameObject.SetActive(true);//Activa el reloj        
    }
    //Calcula los minutos y segundos que el dron tiene que estar viajando
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
        travellingDrones.Remove(drone);//Cuando ha vuelto lo añade a los drones disponibles
        availableDrones.Add(drone);//Lo quita de los drones viajando
        droneTime.gameObject.SetActive(false);//Desactiva el tiempo
        int rnd = Random.Range(10,1000);
        rock = rnd;
        rnd = Random.Range(10,1000);
        iron = rnd;
        rnd = Random.Range(10,1000);        
        wood = rnd;
        //Calcula los recursos encontrados
        myResourceManager.AddResourceInventory(rock,iron,wood);//Los añade a los recursos
    }
    public Transform GetRecollectingPosition(){return recollectionPosition;}//Devuelve la posicion de recogida de recursos    
    //Activa el gameObject de los drones
    public void ActivateDrones()
    {
        foreach(Drone d in availableDrones)
        {
            d.gameObject.SetActive(true);
        }
    }
}
