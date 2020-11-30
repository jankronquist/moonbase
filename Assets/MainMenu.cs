using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using ScriptableObjectArchitecture;

public class MainMenu : MonoBehaviour {
    public FlyingTransform credits;
    public TextMeshProUGUI what;
    public TextMeshProUGUI who;
    public AnimationCurve curve;
    public float animationTime;
    public float pauseTime;

    public BoolVariable showTutorial;

    private Coroutine creditsCoroutine;
    private Vector3 visiblePos;

    private CreditText[] texts = new CreditText[] {
        new CreditText {
            what = "Design & Development",
            who = "Jan Kronquist"
        },
        new CreditText {
            what = "Moon image",
            who = "NASA"
        },
        new CreditText {
            what = "Sound effects",
            who = "Kenney.nl"
        },
        new CreditText {
            what = "UI Icons",
            who = "Kenney.nl"
        },
        new CreditText {
            what = "Basic tiles",
            who = "Kenney.nl"
        },
        new CreditText {
            what = "SO Architecture",
            who = "Daniel Everland"
        },
        new CreditText {
            what = "Noun Project icons",
            who = ""
        },
        new CreditText {
            what = "Noun: Water",
            who = "Academic Technologies"
        },
        new CreditText {
            what = "Noun: Flask",
            who = "Vectors Market"
        },
        new CreditText {
            what = "Noun: Skull",
            who = "Vectors Market"
        },
        new CreditText {
            what = "Noun: Sun",
            who = "mikicon"
        },
        new CreditText {
            what = "Noun: moonbase",
            who = "Akriti Bhusal"
        },
        new CreditText {
            what = "Noun: Astronaut",
            who = "Vectors Point"
        }
    };

    public void ShowTutorial(bool v) {
        showTutorial.Value = v;
        Debug.Log("showTutorial.Value=" + showTutorial.Value);
    }
   
    void Start() {
        credits.gameObject.SetActive(false);
        RectTransform creditsTransform = credits.transform as RectTransform;
        visiblePos = creditsTransform.position;
    }

    void StartGame() {
        SceneManager.LoadScene("Scenes/Game");
    }

    public void ShowCredits() {
        creditsCoroutine = StartCoroutine(ShowCreditsAnimation());
    }

    public void AbortCredits() {
        if (creditsCoroutine != null) {
            StopCoroutine(creditsCoroutine);
            EndCredits();
        }
    }

    public IEnumerator ShowCreditsAnimation() {
        RectTransform creditsTransform = credits.transform as RectTransform;
        Vector3 startPos = new Vector3(850, -150, 0);
        Vector3 endPos = new Vector3(2200, visiblePos.y, 0);
        credits.gameObject.SetActive(true);
        foreach (CreditText ct in texts) {
            what.text = ct.what;
            who.text = ct.who;
            creditsTransform.position = startPos;
            yield return credits.Animate(PathCurve.CreateWithRandomMiddle(startPos, visiblePos), curve, 0, animationTime);
            yield return new WaitForSeconds(pauseTime);
            yield return credits.Animate(PathCurve.CreateStraight(visiblePos, endPos), curve, 0, animationTime);
        }
        creditsCoroutine = null;
        EndCredits();
    }

    public void EndCredits() {
        credits.gameObject.SetActive(false);
        this.GetComponent<Animator>().Play("EndCredits");
    }

}

public struct CreditText {
    public string what;
    public string who;
}