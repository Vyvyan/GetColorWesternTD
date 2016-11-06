using UnityEngine;
using System.Collections;

public class GhostBlockade : MonoBehaviour {

    GameManager gameManager;
    Renderer rend;

	// Use this for initialization
	void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rend.enabled = gameManager.buildMode;
	}
}
