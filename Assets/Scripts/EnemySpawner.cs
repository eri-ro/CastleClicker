using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab;
    public Transform castleTarget; 
    public float spawnInterval = 1.0f;
    public float spawnRadius = 8f;     // distance from castle
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 center = castleTarget.position;

        float rad = Random.Range(0f, 2f * Mathf.PI);

        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        Vector3 spawnPos = center + dir * spawnRadius;

        Enemy enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        enemy.target = castleTarget;
    }
}