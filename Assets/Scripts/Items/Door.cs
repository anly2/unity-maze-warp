using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Resetable {
    public bool startsClosed = true;

    private Animator animator;
    private Collider2D collider;

    void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }

    void Start()
    {
        (this as Resetable).Register();
        Reset();
    }

    void OnDestroy()
    {
        (this as Resetable).Unregister();
    }


    public void Reset()
    {
        SetClosed(startsClosed);
    }

    public void Close()
    {
        SetClosed(true);
    }

    public void Open()
    {
        SetClosed(false);
    }


    void SetClosed(bool flag)
    {
        if (collider != null)
        {
            if (animator != null)
                animator.SetBool("On", flag);

            collider.enabled = flag;
        }
        else
            gameObject.SetActive(flag);
    }
}
