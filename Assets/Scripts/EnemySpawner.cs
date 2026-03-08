using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public Enemy basicEnemy;
    public Enemy fastEnemy;
    public Enemy tankEnemy;

    [Header("Target")]
    public Transform castleTarget;

    [Header("Endless Spawn")]
    public float baseSpawnInterval = 1.0f;
    public float spawnRadius = 8f;

    [Header("Massive Wave")]
    public int massiveWaveSize = 15;
    public float timeBetweenMassiveWaves = 30f;
    public float timeBetweenWaveSpawns = 0.15f;

    [Header("Unlock Times")]
    public float fastUnlockTime = 30f;
    public float tankUnlockTime = 90f;

    [Header("Scaling")]
    public float hpMultiplier = 1f;
    public float hpGrowthRate = 0.15f;
    public float scalingInterval = 30f;

    [Header("Wave Rewards")]
    public double waveClearReward = 25;
    public double waveClearRewardGrowth = 1.25;

    [Header("Wave UI")]
    public TMP_Text waveStatusText;
    public float clearedMessageDuration = 2f;

    float clearedMessageTimer = 0f;

    bool waveRewardGranted = false;
    bool waveActive = false;

    float spawnTimer = 0f;
    float elapsedTime = 0f;
    float waveTimer = 0f;
    float scalingTimer = 0f;

    bool spawningMassiveWave = false;

    public List<Enemy> currentWaveEnemies = new List<Enemy>();

    void Update()
    {
        elapsedTime += Time.deltaTime;

        HandleScaling();
        CleanupWaveEnemyList();

        if (!spawningMassiveWave)
        {
            HandleEndlessSpawn();

            waveTimer += Time.deltaTime;
            if (waveTimer >= timeBetweenMassiveWaves)
            {
                waveTimer = 0f;
                StartCoroutine(SpawnMassiveWave());
            }
        }

        CheckWaveCleared();
        UpdateWaveUI();
    }

    void HandleScaling()
    {
        scalingTimer += Time.deltaTime;
        if (scalingTimer >= scalingInterval)
        {
            scalingTimer = 0f;
            hpMultiplier += hpGrowthRate;
            baseSpawnInterval = Mathf.Max(0.2f, baseSpawnInterval * 0.95f);
            massiveWaveSize += 3;
            waveClearReward *= waveClearRewardGrowth;
        }
    }

    void HandleEndlessSpawn()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= baseSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnSingleEnemy();
        }
    }

    IEnumerator SpawnMassiveWave()
    {
        spawningMassiveWave = true;
        waveRewardGranted = false;
        currentWaveEnemies.Clear();

        waveStatusText.text = "Massive Wave Incoming!";

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < massiveWaveSize; i++)
        {
            Enemy enemy = SpawnSingleEnemy(true);
            if (enemy != null)
                currentWaveEnemies.Add(enemy);

            yield return new WaitForSeconds(timeBetweenWaveSpawns);
        }

        CleanupWaveEnemyList();

        // Mark wave active only after enemies exist
        waveActive = currentWaveEnemies.Count > 0;

        spawningMassiveWave = false;
    }

    void CleanupWaveEnemyList()
    {
        currentWaveEnemies.RemoveAll(enemy => enemy == null);
    }

    public int GetAliveWaveEnemyCount()
    {
        CleanupWaveEnemyList();
        return currentWaveEnemies.Count;
    }

    Enemy SpawnSingleEnemy(bool isWaveEnemy = false)
    {
        Vector3 center = castleTarget.position;

        float rad = Random.Range(0f, 2f * Mathf.PI);
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        Vector3 spawnPos = center + dir * spawnRadius;

        Enemy prefab = ChooseEnemyType();

        Enemy enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemy.target = castleTarget;
        enemy.hp = Mathf.RoundToInt(enemy.hp * hpMultiplier);
        enemy.isMassiveWaveEnemy = isWaveEnemy;

        return enemy;
    }

    Enemy ChooseEnemyType()
    {
        float roll = Random.value;

        if (elapsedTime < fastUnlockTime)
            return basicEnemy;

        if (elapsedTime < tankUnlockTime)
        {
            if (roll < 0.7f)
                return basicEnemy;
            else
                return fastEnemy;
        }

        if (roll < 0.55f)
            return basicEnemy;
        else if (roll < 0.80f)
            return fastEnemy;
        else
            return tankEnemy;
    }

    void CheckWaveCleared()
    {
        if (!waveActive) return;
        if (waveRewardGranted) return;

        CleanupWaveEnemyList();

        if (currentWaveEnemies.Count == 0)
        {
            waveRewardGranted = true;
            waveActive = false;
            clearedMessageTimer = clearedMessageDuration;

            if (GameManager.Instance != null)
                GameManager.Instance.AddCoins(waveClearReward);

            if (waveStatusText != null)
                waveStatusText.text = "Wave Cleared! +" + FormatWaveReward(waveClearReward) + " Coins";
        }
    }

    void UpdateWaveUI()
    {
        if (waveActive)
        {
            CleanupWaveEnemyList();
            waveStatusText.text = "Wave Enemies Left: " + currentWaveEnemies.Count;
            return;
        }

        if (clearedMessageTimer > 0f)
        {
            clearedMessageTimer -= Time.deltaTime;
            if (clearedMessageTimer <= 0f)
                waveStatusText.text = "";
        }
    }

    string FormatWaveReward(double value)
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.FormatNumber(value);

        return value.ToString("0");
    }
}