using System.Collections.Generic;
using UnityEngine;

public class WFCCell
{
    public CellType type;
    //private Dictionary<CellDirection, List<CellWall>> cellData;
    public Dictionary<CellDirection, CellWall> cellData;
    public GameObject spriteObject;

    public WFCCell(CellType type, Dictionary<CellDirection, CellWall> cellData, GameObject spriteObject) {
        this.type = type;
        this.cellData = cellData;
        this.spriteObject = spriteObject;
    }
}
