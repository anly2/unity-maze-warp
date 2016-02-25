using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float turnDuration;
    public List<Transform> playerSpawnPlaces;
    public GameObject ghostObject;

    private bool turnInProgress;
    private List<TurnBased> turnListeners;
    private List<GhostInfo> ghostData;
    private IEnumerator<Vector3> playerSpawnPoints;

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

        turnInProgress = false;
        turnListeners = new List<TurnBased>();
        ghostData = new List<GhostInfo>();
        playerSpawnPoints = GetPlayerSpawnsEnumeration();
        playerSpawnPoints.MoveNext();

        GameObject.FindWithTag("Player").transform.position = playerSpawnPoints.Current;
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
        
        AddGhost(1);

        //#!increment death count
        playerSpawnPoints.MoveNext();

        ResetActors();
        StartGhostSpawners();

        turnInProgress = false;
    }

    void FlashRed() { } //UI Manager?

    void ResetActors()
    {
        //fade out player, ghosts, monkeys
        //reset player position
        //destroy? all the rest
    }

    void AddGhost(int turnDelay)
    {
        GameObject player = GameObject.FindWithTag("Player");
        Trajectory targetTrajectory = player.GetComponent<Movement>().GetTrajectory();

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
