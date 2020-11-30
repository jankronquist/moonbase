using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeCard : FlyingTransform {

    public GameObject prefab;
    public RectTransform target;
    public Vector2Int tileSize;
    public Shape shape;
    public Color selectedColor;
    public Color deselectedColor;
    public Image selectedImage;
    public Image type;
    public Sprite select;
    public Sprite destroy;

    private Dictionary<Vector3Int, GameObject> tiles = new Dictionary<Vector3Int, GameObject>();

    void Awake() {
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 5; y++) {
                GameObject go = Instantiate(prefab, target);
                go.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                go.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                go.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector3(x * tileSize.x, y * tileSize.y, 0);
                tiles.Add(new Vector3Int(x, y, 0), go);
            }
        }
    }

    public void ShowShape(Shape shape) {
        gameObject.SetActive(true);
        this.shape = shape;
        foreach (GameObject go in tiles.Values) {
            go.SetActive(false);
        }
        foreach (Vector3Int pos in shape.right) {
            try {
                tiles[pos-shape.rightBounds.min].SetActive(true);
                type.sprite = shape.sprite;
            } catch (KeyNotFoundException e) {
                Debug.Log("Not found: " + pos + " e=" + e);
            }

        }

        target.anchoredPosition = new Vector2((4 - (shape.rightBounds.size.x)) * tileSize.x / 2, (4 - shape.rightBounds.size.y) * tileSize.y / 2);
    }

    public void ShowSelected(bool selected) {
        if (selected) {
            selectedImage.enabled = true;
            selectedImage.sprite = select;
        } else {
            selectedImage.enabled = false;
        }
    }

    public void ShowDestroy() {
        selectedImage.enabled = true;
        selectedImage.sprite = destroy;
    }

}
