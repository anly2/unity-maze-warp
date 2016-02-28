using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    public string Name;
    public List<Transform> playerSpawnPlaces;
    public int monkeySpawnTurnDelay = 3;
    public GameObject monkeyObject;
    public GameObject ghostObject;
    public float actorFadeOutDuration = 1;

    [HideInInspector]
    public Statistics Stats { get; private set; }

    private List<GhostInfo> ghostData;
    private IEnumerator<Vector3> playerSpawnPoints; //converted+
    private GameObject player; //for easy access


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Stats = new Statistics();
        ghostData = new List<GhostInfo>();

        playerSpawnPoints = GetPlayerSpawnsEnumeration();
        playerSpawnPoints.MoveNext();

        player = GameObject.FindWithTag("Player");
        ResetPlayer();
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Start() {
        Managers.UI.ShowPreScreen();
    }


    /* Inner Classes */

    public class Statistics
    {
        public int DeathCount = 0;
        public int TurnCount = 0;

        private float _startTime = Time.time;
        public float TimeElapsed
        {
            get {
                return Time.time - _startTime;
            }

            internal set {
                _startTime = value;
            }
        }
    }

    private struct GhostInfo
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


    /* Functionality */

    // Player was caught! Wrap the level back to starting positions
    public void Warp()
    {
        Managers.Turn.TurnInProgress = true;

        Managers.UI.Hit();
        int delay = Managers.Level.Stats.DeathCount++;
        AddGhost(delay);

        //#!increment death count
        playerSpawnPoints.MoveNext();

        StartCoroutine(ResetActors()
            .Then(() => StartGhostSpawners())
            .Then(() => Managers.Turn.TurnInProgress = false));
    }

    // Player finished the level!
    public void Complete()
    {
        Managers.UI.ShowPostScreen();
    }


    /* Helper methods */

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
        c.a = 1; //#! potentially different than 1
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
}
