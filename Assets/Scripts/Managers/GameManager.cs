using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public float turnDuration;
    public List<Transform> playerSpawnPlaces;
    public int monkeySpawnTurnDelay = 3;
    public GameObject monkeyObject;
    public GameObject ghostObject;
    public float actorFadeOutDuration = 1;

    public bool TurnInProgress { get; private set; }
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
        TurnInProgress = true;

        DontDestroyOnLoad(gameObject);

        turnListeners = new List<TurnBased>();
        ghostData = new List<GhostInfo>();

        playerSpawnPoints = GetPlayerSpawnsEnumeration();
        playerSpawnPoints.MoveNext();

        player = GameObject.FindWithTag("Player");
        ResetPlayer();

        TurnInProgress = false;
    }

    IEnumerator<Vector3> GetPlayerSpawnsEnumeration()
    {
        IEnumerator<Transform> places = playerSpawnPlaces.GetEnumerator();

        while (true)
        {
            while (places.MoveNext())
                yield return places.Current.position;

            places.Reset();
        }
    }


    public void AddTurnBasedListener(TurnBased listener)
    {
        turnListeners.Add(listener);
    }

    public bool RemoveTurnBasedListener(TurnBased listener)
    {
        return turnListeners.Remove(listener);
    }
    
    public bool TakeTurn()
    {
        if (TurnInProgress)
            return false;

        TurnInProgress = true;
        StartCoroutine(NotifyTurnListeners());
        Invoke("EndTurn", turnDuration);
        return true;
    }

    void EndTurn()
    {
        TurnInProgress = false;
    }

    IEnumerator NotifyTurnListeners()
    {
        foreach (TurnBased listener in turnListeners)
        {
            listener.Turn();
        }

        yield break;
    }


    // Player was caught! Wrap the level back to starting positions
    public void Warp()
    {
        CancelInvoke("EndTurn");
        TurnInProgress = true;

        UIManager.instance.Hit();
        int delay = LevelManager.instance.Stats.DeathCount++;
        AddGhost(delay);

        //#!increment death count
        playerSpawnPoints.MoveNext();

        StartCoroutine(ResetActors()
            .Then(() => StartGhostSpawners())
            .Then(() => TurnInProgress = false));
    }

    void FlashRed() { } //UI Manager?

    IEnumerator ResetActors()
    {
        //fade out player, ghosts, monkeys
        List<GameObject> actors = GetActors();
        foreach (GameObject actor in actors)
            FadeOut(actor);

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
    void FadeOut(GameObject actor)
    {
        SpriteRenderer sprite = actor.GetComponent<SpriteRenderer>();

        if (sprite == null)
            return;

        Color startColor = sprite.color;
        Color endColor = startColor;
        endColor.a = 0;

        StartCoroutine(new Animation(delegate (float p) {
            sprite.color = Color.Lerp(startColor, endColor, p);
        }, actorFadeOutDuration));
    }

    void ResetPlayer()
    {
        //position
        player.transform.position = playerSpawnPoints.Current;
        player.GetComponent<PlayerMovement>().trajectory.Clear();
        
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
        Trajectory targetTrajectory = player.GetComponent<Movement>().trajectory.Clone();

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
