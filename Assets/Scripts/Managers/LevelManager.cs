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
    [HideInInspector]
    public Vector3 PlayerSpawn
    {
        get
        {
            return playerSpawns.Current;
        }
        private set
        {
            playerSpawns.MoveNext();
        }
    }

    private List<Resetable> resetables;
    private List<GhostInfo> ghostData;
    private IEnumerator<Vector3> playerSpawns; //converted + cycling
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
        resetables = new List<Resetable>();
        ghostData  = new List<GhostInfo>();

        playerSpawns = GetPlayerSpawnsEnumeration();
        playerSpawns.MoveNext();

        player = GameObject.FindWithTag("Player");
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


    /* Manage Spawn Points */

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



    /* Manage Subscriptions */

    public void RegisterResetable(Resetable resetable)
    {
        this.resetables.Add(resetable);
    }

    public bool UnregisterResetable(Resetable resetable)
    {
        return this.resetables.Remove(resetable);
    }


    /* Functionality */

    // Player was caught! Wrap the level back to starting positions
    public void Warp()
    {
        Managers.Turn.TurnInProgress = true;

        Managers.UI.Hit();
        int delay = Managers.Level.Stats.DeathCount++;
        AddGhost(delay);

        playerSpawns.MoveNext();

        StartCoroutine(ResetAll()
            .Then(() => StartGhostSpawners())
            .Then(() => Managers.Turn.TurnInProgress = false));
    }

    // Player finished the level!
    public void Complete()
    {
        Managers.UI.ShowPostScreen();
    }


    /* Helper methods */

    void AddGhost(int turnDelay)
    {
        Trajectory targetTrajectory = player.GetComponent<Movement>().trajectory.Clone();

        Vector3 spawnPoint = playerSpawns.Current;

        ghostData.Add(new GhostInfo(turnDelay, spawnPoint, targetTrajectory));
    }

    IEnumerator ResetAll()
    {
        foreach (Resetable thing in resetables)
            thing.Reset();

        yield return new WaitForSeconds(actorFadeOutDuration);
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
