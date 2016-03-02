using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActionHistory : List<ActionHistory.Action>
{
    public delegate bool Action(GameObject self);

    public ActionHistory() {}
    public ActionHistory(List<Action> history) : base(history) { }

    public IEnumerator<Action> GetEnumaration()
    {
        return CreateEnumerable().GetEnumerator();
    }

    private IEnumerable<Action> CreateEnumerable()
    {
        int i = 0;
        while (true)
        {
            if (i >= base.Count)
                yield break;

            yield return base[i++];
        }
    }

    public ActionHistory Clone()
    {
        return new ActionHistory(this.GetRange(0, this.Count));
    }
}