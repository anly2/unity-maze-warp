using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {

    public GameObject linkedDoor;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (linkedDoor != null)
                linkedDoor.SetActive(false);
        }
    }
}
