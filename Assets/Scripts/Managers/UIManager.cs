using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager instance = null;

    public GameObject HUD;

    private Animator animator;
    private Text vMessage;
    private GameObject vStats;
    private LabelledValue vDeathCount;
    private LabelledValue vTurnCount;
    private LabelledValue vTimeElapsed;

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

        if (HUD == null)
            HUD = GameObject.Find("HUD");

        FetchComponents();
    }

    void FetchComponents()
    {
        animator = HUD.GetComponent<Animator>();
        vMessage = HUD.transform.Find("Message").GetComponent<Text>();
        vStats = HUD.transform.Find("Stats").gameObject;
        vDeathCount  = vStats.transform.Find("Death Count").GetComponent<LabelledValue>();
        vTurnCount   = vStats.transform.Find("Turn Count").GetComponent<LabelledValue>();
        vTimeElapsed = vStats.transform.Find("Time Elapsed").GetComponent<LabelledValue>();
    }


    public void Hit()
    {
        animator.SetTrigger("Hit");
    }


    public void ShowPreScreen()
    {
        ShowPreScreen(LevelManager.instance.Name);
    }

    public void ShowPreScreen(string message)
    {
        vMessage.text = message;
        animator.SetTrigger("Pre-Screen Ready");

        TurnManager.instance.TurnInProgress = false;
    }


    public void ShowPostScreen()
    {
        ShowPostScreen(LevelManager.instance.Name + "\ncompleted");
    }

    public void ShowPostScreen(string message)
    {
        vMessage.text = message;

        UpdateStats();

        animator.SetTrigger("Post-Screen Ready");
    }

    void UpdateStats()
    {
        vDeathCount.Set(LevelManager.instance.Stats.DeathCount);
        vTurnCount.Set(LevelManager.instance.Stats.TurnCount);
        vTimeElapsed.Set(LevelManager.instance.Stats.TimeElapsed);
    }
}
