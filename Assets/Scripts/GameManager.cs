using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public double coinsPerSecond = 1;
    public double manaPerSecond = 1; 
    
    [Header("Castle Health")]
    public int maxCastleHealth = 100;
    public int currentCastleHealth;
    public bool gameOver = false;

    [Header("Combat Stats")]
    public double castleDamage = 1;
    public double turretDamage = 1;
    public double lavaMoatDps = 5;

    [Header("Damage Upgrade")]
    public int castleDamageLevel = 0;
    public double castleDamageBaseCost = 10;
    public double castleDamageCostGrowth = 1.15;

    [Header("Turret Damage Upgrade")]
    public int turretDamageLevel = 0;
    public double turretDamageBaseCost = 25;
    public double turretDamageCostGrowth = 1.18;

    [Header("Lava Moat Upgrade")]
    public int lavaMoatLevel = 0;
    public double lavaMoatBaseCost = 100;
    public double lavaMoatCostGrowth = 1.22;
    public double lavaMoatGrowth = 1.25;

    [Header("CPS Upgrade")]
    public int cpsLevel = 0;
    public double cpsBaseCost = 20;
    public double cpsCostGrowth = 1.17;
    public double cpsGainPerLevel = 1;

    [Header("Turrets")]
    public Turret turretPrefab;
    public Transform castleTransform;
    public int turretCount = 0;
    public double turretBaseCost = 50;
    public double turretCostGrowth = 1.25;

    [Header("Moat")]
    public Moat moatPrefab;
    public bool moatPurchased = false;
    public bool lavaMoatPurchased = false;
    public double moatCost = 150;
    public double lavaMoatUnlockCost = 300;

    Moat activeMoat;

    [Header("Knights")]
    public Knight knightPrefab;
    public int knightCount = 0;
    public double knightDamage = 1;
    public double knightBaseCost = 75;
    public double knightCostGrowth = 1.22;
    public float knightOrbitRadius = 2.0f;
    public float knightOrbitSpeed = Mathf.PI / 2f;

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text cpsText;
    public TMP_Text castleHealthText;
    public TMP_Text manaText;
    public TMP_Text defenderText;

    public System.Action OnUIDataChanged;

    float passiveTimer = 0f;

    public InputActionReference getCoinInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentCastleHealth = maxCastleHealth;

        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;

        SyncDefenderStats();
        NotifyUIChanged();
    }

    void Update()
    {
        passiveTimer += Time.deltaTime;

        if (passiveTimer >= 1f)
        {
            passiveTimer -= 1f;
            ResourceManager.Instance.AddResource(ResourceType.Coins, coinsPerSecond);
            ResourceManager.Instance.AddResource(ResourceType.Mana, manaPerSecond);
        }

        if (getCoinInput.action.triggered)
            AddCoins(50);
    }

    public void NotifyUpgradeSystemChanged()
    {
        NotifyUIChanged();
    }

    void NotifyUIChanged()
    {
        UpdateUI();
        OnUIDataChanged?.Invoke();
    }

    public void AddCoins(double amount)
    {
        ResourceManager.Instance.AddResource(ResourceType.Coins, amount);
    }

    public bool SpendCoins(double cost)
    {
        return ResourceManager.Instance.SpendResource(ResourceType.Coins, cost);
    }

    void UpdateUI()
    {
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        double mana = ResourceManager.Instance.GetResource(ResourceType.Mana);

        coinsText.text = "Coins: " + FormatNumber(coins);
        cpsText.text = "CPS: " + FormatNumber(coinsPerSecond);

        castleHealthText.text =
            "Castle HP: " + currentCastleHealth + " / " + maxCastleHealth;

        manaText.text = "Mana: " + FormatNumber(mana);

        int cannon = DefenderManager.Instance.GetCount(DefenderType.CastleCannon);
        int turrets = DefenderManager.Instance.GetCount(DefenderType.Turret);
        int moat = DefenderManager.Instance.GetCount(DefenderType.Moat);
        int knights = DefenderManager.Instance.GetCount(DefenderType.Knight);

        double cannonDamageValue = DefenderManager.Instance.GetDamage(DefenderType.CastleCannon);
        double turretDamageValue = DefenderManager.Instance.GetDamage(DefenderType.Turret);
        double moatDamageValue = DefenderManager.Instance.GetDamage(DefenderType.Moat);
        double knightDamageValue = DefenderManager.Instance.GetDamage(DefenderType.Knight);

        defenderText.text =
            "Defenders" +
            "\nCannon: " + cannon + " | Dmg: " + FormatNumber(cannonDamageValue) +
            "\nTurrets: " + turrets + " | Dmg: " + FormatNumber(turretDamageValue) +
            "\nMoat: " + moat + " | DPS: " + FormatNumber(moatDamageValue) +
            "\nKnights: " + knights + " | Dmg: " + FormatNumber(knightDamageValue);
    }

    void SyncDefenderStats()
    {
        DefenderManager.Instance.SetDamage(DefenderType.CastleCannon, castleDamage);
        DefenderManager.Instance.SetDamage(DefenderType.Turret, turretDamage);
        DefenderManager.Instance.SetDamage(DefenderType.Knight, knightDamage);

        if (lavaMoatPurchased)
            DefenderManager.Instance.SetDamage(DefenderType.Moat, lavaMoatDps);
        else
            DefenderManager.Instance.SetDamage(DefenderType.Moat, 0);
    }

    public double GetCastleDamageUpgradeCost()
    {
        return castleDamageBaseCost * System.Math.Pow(castleDamageCostGrowth, castleDamageLevel);
    }

    public bool TryBuyCastleDamageUpgrade()
    {
        double cost = GetCastleDamageUpgradeCost();
        if (!SpendCoins(cost)) return false;

        castleDamageLevel++;
        castleDamage += 1;

        SyncDefenderStats();
        NotifyUIChanged();
        return true;
    }

    public double GetTurretDamageUpgradeCost()
    {
        return turretDamageBaseCost * System.Math.Pow(turretDamageCostGrowth, turretDamageLevel);
    }

    public bool TryBuyTurretDamageUpgrade()
    {
        double cost = GetTurretDamageUpgradeCost();
        if (!SpendCoins(cost)) return false;

        turretDamageLevel++;
        turretDamage += 1;

        SyncDefenderStats();
        NotifyUIChanged();
        return true;
    }

    public double GetCpsUpgradeCost()
    {
        return cpsBaseCost * System.Math.Pow(cpsCostGrowth, cpsLevel);
    }

    public bool TryBuyCpsUpgrade()
    {
        double cost = GetCpsUpgradeCost();
        if (!SpendCoins(cost)) return false;

        cpsLevel++;
        coinsPerSecond += cpsGainPerLevel;

        NotifyUIChanged();
        return true;
    }

    public double GetTurretCost()
    {
        return turretBaseCost * System.Math.Pow(turretCostGrowth, turretCount);
    }

    public bool TryBuyTurret()
    {
        double cost = GetTurretCost();
        if (!SpendCoins(cost)) return false;

        turretCount++;
        SpawnTurret();
        DefenderManager.Instance.AddDefender(DefenderType.Turret);

        NotifyUIChanged();
        return true;
    }

    void SpawnTurret()
    {
        Vector3 center = castleTransform.position;

        float radius = 0.9f;
        float rad = (turretCount - 1) * Mathf.PI / 4f;

        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        Vector3 spawnPos = center + offset;

        Instantiate(turretPrefab, spawnPos, Quaternion.identity, SceneContainers.Instance.turrets);
    }

    public bool TryBuyMoat()
    {
        if (moatPurchased) return false;

        if (!SpendCoins(moatCost)) return false;

        moatPurchased = true;
        SpawnMoat();
        DefenderManager.Instance.AddDefender(DefenderType.Moat);

        SyncDefenderStats();
        NotifyUIChanged();
        return true;
    }

    void SpawnMoat()
    {
        Vector3 pos = castleTransform.position;

        activeMoat = Instantiate(moatPrefab, pos, Quaternion.identity, castleTransform);
        activeMoat.center = castleTransform;
        activeMoat.moatType = Moat.MoatType.Water;
        activeMoat.ApplyVisuals();
    }

    public bool TryUpgradeToLavaMoat()
    {
        if (!moatPurchased) return false;

        if (lavaMoatPurchased) return false;

        if (!SpendCoins(lavaMoatUnlockCost)) return false;

        lavaMoatPurchased = true;
        activeMoat.moatType = Moat.MoatType.Lava;
        activeMoat.ApplyVisuals();

        SyncDefenderStats();
        NotifyUIChanged();
        return true;
    }

    public double GetLavaMoatUpgradeCost()
    {
        return lavaMoatBaseCost * System.Math.Pow(lavaMoatCostGrowth, lavaMoatLevel);
    }

    public bool TryBuyLavaMoatDamageUpgrade()
    {
        if (!lavaMoatPurchased) return false;

        double cost = GetLavaMoatUpgradeCost();
        if (!SpendCoins(cost)) return false;

        lavaMoatLevel++;
        lavaMoatDps *= lavaMoatGrowth;

        SyncDefenderStats();
        NotifyUIChanged();
        return true;
    }

    public double GetKnightCost()
    {
        return knightBaseCost * System.Math.Pow(knightCostGrowth, knightCount);
    }

    public bool TryBuyKnight()
    {
        double cost = GetKnightCost();
        if (!SpendCoins(cost)) return false;

        knightCount++;
        SpawnKnight();
        DefenderManager.Instance.AddDefender(DefenderType.Knight);

        NotifyUIChanged();
        return true;
    }

    void SpawnKnight()
    {
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
        knight.SetDamage(Mathf.FloorToInt((float)knightDamage));
    }

    public void DamageCastle(int amount)
    {
        if (gameOver)
            return;

        currentCastleHealth -= amount;
        currentCastleHealth = Mathf.Max(0, currentCastleHealth);

        NotifyUIChanged();

        if (currentCastleHealth <= 0)
            GameOver();
    }

    void OnResourceChanged(ResourceType type, double amount)
    {
        UpdateUI();
        OnUIDataChanged?.Invoke();
    }

    void OnDestroy()
    {
        ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
    }

    void GameOver()
    {
        gameOver = true;
        NotifyUIChanged();
        Debug.Log("Game Over");
    }

    public string FormatNumber(double value)
    {
        if (value < 100000)
            return Mathf.FloorToInt((float)value).ToString();

        return value.ToString("0.###e0");
    }
}