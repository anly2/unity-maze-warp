using UnityEngine;
using System.Collections;

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


public class WaitForTurns : YieldInstruction, IEnumerator, TurnBased
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
    public WaitNextTurn() : base(0) { }
}