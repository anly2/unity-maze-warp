using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public Sprite openState;
    public Sprite closedState;

    public bool startsClosed = true;

    void Start () {
        if (startsClosed)
            Close();
        else
            Open();
	}

    public void Close()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        renderer.sprite = closedState;
    }

    public void Open()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        renderer.sprite = openState;
    }
}
