using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Boxy/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player's Names")]
    public string Player1Name = null;
    public string Player2Name = null;

    [Header("Game Variables")]
    public int BoardSize = -1;
    public GameLevel level = GameLevel.EASY;
    public GameMode mode = GameMode.ONE_PLAYER;

    // Contains Save & Load Data
    public SaveLoadData saveLoadData = null;

    public void Initialize()
    {
        Player1Name = "Player1";
        Player2Name = "Player2";

        BoardSize = 3;
    }
}
