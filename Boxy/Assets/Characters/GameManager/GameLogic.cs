using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private GamePlayPanel gamePlayPanel;

    private int boardSize;

    // Board Walls
    //------------
    private SquareBox[,] wall;

    // Peg Board
    //----------
    private Peg[,] pegBoard;

    public SquareBox[,] GetWalls() => wall;
    public Peg[,] GetPegBoard() => pegBoard;
    public void SetPegBoard(int col, int row, Peg peg) => pegBoard[col, row] = peg;
    public void InitWall(int col, int row) => wall[col, row] = new SquareBox();

    public void Initialize(int boardSize)
    {
        this.boardSize = boardSize;

        wall = new SquareBox[boardSize - 1, boardSize - 1];
        pegBoard = new Peg[boardSize, boardSize];
    }

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

        string rowLinks = "";

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                rowLinks += pegBoard[col, row].GetColorEast;
            }
        }

        saveLoadData.rowLinks = rowLinks;

        string colLinks = "";

        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize - 1; row++)
            {
                colLinks += pegBoard[col, row].GetColorNorth;
            }
        }

        saveLoadData.colLinks = colLinks;
    }

    public GameMove MakeBoxyMove(int value)
    {
        GameMove gameMove = null;

        BoxPos boxPos = SelectRandonBox(value);

        if (boxPos != null)
        {
            WallDirection direction = GetRandomWall(boxPos, value);

            if (direction != WallDirection.UNKNOWN)
            {
                gameMove = CreateMove(boxPos, direction);
            }
        }

        return (gameMove);
    }

    public BoxPos SelectRandonBox(int value)
    {
        BoxPosList boxPosList = new BoxPosList();

        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                if (wall[col, row].IsOpen())
                {
                    if (value == -1)
                    {
                        boxPosList.Add(col, row);
                    }
                    else if (wall[col, row].GetCount() == value)
                    {
                        boxPosList.Add(col, row);
                    }
                }
            }
        }

        return (boxPosList.PickRandomBox());
    }

    public bool LegalMove(Peg pegStart, Peg pegEnd)
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
                duplicate = (pegStart.IsEastLinked && pegEnd.IsWestLinked);
            }
            else
            {
                duplicate = (pegStart.IsWestLinked && pegEnd.IsEastLinked);
            }
        }

        if (colMove)
        {
            if (r1 < r2)
            {
                duplicate = (pegStart.IsNorthLinked && pegEnd.IsSouthLinked);
            }
            else
            {
                duplicate = (pegStart.IsSouthLinked && pegEnd.IsNorthLinked);
            }
        }

        bool legalMove = rowMove || colMove;

        return (legalMove && !duplicate);
    }

    public void LinkPegs(Peg p1, Peg p2, GameState gameState)
    {
        float c1 = p1.X;
        float r1 = p1.Y;

        float c2 = p2.X;
        float r2 = p2.Y;

        if (r1 == r2)
        {
            if (c1 < c2)
            {
                p1.SetEast(gameState);
                p2.SetWest(gameState);
            }
            else
            {
                p2.SetEast(gameState);
                p1.SetWest(gameState);
            }
        }

        if (c1 == c2)
        {
            if (r1 < r2)
            {
                p1.SetNorth(gameState);
                p2.SetSouth(gameState);
            }
            else
            {
                p2.SetNorth(gameState);
                p1.SetSouth(gameState);
            }
        }
    }

    public void UpdateScore(int col, int row, GameState gameState)
    {
        gamePlayPanel.UpdateScore(gameState);

        wall[col, row].SetState(gameState);
    }

    public void UpdateWallCount(Peg pegStart, Peg pegEnd, GameObject square, int boardSize, GameState gameState, bool loadMode, BoxPosList boxPosList)
    {
        if (pegStart.Y == pegEnd.Y)
        {
            int col = Mathf.Min((int)pegStart.X, (int)pegEnd.X);
            int row = (int)pegStart.Y;

            AddOneToWallCount(col, row, boardSize, gameState, loadMode, boxPosList);
            AddOneToWallCount(col, row - 1, boardSize, gameState, loadMode, boxPosList);
        }

        if (pegStart.X == pegEnd.X)
        {
            int col = (int)pegStart.X;
            int row = Mathf.Min((int)pegStart.Y, (int)pegEnd.Y);

            AddOneToWallCount(col, row, boardSize, gameState, loadMode, boxPosList);
            AddOneToWallCount(col - 1, row, boardSize, gameState, loadMode, boxPosList);
        }
    }

    private bool AddOneToWallCount(int col, int row, int boardSize, GameState gameState, bool loadMode, BoxPosList boxPosList)
    {
        bool getAnotherTurn = false;

        if ((col >= 0) && (row >= 0) && (col < boardSize - 1) && (row < boardSize - 1))
        {
            if (wall[col, row].AddAndCheckBox() && !loadMode)
            {
                boxPosList.Add(col, row);

                UpdateScore(col, row, gameState);

                getAnotherTurn = true;
            }
        }

        return (getAnotherTurn);
    }

    private WallDirection GetRandomWall(BoxPos boxPos, int value)
    {
        WallDirection direction = WallDirection.UNKNOWN;
        int col = boxPos.Col;
        int row = boxPos.Row;

        List<WallDirection> directions = new List<WallDirection>();

        if (!pegBoard[col, row].North.IsLinked && (!CheckForTwo(col - 1, row) || (value == 3))) directions.Add(WallDirection.WEST);
        if (!pegBoard[col, row].East.IsLinked && (!CheckForTwo(col, row - 1) || (value == 3))) directions.Add(WallDirection.SOUTH);
        if (!pegBoard[col+1, row+1].West.IsLinked && (!CheckForTwo(col, row + 1) || (value == 3))) directions.Add(WallDirection.NORTH);
        if (!pegBoard[col+1, row+1].South.IsLinked && (!CheckForTwo(col + 1, row) || (value == 3))) directions.Add(WallDirection.EAST);

        if (directions.Count > 0)
        {
            direction = directions[UnityEngine.Random.Range(0, directions.Count)];
        }

        return (direction);
    }

    private bool CheckForTwo(int col, int row)
    {
        return (
            (col >= 0) && 
            (row >= 0) && 
            (col < boardSize-1) && 
            (row < boardSize-1) && 
            (wall[col, row].GetCount() == 2)
        );
    }

    public GameMove CreateMove(BoxPos boxPos, WallDirection direction)
    {
        GameMove gameMove = null;
        int c = boxPos.Col;
        int r = boxPos.Row;

        switch(direction)
        {
            case WallDirection.NORTH:
                gameMove = new GameMove(pegBoard[c, r + 1], pegBoard[c+1, r+1]);
                break;
            case WallDirection.SOUTH:
                gameMove = new GameMove(pegBoard[c, r], pegBoard[c + 1, r]);
                break;
            case WallDirection.EAST:
                gameMove = new GameMove(pegBoard[c + 1, r], pegBoard[c + 1, r + 1]);
                break;
            case WallDirection.WEST:
                gameMove = new GameMove(pegBoard[c, r], pegBoard[c, r + 1]);
                break;
        }

        return (gameMove);
    }
}

public class BoxPosList
{
    private List<BoxPos> listOfBoxPos;

    public bool HasCount() => (listOfBoxPos.Count > 0);

    public BoxPosList()
    {
        listOfBoxPos = new List<BoxPos>();
    }

    public void Add(int col, int row)
    {
        listOfBoxPos.Add(new BoxPos(col, row));
    }

    public void ForEachBox(Action<BoxPos> action)
    {
        listOfBoxPos.ForEach(action);
    }

    public BoxPos PickRandomBox()
    {
        BoxPos boxPos = null;

        if (HasCount())
        {
            boxPos = listOfBoxPos[UnityEngine.Random.Range(0, listOfBoxPos.Count)];
        }

        return (boxPos);
    }
}

public class BoxPos
{
    public int Col { get; }
    public int Row { get; }

    public BoxPos(int col, int row)
    {
        this.Col = col;
        this.Row = row;
    }
}

public enum WallDirection
{
    NORTH,
    SOUTH,
    EAST,
    WEST,
    UNKNOWN
}
