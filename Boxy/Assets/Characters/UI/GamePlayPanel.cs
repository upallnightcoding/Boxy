using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayPanel : MonoBehaviour
{
    // Class Constants
    private const int LEFT_MOUSE_BUTTON = 0;
    private const float PEG_LINK_WIDTH = 0.089f;
    private Color PLAYER1_COLOR = Color.white;
    private Color PLAYER2_COLOR = Color.black;

    // Inspector Fields
    [SerializeField] private Transform cameraPos;
    [SerializeField] private GameData gameData;
    [SerializeField] private Material drawLineMaterial;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject pegPreFab;
    [SerializeField] private GameObject player1PreFab;
    [SerializeField] private GameObject player2PreFab;

    // Player Names
    private string player1Name = null;
    private string player2Name = null;

    private int boardSize;

    // Initial Game State
    private GameState gameState = GameState.IDLE;
    private SelectionState selection = SelectionState.ANCHOR;

    // List of Objects Collected for Later Deletions
    private List<GameObject> listOfGameObjects = new List<GameObject>();

    // Input Control Attributes
    private bool leftMouseButton;
    private Vector3 mousePos;

    private Wall[,] wall;

    private Peg pegStart;

    void Update()
    {
        leftMouseButton = Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON);
        mousePos = Input.mousePosition;

        Debug.Log(gameState);

        switch(gameState)
        {
            case GameState.IDLE:
                break;
            case GameState.PLAYER1:
                MakeMove(PLAYER1_COLOR, player1PreFab);
                break;
            case GameState.PLAYER2:
                MakeMove(PLAYER2_COLOR, player2PreFab);
                break;
            case GameState.STOP:
                break;
        }
    }

    private void MakeMove(Color playerColor, GameObject square)
    {
        switch (selection)
        {
            case SelectionState.ANCHOR:
                selection = SelectAnchorPeg();
                break;
            case SelectionState.PIN:
                selection = SelectPinPeg(playerColor, square);

                switch(selection)
                {
                    case SelectionState.ANCHOR:
                        TogglePlayer();
                        break;
                    case SelectionState.CANCEL:
                        selection = SelectionState.ANCHOR;
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
                    peg.Selected();

                    pegStart = peg;

                    selection = SelectionState.PIN;
                }
            }
        }

        return (selection);
    }

    private SelectionState SelectPinPeg(Color playerColor, GameObject square)
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

                    CreatePegLink(pegStart, peg, playerColor);
                    LinkPegs(pegStart, peg);
                    UpDateBoxSideCount(pegStart, peg, square);

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
                    //AudioManager.Instance.SoundIllegalMove();
                }
            }
        }

        return (state);
    }

    private void UpDateBoxSideCount(Peg pegStart, Peg pegEnd, GameObject square)
    {
        if (pegStart.Y == pegEnd.Y)
        {
            int col = Mathf.Min((int)pegStart.X, (int)pegEnd.X);
            int row = (int)pegStart.Y;

            AddOneToBoxSideCount(col, row, square);
            AddOneToBoxSideCount(col, row - 1, square);
        }

        if (pegStart.X == pegEnd.X)
        {
            int col = (int)pegStart.X;
            int row = Mathf.Min((int)pegStart.Y, (int)pegEnd.Y);

            AddOneToBoxSideCount(col, row, square);
            AddOneToBoxSideCount(col - 1, row, square);
        }
    }

    private void AddOneToBoxSideCount(int col, int row, GameObject square)
    {
        if ((col >= 0) && (row >= 0) && (col < boardSize - 1) && (row < boardSize - 1))
        {
            if (wall[col, row].Add())
            {
                Vector3 position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);

                GameObject go = Instantiate(square, position, Quaternion.identity);

                listOfGameObjects.Add(go);

                //AudioManager.Instance.SoundCompleteBox();
            }
        }

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

    public void StartGamePlay()
    {
        gameObject.SetActive(true);

        player1Name = gameData.Player1Name;
        player2Name = gameData.Player2Name;
        boardSize = gameData.BoardSize;

        Debug.Log($"Player 1: {player1Name} Player 2: {player2Name}");

        DrawGameBoard();

        gameState = GameState.PLAYER1;
        selection = SelectionState.ANCHOR;
    }

    public void EndGamePlay()
    {
        foreach(GameObject go in listOfGameObjects)
        {
            Destroy(go);
        }
    }

    private void CreatePegLink(Peg pegStart, Peg pegEnd, Color playerColor)
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
                CreatePeg(col, row);
            }
        }

        float position = (boardSize - 1.0f) / 2.0f;
        cameraPos.transform.position = new Vector3(position, position, -10.0f);
    }

    private void CreatePeg(int col, int row)
    {
        GameObject go = Instantiate(pegPreFab, new Vector3(col, row, 0.0f), Quaternion.identity);
        go.transform.parent = transform;

        Peg peg = go.GetComponent<Peg>();
        peg.SetMaxLinks(col, row, boardSize);

        go.name = $"Peg: {col}, {row} {peg.MaxLinks}";

        listOfGameObjects.Add(go);
    }
}

public class Wall
{
    private int count;

    public Wall()
    {
        count = 0;
    }

    public bool Add()
    {
        return (++count == 4);
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

public enum SelectionState
{
    ANCHOR,
    PIN,
    COMPLETE,
    CANCEL
}
