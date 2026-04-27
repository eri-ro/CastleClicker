using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Starts and ends a run, spawns turrets/moat/monsters, and tells the UI when numbers change.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turrets")]
    public Turret turretPrefab;
    public Transform castleTransform;

    [Header("Moat")]
    public Moat moatPrefab;
    Moat activeMoat;

    [Header("Moat monsters")]
    public MoatMonster moatMonsterPrefab;
    public float moatMonsterOrbitRadius = 2.0f;
    public float moatMonsterOrbitSpeed = Mathf.PI / 2f;

    MoatMonster firstMoatMonster;

    public const int MaxMoatMonsters = 8;
    // this sets the spacing of the monsters so they aren't lopsided on one side
    static readonly float[] MoatMonsterRelativeOffsetsRad =
    {
        0f,                 //0 degrees
        Mathf.PI,           //180
        Mathf.PI / 2f,      //90.. etc
        3f * Mathf.PI / 2f,
        Mathf.PI / 4f,
        5f * Mathf.PI / 4f,
        3f * Mathf.PI / 4f,
        7f * Mathf.PI / 4f, //315
    };

    public EnemySpawner enemySpawner;

    public System.Action OnUIDataChanged;

    [Header("Game Over")]
    public MainMenuUI mainMenuUI;

    [HideInInspector]
    public bool gameStarted; // false until Play is clicked; gates resource gain, spawning, cannon

    void Awake()
    {
        Instance = this;
        EnemyTypeStatsRegistry.LoadFromXml();
    }

    void Start()
    {
        SetGameplayWorldVisible(false);

        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        ResourceManager.Instance.OnResourcePerSecondChanged += OnResourcePerSecondChanged;
        CastleManager.Instance.OnCastleHealthChanged += OnCastleHealthOrGameOverChanged;
        CastleManager.Instance.OnGameOver += OnGameOver;

        SyncDefenderStats();
        NotifyUIChanged();

        SaveManager.instance.OnSaveFailed += OnSaveFailedFromSaveManager;
    }

    void OnSaveFailedFromSaveManager(string message)
    {
        Debug.LogWarning("[SaveManager] " + message);
    }

    void OnGameOver()
    {
        gameStarted = false;
        ClearSpawnedDefenders();
        SetGameplayWorldVisible(false);
        OnCastleHealthOrGameOverChanged();
        mainMenuUI.ShowGameOverScreen();
    }

    void SetGameplayWorldVisible(bool visible)
    {
        castleTransform.gameObject.SetActive(visible);

        SceneContainers sc = SceneContainers.Instance;
        sc.enemies.gameObject.SetActive(visible);
        sc.turrets.gameObject.SetActive(visible);
        sc.projectiles.gameObject.SetActive(visible);
    }

    void Update()
    {
        if (!gameStarted) return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            ResourceManager.Instance.AddCoins(50);
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            mainMenuUI.OnEscapePressedDuringGameplay();
        }
    }

    public void ReturnToMainMenu()
    {
        // Allow cleanup after normal game over (gameStarted already false, castle still in game-over state).
        if (!gameStarted && !CastleManager.Instance.gameOver)
            return;
        gameStarted = false;
        SetGameplayWorldVisible(false);
        LegacyManager.Instance.BankRunMana();
        ClearSpawnedDefenders();
        enemySpawner.ResetForNewGame();
        ResourceManager.Instance.ResetForNewGame();
        CastleManager.Instance.ResetForNewGame();
        UpgradeManager.Instance.ResetForNewGame();
        DefenderManager.Instance.ResetForNewGame();
        SyncDefenderStats();
        NotifyUIChanged();
        GameStatsTracker.Instance.EndRun();
        mainMenuUI.ApplyReturnToMainMenuUI();
    }

    public void NotifyUpgradeSystemChanged() => NotifyUIChanged();

    void NotifyUIChanged()
    {
        OnUIDataChanged?.Invoke();
    }

    public void AddCoins(double amount)
    {
        ResourceManager.Instance.AddCoins(amount);
    }

    public void SyncDefenderStats()
    {
        DefenderManager.Instance.SetDamage(DefenderType.CastleCannon, UpgradeManager.Instance.GetCastleDamage());
        DefenderManager.Instance.SetDamage(DefenderType.Turret, UpgradeManager.Instance.GetTurretDamage());
        DefenderManager.Instance.SetDamage(DefenderType.MoatMonster, UpgradeManager.Instance.GetMoatMonsterDamage());
        DefenderManager.Instance.SetDamage(DefenderType.Moat, UpgradeManager.Instance.GetLavaMoatDps());
        ApplyMoatMonsterContactDamageToScene();
    }

    void ApplyMoatMonsterContactDamageToScene()
    {
        int dmg = Mathf.Max(1, Mathf.RoundToInt((float)UpgradeManager.Instance.GetMoatMonsterDamage()));
        Transform root = SceneContainers.Instance.turrets;
        for (int i = 0; i < root.childCount; i++)
        {
            MoatMonster mm = root.GetChild(i).GetComponent<MoatMonster>();
            if (mm != null)
                mm.SetDamage(dmg);
        }
    }

    public void SpawnTurret()
    {
        int turretCount = UpgradeManager.Instance.GetTurretCount();
        Vector3 center = castleTransform.position;
        float radius = 1f;
        int i = turretCount - 1;
        float rad = (i % 8) * Mathf.PI / 4f;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        Vector3 spawnPos = center + offset;
        Quaternion rot = i >= 8 ? Quaternion.Euler(0f, 0f, 180f) : Quaternion.identity;
        Instantiate(turretPrefab, spawnPos, rot, SceneContainers.Instance.turrets);
    }

    public void SpawnMoat()
    {
        Vector3 pos = castleTransform.position;

        activeMoat = Instantiate(moatPrefab, pos, Quaternion.identity, castleTransform);
        activeMoat.center = castleTransform;
        activeMoat.moatType = Moat.MoatType.Water;
        activeMoat.ApplyVisuals();
    }

    public void UpgradeMoatToLava()
    {
        activeMoat.moatType = Moat.MoatType.Lava;
        activeMoat.ApplyVisuals();
    }

    public void SpawnMoatMonster()
    {
        int count = UpgradeManager.Instance.GetMoatMonsterCount();
        Vector3 center = castleTransform.position;
        int slot = Mathf.Clamp(count - 1, 0, MoatMonsterRelativeOffsetsRad.Length - 1);
        float offsetRad = MoatMonsterRelativeOffsetsRad[slot];

        MoatMonster monster = Instantiate(
            moatMonsterPrefab,
            center,
            Quaternion.identity,
            SceneContainers.Instance.turrets
        );

        monster.center = castleTransform;
        monster.orbitRadius = moatMonsterOrbitRadius;
        monster.orbitSpeed = moatMonsterOrbitSpeed;

        if (slot == 0)
        {
            monster.angle = Mathf.Repeat(offsetRad, Mathf.PI * 2f);
            firstMoatMonster = monster;
        }
        else
        {
            float refAngle = firstMoatMonster != null ? firstMoatMonster.angle : 0f;
            monster.angle = Mathf.Repeat(refAngle + offsetRad, Mathf.PI * 2f);
        }

        monster.SetDamage(Mathf.FloorToInt((float)UpgradeManager.Instance.GetMoatMonsterDamage()));
    }

    public void StartNewGame()
    {
        gameStarted = true;
        SetGameplayWorldVisible(true);
        GameStatsTracker.Instance.StartRun();
        ClearSpawnedDefenders();
        enemySpawner.ResetForNewGame();
        ResourceManager.Instance.ResetForNewGame();
        CastleManager.Instance.ResetForNewGame();
        UpgradeManager.Instance.ResetForNewGame();
        DefenderManager.Instance.ResetForNewGame();
        SyncDefenderStats();
        NotifyUIChanged();
    }

    void ClearSpawnedDefenders()
    {
        firstMoatMonster = null;

        if (activeMoat != null)
        {
            Destroy(activeMoat.gameObject);
            activeMoat = null;
        }

        Transform turretsRoot = SceneContainers.Instance.turrets;
        for (int i = turretsRoot.childCount - 1; i >= 0; i--)
            Destroy(turretsRoot.GetChild(i).gameObject);
        Transform enemiesRoot = SceneContainers.Instance.enemies;
        for (int i = enemiesRoot.childCount - 1; i >= 0; i--)
            Destroy(enemiesRoot.GetChild(i).gameObject);
        Transform projectilesRoot = SceneContainers.Instance.projectiles;
        for (int i = projectilesRoot.childCount - 1; i >= 0; i--)
            Destroy(projectilesRoot.GetChild(i).gameObject);
    }

    void OnCastleHealthOrGameOverChanged()
    {
        NotifyUIChanged();
    }

    void OnResourceChanged(ResourceType type, double amount)
    {
        OnUIDataChanged?.Invoke();
    }

    void OnResourcePerSecondChanged()
    {
        NotifyUIChanged();
    }

    void OnDestroy()
    {
        SaveManager.instance.OnSaveFailed -= OnSaveFailedFromSaveManager;

        ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        ResourceManager.Instance.OnResourcePerSecondChanged -= OnResourcePerSecondChanged;
        CastleManager.Instance.OnCastleHealthChanged -= OnCastleHealthOrGameOverChanged;
        CastleManager.Instance.OnGameOver -= OnGameOver;
    }
}
