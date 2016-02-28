using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeMapLoader : MonoBehaviour {
    public GameObject board;
    public TextAsset mapAsset;
    private byte[] map;
    
    public GameObject traversableTile;
    public GameObject wallTile;
    public GameObject doorTile;
    public GameObject leverTile;
    public GameObject bombTile;
    public GameObject bananaTile;

    void Start()
    {
        map = mapAsset.bytes;
    }


    void Awake()
    {
        Instantiate(traversableTile, new Vector3(0, 0), Quaternion.identity);
        Instantiate(wallTile, new Vector3(1, 0), Quaternion.identity);
        Instantiate(bombTile, new Vector3(0, 1), Quaternion.identity);
    }
    void Awakea() {
        Dictionary<byte, GameObject> tileTypes = ReadTileTypes(map);

        int w = 45; //map.width;
        int h = 40; //map.height;

        for (int y = 1; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                try {
                    GameObject tile = tileTypes[GetValue(map, x, y, w)];
                    Instantiate(tile, new Vector3(x, y), Quaternion.identity);
                }
                catch (KeyNotFoundException){
                }
            }
        }
    }

    private static byte GetValue(byte[] map, int x, int y, int w)
    {
        return map[y * w + x];
    }

    Dictionary<byte, GameObject> ReadTileTypes(byte[] map)
    {
        Dictionary<byte, GameObject> tileTypes = new Dictionary<byte, GameObject>();

        Debug.Log("A "+map[1]);
        //tileTypes.Add(map[0], emptyTile);
        tileTypes.Add(map[1], traversableTile);
        tileTypes.Add(map[2], wallTile);
        tileTypes.Add(map[3], doorTile);
        tileTypes.Add(map[4], leverTile);
        tileTypes.Add(map[5], bombTile);
        tileTypes.Add(map[6], bananaTile);
        return tileTypes;
    }
}
