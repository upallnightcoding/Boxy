using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // Class Constants
    private const int LEFT_MOUSE_BUTTON = 0;
    

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Material drawLineMaterial;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject squareBlackPreFab;
    [SerializeField] private GameObject squareWhitePreFab;

    [SerializeField] private GameRenderer gameRenderer;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private GameAudio gameAudio;

    [SerializeField] private MakePlayerMove makePlayerMove;

    [SerializeField] GameObject gamePlayPanel;

    private int boardSize;

    private bool loadMode = false;

    // Input Control Attributes
    private bool leftMouseButton;
    private Vector3 mousePos;

    //private SquareWall[,] wall;
    private Peg[,] pegBoard;

    // Initial Game State
    private GameState gameState = GameState.IDLE;

    private GameObject GetPlayer() => (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

    // Return the current color
    private Color GetPlayerColor() => GetPlayer().GetComponent<SpriteRenderer>().color;

    // Update is called once per frame
    void Update()
    {
        leftMouseButton = Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON);
        mousePos = Input.mousePosition;

        GameMove gameMove = null;

        switch (gameState)
        {
            case GameState.IDLE:
                break;
            case GameState.PLAYER1:
                gameMove = makePlayerMove.MakeMove(leftMouseButton, mousePos, gameState);
                if (gameMove != null)
                {
                    RenderMove(gameMove.StartPeg, gameMove.EndPeg, GetPlayerColor(), gameState);
                    TogglePlayer();
                }
                break;
            case GameState.PLAYER2:
                gameMove = makePlayerMove.MakeMove(leftMouseButton, mousePos, gameState);
                if (gameMove != null)
                {
                    RenderMove(gameMove.StartPeg, gameMove.EndPeg, GetPlayerColor(), gameState);
                    TogglePlayer();
                }
                break;
            case GameState.STOP:
                break;
        }
    }

    /// <summary>
    /// StartGamePlay() - 
    /// </summary>
    /// <param name="gameData"></param>
    public void StartGamePlay(int boardSize)
    {
        gameState = GameState.PLAYER1;
        this.boardSize = boardSize;

        gameLogic.Initialize(boardSize);
        wall = gameLogic.GetWalls();
        pegBoard = gameLogic.GetPegBoard();

        gameRenderer.DrawGameBoard(boardSize);
        cameraPos.transform.position = gameRenderer.GetNewCameraPosition();
    }

    public void StopGamePlay()
    {
        foreach (GameObject go in gameRenderer.GetListOfGameObjects())
        {
            Destroy(go);
        }
    }

    /// <summary>
    /// SaveGameData() - Saves the game state to be loaded later.  Data that
    /// needs to be saved included the size of the board, the squares that 
    /// have been set to black or white and all links.
    /// </summary>
    /// <param name="saveLoadData"></param>
    public void SaveGameData(SaveLoadData saveLoadData)
    {
        gameLogic.SaveGameData(saveLoadData);
    }

    /// <summary>
    /// LoadGameData() - Load the saved data and reset for a new game.
    /// </summary>
    /// <param name="saveLoadData"></param>
    public void LoadGameData(SaveLoadData saveLoadData)
    {
        loadMode = true;

        ResetScore();

        boardSize = saveLoadData.boardSize;

        StartGamePlay(boardSize);

        string squares = saveLoadData.squares;
        int squareIndex = 0;

        // Draw all boxes that loaded from the saved game
        //-----------------------------------------------
        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                string squareColor = squares.Substring(squareIndex++, 1);

                if (squareColor != PlayerColor.EMPTY)
                {
                    GameObject square = (squareColor == PlayerColor.BLACK) ? squareBlackPreFab : squareWhitePreFab;

                    GameState gameState = (squareColor == PlayerColor.BLACK) ? GameState.PLAYER1 : GameState.PLAYER2;

                    gameRenderer.LoadBox(col, row, square, gameState);

                    gameLogic.UpdateScore(col, row, gameState);

                    //gameLogic.SetBoxState(col, row, gameState);
                }
            }
        }

        int rowIndex = 0;

        // Draw the horizontal links loaded from the saveed game
        //------------------------------------------------------
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                if (saveLoadData.IsRowLink(rowIndex)) 
                {
                    Color color = saveLoadData.GetRowColor(rowIndex);
                    GameState gameState = (saveLoadData.GetRowLink(rowIndex) == PlayerColor.BLACK) ? GameState.PLAYER1 : GameState.PLAYER2;
                    RenderMove(pegBoard[col, row], pegBoard[col+1,row], color, gameState);
                }

                rowIndex++;
            }
        }

        int colIndex = 0;

        // Draw the vertical links loaded from the saved game
        //---------------------------------------------------
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize - 1; row++)
            {
                if (saveLoadData.IsColLink(colIndex))
                {
                    Color color = saveLoadData.GetColColor(colIndex);
                    GameState gameState = (saveLoadData.GetColLink(colIndex) == PlayerColor.BLACK) ? GameState.PLAYER1 : GameState.PLAYER2;
                    RenderMove(pegBoard[col, row], pegBoard[col, row + 1], color, gameState);
                }

                colIndex++;
            }
        }

        loadMode = false;
    }

    private void TogglePlayer()
    {
        gameState = (gameState == GameState.PLAYER1) ? GameState.PLAYER2 : GameState.PLAYER1;
    }

    private void RenderMove(Peg pegStart, Peg pegEnd, Color color, GameState gameState)
    {
        GameObject square = (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

        BoxPosList boxPosList = new BoxPosList(); 

        gameRenderer.DrawWall(pegStart, pegEnd, color);

        gameLogic.LinkPegs(pegStart, pegEnd, gameState);
        gameLogic.UpdateWallCount(pegStart, pegEnd, square, boardSize, gameState, loadMode, boxPosList);
        CheckBoxCount(boxPosList);

        pegStart.Reset();
        pegEnd.Reset();
    }

    private void CheckBoxCount(BoxPosList boxPosList)
    {
        if (boxPosList.HasCount())
        {
            gameAudio.PlayCreateBox();
        }

        boxPosList.ForEachBox(delegate (BoxPos boxPos)
        {
            gameRenderer.DrawBox(boxPos, GetPlayer(), gameState);
        });
    }

    private void ResetScore()
    {
        gamePlayPanel.GetComponent<GamePlayPanel>().ResetScore();
    }

 

    public void EndGamePlay()
    {
        foreach (GameObject go in gameRenderer.GetListOfGameObjects())
        {
            Destroy(go);
        }
    }
}

public class SquareWall
{
    public string State { get; private set; }

    private int count;

    public SquareWall()
    {
        count = 0;
        State = PlayerColor.EMPTY;
    }

    public void SetState(GameState player)
    {
        State = player switch
        {
            GameState.PLAYER1 => PlayerColor.BLACK,
            GameState.PLAYER2 => PlayerColor.WHITE,
            _ => PlayerColor.EMPTY,
        };
    }

    public bool Add()
    {
        return (++count >= 4);
    }
}

/// <summary>
/// PlayerColor() - Defines the colors that are assoicated with
/// each player.  
/// </summary>
public static class PlayerColor
{
    public static readonly string BLACK = "B";
    public static readonly string WHITE = "W";
    public static readonly string EMPTY = "E";
}

public enum GameState
{
    PLAYER1 = 0,    // This enumeration must be "0"
    PLAYER2 = 1,    // This enumeration must be "1"
    IDLE = 2,
    WON = 3,
    RESIGN = 4,
    STOP = 5
}


