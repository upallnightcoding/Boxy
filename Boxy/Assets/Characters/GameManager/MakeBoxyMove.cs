using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBoxyMove : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    public GameMove MakeEasyMove(GameState gameState)
    {
        GameMove gameMove = null;

        BoxPos boxPos = gameLogic.SelectRandonBox(-1);

        if (boxPos != null)
        {
            WallDirection direction = gameLogic.GetRandomWall(boxPos);

            if (direction != WallDirection.UNKNOWN)
            {
                gameMove = gameLogic.CreateMove(boxPos, direction);
            }
        }

        return (gameMove);
    }

    public GameMove MakeMediumMove(GameState gameState)
    {
        GameMove gameMove = null;

        BoxPos boxPos = gameLogic.SelectRandonBox(3);

        if (boxPos != null)
        {
            WallDirection direction = gameLogic.GetRandomWall(boxPos);

            if (direction != WallDirection.UNKNOWN)
            {
                gameMove = gameLogic.CreateMove(boxPos, direction);
            }
        } else
        {
            gameMove = MakeEasyMove(gameState);
        }

        return (gameMove);
    }
}
