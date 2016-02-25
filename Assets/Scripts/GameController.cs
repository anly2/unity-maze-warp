using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float turnDuration;

    private bool turnInProgress;
    private List<TurnBased> turnListeners;

    public GameObject HUD;

    UIManager uiManager;

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        turnInProgress = false;
        turnListeners = new List<TurnBased>();
        uiManager = HUD.GetComponent<UIManager>();

        InitGame();
    }

    void InitGame()
    {
        uiManager.GameStart();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            uiManager.GameOver();
        }
    }

    public void AddTurnBasedListener(TurnBased listener)
    {
        turnListeners.Add(listener);
    }

    public bool RemoveTurnBasedListener(TurnBased listener)
    {
        return turnListeners.Remove(listener);
    }
    
    
    public bool TakeTurn()
    {
        if (turnInProgress)
            return false;

        StartCoroutine(NotifyTurnListeners());
        StartCoroutine(StartTurn());
        return true;
    }

    IEnumerator NotifyTurnListeners()
    {
        foreach (TurnBased listener in turnListeners)
        {
            listener.Turn();
        }

        yield return 0;
    }

    IEnumerator StartTurn()
    {
        turnInProgress = true;
        yield return new WaitForSeconds(turnDuration);
        turnInProgress = false;
    }
}
