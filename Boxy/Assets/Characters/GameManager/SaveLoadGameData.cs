using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadGameData 
{
    // Class Constants
    private const string SAVE_LOAD_DATA_FILE = "/SaveLoadData.data";

    public static void SaveGameData(SaveLoadData saveLoadData)
    {
        string path = Application.persistentDataPath + SAVE_LOAD_DATA_FILE;

        FileStream fileStream = new FileStream(path, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(fileStream, saveLoadData);

        fileStream.Close();
    }

    public static SaveLoadData LoadGameData()
    {
        SaveLoadData saveLoadData = null;

        string path = Application.persistentDataPath + SAVE_LOAD_DATA_FILE;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fileStream = new FileStream(path, FileMode.Open);

            saveLoadData = formatter.Deserialize(fileStream) as SaveLoadData;

            fileStream.Close();
        } 

        return (saveLoadData);
    }
}

[System.Serializable]
public class SaveLoadData
{
    public string player1Name;
    public string player2Name;

    public int boardSize;

    public string rowLinks;
    public string colLinks;

    public string squares;
}
