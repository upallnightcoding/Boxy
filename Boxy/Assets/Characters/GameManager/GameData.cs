using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Boxy/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player Name")]
    public string Player1Name = null;
    public string Player2Name = null;

    [Header("Game Variables")]
    public int BoardSize;

    public void Initialize()
    {
        Player1Name = "Player1";
        Player2Name = "Player2";

        BoardSize = 3;
    }
}
