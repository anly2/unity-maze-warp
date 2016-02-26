using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trajectory : List<Vector3>
{
    public Trajectory() {}
    public Trajectory(List<Vector3> traj) : base(traj) { }

    public IEnumerator<Vector3> GetEnumaration()
    {
        return CreateEnumerable().GetEnumerator();
    }

    private IEnumerable<Vector3> CreateEnumerable()
    {
        int i = 0;
        while (true)
        {
            if (i >= base.Count)
                yield break;

            yield return base[i++];
        }
    }

    public Trajectory Clone()
    {
        return new Trajectory(this.GetRange(0, this.Count));
    }
}