using UnityEngine;
using System.Collections;

public class TimeTracker : MonoBehaviour {

    public static float timer;

    void Update()
    {
        timer += Time.deltaTime;
    }
}
