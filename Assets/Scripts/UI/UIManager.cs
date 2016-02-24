using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

    public float restartDelay = 2f;

    public Image screenCover;
    public Text levelText;
    public Text deathText;
    public Text turnText;
    public Text timeText;

    void Awake()
    {
    }

    void Update()
    {
        timeText.text = "Time: " + (int) TimeTracker.timer;
        Debug.Log(TimeTracker.timer);
        deathText.text = "Death: " + 0;
        turnText.text = "Turn: " + 0 + "/" + 10;
        //deathText.text = "Death: " + GameController.instance.GetDeathCount();
        //turnText.text = "Turn: " + GameController.instance.GetTurnCount();
    }

    public void UpdateLevelInfo(int level)
    {
        levelText.text = "Level: " + level;
    }

    public void UpdateDeathCount(int death)
    {
        deathText.text = "Death: " + death;
    }

    public void UpdateTurnCount(int leftTurn, int totalTurns)
    {
        turnText.text = "Turn: " + leftTurn + "/" + totalTurns;
    }

    public void GameStart()
    {
        deathText.enabled = false;
        turnText.enabled = false;
        timeText.enabled = false;
        screenCover.enabled = true;
        levelText.enabled = true;
        StartCoroutine(RestartLevel());
    }

    public void GameOver()
    {
        screenCover.enabled = true;
        //levelText.enabled = true;
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(restartDelay);

        Application.LoadLevel(Application.loadedLevel);
        screenCover.enabled = false;
        //levelText.enabled = false;
    }
}
