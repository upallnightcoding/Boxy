using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBoxyMove : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    public GameMove MakeEasyMove(GameState gameState)
    {
        GameMove gameMove = gameLogic.MakeBoxyMove(-1);

        return (gameMove);
    }

    public GameMove MakeMediumMove(GameState gameState)
    {
        GameMove gameMove = gameLogic.MakeBoxyMove(3);

        if (gameMove == null)
        {
            gameMove = MakeEasyMove(gameState);
        }

        return (gameMove);
    }

    public GameMove MakeHardMove(GameState gameState)
    {
        GameMove gameMove = gameLogic.MakeBoxyMove(3);

        if (gameMove == null)
        {
            gameMove = gameLogic.MakeBoxyMove(0);

            if (gameMove == null)
            {
                gameMove = gameLogic.MakeBoxyMove(1);

                if (gameMove == null)
                {
                    gameMove = MakeEasyMove(gameState);
                }
            }
        }

        return (gameMove);
    }
}
