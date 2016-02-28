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

    public static bool Unregister(this TurnBased self)
    {
        if (Managers.Turn == null)
            return false;

        return Managers.Turn.RemoveTurnBasedListener(self);
    }
}