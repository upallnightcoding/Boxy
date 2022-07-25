using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    public Sprite pegOpen;
    public Sprite pegSelected;
    public Sprite pegClosed;
    public Sprite pegIllegal;


    public PegLink North;
    public PegLink South;
    public PegLink West;
    public PegLink East;

    public Vector3 GetPosition => gameObject.transform.position;
    public int X => (int) gameObject.transform.position.x;
    public int Y => (int) gameObject.transform.position.y;

    public bool IsNorthLinked => North.IsLinked;
    public bool IsSouthLinked => South.IsLinked;
    public bool IsWestLinked => West.IsLinked;
    public bool IsEastLinked => East.IsLinked;

    public string GetColorNorth => North.GetColor;
    public string GetColorSouth => South.GetColor;
    public string GetColorWest => West.GetColor;
    public string GetColorEast => East.GetColor;

    public void SetNorth(GameState player) { North.Set(player); }
    public void SetSouth(GameState player) { South.Set(player); }
    public void SetEast(GameState player) { East.Set(player); }
    public void SetWest(GameState player) { West.Set(player); }

    private SpriteRenderer spriteRenderer;

    private int MaxLinks = 0 ;

    private bool selected;

    public void Initialize(int col, int row, int boardSize)
    {
        selected = false;

        North = new PegLink();
        South = new PegLink();
        East = new PegLink();
        West = new PegLink();

        spriteRenderer = GetComponent<SpriteRenderer>();

        MaxLinks = 4;

        if (row == 0 || row == (boardSize - 1))
        {
            MaxLinks = ((col == 0) || (col == boardSize - 1)) ? 2 : 3;
        }

        if (col == 0 || col == (boardSize - 1))
        {
            MaxLinks = ((row == 0) || (row == boardSize - 1)) ? 2 : 3;
        }
    }

    public void Reset()
    {
        spriteRenderer.sprite = IsOpen() ? pegOpen : pegClosed;
        selected = false;
    }

    public void Illegal()
    {
        if (!selected)
        {
            spriteRenderer.sprite = pegIllegal;
        }
    }

    public void Selected()
    {
        spriteRenderer.sprite = pegSelected;
        selected = true;
    }

    public bool IsOpen()
    {
        int count = 0;

        if (North.IsLinked) count++;
        if (South.IsLinked) count++;
        if (East.IsLinked) count++;
        if (West.IsLinked) count++;

        return (count < MaxLinks);
    }
}

public class PegLink {
    private bool linked = false;
    private string color = PlayerColor.EMPTY;

    public bool IsLinked => linked;
    public string GetColor => color;

    /// <summary>
    /// Set() - 
    /// </summary>
    /// <param name="player"></param>
    public void Set(GameState player)
    {
        linked = true;

        color = player switch
        {
            GameState.PLAYER1 => PlayerColor.BLACK,
            GameState.PLAYER2 => PlayerColor.WHITE,
            _ => PlayerColor.EMPTY,
        };
    }
}
