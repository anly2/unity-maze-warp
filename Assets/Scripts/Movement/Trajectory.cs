using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trajectory : List<Vector3>
{
    public IEnumerator<Vector3> GetEnumaration()
    {
        return CreateEnumerable().GetEnumerator();
    }

    private IEnumerable<Vector3> CreateEnumerable()
    {
        int i = 0;
        while (true)
        {
            yield return base[i++];
        }
    }
}