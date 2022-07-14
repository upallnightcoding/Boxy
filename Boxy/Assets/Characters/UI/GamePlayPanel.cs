using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GamePlayPanel : MonoBehaviour
{
    // Inspector Fields
    [SerializeField] private GameData gameData;

    [SerializeField] private TMP_Text player1NameDisplay;
    [SerializeField] private TMP_Text player2NameDisplay;

    [SerializeField] private TMP_Text player1Score;
    [SerializeField] private TMP_Text player2Score;

    [SerializeField] private GamePlay gamePlay;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winPanelName;
    [SerializeField] private TMP_Text message;

    private int squareCount = 0;
    private int boardSize = 0;
    private int totalSquareCount = 0;

    private PlayerUI[] playerUI = null;

    /// <summary>
    /// StartGamePlay() - Start the game play by setting the UI components with
    /// the player's names.  The actual game play code is started by the UI
    /// seperating the UI and game play logic.
    /// </summary>
    public void StartGamePlay()
    {
        gameObject.SetActive(true);

        boardSize = gameData.BoardSize;
        totalSquareCount = (boardSize - 1) * (boardSize - 1);

        playerUI = new PlayerUI[2];
        playerUI[(int)GameState.PLAYER1] = new PlayerUI(player1NameDisplay, player1Score, gameData.Player1Name);
        playerUI[(int)GameState.PLAYER2] = new PlayerUI(player2NameDisplay, player2Score, gameData.Player2Name);

        gamePlay.StartGamePlay();
    }

    /// <summary>
    /// StopGamePlay() - 
    /// </summary>
    public void StopGamePlay()
    {
        gamePlay.StopGamePlay();

        gameObject.SetActive(false);
    }

    public void SaveGamePlay()
    {
        SaveLoadData saveLoadData = new SaveLoadData();
        saveLoadData.player1Name = player1NameDisplay.text;
        saveLoadData.player2Name = player2NameDisplay.text;

        gamePlay.SaveGameData(saveLoadData);

        SaveLoadGameData.SaveGameData(saveLoadData);

        LoadGamePlay();

        StopGamePlay();
    }

    public void LoadGamePlay()
    {
        SaveLoadData saveLoadData = SaveLoadGameData.LoadGameData();

        if(saveLoadData != null)
        {
            player1NameDisplay.text = saveLoadData.player1Name;
            player2NameDisplay.text = saveLoadData.player2Name;

            gamePlay.LoadGameData(saveLoadData);
        }
    }

    public void UpdateScore(GameState gameState)
    {
        playerUI[(int) gameState].IncScore();

        IsGameOver(gameState);
    }

    public void CloseWinnerPanel()
    {
        winPanel.SetActive(false);

        StopGamePlay();
    }

    private void IsGameOver(GameState lastPlayerToMove)
    {
        if (++squareCount == totalSquareCount)
        {
            message.text = "You Won";

            if (playerUI[0].Score > playerUI[1].Score)
            {
                winPanelName.text = playerUI[0].PlayerName();
            } else if (playerUI[0].Score < playerUI[1].Score)
            {
                winPanelName.text = playerUI[1].PlayerName();
            } else
            {
                winPanelName.text = "";
                message.text = "You Tied";
            }

            winPanel.SetActive(true);
        }
    }
}

public class PlayerUI
{
    public int Score { get; private set; } = 0;

    private readonly TMP_Text displayName;
    private readonly TMP_Text displayScore;

    public PlayerUI(TMP_Text displayName, TMP_Text displayScore, string name)
    {
        this.displayName = displayName;
        this.displayScore = displayScore;

        this.displayName.text = name;
    }

    public void IncScore()
    {
        Score++;
        displayScore.text = Score.ToString();
    }

    public string PlayerName()
    {
        return (displayName.text);
    }
}

