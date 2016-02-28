using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {
    public static TurnManager instance;

    public float turnDuration;

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
    }

    void OnDestroy()
    {
        instance = null;
    }


    [HideInInspector]
    public bool TurnInProgress
    {
        get {
            return _turnInProgress;
        }
        internal set {
            if (_turnInProgress && value)
                CancelInvoke("EndTurn");

            _turnInProgress = value;
        }
    }
    private bool _turnInProgress = true;


    private List<TurnBased> turnListeners = new List<TurnBased>();

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
        if (TurnInProgress)
            return false;

        TurnInProgress = true;
        Invoke("NotifyTurnListeners", 0f);
        Invoke("EndTurn", turnDuration);
        return true;
    }

    void EndTurn()
    {
        TurnInProgress = false;
    }


    void NotifyTurnListeners()
    {
        foreach (TurnBased listener in turnListeners)
            listener.Turn();
    }

}
