using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour {
    public List<ShapeCard> cards;
    public Canvas parentCanvas;
    public float animationLength;
    public AnimationCurve curve;
    public Tilemap shapeMap;
    public Tilemap tutorialShapes;

    public AudioClip shuffleSound;
    public AudioClip drawCardSound;
    public AudioClip placeShapeSound;
    public AudioClip slideCardSound;

    public BoolVariable showTutorial;

    private int deckOffset;
    private List<Shape> shapes = new List<Shape>();
    private List<Vector3> cardPositions = new List<Vector3>();
    private int visibleCards;

    private AudioSource audioSource;

    public void Init() {
        foreach (ShapeCard c in cards) {
            c.gameObject.SetActive(true);
            cardPositions.Add(c.GetComponent<RectTransform>().position);
        }
        audioSource = this.GetComponent<AudioSource>();
        visibleCards = cards.Count;
        List<Shape> allShapes = CreateShapes(showTutorial.Value ? tutorialShapes : shapeMap);
        if (!showTutorial.Value) {
            this.shapes = RandomUtil.Shuffle(allShapes);
            this.shapes.AddRange(RandomUtil.Shuffle(allShapes));
        } else {
            this.shapes = allShapes;
        }
        NewCards();
    }

    public int VisibleCards() {
        return visibleCards;
    }

    public void HideCards() {
        foreach (ShapeCard c in cards) {
            c.gameObject.SetActive(false);
        }
    }

    public void NewCards() {
        audioSource.PlayOneShot(shuffleSound);
        List<Shape> s = shapes.GetRange(deckOffset, 3);
        ShowShapes(s);
        deckOffset += 3;

        Vector3 outsidePos = new Vector3(1500, cardPositions[0].y, 0);
        foreach (ShapeCard c in cards) {
            c.GetComponent<RectTransform>().position = outsidePos;
        }

        StartCoroutine(WithDelay(cards[0].Animate(PathCurve.CreateStraight(outsidePos, cardPositions[0]), curve, 0, animationLength), 0.5f));
        StartCoroutine(WithDelay(cards[1].Animate(PathCurve.CreateStraight(outsidePos, cardPositions[1]), curve, 0, animationLength), 1f));
        StartCoroutine(WithDelay(cards[2].Animate(PathCurve.CreateStraight(outsidePos, cardPositions[2]), curve, 0, animationLength), 1.5f));
    }

    private List<Shape> CreateShapes(Tilemap tilemap) {
        BoundsInt bounds = tilemap.cellBounds;
        List<Shape> result = new List<Shape>();
        foreach (Vector3Int pos in bounds.allPositionsWithin) {
            Tile t = (UnityEngine.Tilemaps.Tile)tilemap.GetTile(pos);
            if (t != null) {
                List<Vector3Int> positions = new List<Vector3Int>();
                CreateShapeFrom(tilemap, pos, positions);
                result.Add(new Shape(positions, t.sprite));
            }
        }
        Debug.Log("Created shapes=" + result.Count);
        return result;
    }

    private void CreateShapeFrom(Tilemap tilemap, Vector3Int pos, List<Vector3Int> positions) {
        if (tilemap.GetTile(pos) != null) {
            positions.Add(pos);
            tilemap.SetTile(pos, null);
            CreateShapeFrom(tilemap, pos + Vector3Int.up, positions);
            CreateShapeFrom(tilemap, pos + Vector3Int.down, positions);
            CreateShapeFrom(tilemap, pos + Vector3Int.left, positions);
            CreateShapeFrom(tilemap, pos + Vector3Int.right, positions);
        }
    }

    public ShapeCard SelectCard(int index) {
        SelectCard(cards[index]);
        return cards[index];
    }

    public void SelectCard(ShapeCard card) {
        foreach (ShapeCard c in cards) {
            c.ShowSelected(false);
        }
        if (card != cards[0]) {
            cards[0].ShowDestroy();
        }
        card.ShowSelected(true);
    }

    private void ShowShapes(List<Shape> shapes) {
        for (int i = 0; i < shapes.Count; i++) {
            cards[i].ShowShape(shapes[i]);
        }
    }

    private void ReplaceCard(ShapeCard targetCard, Shape shape) {
        targetCard.ShowShape(shape);
    }

    public IEnumerator PlaceCard(ShapeCard targetCard, Vector3 worldPos, float rotation) {
        RectTransform rect = targetCard.GetComponent<RectTransform>();
        PathCurve path = PathCurve.CreateWithRandomMiddle(rect.position, worldToUISpace(worldPos, rect));
        yield return targetCard.Animate(path, curve, rotation, animationLength);
        Vector3 outsidePos = new Vector3(1500, path.startPos.y, 0);
        rect.position = outsidePos;
        targetCard.ShowSelected(false);

        bool drawCard = true;
        if (deckOffset < shapes.Count) {
            ReplaceCard(targetCard, shapes[deckOffset]);
            deckOffset++;
        } else {
            targetCard.gameObject.SetActive(false);
            visibleCards--;
            drawCard = false;
        }


        int index = cards.IndexOf(targetCard);
        if (index > 0) {
            ShapeCard first = cards[0];
            first.GetComponent<RectTransform>().position = outsidePos;
            cards.RemoveAt(index);
            cards.RemoveAt(0);
            cards.Add(targetCard);
            cards.Add(first);
            if (index == 1) {
                StartCoroutine(cards[0].Animate(PathCurve.CreateStraight(cardPositions[2], cardPositions[0]), curve, 0, animationLength));
            } else {
                StartCoroutine(cards[0].Animate(PathCurve.CreateStraight(cardPositions[1], cardPositions[0]), curve, 0, animationLength));
            }
            if (drawCard) {
                StartCoroutine(WithDelay(targetCard.Animate(PathCurve.CreateStraight(outsidePos, cardPositions[1]), curve, 0, animationLength), 0.2f));
            }

            if (deckOffset < shapes.Count) {
                StartCoroutine(WithDelay(first.Animate(PathCurve.CreateWithRandomMiddle(outsidePos, cardPositions[2]), curve, 0, animationLength), 0.6f));
                ReplaceCard(first, shapes[deckOffset]);
                deckOffset++;
            } else {
                first.gameObject.SetActive(false);
                visibleCards--;
            }

        } else {
            cards.RemoveAt(index);
            for (int i = index; i < 2; i++) {
                StartCoroutine(cards[i].Animate(PathCurve.CreateStraight(cardPositions[i + 1], cardPositions[i]), curve, 0, animationLength));
            }
            cards.Add(targetCard);
            if (drawCard) {
                StartCoroutine(WithDelay(targetCard.Animate(PathCurve.CreateWithRandomMiddle(outsidePos, cardPositions[2]), curve, 0, animationLength), 0.2f));
            }
        }
        if (!drawCard && visibleCards > 0){
            audioSource.PlayOneShot(slideCardSound);
        }
    }

    private IEnumerator WithDelay(IEnumerator e, float t) {
        yield return new WaitForSeconds(t);
        audioSource.PlayOneShot(drawCardSound);
        yield return e;
    }

    public Vector3 worldToUISpace(Vector3 worldPos, RectTransform transform) {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, screenPos, parentCanvas.worldCamera, out movePos);

        //Convert the local point to world point
        return transform.TransformPoint(movePos);
    }

}

public class PathCurve {
    public Vector3 startPos;
    public Vector3 middlePos;
    public Vector3 targetPos;

    public PathCurve(Vector3 startPos, Vector3 middlePos, Vector3 targetPos) {
        this.startPos = startPos;
        this.middlePos = middlePos;
        this.targetPos = targetPos;
    }

    public static PathCurve CreateStraight(Vector3 startPos, Vector3 targetPos) {
        return new PathCurve(startPos, (startPos+targetPos) / 2, targetPos);
    }

    public static PathCurve CreateWithRandomMiddle(Vector3 startPos, Vector3 targetPos) {
        return new PathCurve(startPos, (startPos + targetPos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * Vector3.Distance(startPos, targetPos)) / 2, targetPos);
    }

    public Vector3 Lerp(float t) {
        return Vector3.Lerp(Vector3.Lerp(startPos, middlePos, t), Vector3.Lerp(middlePos, targetPos, t), t);
    }
}
