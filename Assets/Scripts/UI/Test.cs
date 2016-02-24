using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public static Test instance = null;

    public GameObject HUD;

    UIManager uiManager;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        uiManager = HUD.GetComponent<UIManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            uiManager.GameStart();
    }
}
