using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Vector2Int pos = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        Gen(cellPrefabs, pos);

        //grid[Random.Range(0, size), Random.Range(0, size)] = Instantiate(cellPrefabs[Random.Range(0, cellPrefabs.Count)]);
        //grid[0, 0] = Instantiate(cellPrefabs[0]);
    }

    void Update()
    {
        drawRoomConnection();
    }

    public int[] getCellDir(GameObject obj)
    {
        return new int[] { (int)obj.transform.position.x / 22, (int)obj.transform.position.y / 24 };
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
            if (grid[tPos.x, tPos.y].GetComponent<Cell>().RIGHT != tryGenCell.LEFT)
            {
                return false;
            }
        }


        //Check DOWN-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x, tryGenCellPos.y - 1);
        if (tPos.y >= 0 && grid[tPos.x, tPos.y] != null)
        {
            if (grid[tPos.x, tPos.y].GetComponent<Cell>().UP != tryGenCell.DOWN)
            {
                return false;
            }
        }

        //Check RIGHT-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x + 1, tryGenCellPos.y);
        if (tPos.x < size && grid[tPos.x, tPos.y] != null)
        {
            if (grid[tPos.x, tPos.y].GetComponent<Cell>().LEFT != tryGenCell.RIGHT)
            {
                return false;
            }
        }

        //Check UP-SIDED Cell
        tPos = new Vector2Int(tryGenCellPos.x, tryGenCellPos.y + 1);
        if (tPos.y < size && grid[tPos.x, tPos.y] != null)
        {
            if (grid[tPos.x, tPos.y].GetComponent<Cell>().DOWN != tryGenCell.UP)
            {
                return false;
            }
        }

        return true;
    }

    private void Gen(List<GameObject> ableCells, Vector2Int pos)
    {
        GameObject pick = ableCells[Random.Range(0, ableCells.Count)];
        grid[pos.x, pos.y] = pick;
        Cell cell = pick.GetComponent<Cell>();

        print($"Picked : {pick.name} || X : {pos.x} / Y : {pos.y}");

        foreach (GameObject obj in ableCells)
        {
            //print($"{obj.name}");
        }

        Instantiate(pick, new Vector2(22 * pos.x, 24 * pos.y), Quaternion.identity);
    }

    public void genCell(int xNow, int yNow, CellDirection dir)
    {
        float t = Time.deltaTime;
        //print($"START TIME : {t}");
        switch (dir)
        {
            case CellDirection.UP:
                if (yNow + 1 < size && grid[xNow, yNow + 1] == null)
                {
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
                                if (yNow + 2 == size) {
                                    if (!c.UP) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        Gen(ableCells, new Vector2Int(xNow, yNow + 1));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.DOWN:
                if (yNow - 1 >= 0 && grid[xNow, yNow - 1] == null)
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
                                if (yNow - 1 == 0) {
                                    if (!c.DOWN) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        Gen(ableCells, new Vector2Int(xNow, yNow - 1));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.LEFT:
                if (xNow - 1 >= 0 && grid[xNow - 1, yNow] == null)
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
                                if (xNow - 1 == 0) {
                                    if (!c.LEFT) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        Gen(ableCells, new Vector2Int(xNow - 1, yNow));
                    }

                    else
                    {
                        print("FUCKYOU");
                    }
                }
                break;
            case CellDirection.RIGHT:
                if (xNow + 1 < size && grid[xNow + 1, yNow] == null)
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
                                if (xNow + 2 == size) {
                                    if (!c.RIGHT) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        Gen(ableCells, new Vector2Int(xNow + 1, yNow));
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

    public void checkRoomDone() { //0 : done | 1 : left 1 room | 2+ : not done
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                if (grid[x, y] != null) {
                    GameObject obj = grid[x, y];
                    Cell cell = obj.GetComponent<Cell>();
                }
            }
        }
    }

    public void drawRoomConnection()
    {
        //추후에 고치기
        //List<Dictionary<Vector2Int, Vector2Int>> roomConnections = new List<Dictionary<Vector2Int, Vector2Int>>();\
        Dictionary<Vector2Int, List<Vector2Int>> connections = new Dictionary<Vector2Int, List<Vector2Int>>();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] == null) continue;
                Cell cell = grid[x, y].GetComponent<Cell>();
                Vector2Int pos = new Vector2Int(x, y);

                Vector2Int tPos;

                //Check LEFT-SIDED Cell
                tPos = new Vector2Int(x - 1, y);
                if (tPos.x >= 0 && grid[tPos.x, tPos.y] != null)
                {
                    if (grid[tPos.x, tPos.y].GetComponent<Cell>().RIGHT == cell.LEFT && cell.LEFT)
                    {
                        if (connections.ContainsKey(pos)) {
                            connections[pos].Add(tPos);
                        } else {
                            connections.Add(pos, new List<Vector2Int>(){tPos});
                        }
                    }
                }


                //Check DOWN-SIDED Cell
                tPos = new Vector2Int(x, y - 1);
                if (tPos.y >= 0 && grid[tPos.x, tPos.y] != null)
                {
                    if (grid[tPos.x, tPos.y].GetComponent<Cell>().UP == cell.DOWN && cell.DOWN)
                    {
                        if (connections.ContainsKey(pos)) {
                            connections[pos].Add(tPos);
                        } else {
                            connections.Add(pos, new List<Vector2Int>(){tPos});
                        }
                    }
                }

                //Check RIGHT-SIDED Cell
                tPos = new Vector2Int(x + 1, y);
                if (tPos.x < size && grid[tPos.x, tPos.y] != null)
                {
                    if (grid[tPos.x, tPos.y].GetComponent<Cell>().LEFT == cell.RIGHT && cell.RIGHT)
                    {
                        if (connections.ContainsKey(pos)) {
                            connections[pos].Add(tPos);
                        } else {
                            connections.Add(pos, new List<Vector2Int>(){tPos});
                        }
                    }
                }

                //Check UP-SIDED Cell
                tPos = new Vector2Int(x, y + 1);
                if (tPos.y < size && grid[tPos.x, tPos.y] != null)
                {
                    if (grid[tPos.x, tPos.y].GetComponent<Cell>().DOWN == cell.UP && cell.UP)
                    {
                        if (connections.ContainsKey(pos)) {
                            connections[pos].Add(tPos);
                        } else {
                            connections.Add(pos, new List<Vector2Int>(){tPos});
                        }
                    }
                }
            }
        }

        foreach (Vector2Int k in connections.Keys) {
            foreach (Vector2Int vv in connections[k]) {
                //print($"K:{k} || vv:{vv}");
                //22 * pos.x, 24 * pos.y
                int x = 22;
                int y = 24;
                Debug.DrawLine(new Vector3(x * k.x + (x / 2), y * k.y + (y / 2), -3), new Vector3(x * vv.x + (x / 2), y * vv.y + (y / 2), -3), Color.red, 0.3f);
            }
        }
    }

    public bool isRoomEnd()
    {
        return false;
    }
}
