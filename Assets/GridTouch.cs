using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;

public class GridTouch : MonoBehaviour, IPointerClickHandler {
    private Tilemap map;
    public Tilemap backgroundMap;
    public Tile blankTile;
    public Tile impossibleTile;
    public Tile selectedTile;
    public Tile possibleTile;
    public Tile automaticTile;

    public Sprite astronaut;
    public Sprite water;
    public Sprite sun;
    public Sprite research;
    public Sprite death;
    public Sprite spaceBase;

    public Texture2D topBorder;
    private TerrainTileTextureCreator markedTileCreator;

    public Sprite markedSprite;
    public Gradient markedColors;
    public CardSelector cardSelector;
    public GameEvent gameOverEvent;

    public BoolVariable hasSelection;
    public BoolVariable possibleToPlace;
    public IntVariable score;
    public IntVariable waterVariable;
    public IntVariable astronautsVariable;
    public IntVariable researchVariable;
    public BoolVariable showTutorial;

    public GameObject preventInteraction;

    private float colorOffset;
    private ShapeCard targetCard;
    private Shape targetShape;
    private BoundsInt mapBounds;

    private HashSet<Vector3Int> selectedPositions = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> possiblePositions = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> requiredPositions = new HashSet<Vector3Int>();

    void Start() {
        cardSelector.Init();

        this.map = this.GetComponent<Tilemap>();

        // mapBounds = new BoundsInt(-3, -3, 0, 7, 7, 0); // 7x7
        mapBounds = new BoundsInt(-4, -4, 0, 9, 9, 0); // 9x9

        for (int y = mapBounds.yMin; y < mapBounds.yMax; y++) {
            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++) {
                map.SetTile(new Vector3Int(x, y, 0), blankTile);
            }
        }
        Tile baseTile = new MarkedTile {
            sprite = spaceBase
        };
        Vector3Int basePos = showTutorial.Value
                         ? new Vector3Int(mapBounds.xMin, mapBounds.yMin, 0)
                         : new Vector3Int(Random.Range(mapBounds.xMin, mapBounds.xMax), Random.Range(mapBounds.yMin, mapBounds.yMax), 0);
        map.SetTile(basePos, baseTile);
        AddAdjecentPositionsToRequired(basePos);

        markedTileCreator = TerrainTileTextureCreator.Init(topBorder);

        score.Value = 0;
        colorOffset = Random.value;

