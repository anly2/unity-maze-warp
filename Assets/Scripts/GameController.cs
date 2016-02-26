using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float turnDuration;
    public List<Transform> playerSpawnPlaces;
    public GameObject ghostObject;
    public float actorFadeOutDuration = 1;

    private bool turnInProgress;
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

        turnInProgress = false;
        turnListeners = new List<TurnBased>();
        ghostData = new List<GhostInfo>();
        playerSpawnPoints = GetPlayerSpawnsEnumeration();
        playerSpawnPoints.MoveNext();

        player = GameObject.FindWithTag("Player");
        player.transform.position = playerSpawnPoints.Current;
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
        player.transform.position = playerSpawnPoints.Current;
        //color
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        Color c = playerSprite.color;
        c.a = 1;
        playerSprite.color = c;

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
            yield break;


        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        yield break;
        /*
        Color startColor = sprite.color;
        Color endColor = startColor;
        endColor.a = 0;
        Color currentColor = startColor;
        float speed = 1 / actorFadeOutDuration;

        while (currentColor.a > float.Epsilon)
        {
            sprite.color = Color.Lerp(startColor, endColor, speed*Time.deltaTime);
            yield return null;
        }
        */
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
