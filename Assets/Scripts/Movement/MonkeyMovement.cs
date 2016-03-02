using UnityEngine;
using System.Collections.Generic;

public class MonkeyMovement : ChaserMovement {
    public GameObject target {
        get { return targets.Peek(); }
        set {
            targets.Push(value);
            Vector3 dest = value.transform.position;

            Movement movement = value.GetComponent<Movement>();
            if (movement != null)
            {
                targetMovement = movement;
                dest = movement.projectedLocation;
            }
            else
                targetMovement = null;
            
            targetPath = this.FindPath(dest);
        }
    }

    private Stack<GameObject> targets = new Stack<GameObject>();
    private Movement targetMovement = null;
    private IEnumerator<Vector3> targetPath = null;


    protected override void Turn()
    {
        if (target == null)
            return;

        if (targetMovement != null)
            targetPath = this.FindPath(targetMovement.projectedLocation);

        if (targetPath.MoveNext())
            Move(targetPath.Current);
        else
        {
            Banana banana = target.GetComponent<Banana>();
            if (banana != null)
                banana.Eat(); //Eat() will trigger GiveUpOnCurrentTarget()
            else
                GiveUpOnCurrentTarget(); //becuase it has been reached
        }
    }

    public void GiveUpOnCurrentTarget()
    {
        if (targets.Count > 1)
        {
            targets.Pop(); //Remove the current one
            target = targets.Pop(); //Set the next target;
            //set (=) is needed to trigger the Path calculation
            //Pop is needed to counter the Push called in set
        }
        else
            target = GameObject.FindWithTag("Player");
    }
}
