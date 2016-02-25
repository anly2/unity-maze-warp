using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour 
{
    private Rigidbody2D rb;
    private List<Vector3> trajectory;
    private float requiredSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trajectory = new List<Vector3>();

        float moveTime = GameController.instance.turnDuration;
        requiredSpeed = 1f / moveTime;
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


    public void Move(Vector3 dest)
    {
        trajectory.Add(dest);
        StartCoroutine(SmoothMovement(dest));
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
        float distance = CalcuEuclideanDistance(transform.position, end);
        while (distance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, end, requiredSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            distance = CalcuEuclideanDistance(newPos, end);
            yield return null;
        }
    }

    float CalcuEuclideanDistance(Vector3 start, Vector3 end)
    {
        return  Mathf.Sqrt(
                Mathf.Pow((end.x - start.x), 2) + 
                Mathf.Pow((end.y - start.y), 2) + 
                Mathf.Pow((end.z - start.z), 2)
                );
    }
}