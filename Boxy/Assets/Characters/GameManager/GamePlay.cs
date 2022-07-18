using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // Class Constants
    private const int LEFT_MOUSE_BUTTON = 0;
    private const float PEG_LINK_WIDTH = 0.089f;

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Material drawLineMaterial;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject pegPreFab;
    [SerializeField] private GameObject squareBlackPreFab;
    [SerializeField] private GameObject squareWhitePreFab;

    [SerializeField] GameObject backGround1;
    [SerializeField] GameObject backGround2;

    [SerializeField] GameObject gamePlayPanel;

    private int boardSize;

    // Input Control Attributes
    private bool leftMouseButton;
    private Vector3 mousePos;

    private Wall[,] wall;
    private Peg[,] pegBoard;

    private Peg illegalPeg = null;

    private Peg pegStart;

    // Initial Game State
    private GameState gameState = GameState.IDLE;
    private SelectionState selectionState = SelectionState.ANCHOR;

    private List<GameObject> listOfGameObjects = new List<GameObject>();

    private GameObject GetPlayer => (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

    private Color GetPlayerColor
    {
        get { return (GetPlayer.GetComponent<SpriteRenderer>().color); }
    }

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

    public void StartGamePlay(int BoardSize)
    {
        boardSize = BoardSize;

        wall = new Wall[boardSize - 1, boardSize - 1];
        pegBoard = new Peg[boardSize, boardSize];

        DrawGameBoard();

        gameState = GameState.PLAYER1;
        selectionState = SelectionState.ANCHOR;
    }

    public void StopGamePlay()
    {
        foreach (GameObject go in listOfGameObjects)
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
        string squares = "";

        saveLoadData.boardSize = boardSize;

        // Save the squares that have been set to black or white
        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                squares += wall[col, row].State;
            }
        }

        saveLoadData.squares = squares;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                //pegBoard[col, row].East;
            }
        }
    }

    public void LoadGameData(SaveLoadData saveLoadData)
    {
        boardSize = saveLoadData.boardSize;

        StartGamePlay(boardSize);

        string squares = saveLoadData.squares;
        int squareIndex = 0;

        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                string squareColor = squares.Substring(squareIndex, 1);

                if (squareColor != "E")
                {
                    Vector3 position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);

                    GameObject square = (squareColor == "B") ? squareBlackPreFab : squareWhitePreFab;

                    GameObject go = Instantiate(square, position, Quaternion.identity);

                    listOfGameObjects.Add(go);
                }

                squareIndex++;

                //UpdateScore(gameState);


                //wall[col, row].SetState(gameState);
            }
        }
    }

    private void MakeMove()
    {
        switch (selectionState)
        {
            case SelectionState.ANCHOR:
                selectionState = SelectAnchorPeg(GetPlayerColor);
                break;
            case SelectionState.PIN:
                selectionState = SelectPinPeg(GetPlayerColor);

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

    private SelectionState SelectAnchorPeg(Color playerColor)
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
                    peg.Selected();

                    pegStart = peg;

                    selection = SelectionState.PIN;

                    lineRenderer.startColor = playerColor;
                    lineRenderer.endColor = playerColor;
                }
            }
        }

        return (selection);
    }

    private SelectionState SelectPinPeg(Color playerColor)
    {
        SelectionState state = SelectionState.PIN;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (leftMouseButton)
        {
            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (peg.IsOpen() && LegalMove(pegStart, peg))
                {
                    lineRenderer.SetPosition(0, pegStart.GetPosition);
                    lineRenderer.SetPosition(1, pegStart.GetPosition);

                    CreateAndRenderLink(pegStart, peg, playerColor);
                    LinkPegs(pegStart, peg);
                    UpDateSquareSideCount(pegStart, peg);

                    pegStart.Reset();
                    peg.Reset();

                    state = SelectionState.ANCHOR;

                    //AudioManager.Instance.SoundLegalMove();
                }
            }
            else
            {
                lineRenderer.SetPosition(0, pegStart.GetPosition);
                lineRenderer.SetPosition(1, pegStart.GetPosition);

                pegStart.Reset();

                state = SelectionState.CANCEL;
            }
        }
        else
        {
            lineRenderer.SetPosition(0, pegStart.GetPosition);
            lineRenderer.SetPosition(1, ray.GetPoint(0.0f));

            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (!LegalMove(pegStart, peg))
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

    private void UpDateSquareSideCount(Peg pegStart, Peg pegEnd)
    {
        if (pegStart.Y == pegEnd.Y)
        {
            int col = Mathf.Min((int)pegStart.X, (int)pegEnd.X);
            int row = (int)pegStart.Y;

            AddOneToBoxSideCount(col, row);
            AddOneToBoxSideCount(col, row - 1);
        }

        if (pegStart.X == pegEnd.X)
        {
            int col = (int)pegStart.X;
            int row = Mathf.Min((int)pegStart.Y, (int)pegEnd.Y);

            AddOneToBoxSideCount(col, row);
            AddOneToBoxSideCount(col - 1, row);
        }
    }

    private void AddOneToBoxSideCount(int col, int row)
    {
        if ((col >= 0) && (row >= 0) && (col < boardSize - 1) && (row < boardSize - 1))
        {
            if (wall[col, row].Add())
            {
                Vector3 position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);

                GameObject square = (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

                GameObject go = Instantiate(square, position, Quaternion.identity);

                UpdateScore(gameState);

                listOfGameObjects.Add(go);

                wall[col, row].SetState(gameState);

                //AudioManager.Instance.SoundCompleteBox();
            }
        }

    }

    private void UpdateScore(GameState gameState)
    {
        gamePlayPanel.GetComponent<GamePlayPanel>().UpdateScore(gameState);
    }

    private bool LegalMove(Peg pegStart, Peg pegEnd)
    {
        float c1 = pegStart.X;
        float r1 = pegStart.Y;

        float c2 = pegEnd.X;
        float r2 = pegEnd.Y;

        bool rowMove = (r1 == r2) && (Mathf.Abs(c1 - c2) == 1);
        bool colMove = (c1 == c2) && (Mathf.Abs(r1 - r2) == 1);

        bool duplicate = false;

        if (rowMove)
        {
            if (c1 < c2)
            {
                duplicate = (pegStart.East && pegEnd.West);
            }
            else
            {
                duplicate = (pegStart.West && pegEnd.East);
            }
        }

        if (colMove)
        {
            if (r1 < r2)
            {
                duplicate = (pegStart.North && pegEnd.South);
            }
            else
            {
                duplicate = (pegStart.South && pegEnd.North);
            }
        }

        bool legalMove = rowMove || colMove;

        return (legalMove && !duplicate);
    }

    /// <summary>
    /// LinkPegs() - 
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    private void LinkPegs(Peg p1, Peg p2)
    {
        float c1 = p1.X;
        float r1 = p1.Y;

        float c2 = p2.X;
        float r2 = p2.Y;

        if (r1 == r2)
        {
            if (c1 < c2)
            {
                p1.East = true;
                p2.West = true;
            }
            else
            {
                p2.East = true;
                p1.West = true;
            }
        }

        if (c1 == c2)
        {
            if (r1 < r2)
            {
                p1.North = true;
                p2.South = true;
            }
            else
            {
                p2.North = true;
                p1.South = true;
            }
        }
    }

    private void TogglePlayer()
    {
        gameState = (gameState == GameState.PLAYER1) ? GameState.PLAYER2 : GameState.PLAYER1;
    }

    public void EndGamePlay()
    {
        foreach (GameObject go in listOfGameObjects)
        {
            Destroy(go);
        }
    }

    private void CreateAndRenderLink(Peg pegStart, Peg pegEnd, Color playerColor)
    {
        GameObject go = new GameObject();
        LineRenderer link = go.AddComponent<LineRenderer>();
        link.material = drawLineMaterial;
        link.startWidth = PEG_LINK_WIDTH;
        link.endWidth = PEG_LINK_WIDTH;
        link.useWorldSpace = true;
        link.positionCount = 2;
        link.SetPosition(0, pegStart.GetPosition);
        link.SetPosition(1, pegEnd.GetPosition);
        link.startColor = playerColor;
        link.endColor = playerColor;
        link.transform.parent = transform;

        listOfGameObjects.Add(go);
    }

    private void DrawGameBoard()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                CreateAndRenderPeg(col, row);
            }
        }

        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                wall[col, row] = new Wall();
            }
        }

        Vector3 oldCameraPosition = cameraPos.transform.position;

        float position = (boardSize - 1.0f) / 2.0f;
        cameraPos.transform.position = new Vector3(position, position, -10.0f);

        Vector3 delta = cameraPos.transform.position;

        backGround1.transform.position = new Vector3(delta.x, delta.y, 0.0f);
        backGround2.transform.position = new Vector3(delta.x, delta.y + 9.96f, 0.0f);
    }

    /// <summary>
    /// CreateAndRenderPeg() - Create and renders a peg at the specified col
    /// and column position.  The parent of the Peg is set as the transform
    /// object.  The maximum number of links is determined by the col and row
    /// position.  The peg is then placed in the list of objects to be deleted
    /// at the end of the game.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    private void CreateAndRenderPeg(int col, int row)
    {
        GameObject go = Instantiate(pegPreFab, new Vector3(col, row, 0.0f), Quaternion.identity);
        go.transform.parent = transform;

        Peg peg = go.GetComponent<Peg>();
        peg.SetMaxLinks(col, row, boardSize);
        pegBoard[col, row] = peg;

        go.name = $"Peg: {col}, {row}, {peg.MaxLinks}";

        listOfGameObjects.Add(go);
    }
}

public class Wall
{
    public string State { get; private set; }

    private int count;

    public Wall()
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
        return (++count == 4);
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
