using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogManager : MonoBehaviour {
    public static FogManager instance = null;

    public GameObject fogObject;
    public string fogTileNameFormat = "Fog Tile at ({0:0}, {1:0})";
    public float defaultExploredRadius = 3;

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
        float r = defaultExploredRadius - 0.01f; //avoid rounding erros
        Rect area = new Rect(0,0, r, r);
        area.center = loc;

        return Explore(area);

    }

    public Coroutine[] Explore(Vector3 loc, float fadeTime)
    {
        float r = defaultExploredRadius - 0.01f; //avoid rounding erros
        Rect area = new Rect(0, 0, r, r);
        area.center = loc;

        return Explore(area, fadeTime);
    }

    
    public GameObject[] ExploreArea(Vector3 loc, float radius)
    {
        Rect area = new Rect(0, 0, radius, radius);
        area.center = loc;

        return Explore(area);
    }

    public Coroutine[] ExploreArea(Vector3 loc, float radius, float fadeTime)
    {
        Rect area = new Rect(0, 0, radius, radius);
        area.center = loc;

        return Explore(area, fadeTime);
    }


    public GameObject[] Explore(Rect area)
    {
        return ApplyToArea<GameObject>(area, Unfog);
    }

    public Coroutine[] Explore(Rect area, float fadeTime)
    {
        return ApplyToArea<Coroutine>(area, tile => Unfog(tile, fadeTime));
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
        return string.Format(fogTileNameFormat, Mathf.RoundToInt(loc.x), Mathf.RoundToInt(loc.y));
    }


    delegate R Action<R>(Vector3 loc);

    R[] ApplyToArea<R>(Rect area, Action<R> action)
    {
        List<R> affected = new List<R>();

        float sx = 1;
        float sy = 1;

        for (float i = area.yMin; i < area.yMax; i += sy)
        {
            for (float j = area.xMin; j < area.xMax; j += sx)
            {
                R fogTile = action(new Vector2(j, i));

                if (fogTile != null)
                    affected.Add(fogTile);
            }
        }

        return affected.ToArray();
    }
}
