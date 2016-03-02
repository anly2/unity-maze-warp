using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class Bomb : PickableItem {

    [Header("Effect Prefabs")]
    public GameObject explosionPeripheralObject;
    public GameObject explosionEpicentralObject;
    public GameObject flameObject;

    [Header("Bomb Properties")]
    public int fuseTurnDuration = 3;
    //public int explosionRadius = 2;

    private Coroutine armed = null;

    protected override void Reset()
    {
        if (armed != null)
            StopCoroutine(armed);
        armed = null;

        gameObject.SetActive(true);

        base.Reset();
    }

    protected override void PickUp(GameObject actor)
    {
        if (armed == null)
            base.PickUp(actor);
    }

    protected override void Activate()
    {
        if (acting)
            return;

        base.Activate();

        Drop();

        armed = StartCoroutine(Arm());
    }

    IEnumerator Arm()
    {
        LitFlame();

        yield return new WaitForTurns(fuseTurnDuration);

        Explode();
        
        yield return new WaitForSeconds(Managers.Turn.turnDuration);

        Vector3 loc = transform.position;
        gameObject.SetActive(false);

        GameObject[] affected = GetAffected(loc);

        foreach (GameObject thing in affected)
        {
            if (!isDestructible(thing))
                continue;

            if (thing.tag == "Player")
            {
                Managers.Level.Warp();
                yield break;
            }

            Door door = thing.GetComponent<Door>();
            if (door != null)
            {
                door.Open();
                continue;
            }

            Resetable resetable = thing.GetComponent<Resetable>();
            if (resetable != null)
            {
                resetable.Reset();
                continue;
            }

            //else
            thing.SetActive(false);
        }
    }

    void LitFlame()
    {
        GameObject flame = Instantiate(flameObject);
        flame.transform.parent = transform;
        flame.transform.localPosition = flameObject.transform.position;
    }

    void Explode()
    {
        Vector3 bombLoc = transform.position;
        Vector3[] affected = GetAffectedLocations(bombLoc);

        foreach (Vector3 loc in affected)
        {
            GameObject explosion = (loc == bombLoc) ? explosionEpicentralObject : explosionPeripheralObject;
            Instantiate(explosion, loc, Quaternion.identity);
        }
    }


    Vector3[] GetAffectedLocations(Vector3 loc)
    {
        Vector3[] affected = {
            new Vector3(loc.x - 1, loc.y),
            new Vector3(loc.x + 1, loc.y),
            loc,
            new Vector3(loc.x, loc.y - 1),
            new Vector3(loc.x, loc.y + 1),
        };
        return affected;
    }

    GameObject[] GetAffected(Vector3 loc)
    {
        /*
        float tol = 0.5f; //so not to catch tiles by just a pixel or so (same reason Actor colliders are not of size 1)
        return Physics2D.OverlapCircleAll(transform.position, explosionRadius - tol);
        */

        Vector3[] locs = GetAffectedLocations(loc);
        List<GameObject> affected = new List<GameObject>();

        foreach (Vector3 p in locs)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(p);

            foreach (Collider2D collider in colliders)
                if (collider != null)
                    affected.Add(collider.gameObject);
        }

        return affected.ToArray();
    }


    static string[] destructibleTags = { "Destructible", "Enemy", "Ghost", "Player" };
    static bool isDestructible(GameObject gameObject)
    {
        return (destructibleTags.Contains(gameObject.tag));
    }
}
