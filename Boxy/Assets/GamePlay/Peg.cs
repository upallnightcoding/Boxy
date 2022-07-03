using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    public Sprite pegOpen;
    public Sprite pegSelected;
    public Sprite pegClosed;
    public Sprite pegIllegal;

    public bool North { get; set; }
    public bool South { get; set; }
    public bool West { get; set; }
    public bool East { get; set; }

    public int MaxLinks { set; get; } = 0;

    private SpriteRenderer spriteRenderer;

    private bool selected;

    public Vector3 GetPosition => gameObject.transform.position;
    public float X => gameObject.transform.position.x;
    public float Y => gameObject.transform.position.y;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;

        North = false;
        South = false;
        East = false;
        West = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMaxLinks(int col, int row, int boardSize)
    {
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
        if (IsOpen())
        {
            spriteRenderer.sprite = pegOpen;
        }
        else
        {
            spriteRenderer.sprite = pegClosed;
        }
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

        if (North) count++;
        if (South) count++;
        if (East) count++;
        if (West) count++;

        return (count < MaxLinks);
    }
}
