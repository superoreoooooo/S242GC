using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject {
    public int SpawnX;
    public int SpawnY;

    public int RoomSizeX;
    public int RoomSizeY;

    public int grid_size;

    public GameObject SpawnRoomPrefab;

    public int playerHealth;
}
