﻿using UnityEngine;
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


    public static IEnumerator FadeOut(this GameObject gameObject)
    {
        return gameObject.FadeOut(Managers.Level.actorFadeOutDuration);
    }

    public static IEnumerator FadeOut(this GameObject gameObject, float duration)
    {
        float start = gameObject.GetOpacity();
        float end = 0;
        float diff = end - start;

        return new Animation(delegate (float p)
        {
            gameObject.SetOpacity(start + p * diff);
        }, duration)
        .Then(() => gameObject.SetOpacity(end));
    }


    public static IEnumerator FadeIn(this GameObject gameObject)
    {
        return gameObject.FadeIn(Managers.Level.actorFadeOutDuration);
    }

    public static IEnumerator FadeIn(this GameObject gameObject, float duration)
    {
        float start = gameObject.GetOpacity();
        float end = 1;
        float diff = end - start;

        return new Animation(delegate (float p)
        {
            gameObject.SetOpacity(start + p * diff);
        }, duration)
        .Then(() => gameObject.SetOpacity(end));
    }
}