using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour 
{
    public LayerMask blockingLayer;

    [HideInInspector]
    public Trajectory trajectory { get; private set; }

    public Movement() : base()
    {
        trajectory = new Trajectory();
    }

    public void Move(Vector3 dest)
    {
        if (!CanMove(dest))
            return;

        trajectory.Add(dest);
        SmoothMovement(dest);
    }

    public bool CanMove(Vector3 dest)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, dest, blockingLayer);
        return (hit.transform == null);
    }

    Coroutine SmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, end);

        return StartCoroutine(new Animation(delegate (float p)
        {
            transform.position = Vector3.MoveTowards(start, end, p * distance);
        }, GameManager.instance.turnDuration));
    }
}