using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance { get; private set; }

    public List<Enemy> activeEnemies = new List<Enemy>();
    public List<Enemy> currentWaveEnemies = new List<Enemy>();

    void Awake()
    {
        Instance = this;
    }

    public void Register(Enemy enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        currentWaveEnemies.Remove(enemy);
    }

    public void AddToCurrentWave(Enemy enemy)
    {
        currentWaveEnemies.Add(enemy);
    }

    public void ClearCurrentWave()
    {
        currentWaveEnemies.Clear();
    }

    public void CleanupNulls()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        currentWaveEnemies.RemoveAll(enemy => enemy == null);
    }

    public int GetAliveWaveEnemyCount()
    {
        CleanupNulls();
        return currentWaveEnemies.Count;
    }
}