using UnityEngine;
using System.Collections;

public class KillByContact : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameController.instance.Warp();
        }
    }
}
