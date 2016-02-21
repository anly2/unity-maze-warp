using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeMapLoader : MonoBehaviour {
    public GameObject board;
    public Texture2D map;

    public GameObject emptyTile;
    public GameObject traversableTile;
    public GameObject wallTile;
    public GameObject doorTile;
    public GameObject leverTile;
    public GameObject bombTile;
    public GameObject bananaTile;

    void Awake() {
        Dictionary<Color, GameObject> tileTypes = ReadTileTypes(map);

        for (int y = 1; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                GameObject tile;
                try {
                    tile = tileTypes[map.GetPixel(x, y)];
                }
                catch (KeyNotFoundException e){
                    tile = emptyTile;
                }
                
                Instantiate(tile, new Vector3(x, y), Quaternion.identity);
            }
        }
    }

    Dictionary<Color, GameObject> ReadTileTypes(Texture2D map)
    {
        Dictionary<Color, GameObject> tileTypes = new Dictionary<Color, GameObject>();
        tileTypes.Add(map.GetPixel(0, 0), emptyTile);
        tileTypes.Add(map.GetPixel(0, 0), traversableTile);
        tileTypes.Add(map.GetPixel(1, 0), wallTile);
        tileTypes.Add(map.GetPixel(2, 0), doorTile);
        tileTypes.Add(map.GetPixel(3, 0), leverTile);
        tileTypes.Add(map.GetPixel(4, 0), bombTile);
        tileTypes.Add(map.GetPixel(5, 0), bananaTile);
        return tileTypes;
    }
}
