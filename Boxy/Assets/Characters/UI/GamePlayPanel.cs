using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayPanel : MonoBehaviour
{
    private int boardSize;

    [SerializeField] private GameObject pegPreFab;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private GameData gameData;

    private string player1Name = null;
    private string player2Name = null;

    private static GameState currentState = GameState.IDLE;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);

        switch(currentState)
        {
            case GameState.IDLE:
                break;
            case GameState.PLAYER1:
                break;
        }
    }

    public void StartGamePlay()
    {
        gameObject.SetActive(true);

        player1Name = gameData.Player1Name;
        player2Name = gameData.Player2Name;

        boardSize = gameData.BoardSize;

        DrawGameBoard();

        currentState = GameState.PLAYER1;
    }

    private void DrawGameBoard()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                GameObject go = Instantiate(pegPreFab, new Vector3(col, row, 0.0f), Quaternion.identity);
                go.transform.parent = transform;

                Peg peg = go.GetComponent<Peg>();
                peg.MaxLinks = 4;

                go.name = $"Peg: {col}, {row} {peg.MaxLinks}";
            }
        }

        float position = (boardSize - 1.0f) / 2.0f;
        cameraPos.transform.position = new Vector3(position, position, -10.0f);
    }
}

public enum GameState
{
    IDLE,
    PLAYER1,
    PLAYER2,
    WON,
    RESIGN,
    STOP
}
