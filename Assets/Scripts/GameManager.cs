using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public int waveNumber;
    public EnemySpawner[] enemySpawners;
    public GameObject groundEnemy;
    public enum GameState { PreGame, SpawningEnemies, Dead, Win, InBetweenWaves, AllEnemiesSpawnedThisWave};
    public GameState gameState;
    public int spawnIndex;
    public static int numberOfEnemiesAlive;

    // wave variables
    public float spawnRate, spawnRateTimer;

	// Use this for initialization
	void Start ()
    {
        gameState = GameState.PreGame;
        enemySpawners = GameObject.FindObjectsOfType<EnemySpawner>();
        waveNumber = 0;
        spawnIndex = 0;
        numberOfEnemiesAlive = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // PRE GAME
        if (gameState == GameState.PreGame)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                gameState = GameState.SpawningEnemies;
            }
        }
        // SPAWNING ENEMIES
	    else if (gameState == GameState.SpawningEnemies)
        {
            // spawn enemies
            if (spawnRateTimer > 0)
            {
                spawnRateTimer -= Time.deltaTime;
            }
            else
            {
                // this keeps track of how many spawners are done spawning
                int expiredSpawners = 0;
                // for each spawner, get the wave string
                foreach(EnemySpawner spawner in enemySpawners)
                {
                    string tempWave = spawner.WaveStrings[waveNumber];
                    Debug.Log("Yo." + spawner.name.ToString());
                    if (spawnIndex + 1 <= tempWave.Length)
                    {
                        SpawnEnemy(tempWave[spawnIndex], spawner.gameObject);
                    }
                    else
                    {
                        expiredSpawners++;
                    }                 
                }
                // increase the spawn index
                spawnIndex++;
                //reset the timer
                spawnRateTimer = spawnRate;

                // if all spawners are expired, then we go to the next wave, it's greater than because idk, fail safe?
                if (expiredSpawners >= enemySpawners.Length)
                {
                    gameState = GameState.AllEnemiesSpawnedThisWave;
                }
            }
        }
        // ALL ENEMIES SPAWNED
        else if (gameState == GameState.AllEnemiesSpawnedThisWave)
        {
            // are we on the last wave?
            if (waveNumber == 29)
            {
                gameState = GameState.Win;
            }
            // all the enemies are dead, so we switch modes
            else if (numberOfEnemiesAlive <= 0)
            {
                // this starts the coroutine, the beginning of the coroutine changes game states, so this only actually fires once
                StartCoroutine(inBetweenWavesTimer());
            }
        }
	}

    IEnumerator inBetweenWavesTimer()
    {
        Debug.Log("In between waves!");
        gameState = GameState.InBetweenWaves;
        yield return new WaitForSeconds(10);
        Debug.Log("NEXT WAVE!");
        NextWave();
    }

    void SpawnEnemy(char enemyCode, GameObject enemySpawner)
    {
        if (enemyCode == '0')
        {
            return;
        }
        else if (enemyCode == '1')
        {
            Instantiate(groundEnemy, enemySpawner.transform.position, enemySpawner.transform.rotation);
            numberOfEnemiesAlive++;
        }
    }

    void NextWave()
    {
        if (waveNumber < 29)
        {
            waveNumber++;
        }
        spawnIndex = 0;
        gameState = GameState.SpawningEnemies;
    }

    void OnGUI()
    {
        if (gameState == GameState.PreGame)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 600, 600), "Press Space To Start");
        }
    }
}
