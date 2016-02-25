using UnityEngine;
using System.Collections;
using System;

public class PickUp : MonoBehaviour {

    public GameObject bananaItem;
    public GameObject bombItem;
    public Transform[] itemShowUpPos;

    Transform player;
    CarriedItems carriedItems;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;

        player = other.gameObject.transform;
        carriedItems = player.GetComponent<CarriedItems>();

        DestoryPickUp();     
    }

    void DestoryPickUp()
    {
        int index = carriedItems.count;

        //if (index > itemShowUpPos.Length)
        if (index >= 1)
            return;

        GameObject tobeInstantiate;
        if (gameObject.tag == "Bomb")
        {
            tobeInstantiate = Instantiate(bombItem, itemShowUpPos[index].position, Quaternion.identity) as GameObject;
            tobeInstantiate.transform.SetParent(player, false);
            carriedItems.count++;
            Destroy(gameObject);
        }

        if (gameObject.tag == "Banana")
        {
            tobeInstantiate = Instantiate(bananaItem, itemShowUpPos[index].position, Quaternion.identity) as GameObject;
            tobeInstantiate.transform.SetParent(player, false);
            carriedItems.count++;
            Destroy(gameObject);
        }
    }
}
