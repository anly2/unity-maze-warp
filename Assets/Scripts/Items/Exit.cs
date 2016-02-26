﻿using UnityEngine;
using System.Collections;
using System;

public class Exit : MonoBehaviour {

    public float restartDelay = 0.3f;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;

        Invoke("ReachNextLevel", restartDelay);
    }

    void ReachNextLevel()
    {
        //uiManager.GameNextLevel();
    }

}