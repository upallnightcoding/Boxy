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

    // Player Names
    private string player1Name = null;
    private string player2Name = null;

    private void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGamePlay()
    {
        gameObject.SetActive(true);

        player1Name = gameData.Player1Name;
        player2Name = gameData.Player2Name;

        player1NameDisplay.text = player1Name;
        player2NameDisplay.text = player2Name;

        gamePlayObject.GetComponent<GamePlay>().StartGamePlay();
    }

    private void UpdateScore()
    {
        int score = -1;

        try
        {
            //TMP_Text scoreTxt = GetScore;
            //score = System.Int32.Parse(scoreTxt.text);
            //score += 1;
            //scoreTxt.text = score.ToString();
        }
        catch (FormatException)
        {
            score = -1;
        }
    }
}

