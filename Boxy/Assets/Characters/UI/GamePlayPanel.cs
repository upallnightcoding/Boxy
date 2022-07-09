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

    [SerializeField] private GameObject gamePlayObject;

    private void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// StartGamePlay() - Start the game play by setting the UI components with
    /// the player's names.  The actual game play code is started by the UI
    /// seperating the UI and game play logic.
    /// </summary>
    public void StartGamePlay()
    {
        gameObject.SetActive(true);

        player1NameDisplay.text = gameData.Player1Name;
        player2NameDisplay.text = gameData.Player2Name;

        gamePlayObject.GetComponent<GamePlay>().StartGamePlay();
    }

    public void StopGamePlay()
    {
        gamePlayObject.GetComponent<GamePlay>().StopGamePlay();

        gameObject.SetActive(false);
    }

    public void UpdateScore(GameState gameState)
    {
        int score = -1;

        try
        {
            TMP_Text scoreTxt = (gameState == GameState.PLAYER1) ? player1Score : player2Score;
            score = System.Int32.Parse(scoreTxt.text) + 1;
            scoreTxt.text = score.ToString();
        }
        catch (FormatException)
        {
            score = -1;
        }
    }
}

