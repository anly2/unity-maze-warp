using UnityEngine;
using System.Collections;

public class PlayerMovement : Movement {
	void Update () {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;
        

        if (horizontal != 0 || vertical != 0)
        {
            if (GameController.instance.TakeTurn())
            {
                Vector3 dest = gameObject.transform.position + new Vector3(horizontal, vertical);
                Move(dest);
            }
        }
    }
}
