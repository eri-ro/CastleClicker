using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public double coins = 0;
    public double coinsPerSecond = 0;

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

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text cpsText;
    public TMP_Text castleHealthText;

    float cpsTimer;

    public InputActionReference getCoinInput; // debug cheat

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentCastleHealth = maxCastleHealth;
        UpdateUI();
    }

    void Update()
    {
        // Add coins once per second based on coinsPerSecond
        cpsTimer += Time.deltaTime;

        if (cpsTimer >= 1f)
        {
            cpsTimer -= 1f;

            if (coinsPerSecond > 0)
                AddCoins(coinsPerSecond);
        }
        // for testing
        if (getCoinInput.action.triggered)
            AddCoins(50);
    }

    public void AddCoins(double amount)
    {
        coins += amount;
        UpdateUI();
    }

    public bool SpendCoins(double cost)
    {
        if (coins < cost)
            return false;

        coins -= cost;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        coinsText.text = "Coins: " + Mathf.FloorToInt((float)coins).ToString();
        cpsText.text = "CPS: " + coinsPerSecond.ToString("0.##");
        castleHealthText.text = "Castle HP: " + currentCastleHealth + " / " + maxCastleHealth;
    }

    public double GetCastleDamageUpgradeCost()
    {
        return castleDamageBaseCost * System.Math.Pow(castleDamageCostGrowth, castleDamageLevel);
    }

    public bool TryBuyCastleDamageUpgrade()
    {
        double cost = GetCastleDamageUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        castleDamageLevel++;
        castleDamage += 1;
        return true;
    }

    public double GetTurretDamageUpgradeCost()
    {
        return turretDamageBaseCost * System.Math.Pow(turretDamageCostGrowth, turretDamageLevel);
    }

    public bool TryBuyTurretDamageUpgrade()
    {
        double cost = GetTurretDamageUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        turretDamageLevel++;
        turretDamage += 1;
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

        cpsLevel += 1;
        coinsPerSecond += cpsGainPerLevel;
        return true;
    }

    public double GetTurretCost()
    {
        return turretBaseCost * System.Math.Pow(turretCostGrowth, turretCount); ;
    }

    public bool TryBuyTurret()
    {
        double cost = GetTurretCost();
        if (!SpendCoins(cost)) return false;

        turretCount += 1;
        SpawnTurret();
        return true;
    }

    void SpawnTurret()
    {
        Vector3 center = castleTransform.position;

        // Place turrets in a ring around the castle
        float radius = 0.9f;
        float rad = (turretCount - 1) * Mathf.PI / 4f; // 8 around
        //float rad = angleDeg * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        Vector3 spawnPos = center + offset;

        Instantiate(turretPrefab, spawnPos, Quaternion.identity);
    }

    public bool TryBuyMoat()
    {
        if (moatPurchased) return false;
        if (!SpendCoins(moatCost)) return false;

        moatPurchased = true;
        SpawnMoat();
        return true;
    }

    void SpawnMoat()
    {
        Vector3 pos = castleTransform.position;
        activeMoat = Instantiate(moatPrefab, pos, Quaternion.identity);
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

        return true;
    }

    public bool TryBuyLavaMoatDamageUpgrade()
    {
        if (!lavaMoatPurchased) return false;

        double cost = GetLavaMoatUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        lavaMoatLevel++;
        lavaMoatDps *= lavaMoatGrowth;
        return true;
    }

    public double GetLavaMoatUpgradeCost()
    {
        return lavaMoatBaseCost * System.Math.Pow(lavaMoatCostGrowth, lavaMoatLevel);
    }

    public void DamageCastle(int amount)
    {
        if (gameOver) return;

        currentCastleHealth -= amount;
        currentCastleHealth = Mathf.Max(0, currentCastleHealth);

        UpdateUI();

        if (currentCastleHealth <= 0)
            GameOver();
    }

    void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over");
    }

    public string FormatNumber(double value)
    {
        if (value < 1000)
            return Mathf.FloorToInt((float)value).ToString();

        return value.ToString("0.###e0");
    }
}