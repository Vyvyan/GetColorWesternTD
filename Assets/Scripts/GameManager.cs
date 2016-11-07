using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int waveNumber;
    public EnemySpawner[] enemySpawners;
    public GameObject groundEnemy_Prefab, undergroundEnemy_Prefab;
    public enum GameState { PreGame, SpawningEnemies, Dead, Win, InBetweenWaves, AllEnemiesSpawnedThisWave};
    public GameState gameState;
    public int spawnIndex;
    public static int numberOfEnemiesAlive;
    public Tower[] towers;
    Tower selectedTower, lastFrameSelectedTower;
    public int towerIndex;
    public int numberOfBlockadges;
    public bool buildMode;
    public GameObject blockade;
    Projector buildModeGrid;
    GameObject ghostBlockadeObject;
    Vector3 ghostBlockadeHiddenPosition;

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
        numberOfBlockadges = 100;
        buildModeGrid = GameObject.FindGameObjectWithTag("buildModeGrid").GetComponent<Projector>();
        buildModeGrid.enabled = false;
        ghostBlockadeObject = GameObject.FindGameObjectWithTag("GhostBlockade");
        ghostBlockadeHiddenPosition = ghostBlockadeObject.transform.position;
        // load our second nav mesh
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
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
        
        // build mode stuff
        if (gameState == GameState.PreGame || gameState == GameState.InBetweenWaves)
        {
            if (Input.GetKeyUp(KeyCode.B))
            {
                buildMode = !buildMode;
            }

            if (buildMode)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "Ground")
                    {
                        Debug.Log("hitting ground");
                        ghostBlockadeObject.transform.position = new Vector3(SnapPosition(hit.point.x, 2), 3, SnapPosition(hit.point.z, 2));
                        
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            if (numberOfBlockadges > 0)
                            {
                                Instantiate(blockade, ghostBlockadeObject.transform.position, ghostBlockadeObject.transform.rotation);
                                numberOfBlockadges--;
                            }
                        }
                    }
                    // if we h it something that isn't the ground
                    else
                    {
                        ghostBlockadeObject.transform.position = ghostBlockadeHiddenPosition;
                    }
                }
                // if we hit nothing
                else
                {
                    ghostBlockadeObject.transform.position = ghostBlockadeHiddenPosition;
                }
            }
        }
        else
        {
            buildMode = false;
        }

        buildModeGrid.enabled = buildMode;
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
            Instantiate(groundEnemy_Prefab, enemySpawner.transform.position, enemySpawner.transform.rotation);
            numberOfEnemiesAlive++;
        }
        else if (enemyCode == '2')
        {
            Instantiate(undergroundEnemy_Prefab, enemySpawner.transform.position - (Vector3.up * .8f), enemySpawner.transform.rotation);
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

    float SnapPosition(float input, float factor)
    {
        if (factor <= 0f)
            throw new UnityException("factor argument must be above 0");

        float x = Mathf.Round(input / factor) * factor;

        return x;
    }
}
