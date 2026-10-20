
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuracion de enemigos")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemiesToSpawn = 10;
    [SerializeField] private int maxAliveEnemies = 3;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Puntos de aparicion")]
    [SerializeField] private Transform[] spawnPoints;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalSpawned;

    private void Start()
    {
        maxEnemiesToSpawn = Mathf.Max(0, maxEnemiesToSpawn);
        maxAliveEnemies = Mathf.Max(1, maxAliveEnemies);
        spawnInterval = Mathf.Max(0.1f, spawnInterval);

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (totalSpawned < maxEnemiesToSpawn)
        {
            aliveEnemies.RemoveAll(enemy => enemy == null);

            if (aliveEnemies.Count < maxAliveEnemies)
            {
                SpawnRandomEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No hay prefabs asignados.");
            return;
        }

        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
            }
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: Los prefabs asignados son nulos.");
            return;
        }

        GameObject selectedPrefab =
            validPrefabs[Random.Range(0, validPrefabs.Count)];

        Transform selectedPoint = transform;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            List<Transform> validPoints = new List<Transform>();

            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    validPoints.Add(point);
                }
            }

            if (validPoints.Count > 0)
            {
                selectedPoint =
                    validPoints[Random.Range(0, validPoints.Count)];
            }
        }

        GameObject enemy = Instantiate(
            selectedPrefab,
            selectedPoint.position,
            selectedPoint.rotation
        );

        aliveEnemies.Add(enemy);
        totalSpawned++;

        Debug.Log($"Enemigo generado: {enemy.name}. Total: {totalSpawned}/{maxEnemiesToSpawn}");
    }
}
