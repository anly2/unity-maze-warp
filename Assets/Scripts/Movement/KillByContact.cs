using UnityEngine;
using System.Collections;

public class KillByContact : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Managers.Level.Warp();
        }
        else if (other.gameObject.tag == "Ghost")
        {
            ChaserMovement ghost = other.gameObject.GetComponent<ChaserMovement>() as ChaserMovement;
            (ghost as Resetable).Reset();
        }
    }
}
