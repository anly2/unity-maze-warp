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
        //#! get dest from target's trajectory
        //#! move monkey

        if (target == null)
            return;

        Vector3 dest = target.Current;
        target.MoveNext();

        Move(dest);
    }
}
