using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public Enemy basicEnemy;
    public Enemy fastEnemy;
    public Enemy tankEnemy;

    [Header("Target")]
    public Transform castleTarget;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.0f;
    public float spawnRadius = 8f;

    float timer;

    [Header("Progression")]
    public float fastUnlockTime = 30f;
    public float tankUnlockTime = 90f;

    float elapsedTime = 0f;

    [Header("Scaling")]
    public float hpMultiplier = 1f;
    public float hpGrowthRate = 0.15f;
    public float scalingInterval = 30f;

    float scalingTimer = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // progressive scaling
        scalingTimer += Time.deltaTime;
        if (scalingTimer >= scalingInterval)
        {
            scalingTimer = 0f;
            hpMultiplier += hpGrowthRate;

            // spawn acceleration
            spawnInterval = Mathf.Max(0.25f, spawnInterval * 0.95f);
        }

        // spawn timer
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

        Enemy prefab = ChooseEnemyType();

        Enemy enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        enemy.target = castleTarget;

        // apply scaling
        enemy.hp = Mathf.RoundToInt(enemy.hp * hpMultiplier);
    }

    Enemy ChooseEnemyType()
    {
        float roll = Random.value;

        // only basic early game
        if (elapsedTime < fastUnlockTime)
        {
            return basicEnemy;
        }

        // basic + fast
        if (elapsedTime < tankUnlockTime)
        {
            if (roll < 0.7f)
                return basicEnemy;
            else
                return fastEnemy;
        }

        // all types
        if (roll < 0.55f)
            return basicEnemy;
        else if (roll < 0.80f)
            return fastEnemy;
        else
            return tankEnemy;
    }
}