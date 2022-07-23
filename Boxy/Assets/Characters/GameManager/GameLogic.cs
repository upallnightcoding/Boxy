using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private GamePlayPanel gamePlayPanel;

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

    public void AddOneToBoxSideCount(int col, int row, int boardSize, SquareWall[,] wall, GameState gameState, GameObject square, bool loadMode)
    {
        if ((col >= 0) && (row >= 0) && (col < boardSize - 1) && (row < boardSize - 1))
        {
            if (wall[col, row].Add() && !loadMode)
            {
                Vector3 position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);

                GameObject go = Instantiate(square, position, Quaternion.identity);
                go.name = $"Box: {col}, {row}";

                //gameRenderer.DrawBox(col, row, gameState);

                //UpdateScore(gameState);

                gamePlayPanel.UpdateScore(gameState);

                //listOfGameObjects.Add(go);

                wall[col, row].SetState(gameState);

                //AudioManager.Instance.SoundCompleteBox();
            }
        }
    }

    public void UpDateSquareSideCount(Peg pegStart, Peg pegEnd, GameObject square, int boardSize, SquareWall[,] wall, GameState gameState, bool loadMode)
    {
        //GameObject square = (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

        if (pegStart.Y == pegEnd.Y)
        {
            int col = Mathf.Min((int)pegStart.X, (int)pegEnd.X);
            int row = (int)pegStart.Y;

            AddOneToBoxSideCount(col, row, boardSize, wall, gameState, square, loadMode);
            AddOneToBoxSideCount(col, row - 1, boardSize, wall, gameState, square, loadMode);
        }

        if (pegStart.X == pegEnd.X)
        {
            int col = (int)pegStart.X;
            int row = Mathf.Min((int)pegStart.Y, (int)pegEnd.Y);

            AddOneToBoxSideCount(col, row, boardSize, wall, gameState, square, loadMode);
            AddOneToBoxSideCount(col - 1, row, boardSize, wall, gameState, square, loadMode);
        }
    }
}
