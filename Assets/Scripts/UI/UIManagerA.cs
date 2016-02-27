using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIManagerA : MonoBehaviour {

    public float restartDelay = 2f;

    public Image screenCover;
    public Text levelText;
    public Text deathText;
    public Text turnText;
    public Text timeText;
    public Text dieText;
    public Text passText;

    Animator passLevelAnim;
    
    void Awake()
    {
        passLevelAnim = GetComponent<Animator>();
    }

    void Update()
    {
        //Debug.Log(TimeTracker.timer);
        //deathText.text = "Death: " + GameController.instance.GetDeathCount();
        //turnText.text = "Turn: " + GameController.instance.GetTurnCount();
    }

    public void UpdateLevelInfo(int level)
    {
        levelText.text = "Level: " + level;
    }

    public void GameStart()
    {
        screenCover.enabled = true;
        levelText.enabled = true;
        deathText.enabled = false;
        turnText.enabled = false;
        timeText.enabled = false;
        dieText.enabled = false;
        Invoke("HideUI", restartDelay);
    }

    public void GameNextLevel()
    {
        UpdateStats();
        deathText.enabled = false;
        turnText.enabled = false;
        timeText.enabled = false;
        dieText.enabled = false;
        screenCover.enabled = true;
        passLevelAnim.SetTrigger("ShowPassText");

        Invoke("HideUI", restartDelay);
    }

    public void GameOver()
    {
        UpdateStats();

        deathText.enabled = true;
        turnText.enabled = true;
        timeText.enabled = true;
        dieText.enabled = true;
        screenCover.enabled = true;
        Invoke("HideUI", restartDelay);
    }

    void HideUI()
    {
        deathText.enabled = false;
        turnText.enabled = false;
        timeText.enabled = false;
        dieText.enabled = false;
        screenCover.enabled = false;
        levelText.enabled = false;
    }

    void UpdateStats()
    {
        timeText.text = "Time: " + TranslateTime();
        deathText.text = "Death: " + 0;
        turnText.text = "Turn: " + 0 + "/" + 10;
    }

    String TranslateTime()
    {
        int timer = (int) TimeTracker.timer;
        String timeString = String.Empty;
        int hour;
        int min;
        int sec;
        if(timer < 60)
        {
            timeString = timer.ToString() + "s"; 
        } else if (timer >= 60 && timer < 3600)
        {
            min = (int)timer / 60;
            sec = (int)timer % 60;
            timeString = min.ToString() + "m" + sec.ToString() + "s";
        } else if (timer >= 3600)
        {
            hour = (int)timer / 3600;
            min = (int)(timer % 3600 ) / 60;
            sec = (int)timer % 60;
            timeString = hour.ToString() + 'h' + min.ToString() + "m" + sec.ToString() + "s";
        }
        return timeString;
    }
}
