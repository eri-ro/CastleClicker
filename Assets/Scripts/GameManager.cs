using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turrets")]
    public Turret turretPrefab;
    public Transform castleTransform;

    [Header("Moat")]
    public Moat moatPrefab;
    Moat activeMoat;

    [Header("Knights")]
    public Knight knightPrefab;
    public float knightOrbitRadius = 2.0f;
    public float knightOrbitSpeed = Mathf.PI / 2f;

    public System.Action OnUIDataChanged;

    public InputActionReference getCoinInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        ResourceManager.Instance.OnResourcePerSecondChanged += OnResourcePerSecondChanged;
        CastleManager.Instance.OnCastleHealthChanged += OnCastleHealthOrGameOverChanged;
        CastleManager.Instance.OnGameOver += OnCastleHealthOrGameOverChanged;

        SyncDefenderStats();
        NotifyUIChanged();
    }

    void Update()
    {
        if (getCoinInput.action.triggered)
            ResourceManager.Instance.AddCoins(50);
    }

    public void NotifyUpgradeSystemChanged()
    {
        NotifyUIChanged();
    }

    void NotifyUIChanged()
    {
        OnUIDataChanged?.Invoke();
    }

    public void AddCoins(double amount)
    {
        ResourceManager.Instance.AddCoins(amount);
    }

    public bool SpendCoins(double cost)
    {
        return ResourceManager.Instance.SpendCoins(cost);
    }

    public void SyncDefenderStats()
    {
        DefenderManager.Instance.SetDamage(DefenderType.CastleCannon, UpgradeManager.Instance.GetCastleDamage());
        DefenderManager.Instance.SetDamage(DefenderType.Turret, UpgradeManager.Instance.GetTurretDamage());
        DefenderManager.Instance.SetDamage(DefenderType.Knight, UpgradeManager.Instance.GetKnightDamage());
        DefenderManager.Instance.SetDamage(DefenderType.Moat, UpgradeManager.Instance.GetLavaMoatDps());
    }

    public void SpawnTurret()
    {
        int turretCount = UpgradeManager.Instance.GetTurretCount();
        Vector3 center = castleTransform.position;
        float radius = 0.9f;
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

    public void SpawnKnight()
    {
        int knightCount = UpgradeManager.Instance.GetKnightCount();
        Vector3 center = castleTransform.position;
        float angleStep = (Mathf.PI * 2f) / Mathf.Max(1, knightCount);
        float spawnAngle = (knightCount - 1) * angleStep;

        Knight knight = Instantiate(
            knightPrefab,
            center,
            Quaternion.identity,
            SceneContainers.Instance.turrets
        );

        knight.center = castleTransform;
        knight.orbitRadius = knightOrbitRadius;
        knight.orbitSpeed = knightOrbitSpeed;
        knight.angle = spawnAngle;
        knight.SetDamage(Mathf.FloorToInt((float)UpgradeManager.Instance.GetKnightDamage()));
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
        ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        ResourceManager.Instance.OnResourcePerSecondChanged -= OnResourcePerSecondChanged;
        CastleManager.Instance.OnCastleHealthChanged -= OnCastleHealthOrGameOverChanged;
        CastleManager.Instance.OnGameOver -= OnCastleHealthOrGameOverChanged;
    }
}