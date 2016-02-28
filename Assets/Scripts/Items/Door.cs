using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Resetable {
    public bool startsClosed = true;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
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
        if (startsClosed)
            Close();
        else
            Open();
    }

    public void Close()
    {
        if (animator != null)
            animator.SetBool("On", true);
        else
            gameObject.SetActive(true);

    }

    public void Open()
    {

        if (animator != null)
            animator.SetBool("On", false);
        else
            gameObject.SetActive(false);
    }
}
