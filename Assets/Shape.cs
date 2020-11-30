using System.Collections.Generic;
using UnityEngine;

public class Shape {
    public readonly HashSet<Vector3Int> right = new HashSet<Vector3Int>();
    public readonly HashSet<Vector3Int> left = new HashSet<Vector3Int>();
    public readonly HashSet<Vector3Int> up = new HashSet<Vector3Int>();
    public readonly HashSet<Vector3Int> down = new HashSet<Vector3Int>();
    public BoundsInt rightBounds;
    public Sprite sprite;

    public Shape(List<Vector3Int> positions, Sprite sprite) {
        this.sprite = sprite;
        HashSet<Vector3Int> converted = RecenterLowerLeft(positions);
        this.rightBounds = TilemapExtras.GetBounds(converted);
        this.right = converted;
        foreach (Vector3Int pos in converted) {
            left.Add(new Vector3Int(-pos.x, -pos.y, 0));
            up.Add(new Vector3Int(-pos.y, pos.x, 0));
            down.Add(new Vector3Int(pos.y, -pos.x, 0));
        }
        left = RecenterLowerLeft(left);
        up = RecenterLowerLeft(up);
        down = RecenterLowerLeft(down);
    }

    private HashSet<Vector3Int> RecenterLowerLeft(ICollection<Vector3Int> positions) {
        Vector3Int origin = TilemapExtras.GetLowerLeft(positions);
        List<Vector3Int> converted = new List<Vector3Int>(positions).ConvertAll((p) => p - origin);
        return new HashSet<Vector3Int>(converted);
    }

    // randomize to prevent "boring" deterministic rotation
    public float GetRotation(List<Vector3Int> positions, bool randomFlip) {
        HashSet<Vector3Int> p = RecenterLowerLeft(positions);
        if (p.IsSubsetOf(right)) {
            return 0;
        } else if (p.IsSubsetOf(left)) {
            return randomFlip ? 180 : -180;
        }
        if (randomFlip) {
            if (p.IsSubsetOf(up)) {
                return 90;
            }
            return -90;
        } else {
            if (p.IsSubsetOf(down)) {
                return -90;
            }
            return 90;
        }
    }
}
