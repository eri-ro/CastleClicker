using UnityEngine;
// One run’s stats: kills, damage dealt, coins spent, time survived (for game over UI).
public class GameStatsTracker : MonoBehaviour
{
    public static GameStatsTracker Instance { get; private set; }

    int enemiesKilled;
    double totalDamageDealt;
    double totalCoinsSpent;
    float runStartTime;
    float runEndTime;
    bool runActive;

    public int EnemiesKilled => enemiesKilled;
    public double TotalDamageDealt => totalDamageDealt;
    public double TotalCoinsSpent => totalCoinsSpent;
    public float TimeSurvivedSeconds => runActive ? Time.time - runStartTime : (runEndTime - runStartTime);

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CastleManager.Instance.OnGameOver += OnGameOver;
    }

    void OnDestroy()
    {
        CastleManager.Instance.OnGameOver -= OnGameOver;
    }

    public void StartRun()
    {
        enemiesKilled = 0;
        totalDamageDealt = 0;
        totalCoinsSpent = 0;
        runStartTime = Time.time;
        runActive = true;
    }

    void OnGameOver()
    {
        EndRun();
    }

    public void EndRun()
    {
        runActive = false;
        runEndTime = Time.time;
    }

    public void RecordEnemyKilled()
    {
        enemiesKilled++;
    }

    public void RecordDamageDealt(double amount)
    {
        if (amount > 0)
            totalDamageDealt += amount;
    }

    public void RecordCoinsSpent(double amount)
    {
        if (amount > 0)
            totalCoinsSpent += amount;
    }

    public string FormatTimeSurvived()
    {
        float seconds = TimeSurvivedSeconds;
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{mins}:{secs:D2}";
    }
}
