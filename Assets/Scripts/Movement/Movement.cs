using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour 
{
    public LayerMask blockingLayer;

    [HideInInspector]
    public ActionHistory actionHistory { get; private set; }

    [HideInInspector]
    public Vector3 projectedLocation { get; private set; }


    public Movement() : base()
    {
        actionHistory = new ActionHistory();
    }

    void Awake()
    {
        projectedLocation = gameObject.transform.position;
    }


    public bool CanMove(Vector3 dest)
    {
        return CanMove(transform.position, dest);
    }

    public bool CanMove(Vector3 from, Vector3 to)
    {
        RaycastHit2D hit = Physics2D.Linecast(from, to, blockingLayer);
        return (hit.transform == null);
    }


    public void Move(Vector3 dest)
    {
        ActionHistory.Action moveAction = null;
        moveAction = delegate (GameObject self) {
            Movement movement = self.GetComponent<Movement>();

            if (!movement.CanMove(dest))
                return false;

            movement.projectedLocation = dest;
            movement.SmoothMovement(dest);
            movement.actionHistory.Add(moveAction);

            return true;
        };
        moveAction(gameObject);
    }

    Coroutine SmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, end);

        return StartCoroutine(new Animation(delegate (float p)
        {
            transform.position = Vector3.MoveTowards(start, end, p * distance);
        }, Managers.Turn.turnDuration));
    }
}