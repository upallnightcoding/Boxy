using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private GameObject twoPlayerContainer;
    [SerializeField] private GameData gameData;
    [SerializeField] private TMP_InputField player1Name;
    [SerializeField] private TMP_InputField player2Name;
    [SerializeField] private GamePlayPanel gamePlayPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartGame()
    {
        gameData.Player1Name = player1Name.text;
        gameData.Player2Name = player2Name.text;

        gameObject.SetActive(false);

        gamePlayPanel.StartGamePlay();
    }

    public void OnePlayerGame()
    {
        twoPlayerContainer.SetActive(false);
        gameObject.SetActive(true);
    }

    public void TwoPlayerGame()
    {
        twoPlayerContainer.SetActive(true);
        gameObject.SetActive(true);
    }
}
