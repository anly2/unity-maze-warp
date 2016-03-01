using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Bomb : PickableItem {

    public int fuseTurnDuration = 3;
    public int explosionRadius = 2;

    private Coroutine armed = null;


    protected override void Reset()
    {
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
        Drop();

        armed = StartCoroutine(Arm());
    }

    IEnumerator Arm()
    {
        yield return new WaitForTurns(fuseTurnDuration);

        gameObject.SetActive(false);
        Debug.Log("EXPLODE");

        //#! explosion effect

        GameObject[] affected = GetAffected();

        foreach (GameObject thing in affected)
        {
            if (!isDestructible(thing))
                continue;

            Debug.Log("Damaged: " + thing);

            if (thing.tag == "Player")
            {
                Managers.Level.Warp();
                yield break;
            }

            Resetable resetable = thing.GetComponent<Resetable>();

            if (resetable != null)
                resetable.Reset();
            else
                thing.SetActive(false);
        }
    }

    GameObject[] GetAffected()
    {
        /*
        float tol = 0.5f; //so not to catch tiles by just a pixel or so (same reason Actor colliders are not of size 1)
        return Physics2D.OverlapCircleAll(transform.position, explosionRadius - tol);
        */

        Vector3 loc = transform.position;
        List<GameObject> affected = new List<GameObject>();

        Vector3[] locs = {
            new Vector3(loc.x - 1, loc.y),
            new Vector3(loc.x,     loc.y),
            new Vector3(loc.x + 1, loc.y),
            new Vector3(loc.x, loc.y - 1),
            new Vector3(loc.x, loc.y + 1),
        };

        foreach (Vector3 p in locs)
        {
            Collider2D collider = Physics2D.OverlapPoint(p);

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
