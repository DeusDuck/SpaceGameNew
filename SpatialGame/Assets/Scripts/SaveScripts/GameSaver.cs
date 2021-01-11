using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaver
{
  public int wood;
  public int iron;
  public int rock;
  public int oxigen;
  public int money;
  public int food;

  public GameSaver(ResourceManager manager)
  {
    List<int> current = manager.GetAllResources();
    food = current[0];
    money = current[1];
    oxigen = current[2];
    rock = current[3];
    iron = current[4];
    wood = current[5];
  }
}
