using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour {
    public Focus focus;
    private RectTransform focusTransform;
    public RectTransform textBox;
    public TextMeshProUGUI label;

    public RectTransform score;
    public RectTransform waterCount;
    public RectTransform card1;
    public RectTransform card2;
    public RectTransform placeTilesButton;
    public RectTransform clearSelectionButton;
    public RectTransform restartButton;
    public RectTransform middle;

    public GameObject gameOverview;
    public RectTransform okButton;
    public CardSelector cardSelector;
    public GridTouch gridTouch;

    public BoolVariable showTutorial;
    public BoolVariable hasSelection;
    public BoolVariable possibleToPlace;

    private ITutorialStep[] steps;
    private int currentStep;
    private bool done;

    public void Start() {
        focusTransform = focus.transform as RectTransform;
        TutorialStep confirmPlacementStep = new TutorialStep() {
            target = placeTilesButton,
            msg = "Confirm placement",
            condition = new BoolVariableCondition() {
                variable = possibleToPlace,
                value = false
            }
        };
        steps = new ITutorialStep[] {
            //new ActionStep() {
            //    action = () => gridTouch.StartGame()
            //},
            new DelayStep() {
                seconds = 2
            },
            new ClickTutorialStep() {
                msg = "Click to begin"
            },
            new ClickTutorialStep() {
                target = card1,
                msg = "Select sun"
            },
            new TutorialStep() {
                target = middle,
                msg = "Select tiles matching shape",
                condition = new BoolVariableCondition() {
                    variable = possibleToPlace,
                    value = true
                }
            },
            confirmPlacementStep,
            new DelayStep() {
                seconds = 1
            },
            new ClickTutorialStep() {
                target = card1,
                msg = "Select water"
            },
            new TutorialStep() {
                target = middle,
                msg = "Place next to sun",
                condition = new BoolVariableCondition() {
                    variable = possibleToPlace,
                    value = true
                }
            },
            confirmPlacementStep,
            new DelayStep() {
                seconds = 1
            },
            new ClickTutorialStep() {
                target = waterCount,
                msg = "Sun powers water generation"
            },
            new ClickTutorialStep() {
                target = card1,
                msg = "Select astronauts"
            },
            new TutorialStep() {
                target = middle,
                msg = "Place anywhere",
                condition = new BoolVariableCondition() {
                    variable = possibleToPlace,
                    value = true
                }

            },
            confirmPlacementStep,
            new DelayStep() {
                seconds = 1
            },
            new ClickTutorialStep() {
                msg = "Astronauts die without water"
            },
            new ClickTutorialStep() {
                target = card1,
                msg = "Select research"
            },
            new TutorialStep() {
                target = middle,
                msg = "Place next to astronauts",
                condition = new BoolVariableCondition() {
                    variable = possibleToPlace,
                    value = true
                }
            },
            confirmPlacementStep,
            new ClickTutorialStep() {
                msg = "Active research generate points",
                target = score
            },
            new ClickTutorialStep() {
                target = card2,
                msg = "Skip astronauts and select sun"
            },
            new ActionStep() {
                action = () => gridTouch.SetTargetShape(cardSelector.SelectCard(1))
            },
            new ClickTutorialStep() {
                msg = "The left shape will be discarded!"
            },
            new TutorialStep() {
                target = middle,
                msg = "Place next to water",
                condition = new BoolVariableCondition() {
                    variable = possibleToPlace,
                    value = true
                }
            },
            confirmPlacementStep,
            new ClickTutorialStep() {
                msg = "You always have three shapes"
            },
            new ActionStep() {
                action = () => gameOverview.SetActive(true)
            },
            new DelayStep() {
                seconds = 6.2f
            },
            new ClickTutorialStep() {
                target = okButton,
            },
            new ActionStep() {
                action = () => gameOverview.SetActive(false)
            },
            new ClickTutorialStep() {
                msg = "Good luck!"
            },
            new ActionStep() {
                action = () => showTutorial.Value = false
            }
        };

        steps[currentStep].Start(this);
    }

    void Update() {
        if (!done && steps[currentStep].IsDone()) {
            currentStep++;
            if (currentStep >= steps.Length) {
                done = true;
                focus.DisableFocus();
                textBox.gameObject.SetActive(false);
            } else {
                steps[currentStep].Start(this);
            }
        }
    }

    private void ShowText(string msg, RectTransform target) {
        label.text = msg;
        if (target != null) {
            float offset = textBox.rect.height + focusTransform.rect.height / 2;
            Debug.Log("focusTransform.anchoredPosition.y=" + focusTransform.anchoredPosition.y + " offset=" + offset);

            if (focusTransform.anchoredPosition.y > 0) {
                textBox.anchoredPosition = new Vector2(0, focusTransform.anchoredPosition.y - offset);
            } else {
                textBox.anchoredPosition = new Vector2(0, focusTransform.anchoredPosition.y + offset);
            }
        } else {
            textBox.anchoredPosition = new Vector2(0, 0);
        }

        textBox.gameObject.SetActive(true);
        textBox.GetComponent<Animation>().Play();
    }

    public interface ITutorialStep {
        void Start(Tutorial tutorial);
        bool IsDone();
    }

    public class ActionStep : ITutorialStep {
        public UnityAction action;
        public bool IsDone() {
            return true;
        }

        public void Start(Tutorial tutorial) {
            action.Invoke();
        }

    }

    public class DelayStep : ITutorialStep {
        public float seconds;
        public float doneTime;
        public bool IsDone() {
            return Time.time > doneTime;
        }

        public void Start(Tutorial tutorial) {
            tutorial.focus.DisableFocus();
            tutorial.textBox.gameObject.SetActive(false);
            doneTime = Time.time + seconds;
        }

    }

    public class TutorialStep : ITutorialStep {
        public RectTransform target;
        public string msg;
        public TutorialStepCondition condition;

        public virtual void Start(Tutorial tutorial) {
            if (target != null) {
                tutorial.focus.FocusOn(target);
            }
            tutorial.focus.DisableClick();
            if (msg != null) {
                tutorial.ShowText(msg, target);
                if (target == null) {
                    tutorial.StartCoroutine(tutorial.focus.DelayedFocusOn(tutorial.textBox, 0.5f));
                }
            } else {
                tutorial.textBox.gameObject.SetActive(false);
            }
        }

        public bool IsDone() {
            return condition.Check();
        }
    }

    public class ClickTutorialStep : TutorialStep {
        public override void Start(Tutorial tutorial) {
            base.Start(tutorial);
            ClickedCondition c = new ClickedCondition();
            condition = c;
            tutorial.focus.EnableClick(c.Click);
        }
    }

    public interface TutorialStepCondition {
        bool Check();
    }

    public class BoolVariableCondition : TutorialStepCondition {
        public BoolVariable variable;
        public bool value;
        public bool Check() {
            return variable.Value == value;
        }
    }

    public class ClickedCondition : TutorialStepCondition {
        private bool clicked;

        public void Click() {
            clicked = true;
        }
        public bool Check() {
            return clicked;
        }
    }
}
