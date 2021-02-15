using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Se encarga de actualizar los recursos
    [SerializeField]
    int oxigen;
    [SerializeField]
    int money;
    [SerializeField]
    int food;
    [SerializeField]
    int wood;
    [SerializeField]
    int iron;
    [SerializeField]
    int rock;
    [SerializeField]
    VisualManager visualManager;

	private void Start()
	{
        //Al empezar el juego mira si hay recursos guardados y si los hay los carga
        GameSaver saveFile = SaveSystem.LoadGame();
		if(saveFile!=null)
		{
            oxigen = saveFile.oxigen;
            wood = saveFile.wood;
            food = saveFile.food;
            iron = saveFile.iron;
            money = saveFile.money;
            rock = saveFile.rock;
		}
		//Recarga la UI con los recursos
        visualManager.UpdateResources(ResourcesRoom.EResource.OXIGEN,oxigen);
        visualManager.UpdateResources(ResourcesRoom.EResource.MONEY,money);
        visualManager.UpdateResources(ResourcesRoom.EResource.FOOD,food);
		visualManager.UpdateInventory(rock,iron,wood);
	}
    //Suma los recursos
	public void AddResource(ResourcesRoom.EResource type, int amount)
    {
        switch(type)
        {
            case ResourcesRoom.EResource.OXIGEN:
                oxigen+=amount;
                
                break;
            case ResourcesRoom.EResource.MONEY:
                money+=amount;
                
                break;
            case ResourcesRoom.EResource.FOOD:
                food+=amount;
                
                break;
        }
        UpdateUI();
    }
    //Suma los recursos del inventario
    public void AddResourceInventory(int rockAmount, int ironAmount, int woodAmount)
    {
        rock+=rockAmount;
        iron+=ironAmount;
        wood+=woodAmount;
        UpdateUI();
    }
    //Comprueba si hay suficientes recursos
    public bool EnoughResources(int[] currency)
    {
        return oxigen>=currency[0] && money>=currency[1] && food>=currency[2];    
    }
    //Gasta los recursos y refresca la UI
    public void SpendResources(int[] currency)
    {
        oxigen-=currency[0];
        money-=currency[1];
        food-=currency[2];
        UpdateUI();
    }
    //Refresca la UI
    void UpdateUI()
    {
        visualManager.UpdateResources(ResourcesRoom.EResource.OXIGEN,oxigen);
        visualManager.UpdateResources(ResourcesRoom.EResource.MONEY,money);
        visualManager.UpdateResources(ResourcesRoom.EResource.FOOD,food);
        visualManager.UpdateInventory(rock,iron,wood);
        SaveSystem.SaveGame(this);
    }
    //Devuelve todos los recursos disponibles
    public List<int> GetAllResources()
    {
        List<int> resources = new List<int>();
        resources.Add(food);
        resources.Add(money);
        resources.Add(oxigen);
        resources.Add(rock);
        resources.Add(iron);
        resources.Add(wood);
        return resources;
    }
}
