using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChaserMovement : Movement, TurnBased {
    public IEnumerator<Vector3> target = null;
    
	void Start () {
        GameController.instance.AddTurnBasedListener(this);
	}
    void OnDestroy()
    {
        GameController.instance.RemoveTurnBasedListener(this);
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
            GameController.instance.warp();
        }
    }
    */
}
