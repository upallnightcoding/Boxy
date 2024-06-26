using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

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

    [SerializeField] private TMP_Text gameLevel;

    [SerializeField] private Button resignButton;
    [SerializeField] private Button saveButton;

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

        squareCount = 0;

        playerUI = new PlayerUI[2];
        playerUI[(int)GameState.PLAYER1] = new PlayerUI(player1NameDisplay, player1Score, gameData.Player1Name);
        playerUI[(int)GameState.PLAYER2] = new PlayerUI(player2NameDisplay, player2Score, gameData.Player2Name);

        boardSize = gameData.BoardSize;
        totalSquareCount = (boardSize - 1) * (boardSize - 1);

        gameLevel.text = (gameData.mode == GameMode.ONE_PLAYER) ? gameData.level.ToString() : "";

        gamePlay.StartGamePlay(boardSize, gameData.mode, gameData.level);
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
        string player1Name = player1NameDisplay.text;
        string player2Name = player2NameDisplay.text;
        SaveLoadData saveLoadData = new SaveLoadData(player1Name, player2Name);

        gamePlay.SaveGameData(saveLoadData);

        SaveLoadGameData.SaveGameData(saveLoadData);

        StopGamePlay();
    }

    public void LoadGamePlay()
    {
        SaveLoadData saveLoadData = SaveLoadGameData.LoadGameData();

        if(saveLoadData != null)
        {
            gameObject.SetActive(true);

            squareCount = 0;

            playerUI = new PlayerUI[2];
            playerUI[(int)GameState.PLAYER1] = new PlayerUI(player1NameDisplay, player1Score, saveLoadData.player1Name);
            playerUI[(int)GameState.PLAYER2] = new PlayerUI(player2NameDisplay, player2Score, saveLoadData.player2Name);

            gamePlay.LoadGameData(saveLoadData);
        }
    }

    public void ResetScore()
    {
        playerUI[(int)GameState.PLAYER1].ResetScore();
        playerUI[(int)GameState.PLAYER2].ResetScore();
    }

    public void UpdateScore(GameState gameState)
    {
        playerUI[(int) gameState].IncScore();

        IsGameOver(gameState);
    }

    public void CloseWinnerPanel()
    {
        winPanel.SetActive(false);
        resignButton.enabled = true;
        saveButton.enabled = true;

        StopGamePlay();
    }

    private void IsGameOver(GameState lastPlayerToMove)
    {
        if (++squareCount == totalSquareCount)
        {
            message.text = "You won";

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
            resignButton.enabled = false;
            saveButton.enabled = false;
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
        this.displayScore.text = "0";
    }

    public void IncScore()
    {
        Score++;
        displayScore.text = Score.ToString();
    }

    /// <summary>
    /// ResetScore() - Resets the score to zero to initialize the score
    /// for the beginning of the game or after a load previous game.
    /// </summary>
    public void ResetScore()
    {
        Score = 0;
        displayScore.text = Score.ToString();
    }

    public string PlayerName()
    {
        return (displayName.text);
    }
}

