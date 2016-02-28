using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteRandomizer : MonoBehaviour {
    public List<Sprite> sprites;

    void Start ()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Sprite sprite = sprites[Random.Range(0, sprites.Count)];
            spriteRenderer.sprite = sprite;
        }
	}
}
