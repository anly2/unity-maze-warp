using UnityEngine;
using System.Collections;

public static class CoroutineExtentions
{
    public delegate void Action();
    public delegate object Expression();


    public static IEnumerator Then(this IEnumerator self, IEnumerator then)
    {
        while (self.MoveNext())
            yield return self.Current;

        while (then.MoveNext())
            yield return then.Current;
    }

    public static IEnumerator Then(this IEnumerator self, Action then)
    {
        return self.Then(() => { then(); return null; });
    }

    public static IEnumerator Then(this IEnumerator self, Expression then)
    {
        while (self.MoveNext())
            yield return self.Current;

        yield return then();
    }


    public static IEnumerator ButFirst(this IEnumerator self, IEnumerator first)
    {
        while (first.MoveNext())
            yield return first.Current;

        while (self.MoveNext())
            yield return self.Current;
    }

    public static IEnumerator ButFirst(this IEnumerator self, Action first)
    {
        return self.ButFirst(() => { first(); return null; });
    }

    public static IEnumerator ButFirst(this IEnumerator self, Expression first)
    {
        yield return first();

        while (self.MoveNext())
            yield return self.Current;
    }

    
    public static IEnumerator Then(this YieldInstruction self, Action then)
    {
        return self.Then(() => { then(); return null; });
    }

    public static IEnumerator Then(this YieldInstruction self, Expression then)
    {
        yield return self;
        yield return then();
    }

    
    public static Coroutine Start(this IEnumerator coroutine)
    {
        return coroutine.Start(Managers.Game);
    }

    public static Coroutine Start(this IEnumerator coroutine, MonoBehaviour script)
    {
        return script.StartCoroutine(coroutine);
    }

}
