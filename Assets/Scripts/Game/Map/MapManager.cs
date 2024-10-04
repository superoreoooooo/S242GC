using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private int size;

    public GameObject[,] grid;

    public List<GameObject> cellPrefabs;

    private void Awake()
    {
        grid = new GameObject[size, size];
    }

    void Start()
    {
        //grid[Random.Range(0, size), Random.Range(0, size)] = Instantiate(cellPrefabs[Random.Range(0, cellPrefabs.Count)]);
        grid[0, 0] = Instantiate(cellPrefabs[0]);
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (grid[i, j] != null)
                {
                    //print($"x:{i} y:{j} {grid[i, j].GetComponent<Cell>().UP}");
                }
            }
        }
    }

    public int[] getCellDir(GameObject obj)
    {
        return new int[] {0, 0};
    }

    public CellDirection getOppositeDir(CellDirection dir)
    {
        switch (dir)
        {
            case CellDirection.UP: return CellDirection.DOWN;
            case CellDirection.DOWN: return CellDirection.UP;
            case CellDirection.LEFT: return CellDirection.RIGHT;
            case CellDirection.RIGHT: return CellDirection.LEFT;
            default: return CellDirection.VOID;
        }
    }

    public bool checkIsAvailableCell(Vector2Int tryGenCellPos, Cell tryGenCell)
    {
        Vector2Int tPos;

        //Check LEFT-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x - 1, tryGenCellPos.y);
        if (tPos.x >= 0 && grid[tPos.x, tPos.y] != null)
        {
            if (tryGenCellPos.x > tPos.x && tryGenCellPos.y == tPos.y) //check LEFT -> RIGHT
            {
                if (grid[tPos.x, tPos.y].GetComponent<Cell>().RIGHT != tryGenCell.LEFT)
                {
                    return false;
                }
            }
        }


        //Check DOWN-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x, tryGenCellPos.y - 1);
        if (tPos.y >= 0 && grid[tPos.x, tPos.y] != null)
        {
            if (tryGenCellPos.x == tPos.x && tryGenCellPos.y < tPos.y)
            {
                if (grid[tPos.x, tPos.y].GetComponent<Cell>().DOWN != tryGenCell.UP)
                {
                    return false;
                }
            }
        }

        //Check RIGHT-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x + 1, tryGenCellPos.y);
        if (tPos.x < size && grid[tPos.x, tPos.y] != null)
        {
            if (tryGenCellPos.x < tPos.x && tryGenCellPos.y == tPos.y) //check LEFT -> RIGHT
            {
                if (grid[tPos.x, tPos.y].GetComponent<Cell>().LEFT != tryGenCell.RIGHT)
                {
                    return false;
                }
            }
        }

        //Check UP-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x, tryGenCellPos.y + 1);
        if (tPos.y < size && grid[tPos.x, tPos.y] != null)
        {
            if (tryGenCellPos.x == tPos.x && tryGenCellPos.y > tPos.y)
            {
                if (grid[tPos.x, tPos.y].GetComponent<Cell>().UP != tryGenCell.DOWN)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void Gen(List<GameObject> ableCells, Vector2 pos)
    {
        GameObject pick = ableCells[Random.Range(0, ableCells.Count)];
        Instantiate(pick, new Vector2(40 * pos.x, 40 * pos.y), Quaternion.identity);
    }

    public void genCell(int xNow, int yNow, CellDirection dir)
    {
        float t = Time.deltaTime;
        print($"START TIME : {t}");
        switch (dir)
        {
            case CellDirection.UP:
                if (grid[xNow, yNow + 1] == null) {
                    List<GameObject> ableCells = new List<GameObject>();
                    foreach (GameObject obj in cellPrefabs)
                    {
                        Cell c = obj.GetComponent<Cell>();
                        if (c == null)
                        {
                            return;
                        }
                        if (c.DOWN)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow, yNow + 1), c))
                            {
                                ableCells.Add(obj);
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        foreach (GameObject obj in ableCells)
                        {
                            print($"{obj.name}");
                        }

                        Gen(ableCells, new Vector2(xNow, yNow + 1));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.DOWN:
                if (grid[xNow, yNow - 1] == null)
                {
                    List<GameObject> ableCells = new List<GameObject>();
                    foreach (GameObject obj in cellPrefabs)
                    {
                        Cell c = obj.GetComponent<Cell>();
                        if (c == null)
                        {
                            return;
                        }
                        if (c.UP)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow, yNow - 1), c))
                            {
                                ableCells.Add(obj);
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        foreach (GameObject obj in ableCells)
                        {
                            print($"{obj.name}");
                        }

                        Gen(ableCells, new Vector2(xNow, yNow - 1));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.LEFT:
                if (grid[xNow - 1, yNow] == null)
                {
                    List<GameObject> ableCells = new List<GameObject>();
                    foreach (GameObject obj in cellPrefabs)
                    {
                        Cell c = obj.GetComponent<Cell>();
                        if (c == null)
                        {
                            return;
                        }
                        if (c.RIGHT)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow - 1, yNow), c))
                            {
                                ableCells.Add(obj);
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        foreach (GameObject obj in ableCells)
                        {
                            print($"{obj.name}");
                        }

                        Gen(ableCells, new Vector2(xNow - 1, yNow));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.RIGHT:
                if (grid[xNow + 1, yNow] == null)
                {
                    List<GameObject> ableCells = new List<GameObject>();
                    foreach (GameObject obj in cellPrefabs)
                    {
                        Cell c = obj.GetComponent<Cell>();
                        if (c == null)
                        {
                            return;
                        }
                        if (c.LEFT)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow + 1, yNow), c))
                            {
                                ableCells.Add(obj);
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        foreach (GameObject obj in ableCells)
                        {
                            print($"{obj.name}");
                        }

                        Gen(ableCells, new Vector2(xNow + 1, yNow));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            default:
                print("니 뭐하니");
                break;
        }
        print($"ETA :: {Time.deltaTime - t}");
    }
}
