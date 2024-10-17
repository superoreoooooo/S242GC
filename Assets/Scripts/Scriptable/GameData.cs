using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject {

    /*
    *
    *  게임 데이터 담는 Scriptable
    *
    */

    public int SpawnX;
    public int SpawnY;

    public int bossRoomX;
    public int bossRoomY;

    public int RoomSizeX;
    public int RoomSizeY;

    public int grid_size;

    public GameObject SpawnRoomPrefab;
    public GameObject BossRoomPrefab;

    public int playerHealth;
}
