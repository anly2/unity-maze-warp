using UnityEngine;
using System;


public class PickableItem : MonoBehaviour, Resetable
{

    public PickedUpTransform pickedUpTransform;

    [Serializable]
    public class PickedUpTransform
    {
        public Vector2 offset = new Vector2(0.2f, -0.2f);
        public Vector2 scale = new Vector2(0.8f, 0.8f);
    }
    
    struct StoredTransform
    {
        internal Transform parent;
        internal Vector3 position;
        internal Vector3 scale;
        internal int sortingLayerID;
        internal int sortingOrder;

        public StoredTransform(Transform t, SpriteRenderer r)
        {
            parent = t.parent;
            position = t.position;
            scale = t.localScale;
            sortingLayerID = r.sortingLayerID;
            sortingOrder = r.sortingOrder;
        }
    };
    private StoredTransform initial;


    public class CarriedItem : MonoBehaviour
    {
        public PickableItem carriedItem;

        void OnDestroy()
        {
            if (carriedItem != null)
                carriedItem.Drop();
        }

        public void Free()
        {
            carriedItem = null; //so not to Drop
            Destroy(this);
        }
    }


    private bool pickedUp = false;
    private CarriedItem slotTaken = null;
    protected bool acting = false; //to avoid infinate recursion with recorded actions

    void Awake()
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        initial = new StoredTransform(gameObject.transform, renderer);
    }


    void Start()
    {
        (this as Resetable).Register();
    }

    void OnDestroy()
    {
        (this as Resetable).Unregister();
    }


    void Resetable.Reset()
    {
        this.Reset();
    }

    protected virtual void Reset()
    {
        gameObject.transform.position = initial.position;
        RestoreTransform();

        if (slotTaken != null) //can be reset without being picked up
            slotTaken.Free();

        pickedUp = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp)
            return;

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ghost")
        {
            if (other.gameObject.GetComponent<CarriedItem>() != null)
                return;

            PickUp(other.gameObject);
        }
    }

    void Update()
    {
        if (!pickedUp)
            return;

        if (slotTaken.gameObject.tag != "Player")
            return;
        

        if (Input.GetButton("Fire1"))
            new WaitForEndOfTurn().Then(() => Activate()).Start(this);
        else
        if (Input.GetButton("Fire2"))
            new WaitForEndOfTurn().Then(() => Drop()).Start(this);

    }


    protected virtual void PickUp(GameObject actor)
    {
        this.gameObject.transform.parent = actor.transform;
        this.gameObject.transform.localPosition = pickedUpTransform.offset;
        this.gameObject.transform.localScale = pickedUpTransform.scale;
        BringInfront(actor);

        slotTaken = actor.AddComponent<CarriedItem>();
        slotTaken.carriedItem = this;

        pickedUp = true;
    }

    void BringInfront(GameObject other)
    {
        SpriteRenderer r0 = this.GetComponent<SpriteRenderer>();
        SpriteRenderer r1 = other.GetComponent<SpriteRenderer>();

        r0.sortingLayerID = r1.sortingLayerID;
        r0.sortingOrder = r1.sortingOrder + 1;
    }

    void RestoreTransform(Vector3 localPosition)
    {
        this.gameObject.transform.localPosition = localPosition;
        RestoreTransform();
    }

    void RestoreTransform()
    {
        this.gameObject.transform.parent = initial.parent;
        this.gameObject.transform.localScale = initial.scale;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerID = initial.sortingLayerID;
        renderer.sortingOrder = initial.sortingOrder;
    }


    protected virtual void Drop()
    {
        if (acting) //avoid infinite recursion
            return;

        acting = true;

        ActionHistory.Action dropAction = delegate (GameObject actor)
        {
            CarriedItem slot = actor.GetComponent<CarriedItem>();
            PickableItem item = slot.carriedItem;

            item.RestoreTransform(new Vector3(0, 0, 0));
            slot.Free();
            item.pickedUp = false;

            return false;
        };

        dropAction(slotTaken.gameObject);
        slotTaken.gameObject.GetComponent<Movement>().actionHistory.Add(dropAction);

        acting = false;
    }

    protected virtual void Activate()
    {
        if (acting) //avoid infinite recursion
            return;

        acting = true;

        ActionHistory.Action activation = null;
        activation = delegate (GameObject actor)
        {
            CarriedItem slot = actor.GetComponent<CarriedItem>();
            PickableItem item = slot.carriedItem;

            item.Activate();
            return false;
        };

        activation(slotTaken.gameObject);
        slotTaken.gameObject.GetComponent<Movement>().actionHistory.Add(activation);

        acting = false;
    }
}
