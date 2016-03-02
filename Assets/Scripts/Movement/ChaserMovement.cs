using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChaserMovement : Movement, TurnBased, Resetable {
    public virtual IEnumerator<ActionHistory.Action> targetActions { get; set; }

    public ChaserMovement()
    {
        targetActions = null; //by default
    }
    
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
        Turn();
    }

    protected virtual void Turn()
    {
        if (targetActions == null)
            return;

        try {
            while (true)
            {
                targetActions.MoveNext();
                if (targetActions.Current(gameObject))
                    break;
            }
        }
        catch (NullReferenceException e) {
            Debug.Log("" + gameObject + e);
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
