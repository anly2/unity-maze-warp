﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float turnDuration;
    public List<Transform> playerSpawnPlaces;
    public int monkeySpawnTurnDelay = 3;
    public GameObject monkeyObject;
    public GameObject ghostObject;
    public float actorFadeOutDuration = 1;

    private bool turnInProgress = true;
    private List<TurnBased> turnListeners;
    private List<GhostInfo> ghostData;
    private IEnumerator<Vector3> playerSpawnPoints;

    private GameObject player; //for easy access

    class GhostInfo
    {
        public int turnDelay;
        public Trajectory trajectory;
        public Vector3 spawnPoint;

        public GhostInfo(int turnDelay, Vector3 spawnPoint, Trajectory trajectory)
        {
            this.turnDelay = turnDelay;
            this.trajectory = trajectory;
            this.spawnPoint = spawnPoint;
        }
    }


	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        turnListeners = new List<TurnBased>();
        ghostData = new List<GhostInfo>();

        playerSpawnPoints = GetPlayerSpawnsEnumeration();
        playerSpawnPoints.MoveNext();

        player = GameObject.FindWithTag("Player");
        ResetPlayer();

        turnInProgress = false;
    }

    IEnumerator<Vector3> GetPlayerSpawnsEnumeration()
    {
        IEnumerator<Transform> places = playerSpawnPlaces.GetEnumerator();
        while (places.MoveNext())
            yield return places.Current.position;
    }


    public void AddTurnBasedListener(TurnBased listener)
    {
        turnListeners.Add(listener);
    }

    public bool RemoveTurnBasedListener(TurnBased listener)
    {
        return turnListeners.Remove(listener);
    }
    

    public bool TurnInProgress()
    {
        return turnInProgress;
    }
    
    public bool TakeTurn()
    {
        if (turnInProgress)
            return false;

        turnInProgress = true;
        StartCoroutine(NotifyTurnListeners());
        Invoke("EndTurn", turnDuration);
        return true;
    }

    void EndTurn()
    {
        turnInProgress = false;
    }

    IEnumerator NotifyTurnListeners()
    {
        foreach (TurnBased listener in turnListeners)
        {
            listener.Turn();
        }

        yield return 0;
    }


    // Player was caught! Wrap the level back to starting positions
    public void Warp()
    {
        turnInProgress = true;
        Debug.Log("DEAD -- SHOULD WARP THE SCENE");

        FlashRed();
        
        AddGhost(1); //#! death count +1

        //#!increment death count
        playerSpawnPoints.MoveNext();

        StartCoroutine(ResetActors());
        StartGhostSpawners();

        turnInProgress = false;
    }

    void FlashRed() { } //UI Manager?

    IEnumerator ResetActors()
    {
        //fade out player, ghosts, monkeys
        List<GameObject> actors = GetActors();
        foreach (GameObject actor in actors)
            StartCoroutine(FadeOut(actor));

        yield return new WaitForSeconds(actorFadeOutDuration);

        //reset player
        ResetPlayer();

        //destroy all the rest
        actors.Remove(player);
        foreach (GameObject actor in actors)
            Destroy(actor);

        yield break;
    }
    List<GameObject> GetActors()
    {
        List<GameObject> actors = new List<GameObject>();
        actors.Add(player);
        actors.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        actors.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
        return actors;
    }
    IEnumerator FadeOut(GameObject actor)
    {
        SpriteRenderer sprite = actor.GetComponent<SpriteRenderer>();

        if (sprite == null)
            return Animation.Empty;

        Color startColor = sprite.color;
        Color endColor = startColor;
        endColor.a = 0;

        return new Animation(delegate (float p) {
            sprite.color = Color.Lerp(startColor, endColor, p);
        }, actorFadeOutDuration);
    }

    void ResetPlayer()
    {
        //position
        player.transform.position = playerSpawnPoints.Current;
        Trajectory traj = player.GetComponent<PlayerMovement>().GetTrajectory();
        if (traj != null) traj.Clear();
        
        //color
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        Color c = playerSprite.color;
        c.a = 1;
        playerSprite.color = c;

        //chaser spawner
        ChaserSpawner spawner = player.AddComponent<ChaserSpawner>() as ChaserSpawner;
        spawner.chaserObject = monkeyObject;
        spawner.spawnTurnDelay = monkeySpawnTurnDelay;
    }

    void AddGhost(int turnDelay)
    {
        Trajectory targetTrajectory = player.GetComponent<Movement>().GetTrajectory().Clone();

        Vector3 spawnPoint = playerSpawnPoints.Current;

        ghostData.Add(new GhostInfo(turnDelay, spawnPoint, targetTrajectory));
    }

    void StartGhostSpawners()
    {
        foreach (GhostInfo ghostInfo in ghostData)
        {
            ChaserSpawner spawner = gameObject.AddComponent<ChaserSpawner>() as ChaserSpawner;
            spawner.chaserObject = ghostObject;
            spawner.spawnPosition = ghostInfo.spawnPoint;
            spawner.spawnTurnDelay = ghostInfo.turnDelay;
            spawner.targetTrajectory = ghostInfo.trajectory;
        }
    }

    //#!door: open makes it not collidable
}
