﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChaserMovement : Movement, TurnBased, Resetable {
    public IEnumerator<ActionHistory.Action> targetActions = null;
    
	void Start ()
    {
        (this as TurnBased).Register();
        (this as Resetable).Register();
    }

    void OnDestroy()
    {
        (this as TurnBased).Unregister();
        (this as Resetable).Unregister();
    }


    void TurnBased.Turn()
    {
        if (targetActions == null)
            return;

        //while (true)
        {
            try {
                targetActions.MoveNext();
                targetActions.Current(gameObject);
                //break;
            }
            catch (NullReferenceException e) {
                //Debug.Log("" + gameObject + e);
            }
        }
    }

    void Resetable.Reset()
    {
        Destroy(GetComponent<KillByContact>());
        gameObject.FadeOut()
            .Then(() => Destroy(gameObject))
            .Start(this);
    }
}
