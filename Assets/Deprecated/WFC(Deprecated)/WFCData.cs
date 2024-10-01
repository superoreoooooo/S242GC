using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WFC Data", menuName = "Scriptable Object/WFC Data")]
public class WFCData : ScriptableObject
{
    public CellType cellType;
    public CellWall wall_UP;
    public CellWall wall_DOWN;
    public CellWall wall_LEFT;
    public CellWall wall_RIGHT;
    public GameObject SpriteObject;

    public Dictionary<CellDirection, CellWall> getCellData() {
    /**
    public Dictionary<CellDirection, List<CellWall>> getCellData()
    {
        
        List<CellWall> UP = new List<CellWall>
        {
            wall_UP_1,
            wall_UP_2,
            wall_UP_3
        };
        List<CellWall> DOWN = new List<CellWall>
        {
            wall_DOWN_1,
            wall_DOWN_2,
            wall_DOWN_3
        };
        List<CellWall> LEFT = new List<CellWall>
        {
            wall_LEFT_1,
            wall_LEFT_2,
            wall_LEFT_3
        };
        List<CellWall> RIGHT = new List<CellWall>
        {
            wall_RIGHT_1,
            wall_RIGHT_2,
            wall_RIGHT_3
        };

        Dictionary<CellDirection, List<CellWall>> data = new Dictionary<CellDirection, List<CellWall>>
        {
            {CellDirection.UP, UP},
            {CellDirection.DOWN, DOWN},
            {CellDirection.LEFT, LEFT},
            {CellDirection.RIGHT, RIGHT}
        };
 */

        Dictionary<CellDirection, CellWall> data = new Dictionary<CellDirection, CellWall>
        {
            {CellDirection.UP, wall_UP},
            {CellDirection.DOWN, wall_DOWN},
            {CellDirection.LEFT, wall_LEFT},
            {CellDirection.RIGHT, wall_RIGHT}
        };

        return data;
    }
}