using UnityEngine;
using System.Collections;
using System;

public class Lever : MonoBehaviour {

    public GameObject linkedDoor;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;

        Toggle();
            
        if (linkedDoor == null)
            return;

        Door door = linkedDoor.GetComponent<Door>();

        if (door == null)
            linkedDoor.SetActive(false);
        else
            door.Open();
    }

    void Toggle()
    {
        if (animator != null)
            animator.SetBool("On", !animator.GetBool("On"));
    }
}
