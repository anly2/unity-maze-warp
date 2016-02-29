using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager instance = null;

    private Animator animator;
    private Text vMessage;
    private GameObject vStats;
    private LabelledValue vDeathCount;
    private LabelledValue vTurnCount;
    private LabelledValue vTimeElapsed;

    private bool showingScreen = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        FetchComponents();
    }

    void OnDestroy()
    {
        instance = null;
    }

    void FetchComponents()
    {
        animator = gameObject.GetComponent<Animator>();
        vMessage = gameObject.transform.Find("Message").GetComponent<Text>();
        vStats = gameObject.transform.Find("Stats").gameObject;
        vDeathCount  = vStats.transform.Find("Death Count").GetComponent<LabelledValue>();
        vTurnCount   = vStats.transform.Find("Turn Count").GetComponent<LabelledValue>();
        vTimeElapsed = vStats.transform.Find("Time Elapsed").GetComponent<LabelledValue>();
    }


    public void Hit()
    {
        animator.SetTrigger("Hit");
    }


    public IEnumerator ShowPreScreen()
    {
        return ShowPreScreen(Managers.Level.Name);
    }

    public IEnumerator ShowPreScreen(string message)
    {
        Managers.Turn.TurnInProgress = true;
        showingScreen = true;

        vMessage.text = message;
        animator.SetTrigger("Pre-Screen Ready");

        return WaitPreScreenHidingDone();
    }

    IEnumerator WaitPreScreenHidingDone()
    {
        while (showingScreen)
            yield return null;
    }

    void DoneHidingPreScreen() {
        showingScreen = false;
    }


    public void ShowPostScreen()
    {
        ShowPostScreen(Managers.Level.Name + "\ncompleted");
    }

    public void ShowPostScreen(string message)
    {
        showingScreen = true;

        vMessage.text = message;
        UpdateStats();

        animator.SetTrigger("Post-Screen Ready");
    }

    void UpdateStats()
    {
        vDeathCount.Set(Managers.Level.Stats.DeathCount);
        vTurnCount.Set(Managers.Level.Stats.TurnCount);
        vTimeElapsed.Set(FormatTimer(Managers.Level.Stats.TimeElapsed));
    }

    string FormatTimer(float time)
    {
        int t = (int)time;
        int seconds = t % 60;
        int minutes = t / 60;
        int hours = t / (60 * 60);

        string formatted = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (hours > 0)
            formatted = hours + ":" + formatted;

        return formatted;
    }
}
