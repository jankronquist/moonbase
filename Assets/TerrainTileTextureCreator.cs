using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTileTextureCreator {

    private static Dictionary<Texture2D, TerrainTileTextureCreator> cache = new Dictionary<Texture2D, TerrainTileTextureCreator>();
    private Sprite[] sprites;

    public static TerrainTileTextureCreator Init(Texture2D topBorder) {
        if (!cache.TryGetValue(topBorder, out TerrainTileTextureCreator result)) {
            result = new TerrainTileTextureCreator(topBorder);
            cache.Add(topBorder, result);
            Debug.Log("CREATING new TerrainTileTextureCreator: " + Time.time);
        }
        return result;
    }

    private TerrainTileTextureCreator(Texture2D topBorder) 
    {
        Texture2D rightBorder = Rotate90(topBorder);
        Texture2D topRightBorder = Intersect(topBorder, rightBorder);
        Texture2D lowerRightBorder = Rotate90(topRightBorder);
        Texture2D lowerBorder = Rotate90(rightBorder);
        Texture2D lowerLeftBorder = Rotate90(lowerRightBorder);
        Texture2D leftBorder = Rotate90(lowerBorder);
        Texture2D topLeftBorder = Rotate90(lowerLeftBorder);


        Texture2D topLowerBorder = Intersect(topBorder, lowerBorder);
        Texture2D rightLeftBorder = Intersect(rightBorder, leftBorder);

        Texture2D allFour = Intersect(topLowerBorder, rightLeftBorder);

        Texture2D lowerLeftCorner = Add(lowerBorder, leftBorder);

        Texture2D lowerRightCorner = Add(lowerBorder, rightBorder);
        Texture2D topLeftCorner = Add(topBorder, leftBorder);
        Texture2D topRightCorner = Add(topBorder, rightBorder);

        Texture2D topRightBorderWithLowerLeftCorner = Intersect(topRightBorder, lowerLeftCorner);

        Texture2D topBorderWithLowerLeftCorner = Intersect(topBorder, lowerLeftCorner);

        Texture2D[] textures = {
            // Filled
            allFour,
            // Three sides
            Intersect(lowerBorder, rightLeftBorder),
            // Two Sides and One Corner
            Intersect(lowerLeftBorder, topRightCorner),
            // Two Adjacent Sides
            lowerLeftBorder,
            // Two Opposite Sides
            rightLeftBorder,
            // One Side and Two Corners
            Intersect(Intersect(leftBorder, lowerRightCorner), topRightCorner),
            // One Side and One Lower Corner
            Intersect(leftBorder, lowerRightCorner),
            // One Side and One Top Corner
            Intersect(leftBorder, topRightCorner),
            // One Side
            leftBorder,
            // Four Corners
            Intersect(Intersect(lowerLeftCorner, lowerRightCorner), Intersect(topLeftCorner, topRightCorner)),
            // Three Corners
            Intersect(Intersect(lowerLeftCorner, lowerRightCorner), topLeftCorner),
            // Two Adjacent Corners
            Intersect(topLeftCorner, lowerLeftCorner),
            // Two Opposite Corners
            Intersect(lowerRightCorner, topLeftCorner),
            // One Corner
            topLeftCorner,
            // Empty?
            Add(topBorder, lowerBorder)
        };
        List <Texture2D>  l = new List<Texture2D>();
        l.AddRange(textures);
        this.sprites = l.ConvertAll((a) => Sprite.Create(a, new Rect(0, 0, a.width, a.height), new Vector2(0.5f, 0.5f), a.width)).ToArray();
    }

    public TerrainTile Create(Color color) {
        TerrainTile tile = new TerrainTile() {
            m_Sprites = sprites,
            color = color
        };
        return tile;
    }

    public static Texture2D Rotate90(Texture2D source) {
        Texture2D result = new Texture2D(source.width, source.height);
        result.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < source.height; y++) {
            for (int x = 0; x < source.width; x++) {
                result.SetPixel(y, source.width - x - 1, source.GetPixel(x, y));
            }
        }
        result.Apply();
        return result;
    }

    public static Texture2D Intersect(Texture2D first, Texture2D second) {
        return Blend(first, second, (Color c1, Color c2) => new Color(Mathf.Min(c1.r, c2.r), Mathf.Min(c1.g, c2.g), Mathf.Min(c1.b, c2.b), Mathf.Min(c1.a, c2.a)));
    }

    public static Texture2D Add(Texture2D first, Texture2D second) {
        return Blend(first, second, (Color c1, Color c2) => new Color(Mathf.Max(c1.r, c2.r), Mathf.Max(c1.g, c2.g), Mathf.Max(c1.b, c2.b), Mathf.Max(c1.a, c2.a)));
    }

    public static Texture2D Multiply(Texture2D first, Texture2D second) {
        return Blend(first, second, (Color c1, Color c2) => new Color(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b, c1.a));
    }

    public static Texture2D Blend(Texture2D first, Texture2D second, Func<Color, Color, Color> blend) {
        Texture2D result = new Texture2D(first.width, first.height);
        result.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < first.height; y++) {
            for (int x = 0; x < first.width; x++) {
                result.SetPixel(x, y, blend(first.GetPixel(x, y), second.GetPixel(x, y)));
            }
        }
        result.Apply();
        return result;
    }

}

[Serializable]
public enum TileNeighbour {
    Empty, Filled, DontCare
}

[Serializable]
public class SmartTileAlternative {
    public TileNeighbour topLeft = TileNeighbour.DontCare;
    public TileNeighbour top = TileNeighbour.DontCare;
    public TileNeighbour topRight = TileNeighbour.DontCare;
    public TileNeighbour left= TileNeighbour.DontCare;
    public TileNeighbour right = TileNeighbour.DontCare;
    public TileNeighbour bottomLeft = TileNeighbour.DontCare;
    public TileNeighbour bottom = TileNeighbour.DontCare;
    public TileNeighbour bottomRight = TileNeighbour.DontCare;
    public Texture2D texture;
}
