﻿using UnityEngine;
using System.Collections;
using System;

public interface TurnBased {
    void Turn();
}

public static class TurnBasedExtensions
{
    public static bool Register(this TurnBased self)
    {
        if (Managers.Turn == null)
            return false;

        Managers.Turn.AddTurnBasedListener(self);
        return true;
    }

    public static void Unregister(this TurnBased self)
    {
        if (Managers.Turn == null)
            return;

        Managers.Turn.RemoveTurnBasedListener(self);
    }
}


public class WaitForTurns : IEnumerator, TurnBased
{
    private int turnDelay;

    public WaitForTurns(int turns)
    {
        turnDelay = turns;
        (this as TurnBased).Register();
    }
    
    public void Turn()
    {
        if (turnDelay-- < 0)
            (this as TurnBased).Unregister();
    }

    public bool MoveNext()
    {
        return (turnDelay > 0);
    }


    public object Current { get { return null; } }

    public void Reset() { }
}

public class WaitNextTurn : WaitForTurns
{
    public WaitNextTurn() : base(1) { }
}

public class WaitForEndOfTurn : IEnumerator
{
    public object Current { get { return null; } }

    public bool MoveNext()
    {
        return Managers.Turn.TurnInProgress;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}