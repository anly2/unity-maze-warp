using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MonkeyMovement : Movement, TurnBased {
    public IEnumerator<Vector3> target = null;
    
	void Start () {
        GameController.instance.AddTurnBasedListener(this);
	}

    void TurnBased.Turn()
    {
        if (target == null)
            return;

        target.MoveNext();
        Vector3 dest = target.Current;
        
        Move(dest);
    }
}
