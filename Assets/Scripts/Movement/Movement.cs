using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private List<Vector3> trajectory;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trajectory = new List<Vector3>();
    }
    

    public IEnumerator<Vector3> GetTrajectory()
    {
        return CreateEnumerable().GetEnumerator();
    }
    
    private IEnumerable<Vector3> CreateEnumerable()
    {
        int i = 0;
        while (true)
        {
            yield return trajectory[i++];
        }
    }


    public void Move(Vector3 direction)
    {
        trajectory.Add(direction);
        SmoothMovement(direction);
    }

    void SmoothMovement(Vector3 direction)
    {
        //#! smooth movement
        rb.position = direction;
    }
}
