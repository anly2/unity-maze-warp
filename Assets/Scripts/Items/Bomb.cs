using UnityEngine;
using System.Linq;
using System.Collections;

public class Bomb : PickableItem {

    public int fuseTurnDuration = 3;
    public int explosionRadius = 2;

    private bool armed = false;


    protected override void PickUp(GameObject actor)
    {
        if (!armed)
            base.PickUp(actor);
    }

    protected override void Activate()
    {
        Drop();

        StartCoroutine(Arm());
    }

    IEnumerator Arm()
    {
        armed = true;
        Debug.Log("Arming bomb. " + fuseTurnDuration + " turns to explosion.");

        yield return new WaitForTurns(fuseTurnDuration);

        Debug.Log("EXPLODE");

        //#! explosion effect

        float tol = 0.5f; //so not to catch tiles by just a pixel or so (same reason Actor colliders are not of size 1)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius-tol);

        foreach (Collider2D collider in hitColliders)
        {
            GameObject thing = collider.gameObject;
            if (!isDestructible(thing))
                continue;

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


    static string[] destructibleTags = { "Destructible", "Monkey", "Ghost", "Player" };
    static bool isDestructible(GameObject gameObject)
    {
        return (destructibleTags.Contains(gameObject.tag));
    }
}
