using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("PATH")]
    public bool UP;
    public bool DOWN;
    public bool LEFT;
    public bool RIGHT;

    public Cell(bool up, bool down, bool left, bool right)
    {
        this.UP = up;
        this.DOWN = down;
        this.LEFT = left;
        this.RIGHT = right;
    }
}
