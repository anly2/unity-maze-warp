using UnityEngine;
using System.Collections;

public class FogManager : MonoBehaviour {
    public static FogManager instance = null;

    public GameObject fogObject;
    public string fogTileNameFormat = "Fog Tile at ({0}, {1})";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    
    void Start () {
        /* Test
        Fog(new Vector3(0, -1, 0));
        Fog(new Vector3(1, -1, 0));
        Fog(new Vector3(2, -1, 0));
        Explore(new Vector3(1, -1, 0));
        //*/
    }


    /* Functionality */

    public GameObject Fog(Vector3 loc)
    {
        string tileName;
        GameObject tile = GetFogTileAt(loc, out tileName);

        if (tile != null)
            return tile;

        tile = Instantiate(fogObject, loc, Quaternion.identity) as GameObject;
        tile.name = tileName;
        tile.transform.parent = this.gameObject.transform;

        return tile;
    }

    public GameObject Explore(Vector3 loc)
    {
        string tileName;
        GameObject tile = GetFogTileAt(loc, out tileName);

        if (tile == null)
            return null;

        tile.SetOpacity(0);

        return tile;
    }


    GameObject GetFogTileAt(Vector3 loc)
    {
        string name;
        return GetFogTileAt(loc, out name);
    }

    GameObject GetFogTileAt(Vector3 loc, out string name)
    {
        name = NameFor(loc);
        Transform t = transform.FindChild(name);
        return (t == null)? null : t.gameObject;
    }

    string NameFor(Vector3 loc)
    {
        return string.Format(fogTileNameFormat, loc.x, loc.y);
    }


    public void FogAll()
    {

    }

    public void ExploreAll()
    {

    }


    //public void FogArea(Area)
    //public void ExploreArea(Area)
}
