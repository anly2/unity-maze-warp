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

    public void RemoveTurnBasedListener(TurnBased listener)
    {
        turnListeners.Remove(listener);
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
        var listeners = turnListeners.ToArray(); //gives us a snapshot
        foreach (TurnBased listener in listeners)
            listener.Turn();
    }

}
