using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {

    public GameObject linkedDoor;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;


        if (linkedDoor == null)
            return;

        Door door = linkedDoor.GetComponent<Door>();

        if (door == null)
            linkedDoor.SetActive(false);
        else
            door.Open();
    }
}
