using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapExtras {

    public static BoundsInt CalculateUsedBounds(Tilemap map) {
        List<Vector3Int> p = new List<Vector3Int>();
        foreach (Vector3Int pos in map.cellBounds.allPositionsWithin) {
            if (map.GetTile(pos) != null) {
                p.Add(pos);
            }
        }

        return GetBounds(p);
    }

    public static BoundsInt GetBounds(ICollection<Vector3Int> positions) {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (Vector3Int pos in positions) {
            minX = Mathf.Min(minX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxX = Mathf.Max(maxX, pos.x);
            maxY = Mathf.Max(maxY, pos.y);
        }
        return new BoundsInt(minX, minY, 0, maxX - minY, maxY - minY, 0);
    }

    public static Vector3Int GetLowerLeft(ICollection<Vector3Int> positions) {
        int minY = int.MaxValue;
        foreach (Vector3Int pos in positions) {
            minY = Mathf.Min(minY, pos.y);
        }
        int minX = int.MaxValue;
        foreach (Vector3Int pos in positions) {
            if (pos.y == minY) minX = Mathf.Min(minX, pos.x);
        }
        return new Vector3Int(minX, minY, 0);
    }


}
