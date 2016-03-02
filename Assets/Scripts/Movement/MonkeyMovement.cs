using UnityEngine;
using System.Collections.Generic;

public class MonkeyMovement : ChaserMovement {
    public GameObject target {
        get { return _target; }
        set {
            _target = value;
            Vector3 dest = _target.transform.position;

            Movement movement = _target.GetComponent<Movement>();
            if (movement != null)
            {
                targetMovement = movement;
                dest = movement.projectedLocation;
            }
            
            targetPath = this.FindPath(dest);
        }
    }

    private GameObject _target = null;
    private Movement targetMovement = null;
    private IEnumerator<Vector3> targetPath = null;

    // Use this for initialization
    void Start() {
        (this as TurnBased).Register();
    }

    protected override void Turn()
    {
        if (target == null)
            return;

        if (targetMovement != null)
            targetPath = this.FindPath(targetMovement.projectedLocation);

        targetPath.MoveNext();
        Move(targetPath.Current);
    }
}
