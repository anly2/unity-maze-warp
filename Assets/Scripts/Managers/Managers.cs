using UnityEngine;
using System.Collections;

public static class Managers {
    public static GameManager Game
    {
        get
        {
            return GameManager.instance;
        }
    }

    public static UIManager UI
    {
        get
        {
            return UIManager.instance;
        }
    }

    public static TurnManager Turn
    {
        get
        {
            return TurnManager.instance;
        }
    }

    public static LevelManager Level
    {
        get
        {
            return LevelManager.instance;
        }
    }

    /*
    public static FogManager Fog
    {
        get
        {
            return FogManager.instance;
        }
    }
    */
}
