using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private int CELL_SIZE_X;
    private int CELL_SIZE_Y;
    private int size;

    public GameData data;

    public GameObject[,] grid;

    public List<GameObject> cellPrefabs;

    public int roomCnt = 0;

    public GameObject nav;

    private void Awake()
    {
        size = data.grid_size;
        grid = new GameObject[size, size];
    }

    void Start()
    {
        CELL_SIZE_X = data.RoomSizeX;
        CELL_SIZE_Y = data.RoomSizeY;

        Gen(new List<GameObject>() { data.SpawnRoomPrefab }, new Vector2Int(data.SpawnX, data.SpawnY));

        //Vector2Int pos = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        //Gen(cellPrefabs, pos);
        //TODO ENEMYCNT

        //grid[Random.Range(0, size), Random.Range(0, size)] = Instantiate(cellPrefabs[Random.Range(0, cellPrefabs.Count)]);
        //grid[0, 0] = Instantiate(cellPrefabs[0]);
    }

    public GameObject getCell(Vector2 pos)
    {
        //print(grid[(int)pos.x / CELL_SIZE_X, (int)pos.y / CELL_SIZE_Y].transform.position);
        return grid[(int) pos.x / CELL_SIZE_X, (int) pos.y / CELL_SIZE_Y];
    }

    public GameObject getCell(Vector2Int cellPos)
    {
        return grid[cellPos.x, cellPos.y];
    }

    public Vector2Int getCellPos(Vector2 pos)
    {
        return new Vector2Int((int)pos.x / CELL_SIZE_X, (int)pos.y / CELL_SIZE_Y);
    }

    public void updateMapSystem()
    {
        if (roomCnt == 19)
        {
            //todo 보스방?
        }
    }

    void Update()
    {
        updateMapSystem();
        drawRoomConnection();
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

    private GameObject Gen(List<GameObject> ableCells, Vector2Int pos)
    {
        GameObject pick = ableCells[Random.Range(0, ableCells.Count)];
        print(pick.name);

        print($"Picked : {pick.name} || X : {pos.x} / Y : {pos.y}");

        GameObject c = Instantiate(pick, Vector2.zero, Quaternion.identity);
        c.GetComponent<Cell>().PosX = pos.x;
        c.GetComponent<Cell>().PosY = pos.y;
        c.transform.position = new Vector2((CELL_SIZE_X + 0.0f) * pos.x, (CELL_SIZE_Y + 0.0f) * pos.y + 8);

        grid[pos.x, pos.y] = c;

        roomCnt += 1;

        nav.GetComponent<Nav>().buildNavMesh();

        return c;
   }

    public GameObject genCell(int xNow, int yNow, CellDirection dir)
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
                            return null;
                        }
                        if (c.DOWN)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow, yNow + 1), c))
                            {
                                if (yNow + 1 == size) {
                                    if (!c.UP) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        return Gen(ableCells, new Vector2Int(xNow, yNow + 1));
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
                            return null;
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
                        return Gen(ableCells, new Vector2Int(xNow, yNow - 1));
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
                            return null;
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
                        return Gen(ableCells, new Vector2Int(xNow - 1, yNow));
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
                            return null;
                        }
                        if (c.LEFT)
                        {
                            if (checkIsAvailableCell(new Vector2Int(xNow + 1, yNow), c))
                            {
                                if (xNow + 1 == size) {
                                    if (!c.RIGHT) ableCells.Add(obj);
                                } else {
                                    ableCells.Add(obj);
                                }
                            }
                        }
                    }

                    if (ableCells.Count > 0)
                    {
                        return Gen(ableCells, new Vector2Int(xNow + 1, yNow));
                    }
                }
                break;
            default:
                return null;
        }
        print($"ETA :: {Time.deltaTime - t}");
        return null;
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
                int x = CELL_SIZE_X;
                int y = CELL_SIZE_Y;
                Debug.DrawLine(new Vector3(x * k.x, y * k.y, -3), new Vector3(x * vv.x, y * vv.y, -3), Color.red, 0.3f);
            }
        }
    }
}
