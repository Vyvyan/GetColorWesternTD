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
    public Tower[] towers;
    Tower selectedTower, lastFrameSelectedTower;
    public int towerIndex;

    // wave variables
    public float spawnRate, spawnRateTimer;

	// Use this for initialization
	void Start ()
    {
        gameState = GameState.PreGame;
        enemySpawners = GameObject.FindObjectsOfType<EnemySpawner>();
        towers = GameObject.FindObjectsOfType<Tower>();
        selectedTower = towers[0];
        waveNumber = 0;
        spawnIndex = 0;
        numberOfEnemiesAlive = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // switching between towers with keys
        if (Input.GetKeyUp(KeyCode.E))
        { 
            if (towerIndex + 1 > towers.Length - 1)
            {
                towerIndex = 0;
                selectedTower = towers[towerIndex];
            }
            else
            {
                towerIndex++;
                selectedTower = towers[towerIndex];
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (towerIndex - 1 < 0)
            {
                towerIndex = towers.Length - 1;
                selectedTower = towers[towerIndex];
            }
            else
            {
                towerIndex--;
                selectedTower = towers[towerIndex];
            }
        }
        // switch towers on click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    selectedTower = hit.collider.gameObject.GetComponent<Tower>();
                }
            }
        }

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

    void LateUpdate()
    {
        // if we just switched from a tower, then change which tower we have selected visually
        if (selectedTower != lastFrameSelectedTower)
        {
            ChangeSelectedTowerVisualsAndMovement();
        }
        
        // update our lastframe tower
        lastFrameSelectedTower = selectedTower;
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

    void ChangeSelectedTowerVisualsAndMovement()
    {
        // turn off all range indicators
        foreach (Tower tempTower in towers)
        {
            tempTower.projector.enabled = false;
            tempTower.GetComponent<MoveToClick>().enabled = false;
        }
        // now turn on the tower that is selected 
        selectedTower.projector.enabled = true;
        selectedTower.GetComponent<MoveToClick>().enabled = true;
    }
}
