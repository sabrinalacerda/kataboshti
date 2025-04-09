using UnityEngine;

public class SpawnManager : MonoBehaviour
{ public GameObject[] spawnShits;
    private float spawnTime;
    private Vector3 spawnPos = new Vector3(-2,3,15);

    private BossController bossControllerScript;
    private int lastBossLife;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossControllerScript = GameObject.Find("Boss").GetComponent<BossController>();
        lastBossLife = bossControllerScript.life;
        UpdateSpawnDifficulty();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bossControllerScript.life != lastBossLife) // Verifica se a vida do boss mudou
        {
            lastBossLife = bossControllerScript.life; // Atualiza a vida conhecida
            UpdateSpawnDifficulty(); // Atualiza a dificuldade
        }
        
    }
    private void SpawnRandomPrefab()
    {
        int prefabIndex = Random.Range(0, spawnShits.Length);
        // Generate a random index to select a prefab from the array
        Instantiate(spawnShits[prefabIndex], spawnPos, spawnShits[prefabIndex].transform.rotation);
        
        // Instantiate the selected prefab at the current position and rotation
    }
    private void UpdateSpawnDifficulty()
    {
        CancelInvoke("SpawnRandomPrefab");
        if (bossControllerScript.life == 3)
        {
            spawnTime = 2.0f;
            InvokeRepeating("SpawnRandomPrefab", 0, spawnTime);
            Debug.Log("SpawnDifficulty: " + spawnTime);
        }
        else if (bossControllerScript.life == 2)
        {
            spawnTime = 1.0f;
            InvokeRepeating("SpawnRandomPrefab", 0, spawnTime);
            Debug.Log("SpawnDifficulty: " + spawnTime);
        }
        else if (bossControllerScript.life == 1)
        {
            spawnTime = 0.5f;
            InvokeRepeating("SpawnRandomPrefab", 0, spawnTime);
            Debug.Log("SpawnDifficulty: " + spawnTime);
        }
        else if (bossControllerScript.life <= 0)
        {
            spawnTime = 0.0f;
            StopSpawn();
            Debug.Log("SpawnDifficulty: " + spawnTime);
        }
    }
    private void StopSpawn()
    {
        spawnTime = 0.0f;
        CancelInvoke("SpawnRandomPrefab");
    }
  
    
}
