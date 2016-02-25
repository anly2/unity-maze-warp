using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    SpriteRenderer openState;
    SpriteRenderer closedState;

    public bool startsClosed = true;

    void Start () {
        if (startsClosed)
            Close();
        else
            Open();
	}
    void Awake()
    {
        closedState = GameObject.Find("DoorClosedTile").GetComponent<SpriteRenderer>();
        openState = GameObject.Find("DoorOpenedTile").GetComponent<SpriteRenderer>();

    }

    public void Close()
    {
        closedState.enabled = true;
        openState.enabled = false;
    }

    public void Open()
    {
        closedState.enabled = false;
        openState.enabled = true;
    }
}
