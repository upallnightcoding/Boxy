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
    public string player1Name = null;
    public string player2Name = null;

    public int boardSize = -1;

    public string rowLinks = null;
    public string colLinks = null;

    public string squares = null;

    public string GetRowLink(int index) => rowLinks.Substring(index, 1);
    public string GetColLink(int index) => colLinks.Substring(index, 1);

    public bool IsRowLink(int index) => GetRowLink(index) != PlayerColor.EMPTY;
    public bool IsColLink(int index) => GetColLink(index) != PlayerColor.EMPTY;

    public Color GetRowColor(int index) => 
        GetRowLink(index) == PlayerColor.BLACK ? Color.black : Color.white;
    public Color GetColColor(int index) => 
        GetColLink(index) == PlayerColor.BLACK ? Color.black : Color.white;

    public void Initialize()
    {
        player1Name = null;
        player2Name = null;

        boardSize = -1;

        rowLinks = null;
        colLinks = null;

        squares = null;
    }
}
