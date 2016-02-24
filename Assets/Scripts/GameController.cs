using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject HUD;

    GameOverManager gameOverManager;

    void Awake()
    {
        gameOverManager = HUD.GetComponent<GameOverManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            gameOverManager.GameOver();
    }
}
