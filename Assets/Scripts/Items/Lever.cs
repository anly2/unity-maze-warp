using UnityEngine;
using System.Collections;
using System;

public class Lever : MonoBehaviour {

    public GameObject linkedDoor;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;

        TurnOnLeverAnim(gameObject);

        if (linkedDoor == null)
            return;

        Door door = linkedDoor.GetComponent<Door>();

        if (door == null)
            linkedDoor.SetActive(false);
        else
            door.Open();
    }

    void TurnOnLeverAnim(GameObject gameObject)
    {
        if (gameObject.tag != "Lever")
            return;

        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.flipX = enabled;
    }
}
