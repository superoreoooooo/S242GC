using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCCore : MonoBehaviour
{
    public List<WFCData> datas;

    [SerializeField]
    private int GRID_SIZE = 10;

    private List<List<GridCell>> grid; // grid[x][z]
    private List<WFCCell> cells;

    private class GridCell
    {
        public bool collapsed = false;
        public List<WFCCell> possibleCells = new List<WFCCell>();
    }

    private void Start()
    {
        Initialize();
        RunWFC();
        SetTiles();
    }

    private void Initialize()
    {
        // 셀 데이터 초기화
        cells = new List<WFCCell>();
        foreach (WFCData data in datas)
        {
            WFCCell cell = new WFCCell(data.cellType, data.getCellData(), data.SpriteObject);
            cells.Add(cell);
        }

        // 그리드 초기화
        grid = new List<List<GridCell>>();
        for (int x = 0; x < GRID_SIZE; x++)
        {
            List<GridCell> row = new List<GridCell>();
            for (int z = 0; z < GRID_SIZE; z++)
            {
                GridCell cell = new GridCell();
                cell.possibleCells = new List<WFCCell>(cells);
                row.Add(cell);
            }
            grid.Add(row);
        }
    }

    private void RunWFC()
    {
        while (true)
        {
            // 관찰 단계: 가장 낮은 엔트로피를 가진 셀 선택
            Vector2Int? cellPos = GetLowestEntropyCell();
            if (cellPos == null)
            {
                // 모든 셀이 붕괴됨
                break;
            }

            // 붕괴 단계: 셀의 상태를 하나로 결정
            CollapseCell(cellPos.Value);

            // 전파 단계: 주변 셀의 가능성을 업데이트
            Propagate(cellPos.Value);
        }

        Debug.Log("WFC 알고리즘 완료!");
    }

    private Vector2Int? GetLowestEntropyCell()
    {
        int minEntropy = int.MaxValue;
        List<Vector2Int> minEntropyCells = new List<Vector2Int>();

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int z = 0; z < GRID_SIZE; z++)
            {
                GridCell cell = grid[x][z];
                if (!cell.collapsed)
                {
                    int entropy = cell.possibleCells.Count;
                    if (entropy < minEntropy)
                    {
                        minEntropy = entropy;
                        minEntropyCells.Clear();
                        minEntropyCells.Add(new Vector2Int(x, z));
                    }
                    else if (entropy == minEntropy)
                    {
                        minEntropyCells.Add(new Vector2Int(x, z));
                    }
                }
            }
        }

        if (minEntropyCells.Count == 0)
        {
            return null;
        }

        // 무작위로 셀 선택
        return minEntropyCells[Random.Range(0, minEntropyCells.Count)];
    }

    private void CollapseCell(Vector2Int pos)
    {
        GridCell cell = grid[pos.x][pos.y];
        if (!cell.collapsed)
        {
            // 가능한 상태 중 하나를 무작위로 선택하여 붕괴
            WFCCell selectedCell = cell.possibleCells[Random.Range(0, cell.possibleCells.Count)];
            cell.possibleCells = new List<WFCCell> { selectedCell };
            cell.collapsed = true;
            Debug.Log($"셀 ({pos.x}, {pos.y}) 붕괴됨: {selectedCell.type}");
        }
    }

    private void Propagate(Vector2Int startPos)
    {
        Queue<Vector2Int> propagationQueue = new Queue<Vector2Int>();
        propagationQueue.Enqueue(startPos);

        while (propagationQueue.Count > 0)
        {
            Vector2Int pos = propagationQueue.Dequeue();
            GridCell cell = grid[pos.x][pos.y];

            // 주변 방향 확인
            foreach (Vector2Int dir in GetDirections())
            {
                int nx = pos.x + dir.x;
                int nz = pos.y + dir.y;

                if (IsInsideGrid(nx, nz))
                {
                    GridCell neighborCell = grid[nx][nz];
                    if (!neighborCell.collapsed)
                    {
                        bool changed = UpdatePossibleCells(cell, neighborCell, dir);
                        if (changed)
                        {
                            propagationQueue.Enqueue(new Vector2Int(nx, nz));
                        }
                    }
                }
            }
        }
    }

    private bool UpdatePossibleCells(GridCell sourceCell, GridCell targetCell, Vector2Int direction)
    {
        List<WFCCell> possibleCells = new List<WFCCell>();

        foreach (WFCCell targetPossibleCell in targetCell.possibleCells)
        {
            foreach (WFCCell sourcePossibleCell in sourceCell.possibleCells)
            {
                if (IsCompatible(sourcePossibleCell, targetPossibleCell, direction))
                {
                    possibleCells.Add(targetPossibleCell);
                    break;
                }
            }
        }

        if (possibleCells.Count < targetCell.possibleCells.Count)
        {
            targetCell.possibleCells = possibleCells;
            return true; // 변화가 생김
        }

        return false; // 변화 없음
    }

    private bool IsCompatible(WFCCell sourceCell, WFCCell targetCell, Vector2Int direction)
    {
        CellDirection dir = GetCellDirection(direction);
        CellDirection oppositeDir = GetOppositeDirection(dir);

        return sourceCell.cellData[dir] == targetCell.cellData[oppositeDir];
    }

    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>
        {
            new Vector2Int(0, 1),  // 위쪽
            new Vector2Int(0, -1), // 아래쪽
            new Vector2Int(-1, 0), // 왼쪽
            new Vector2Int(1, 0)   // 오른쪽
        };
    }

    private CellDirection GetCellDirection(Vector2Int dir)
    {
        if (dir.x == 0 && dir.y == 1) return CellDirection.UP;
        if (dir.x == 0 && dir.y == -1) return CellDirection.DOWN;
        if (dir.x == -1 && dir.y == 0) return CellDirection.LEFT;
        if (dir.x == 1 && dir.y == 0) return CellDirection.RIGHT;
        return CellDirection.VOID;
    }

    private CellDirection GetOppositeDirection(CellDirection dir)
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

    private bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && x < GRID_SIZE && z >= 0 && z < GRID_SIZE;
    }

    private void SetTiles()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int z = 0; z < GRID_SIZE; z++)
            {
                GridCell cell = grid[x][z];
                if (cell.collapsed)
                {
                    WFCCell wfcCell = cell.possibleCells[0];
                    Vector3 position = new Vector3(-2.7f + 0.6f * z, -2.7f + 0.6f * x, 0);

                    GameObject inst = Instantiate(wfcCell.spriteObject, position, wfcCell.spriteObject.transform.rotation);
                    inst.name = $"CELL ({x}/{z}) {{{wfcCell.type}}}";
                }
            }
        }
    }
}
