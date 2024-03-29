using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private GameObject twoPlayerContainer;
    [SerializeField] private GameData gameData;
    [SerializeField] private GamePlayPanel gamePlayPanel;

    public void StartGame()
    {
        gameObject.SetActive(false);

        gamePlayPanel.StartGamePlay();
    }

    public void StartNextGame()
    {
        gameObject.SetActive(true);
    }

    public void LoadGame()
    {
        gameObject.SetActive(false);

        gamePlayPanel.LoadGamePlay();
    }

    public void OnPlayer1Name(string value)
    {
        gameData.Player1Name = value;
    }

    public void OnPlayer2Name(string value)
    {
        gameData.Player2Name = value;
    }

    public void OnePlayerGame()
    {
        twoPlayerContainer.SetActive(false);
        gameObject.SetActive(true);
        gameData.mode = GameMode.ONE_PLAYER;
        gameData.Player2Name = "Boxy";
    }

    public void TwoPlayerGame()
    {
        twoPlayerContainer.SetActive(true);
        gameObject.SetActive(true);
        gameData.mode = GameMode.TWO_PLAYER;
    }
}
