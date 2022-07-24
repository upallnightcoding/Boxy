using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    private const float PEG_LINK_WIDTH = 0.089f;

    [SerializeField] private GameObject ScrollingBackGround1;
    [SerializeField] private GameObject ScrollingBackGround2;
    [SerializeField] private GameObject PegPreFab;
    [SerializeField] private Material drawLineMaterial;
    [SerializeField] private GameObject squareBlackPreFab;
    [SerializeField] private GameObject squareWhitePreFab;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameLogic gameLogic;

    // Board Walls
    //------------
    //private SquareWall[,] wall;

    // Peg Board
    //----------
    //private Peg[,] pegBoard;

    private Peg pegStart;

    // Calculated new camera position over peg board
    //----------------------------------------------
    private Vector3 newCameraPosition;

    // List of Displayed Gaming Objects
    //---------------------------------
    private List<GameObject> listOfGameObjects;

    // Property Get Functions
    //-----------------------
    public Vector3 GetNewCameraPosition() => newCameraPosition;
    public List<GameObject> GetListOfGameObjects() => listOfGameObjects;
    public Peg GetStartPeg() => pegStart;
    public void AddListOfGameObjects(GameObject go) => listOfGameObjects.Add(go);

    public void DrawGameBoard(int boardSize)
    {
        listOfGameObjects = new List<GameObject>();

        CreateAndRenderPegs(boardSize);

        CreateBoardWalls(boardSize);

        PositionCameraAndBackGround(boardSize);
    }

    public void SetWallAnchorPeg(Peg peg, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        pegStart = peg;
        pegStart.Selected();
    }

    public void AnimateRenderLine(Vector3 position)
    {
        lineRenderer.SetPosition(0, pegStart.GetPosition);
        lineRenderer.SetPosition(1, position);
    }

    public void RemoveLineRenderer()
    {
        lineRenderer.SetPosition(0, pegStart.GetPosition);
        lineRenderer.SetPosition(1, pegStart.GetPosition);
    }

    public void DrawWall(Peg pegStart, Peg pegEnd, Color color)
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
        link.startColor = color;
        link.endColor = color;
        link.transform.parent = transform;

        listOfGameObjects.Add(go);
    }

    public void DrawBox(BoxPos boxPos, GameObject square, GameState gameState)
    {
        Vector3 position = new Vector3(boxPos.Col + 0.5f, boxPos.Row + 0.5f, 0.0f);

        GameObject go = Instantiate(square, position, Quaternion.identity);

        listOfGameObjects.Add(go);
    }

    public void LoadBox(int col, int row, GameObject square, GameState gameState)
    {
        Vector3 position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);

        GameObject go = Instantiate(square, position, Quaternion.identity);

        listOfGameObjects.Add(go);
    }

    private void PositionCameraAndBackGround(int boardSize)
    {
        float position = (boardSize - 1.0f) / 2.0f;
        newCameraPosition = new Vector3(position, position, -10.0f);

        Vector3 delta = newCameraPosition;

        ScrollingBackGround1.transform.position = new Vector3(delta.x, delta.y, 0.0f);
        ScrollingBackGround2.transform.position = new Vector3(delta.x, delta.y + 9.96f, 0.0f);
    }

    private void CreateBoardWalls(int boardSize)
    {
        for (int row = 0; row < boardSize - 1; row++)
        {
            for (int col = 0; col < boardSize - 1; col++)
            {
                gameLogic.InitWall(col, row);
            }
        }
    }

    private void CreateAndRenderPegs(int boardSize)
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                GameObject go = Instantiate(PegPreFab, new Vector3(col, row, 0.0f), Quaternion.identity);
                go.transform.parent = transform;  // TODO Not sure this parent is right

                Peg peg = go.GetComponent<Peg>();
                peg.Initialize();
                peg.SetMaxLinks(col, row, boardSize);

                gameLogic.SetPegBoard(col, row, peg);

                go.name = $"Peg: {col}, {row}, {peg.MaxLinks}";

                listOfGameObjects.Add(go);
            }
        }
    }

    
}
