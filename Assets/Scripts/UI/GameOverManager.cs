using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {

    public float restartDelay = 2f;

    public Image screenCover;
    public Text levelText;
    public Text deathText;
    public Text turnText;
    public Text TimeText;

    void Awake()
    {

    }

    public void UpdateLevelInfo(int level)
    {
        levelText.text = "Level: " + level;
    }

    public void GameOver()
    {
        screenCover.enabled = true;
        levelText.enabled = true;
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(restartDelay);

        Application.LoadLevel(Application.loadedLevel);
        screenCover.enabled = false;
        levelText.enabled = false;
    }
}
