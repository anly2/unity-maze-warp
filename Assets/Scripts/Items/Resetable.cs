using UnityEngine;
using System.Collections;

public interface Resetable {
    void Reset();
}

public static class ResetableExtensions
{
    public static bool Register(this Resetable self)
    {
        if (Managers.Level == null)
            return false;

        Managers.Level.RegisterResetable(self);
        return true;
    }

    public static bool Unregister(this Resetable self)
    {
        if (Managers.Level == null)
            return false;

        return Managers.Level.UnregisterResetable(self);
    }
}
