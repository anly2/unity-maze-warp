using UnityEngine;
using System.Collections;
using System;

public class Exit : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;

        Managers.Level.Complete();
    }
}
