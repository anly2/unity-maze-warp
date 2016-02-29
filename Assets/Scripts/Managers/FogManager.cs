using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogManager : MonoBehaviour {
    public static FogManager instance = null;

    public GameObject fogObject;
    public string fogTileNameFormat = "Fog Tile at ({0:0}, {1:0})";

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
        Fog(new Vector3(Mathf.Epsilon, -1.5f, 0));
        Unfog(new Vector3(1, -1, 0));
        //*/
    }


    /* Functionality */

    public GameObject Fog(Vector3 loc)
    {
        string tileName;
        GameObject tile = GetFogTileAt(loc, out tileName);

        if (tile != null)
        {
            tile.SetOpacity(1);
            return tile;
        }

        tile = Instantiate(fogObject, loc, Quaternion.identity) as GameObject;
        tile.name = tileName;
        tile.transform.parent = this.gameObject.transform;

        return tile;
    }

    public Coroutine Fog(Vector3 loc, float fadeTime)
    {
        GameObject fogTile = GetFogTileAt(loc);
        float initialOpacity = fogTile.GetOpacity();

        if (fogTile == null)
            fogTile = Fog(loc); //creates if not existing

        fogTile.SetOpacity(initialOpacity);

        return fogTile.FadeIn(fadeTime).Start(this);
    }


    public GameObject Unfog(Vector3 loc)
    {
        string tileName;
        GameObject tile = GetFogTileAt(loc, out tileName);

        if (tile == null)
            return null;

        tile.SetOpacity(0);

        return tile;
    }

    public Coroutine Unfog(Vector3 loc, float fadeTime)
    {
        GameObject fogTile = GetFogTileAt(loc);

        return fogTile.FadeOut(fadeTime).Start(this);
    }


    public GameObject[] Explore(Vector3 loc)
    {
        List<GameObject> affected = new List<GameObject>();

        foreach (Vector3 tileLoc in Neighbourhood(loc))
        {
            GameObject tile = Unfog(tileLoc);

            if (tile != null)
                affected.Add(tile);
        }
        
        return affected.ToArray();
    }

    public Coroutine[] Explore(Vector3 loc, float fadeTime)
    {
        List<Vector3> neighbourhood = Neighbourhood(loc);

        Coroutine[] fadings = new Coroutine[neighbourhood.Count];
        int i = 0;

        foreach (Vector3 tileLoc in neighbourhood)
        {
            fadings[i++] = Unfog(tileLoc, fadeTime);
        }

        return fadings;
    }

    List<Vector3> Neighbourhood(Vector3 loc)
    {
        List<Vector3> nearby = new List<Vector3>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 tileLoc = new Vector3(loc.x + j, loc.y + i, loc.z);
                nearby.Add(tileLoc);
            }
        }

        return nearby;
    }


    /* Helper methods */

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


    /* Convenience functionality */

    public void FogAll()
    {

    }

    public void ExploreAll()
    {

    }


    //public void FogArea(Area)
    //public void ExploreArea(Area)
}