        TurnCleanup();
    }

    private void AddAdjecentPositionsToRequired(Vector3Int pos) {
        requiredPositions.Add(pos + Vector3Int.up);
        requiredPositions.Add(pos + Vector3Int.down);
        requiredPositions.Add(pos + Vector3Int.left);
        requiredPositions.Add(pos + Vector3Int.right);
    }

    public void SetTargetShape(ShapeCard card) {
        targetCard = card;
        targetShape = card.shape;
        ClearSelection();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int targetPos = map.WorldToCell(worldPos);
        TileBase selected = map.GetTile(targetPos);
        if (selected == selectedTile) {
            ClearPossibleTiles();
            map.SetTile(targetPos, blankTile);
            selectedPositions.Remove(targetPos);
            UpdatePossibleTiles();
        } else if (selected == possibleTile || selected == automaticTile) {
            ClearPossibleTiles();
            map.SetTile(targetPos, selectedTile);
            selectedPositions.Add(targetPos);
            UpdatePossibleTiles();
        }
    }

    private void UpdatePossibleTiles() {
        for (int y = mapBounds.yMin; y < mapBounds.yMax; y++) {
            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (CanBeSelected(map.GetTile(pos))) {
                    UpdateIfShapeFits(pos, targetShape.right);
                    UpdateIfShapeFits(pos, targetShape.left);
                    UpdateIfShapeFits(pos, targetShape.down);
                    UpdateIfShapeFits(pos, targetShape.up);
                }
            }
        }
        hasSelection.Value = selectedPositions.Count > 0;
        if (possiblePositions.Count + selectedPositions.Count == targetShape.right.Count) {
            foreach (Vector3Int pos in possiblePositions) {
                map.SetTile(pos, automaticTile);
            }
            possibleToPlace.Value = true;
        } else {
            possibleToPlace.Value = false;
        }
    }

    private void UpdateIfShapeFits(Vector3Int pos, ICollection<Vector3Int> positions) {
        if (DoesShapeFit(pos, positions) && ShapeContainsRequiredPosition(pos, positions)) {
            foreach (Vector3Int offset in positions) {
                if (map.GetTile(pos + offset) == blankTile) {
                    map.SetTile(pos + offset, possibleTile);
                    possiblePositions.Add(pos + offset);
                }
            }
        }
    }

    private bool ShapeContainsRequiredPosition(Vector3Int pos, ICollection<Vector3Int> positions) {
        foreach (Vector3Int offset in positions) {
            if (requiredPositions.Contains(pos + offset)) {
                return true;
            }
        }
        return false;

    }

    private void ClearPossibleTiles() {
        foreach (Vector3Int pos in possiblePositions) {
            map.SetTile(pos, blankTile);
        }
        possiblePositions.Clear();
    }

    private bool CanBeSelected(TileBase t) {
        if (t == null || t is MarkedTile || t is TerrainTile) {
            return false;
        }
        return true;
    }

    private bool DoesShapeFit(Vector3Int pos, ICollection<Vector3Int> positions) {
        int selectMatches = 0;
        foreach (Vector3Int offset in positions) {
            TileBase tile = map.GetTile(pos + offset);
            if (!CanBeSelected(tile)) {
                return false;
            }
            if (tile == selectedTile) {
                selectMatches++;
            }
        }
        return selectMatches == selectedPositions.Count;
    }

    public void ClearSelection() {
        foreach (Vector3Int pos in selectedPositions) {
            map.SetTile(pos, blankTile);
        }
        selectedPositions.Clear();
        ClearPossibleTiles();
        UpdatePossibleTiles();
    }

    public void PlaceTiles() {
        if (possiblePositions.Count + selectedPositions.Count == targetShape.right.Count) {
            StartCoroutine(PlaceTilesAnimation());
        }
        // TODO: show error?
    }

    private IEnumerator PlaceTilesAnimation() {
        preventInteraction.SetActive(true);
        Vector3 worldPos = new Vector3();
        foreach (Vector3Int v in selectedPositions) {
            worldPos += map.GetCellCenterWorld(v);
        }
        foreach (Vector3Int v in possiblePositions) {
            worldPos += map.GetCellCenterWorld(v);
        }
        worldPos = worldPos / (selectedPositions.Count + possiblePositions.Count);
        List<Vector3Int> all = new List<Vector3Int>(selectedPositions);
        all.AddRange(possiblePositions);
        yield return cardSelector.PlaceCard(targetCard, worldPos, targetShape.GetRotation(all, Random.value < 0.5));
        PlaceTargetShape();
        preventInteraction.SetActive(false);
    }

    private void PlaceTargetShape() {
        TileBase backgroundTile = markedTileCreator.Create(markedColors.Evaluate(colorOffset - Mathf.Floor(colorOffset)));

        foreach (Vector3Int pos in selectedPositions) {
            map.SetTile(pos, CreateTile());
            backgroundMap.SetTile(pos, backgroundTile);
            AddAdjecentPositionsToRequired(pos);
        }
        foreach (Vector3Int pos in possiblePositions) {
            map.SetTile(pos, CreateTile());
            backgroundMap.SetTile(pos, backgroundTile);
            AddAdjecentPositionsToRequired(pos);
        }
        map.RefreshAllTiles();
        TurnCleanup();
    }

    private Tile CreateTile() {
        if (targetShape.sprite == water) {
            return new MarkedTile {
                sprite = targetShape.sprite,
                color = Color.blue,
                requirement = sun
            };
        } else if (targetShape.sprite == research) {
            return new MarkedTile {
                sprite = targetShape.sprite,
                color = Color.magenta,
                requirement = astronaut
            };
        }
        return new MarkedTile {
            sprite = targetShape.sprite
        };
    }

    private void TurnCleanup() {
        ScoreTurn();

        colorOffset += 0.07f + Random.value / 6f;

        selectedPositions.Clear();
        possiblePositions.Clear();

        for (int i = 0; i < cardSelector.VisibleCards(); i++) {
            targetCard = cardSelector.SelectCard(i);
            targetShape = targetCard.shape;

            possibleToPlace.Value = true;
            hasSelection.Value = true;
            UpdatePossibleTiles();
            if (possiblePositions.Count > 0) {
                break;
            } else {
                selectedPositions.Clear();
                possiblePositions.Clear();
            }
        }
        if (possiblePositions.Count == 0) {
            gameOverEvent.Raise();
        }
    }

    private void ScoreTurn() {
        int astronautCount = 0;
        int waterCount = 0;
        int sunCount = 0;
        int researchCount = 0;

        for (int y = mapBounds.yMin; y < mapBounds.yMax; y++) {
            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++) {
                TileBase tb = map.GetTile(new Vector3Int(x, y, 0));
                if (tb is MarkedTile t) {
                    if (t.sprite == astronaut) astronautCount++;
                    else if (t.sprite == water && t.IsRequirementMet()) waterCount++;
                    else if (t.sprite == sun) sunCount++;
                }
            }
        }
        int deaths = waterCount < astronautCount ? astronautCount - waterCount : 0;
        Debug.Log("waterCount =" + waterCount + " astronautCount=" + astronautCount + " deaths=" + deaths);
        if (deaths > 0) {
            List<Vector3Int> positions = new List<Vector3Int>();
            for (int y = mapBounds.yMin; y < mapBounds.yMax; y++) {
                for (int x = mapBounds.xMin; x < mapBounds.xMax; x++) {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Tile t = (UnityEngine.Tilemaps.Tile)map.GetTile(pos);
                    if (t.sprite == astronaut) positions.Add(pos);
                }
            }
            List<Vector3Int> toDie = RandomUtil.Shuffle(positions).GetRange(0, deaths);
            // TODO: animate
            foreach (Vector3Int pos in toDie) {
                Tile tile = new MarkedTile {
                    sprite = death
                };
                map.SetTile(pos, tile);
            }
            map.RefreshAllTiles();
        }
        astronautCount -= deaths;
        for (int y = mapBounds.yMin; y < mapBounds.yMax; y++) {
            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++) {
                TileBase tb = map.GetTile(new Vector3Int(x, y, 0));
                if (tb is MarkedTile t) {
                    if (t.sprite == research && t.IsRequirementMet()) researchCount++;
                }
            }
        }
        waterVariable.Value = waterCount;
        astronautsVariable.Value = astronautCount;
        researchVariable.Value = researchCount;
        score.Value += researchCount;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public class MarkedTile : Tile {
    public Sprite requirement;
    private bool requirementMet;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        bool HasRequirement (Vector3Int offset) {
            Tile t = tilemap.GetTile(position + offset) as Tile;
            return t != null && t.sprite == requirement;
        }
        base.GetTileData(position, tilemap, ref tileData);
        if (requirement != null && !HasRequirement(Vector3Int.up) && !HasRequirement(Vector3Int.down) && !HasRequirement(Vector3Int.left) && !HasRequirement(Vector3Int.right)) {
            tileData.color = Color.grey;
            requirementMet = false;
        } else {
            requirementMet = true;
        }
    }

    public bool IsRequirementMet() {
        return requirementMet;
    }
}