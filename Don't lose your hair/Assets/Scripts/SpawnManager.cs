using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector2 spawnPosJump = new Vector2(20f, -2f);
    [SerializeField] private Vector2 spawnPosCrouch = new Vector2(20f, 0.8f);
    [SerializeField] private Vector2 spawnPosShoot = new Vector2(20f, 0.55f);
    [SerializeField] private Vector2 spawnPosFlyingEnemy = new Vector2(19f, 3f);
    [SerializeField] private float obstacleSpawnRate = 3f;
    [SerializeField] private float enemySpawnRate = 5f;
    [SerializeField] private int enemyWaveTracker = 1;

    [SerializeField] private bool enemySpawnStarted = false;

    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.startEnemyWave += OnEnemySpawnSignal;
        GameManager.instance.playerLost += OnPlayerDie;
      
        StartCoroutine(SpawningObstacles());
    }

    private void OnPlayerDie() // stops coroutines when player lost
    {
        StopAllCoroutines();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObstacle() // spawns a random obstacle
    {
        Vector2 spawnPos = new Vector2();

        int index = UnityEngine.Random.Range(0, obstaclePrefabs.Length);

        if (obstaclePrefabs[index].name == "Obstacle_Jump")
        {
            spawnPos = spawnPosJump;
        }
        else if(obstaclePrefabs[index].name == "Obstacle_Shoot")
        {
            spawnPos = spawnPosShoot;
        }
        else if (obstaclePrefabs[index].name == "Obstacle_Crouch")
        {
            spawnPos = spawnPosCrouch;
        }

        Instantiate(obstaclePrefabs[index], spawnPos, obstaclePrefabs[index].transform.rotation);
    }

    void SpawnEnemy() // spawn random enemy
    {

        Vector2 spawnPos = new Vector2();

        int index = UnityEngine.Random.Range(0, enemyPrefabs.Length);

        if (enemyPrefabs[index].name == "Enemy_Flying")
        {
            spawnPos = spawnPosFlyingEnemy;
        }

        Instantiate(enemyPrefabs[index], spawnPos, enemyPrefabs[index].transform.rotation);

        GameManager.instance.enemyCount++;

    }

    private void OnEnemySpawnSignal() // is called, when condition for enemies is met
    {
        if (!enemySpawnStarted)
        {
            enemySpawnStarted = true;
            StartCoroutine(SpawningEnemies(enemyWaveTracker));
        }
    }

    IEnumerator SpawningEnemies(int waveNumber) // starts enemy spawning with given rate and number of enemies
    {
        
        int enemiesToSpawn = waveNumber * 2;

        while(GameManager.instance.enemyCount < enemiesToSpawn)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(enemySpawnRate);
        }
        enemySpawnStarted = false;
        enemyWaveTracker += 1;
        GameManager.instance.enemyCount = 0;

    }

    IEnumerator SpawningObstacles() // spawns obstacles as long as game isn't over
    {
        while (!GameManager.instance.gameOver)
        {
            yield return new WaitForSeconds(obstacleSpawnRate);
            SpawnObstacle();
        }
    }
}
