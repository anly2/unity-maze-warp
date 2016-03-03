using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    [Header("Global Settings")]
    public float actorFadeOutDuration = 1f;

    [Header("Ad hoc 'Main Menu'")]
    public GameObject firstLevel;


	void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Start()
    {
        //Logic could change to have a 'main menu'
        //Then LoadLevel will be called through the menu

        if (firstLevel != null)
            LoadLevel(firstLevel);
    }

    public void LoadLevel(GameObject level)
    {
        //#! Need to do clean-up for when another level already loaded

        Instantiate(level);
    }
}
