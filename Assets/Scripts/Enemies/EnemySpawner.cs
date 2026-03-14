using System.Collections;
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

    [Header("Wave Modifiers")]
    public WaveModifier currentWaveModifier = WaveModifier.None;

    public enum WaveModifier
    {
        None,
        Armored,
        Swarm,
        Flying,
        Elite
    }

    bool waveRewardGranted = false;
    bool waveActive = false;
    bool spawningMassiveWave = false;

    float spawnTimer = 0f;
    float elapsedTime = 0f;
    float waveTimer = 0f;
    float scalingTimer = 0f;
    float clearedMessageTimer = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        HandleScaling();

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
        waveActive = false;

        EnemyRegistry.Instance.ClearCurrentWave();
        ChooseWaveModifier();

        int waveCount = massiveWaveSize;
        float waveSpawnDelay = timeBetweenWaveSpawns;

        switch (currentWaveModifier)
        {
            case WaveModifier.Swarm:
                waveCount = Mathf.RoundToInt(massiveWaveSize * 1.75f);
                waveSpawnDelay *= 0.6f;
                break;

            case WaveModifier.Elite:
                waveCount = Mathf.Max(3, Mathf.RoundToInt(massiveWaveSize * 0.5f));
                break;
        }

        waveStatusText.text = GetWaveModifierLabel() + " Incoming!";
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < waveCount; i++)
        {
            Enemy enemy = SpawnSingleEnemy(true);
            EnemyRegistry.Instance.AddToCurrentWave(enemy);

            yield return new WaitForSeconds(waveSpawnDelay);
        }

        EnemyRegistry.Instance.CleanupNulls();
        waveActive = EnemyRegistry.Instance.GetAliveWaveEnemyCount() > 0;
        spawningMassiveWave = false;
    }

    Enemy SpawnSingleEnemy(bool isWaveEnemy = false)
    {
        Vector3 center = castleTarget.position;

        float rad = Random.Range(0f, 2f * Mathf.PI);
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        Vector3 spawnPos = center + dir * spawnRadius;

        Enemy prefab = ChooseEnemyType();

        Enemy enemy = Instantiate(
            prefab,
            spawnPos,
            Quaternion.identity,
            SceneContainers.Instance.enemies
        );

        enemy.target = castleTarget;
        enemy.hp = Mathf.RoundToInt(enemy.hp * hpMultiplier);
        enemy.isMassiveWaveEnemy = isWaveEnemy;

        if (isWaveEnemy)
            ApplyWaveModifier(enemy);

        return enemy;
    }

    Enemy ChooseEnemyType()
    {
        if (currentWaveModifier == WaveModifier.Flying)
            return fastEnemy;

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

        if (EnemyRegistry.Instance.GetAliveWaveEnemyCount() == 0)
        {
            waveRewardGranted = true;
            waveActive = false;
            clearedMessageTimer = clearedMessageDuration;

            GameManager.Instance.AddCoins(waveClearReward);
            waveStatusText.text = "Wave Cleared! +" + FormatWaveReward(waveClearReward) + " Coins";
        }
    }

    void UpdateWaveUI()
    {
        if (waveActive)
        {
            waveStatusText.text =
                GetWaveModifierLabel() + " - Enemies Left: " +
                EnemyRegistry.Instance.GetAliveWaveEnemyCount();
            return;
        }

        if (clearedMessageTimer > 0f)
        {
            clearedMessageTimer -= Time.deltaTime;
            if (clearedMessageTimer <= 0f)
                waveStatusText.text = "";
        }
    }

    void ChooseWaveModifier()
    {
        int roll = Random.Range(0, 4);

        switch (roll)
        {
            case 0:
                currentWaveModifier = WaveModifier.Armored;
                break;
            case 1:
                currentWaveModifier = WaveModifier.Swarm;
                break;
            case 2:
                currentWaveModifier = WaveModifier.Flying;
                break;
            case 3:
                currentWaveModifier = WaveModifier.Elite;
                break;
            default:
                currentWaveModifier = WaveModifier.None;
                break;
        }
    }

    string GetWaveModifierLabel()
    {
        switch (currentWaveModifier)
        {
            case WaveModifier.Armored: return "Armored Wave";
            case WaveModifier.Swarm: return "Swarm Wave";
            case WaveModifier.Flying: return "Flying Wave";
            case WaveModifier.Elite: return "Elite Wave";
            default: return "Wave";
        }
    }

    void ApplyWaveModifier(Enemy enemy)
    {
        switch (currentWaveModifier)
        {
            case WaveModifier.Armored:
                enemy.hp = Mathf.RoundToInt(enemy.hp * 2f);
                enemy.coinReward *= 1.5;
                break;

            case WaveModifier.Swarm:
                enemy.hp = Mathf.Max(1, Mathf.RoundToInt(enemy.hp * 0.6f));
                enemy.speed *= 1.15f;
                enemy.coinReward *= 0.8;
                break;

            case WaveModifier.Flying:
                enemy.enemyType = Enemy.EnemyType.FastFlying;
                enemy.ApplyTypeDefaults();
                enemy.speed *= 1.2f;
                break;

            case WaveModifier.Elite:
                enemy.hp = Mathf.RoundToInt(enemy.hp * 3f);
                enemy.speed *= 1.1f;
                enemy.coinReward *= 3.0;
                enemy.castleDamage += 1;
                break;

            case WaveModifier.None:
            default:
                break;
        }
    }

    string FormatWaveReward(double value)
    {
        return GameManager.Instance.FormatNumber(value);
    }
}