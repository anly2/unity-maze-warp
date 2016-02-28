using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    public bool startsClosed = true;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
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
