using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlayerMove : MonoBehaviour
{
    [SerializeField] private GameRenderer gameRenderer;
    [SerializeField] private GameAudio gameAudio;
    [SerializeField] private GameLogic gameLogic;

    [SerializeField] private GameObject squareBlackPreFab;
    [SerializeField] private GameObject squareWhitePreFab;

    private GameObject GetPlayer(GameState gameState) => (gameState == GameState.PLAYER1) ? squareBlackPreFab : squareWhitePreFab;

    private Color GetPlayerColor(GameState gameState) => GetPlayer(gameState).GetComponent<SpriteRenderer>().color;

    private SelectionState selectionState = SelectionState.ANCHOR;

    private Peg illegalPeg = null;

    private Peg startPeg = null;

    public GameMove MakeMove(bool leftMouseButton, Vector3 mousePos, GameState gameState)
    {
        GameMove gameMove = null;

        switch (selectionState)
        {
            case SelectionState.ANCHOR:
                SelectAnchorPeg(leftMouseButton, mousePos, gameState);
                break;
            case SelectionState.PIN:
                gameMove = SelectPinPeg(leftMouseButton, mousePos, gameState);
                break;
        }

        return (gameMove);
    }

    private void SelectAnchorPeg(bool leftMouseButton, Vector3 mousePos, GameState gameState)
    {
        selectionState = SelectionState.ANCHOR;

        if (leftMouseButton)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                startPeg = hits.collider.GetComponent<Peg>();

                if (startPeg.IsOpen())
                {
                    selectionState = SelectionState.PIN;

                    gameRenderer.SetWallAnchorPeg(startPeg, GetPlayerColor(gameState));

                    gameAudio.PlaySelectedAnchorPeg();
                }
            }
        }
    }

    private GameMove SelectPinPeg(bool leftMouseButton, Vector3 mousePos, GameState gameState)
    {
        GameMove gameMove = null;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (leftMouseButton)
        {
            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg endPeg = hits.collider.GetComponent<Peg>();

                if (endPeg.IsOpen() && gameLogic.LegalMove(startPeg, endPeg))
                {
                    gameRenderer.RemoveLineRenderer(startPeg);

                    gameMove = new GameMove(startPeg, endPeg);

                    selectionState = SelectionState.ANCHOR;

                    gameAudio.PlaySelectedPinPeg();
                }
            }
            else
            {
                gameRenderer.RemoveLineRenderer(startPeg);

                startPeg.Reset();

                startPeg = null;

                selectionState = SelectionState.ANCHOR;
            }
        }
        else
        {
            selectionState = SelectionState.PIN;

            gameRenderer.AnimateRenderLine(startPeg, ray.GetPoint(0.0f));

            RaycastHit2D hits = Physics2D.GetRayIntersection(ray);

            if (hits.collider != null)
            {
                Peg peg = hits.collider.GetComponent<Peg>();

                if (!gameLogic.LegalMove(startPeg, peg))
                {
                    peg.Illegal();
                    illegalPeg = peg;
                }
            }
            else
            {
                illegalPeg?.Reset();
                illegalPeg = null;
            }
        }

        return (gameMove);
    }
}

public enum SelectionState
{
    ANCHOR,
    PIN
}

public class GameMove
{
    public Peg StartPeg { get; set; }
    public Peg EndPeg { get; set; }

    public GameMove(Peg startPeg, Peg endPeg)
    {
        StartPeg = startPeg;
        EndPeg = endPeg;
    }
}
