using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
   public static void SaveGame(ResourceManager manager)
   {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameSave.txt";
        FileStream file = new FileStream(path, FileMode.Create);

        GameSaver gameSaver = new GameSaver(manager);

        formatter.Serialize(file, gameSaver);
        file.Close();
   }
    public static GameSaver LoadGame()
    {
      string path = Application.persistentDataPath + "/GameSave.txt";
        if(File.Exists(path))
        { 
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = new FileStream(path, FileMode.Open);

            GameSaver gameSaver = formatter.Deserialize(file) as GameSaver;
            file.Close();

            return gameSaver;
        }
        else
        {
            Debug.Log("The file doesn't exists" + path);
            return null;
        }
    }
}
