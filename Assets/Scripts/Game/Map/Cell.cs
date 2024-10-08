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

    public GameObject DOOR_UP;
    public GameObject DOOR_DOWN;
    public GameObject DOOR_LEFT;
    public GameObject DOOR_RIGHT;

    public int PosX = -1;
    public int PosY = -1;
}
