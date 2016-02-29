using UnityEngine;
using System.Collections;

public class BoardNormalizer : MonoBehaviour {
    
    public GameObject board = null; //null for 'this.gameObject'
    
    void Awake()
    {
        if (this.board == null)
            this.board = this.gameObject;
    }

    void Start()
    {
        SpriteRenderer[] tiles = board.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer tile in tiles)
        {
            if (tile.sortingLayerName != "Floor" && tile.sortingLayerName != "Wall")
                continue;

            /*
            if floor
                check surroundings
                    init abyssTile(get abyssal)
            */

            Managers.Fog.Fog(tile.transform.position);
        }
	}
}
