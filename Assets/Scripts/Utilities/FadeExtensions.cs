using UnityEngine;
using System;
using System.Collections;


public static class FadeExtensions
{
    public static float GetOpacity(this GameObject gameObject)
    {
        try {
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            return renderer.color.a;
        }
        catch (Exception) {
            return -1;
        }
    }

    public static bool SetOpacity(this GameObject gameObject, float opacity)
    {
        try {
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            color.a = opacity;
            renderer.color = color;

            return true;
        }
        catch (Exception) {
            return false;
        }
    }


    public static Animation FadeOut(this GameObject gameObject)
    {
        return gameObject.FadeOut(Managers.Level.actorFadeOutDuration);
    }

    public static Animation FadeOut(this GameObject gameObject, float duration)
    {
        return new Animation(delegate(float p)
        {
            gameObject.SetOpacity(1-p);
        }, duration);
    }
}