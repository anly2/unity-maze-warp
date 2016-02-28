using UnityEngine;
using System;
using System.Collections;

public interface Actor {}

public static class ActorExtensions
{
    public static float GetOpacity(this Actor actor)
    {
        try {
            SpriteRenderer renderer = (actor as MonoBehaviour).gameObject.GetComponent<SpriteRenderer>();
            return renderer.color.a;
        }
        catch (Exception) {
            return -1;
        }
    }

    public static void SetOpacity(this Actor actor, float opacity)
    {
        try {
            SpriteRenderer renderer = (actor as MonoBehaviour).gameObject.GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            color.a = opacity;
            renderer.color = color;
        }
        catch (Exception) {}
    }


    public static Animation FadeOut(this Actor actor)
    {
        return actor.FadeOut(Managers.Level.actorFadeOutDuration);
    }

    public static Animation FadeOut(this Actor actor, float duration)
    {
        return new Animation(delegate(float p)
        {
            actor.SetOpacity(1-p);
        }, duration);
    }
}