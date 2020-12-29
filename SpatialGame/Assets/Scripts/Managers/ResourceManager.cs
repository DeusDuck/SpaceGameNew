using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    int oxigen;
    [SerializeField]
    int money;
    [SerializeField]
    int food;
    [SerializeField]
    VisualManager visualManager;

	private void Start()
	{
		visualManager.UpdateResources(ResourcesRoom.EResource.OXIGEN,oxigen);
        visualManager.UpdateResources(ResourcesRoom.EResource.MONEY,money);
        visualManager.UpdateResources(ResourcesRoom.EResource.FOOD,food);
	}
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
    public bool EnoughResources(int[] currency)
    {
        return oxigen>=currency[0] && money>=currency[1] && food>=currency[2];    
    }
    public void SpendResources(int[] currency)
    {
        oxigen-=currency[0];
        money-=currency[1];
        food-=currency[2];
        UpdateUI();
    }
    void UpdateUI()
    {
        visualManager.UpdateResources(ResourcesRoom.EResource.OXIGEN,oxigen);
        visualManager.UpdateResources(ResourcesRoom.EResource.MONEY,money);
        visualManager.UpdateResources(ResourcesRoom.EResource.FOOD,food);
    }
}
