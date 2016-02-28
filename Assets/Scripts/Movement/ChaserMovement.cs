using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChaserMovement : Movement, TurnBased {
    public IEnumerator<Vector3> target = null;
    
	void Start () {
        Managers.Turn.AddTurnBasedListener(this);
	}
    void OnDestroy()
    {
        if (Managers.Turn != null)
            Managers.Turn.RemoveTurnBasedListener(this);
    }

    void TurnBased.Turn()
    {
        if (target == null)
            return;

        target.MoveNext();
        Vector3 dest = target.Current;
        
        Move(dest);
    }

    /*
    void OnTriggerEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Managers.Level.Warp();
        }
    }
    */
}
