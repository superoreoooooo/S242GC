using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TilemapShadowCaster : MonoBehaviour
{
    /*
    *
    *  !!!this script is fully deprecated.
    *  원래 사용 목적 : 반복 작업 (노가다) 최소화
    *
    */

    public Tilemap tilemap;
    public TileBase[] shadowTiles; // �׸��ڸ� ������ Ÿ�ϵ�

    void Start()
    {
        AddShadowCasters();
    }

    void AddShadowCasters()
    {
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);

            if (tile != null && System.Array.Exists(shadowTiles, t => t == tile))
            {
                Vector3 worldPosition = tilemap.GetCellCenterWorld(position);
                GameObject shadowCasterObject = new GameObject("ShadowCaster");
                shadowCasterObject.transform.position = worldPosition;
                shadowCasterObject.transform.SetParent(tilemap.transform);

                ShadowCaster2D shadowCaster = shadowCasterObject.AddComponent<ShadowCaster2D>();
                shadowCaster.useRendererSilhouette = true;
                shadowCaster.castsShadows = true;
                shadowCaster.selfShadows = true;
            }
        }
    }
}