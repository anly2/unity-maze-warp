using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChaserMovement : Movement, TurnBased, Resetable {
    public IEnumerator<Vector3> target = null;
    
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
        if (target == null)
            return;

        target.MoveNext();
        Vector3 dest = target.Current;
        
        Move(dest);
    }

    void Resetable.Reset()
    {
        Destroy(GetComponent<KillByContact>());
        gameObject.FadeOut()
            .Then(() => Destroy(gameObject))
            .Start(this);
    }
}
