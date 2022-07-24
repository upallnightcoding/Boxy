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

    //[SerializeField] GameObject backGround1;
    //[SerializeField] GameObject backGround2;

    [SerializeField] GameObject gamePlayPanel;

    private int boardSize;

    private bool loadMode = false;

    // Input Control Attributes
    private bool leftMouseButton;
    private Vector3 mousePos;

    private SquareWall[,] wall;
    private Peg[,] pegBoard;

    private Peg illegalPeg = null;

    //private Peg pegStart;

    // Initial Game State
    private GameState gameState = GameState.IDLE;
    private SelectionState selectionState = SelectionState.ANCHOR;

    //private List<GameObject> listOfGameObjects;

    private GameObject GetPlayer() => (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

    // Return the current color
    private Color GetPlayerColor() => GetPlayer().GetComponent<SpriteRenderer>().color;

    // Update is called once per frame
    void Update()
    {
        leftMouseButton = Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON);
        mousePos = Input.mousePosition;

        switch (gameState)
        {
            case GameState.IDLE:
                break;
            case GameState.PLAYER1:
                MakeMove();
                break;
            case GameState.PLAYER2:
                MakeMove();
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
        selectionState = SelectionState.ANCHOR;
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

                    gameLogic.UpdateScore(gameState);

                    gameLogic.SetBoxState(col, row, gameState);
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
                    RenderLink(pegBoard[col, row], pegBoard[col+1,row], color, gameState);
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
                    RenderLink(pegBoard[col, row], pegBoard[col, row + 1], color, gameState);
                }

                colIndex++;
            }
        }

        loadMode = false;
    }

    private void MakeMove()
    {
        switch (selectionState)
        {
            case SelectionState.ANCHOR:
                selectionState = SelectAnchorPeg();
                break;
            case SelectionState.PIN:
                selectionState = SelectPinPeg();

                switch (selectionState)
                {
                    case SelectionState.ANCHOR:
                        TogglePlayer();
                        break;
                    case SelectionState.CANCEL:
                        selectionState = SelectionState.ANCHOR;
                        break;
                }

                break;
        }
    }

    private SelectionState SelectAnchorPeg()
    {
        SelectionState selection = SelectionState.ANCHOR;

        if (leftMouseButton)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (peg.IsOpen())
                {
                    selection = SelectionState.PIN;

                    gameRenderer.SetWallAnchorPeg(peg, GetPlayerColor());
                }
            }
        }

        return (selection);
    }

    private SelectionState SelectPinPeg()
    {
        SelectionState state = SelectionState.PIN;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (leftMouseButton)
        {
            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (peg.IsOpen() && gameLogic.LegalMove(gameRenderer.GetStartPeg(), peg))
                {
                    gameRenderer.RemoveLineRenderer();

                    RenderLink(gameRenderer.GetStartPeg(), peg, GetPlayerColor(), gameState);

                    state = SelectionState.ANCHOR;

                    //AudioManager.Instance.SoundLegalMove();
                }
            }
            else
            {
                gameRenderer.RemoveLineRenderer();

                gameRenderer.GetStartPeg().Reset();

                state = SelectionState.CANCEL;
            }
        }
        else
        {
            gameRenderer.AnimateRenderLine(ray.GetPoint(0.0f));

            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (!gameLogic.LegalMove(gameRenderer.GetStartPeg(), peg))
                {
                    peg.Illegal();

                    illegalPeg = peg;
                    //AudioManager.Instance.SoundIllegalMove();
                }
            } else
            {
                illegalPeg?.Reset();
                illegalPeg = null;
            }
        }

        return (state);
    }

    private void RenderLink(Peg pegStart, Peg pegEnd, Color color, GameState gameState)
    {
        GameObject square = (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

        BoxPosList boxPosList = new BoxPosList(); 

        gameRenderer.DrawWall(pegStart, pegEnd, color);

        gameLogic.LinkPegs(pegStart, pegEnd, gameState);
        gameLogic.UpdateWallCount(pegStart, pegEnd, square, boardSize, wall, gameState, loadMode, boxPosList);
        CheckBoxCount(boxPosList);

        pegStart.Reset();
        pegEnd.Reset();
    }

    private void CheckBoxCount(BoxPosList boxPosList)
    {
        boxPosList.ForEachBox(delegate (BoxPos boxPos)
        {
            gameRenderer.DrawBox(boxPos, GetPlayer(), gameState);
        });
    }

    private void ResetScore()
    {
        gamePlayPanel.GetComponent<GamePlayPanel>().ResetScore();
    }

    private void TogglePlayer()
    {
        gameState = (gameState == GameState.PLAYER1) ? GameState.PLAYER2 : GameState.PLAYER1;
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

public enum SelectionState
{
    ANCHOR,
    PIN,
    COMPLETE,
    CANCEL
}
