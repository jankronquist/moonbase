using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;

public class HighscoreManager : MonoBehaviour {
    private const string DATE_FORMAT = "yyyy'-'MM'-'dd";
    private const string KEY_HIGHSCORE_TODAY = "HighscoreToday";
    private const string KEY_HIGHSCORE_TODAY_DATE = "HighscoreTodayDate";
    private const string KEY_HIGHSCORE_ALL_TIME = "HighscoreAllTime";
    public IntVariable score;
    public IntVariable highscoreToday;
    public IntVariable highscoreAllTime;
    public TMPro.TMP_Text resultText;

    private static bool scoresLoaded;

    public void Start() {
        if (!scoresLoaded) {
            string today = DateTime.Now.ToString(DATE_FORMAT);
            string highscoreTodayDate = DateTime.ParseExact(PlayerPrefs.GetString(KEY_HIGHSCORE_TODAY_DATE, today), DATE_FORMAT, null).ToString(DATE_FORMAT);
            Debug.Log("Loading highscores: today=" + today + " highscoreTodayDate=" + highscoreTodayDate);
            if (today == highscoreTodayDate) {
                highscoreToday.Value = PlayerPrefs.GetInt(KEY_HIGHSCORE_TODAY);
            }
            highscoreAllTime.Value = PlayerPrefs.GetInt(KEY_HIGHSCORE_ALL_TIME);
            Debug.Log("highscoreToday=" + highscoreToday.Value + " highscoreAllTime=" + highscoreAllTime.Value);
            scoresLoaded = true;
        }
    }

    public void UpdateHighScores() {
        string result = "Research more!";
        if (score > 20) {
            result = "Try again!";
        } else if (score > 70) {
            result = "Well done!";
        } else if (score > 100) {
            result = "Great job!";
        } else if (score > 130) {
            result = "Amazing!";
        }
        if (score > 10 && score < highscoreAllTime.Value / 2) {
            result = "You can do better!";
        }
        if (score.Value > highscoreToday.Value) {
            highscoreToday.Value = score.Value;
            PlayerPrefs.SetInt(KEY_HIGHSCORE_TODAY, score.Value);
            PlayerPrefs.SetString(KEY_HIGHSCORE_TODAY_DATE, DateTime.Now.ToString(DATE_FORMAT));
            result = "Today's best!";
        }
        if (score.Value > highscoreAllTime.Value) {
            highscoreAllTime.Value = score.Value;
            PlayerPrefs.SetInt(KEY_HIGHSCORE_ALL_TIME, score.Value);
            result = "New highscore!";
        }
        resultText.text = result;
    }
}
