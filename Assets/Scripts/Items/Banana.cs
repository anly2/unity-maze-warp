using UnityEngine;
using System.Collections.Generic;

public class Banana : PickableItem
{
    public Collider2D innerCollider;

    private List<MonkeyMovement> attracted = new List<MonkeyMovement>();
    private Coroutine fadeOut = null;

    protected override void PickUp(GameObject actor)
    {
        base.PickUp(actor);

        foreach (MonkeyMovement monkey in attracted)
            monkey.target = actor;
    }

    protected override void Drop()
    {
        base.Drop();

        foreach (MonkeyMovement monkey in attracted)
            monkey.target = gameObject;
    }


    public void Eat()
    {
        foreach (MonkeyMovement monkey in attracted)
            monkey.GiveUpOnCurrentTarget();

        if (!gameObject.activeInHierarchy)
            return;

        fadeOut = gameObject.FadeOut()
            .Then(() => gameObject.SetActive(false))
            .Start(this);
    }

    protected override void Reset()
    {
        if (fadeOut != null)
            StopCoroutine(fadeOut);
        fadeOut = null;

        gameObject.SetOpacity(1f);
        gameObject.SetActive(true);

        base.Reset();
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (innerCollider.IsTouching(other))
        {
            base.OnTriggerEnter2D(other); //handles picking up
            return;
        }


        if (other.gameObject.tag != "Enemy")
            return;

        MonkeyMovement monkey = other.GetComponent<MonkeyMovement>();

        if (monkey == null)
            return;
        
        monkey.target = (holder != null) ? holder : gameObject;
        attracted.Add(monkey);
    }
}