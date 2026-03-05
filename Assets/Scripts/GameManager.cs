using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int coins = 0;
    public float coinsPerSecond = 0f;
    float cpsTimer;

    public int cpsLevel = 0;
    public int cpsBaseCost = 25;
    public float cpsCostGrowth = 1.17f;
    public float cpsGainPerLevel = 1f;

    public int damagePerShot = 1;
    public int damageLevel = 0;
    public int damageBaseCost = 10;
    public float damageCostGrowth = 1.15f;

    public Turret turretPrefab;
    public Transform castleTransform; // drag your castle here
    public int turretCount = 0;
    public int turretBaseCost = 50;
    public float turretCostGrowth = 1.25f;

    public TMP_Text coinsText;
    public TMP_Text cpsText;

    public InputActionReference getCoinInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // Add coins once per second based on coinsPerSecond
        cpsTimer += Time.deltaTime;

        if (cpsTimer >= 1f)
        {
            cpsTimer -= 1f;

            int payout = Mathf.FloorToInt(coinsPerSecond);
            if (payout > 0)
                AddCoins(payout);
        }
        // for testing
        if (getCoinInput.action.triggered)
            AddCoins(50);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    public bool SpendCoins(int cost)
    {
        if (coins < cost)
            return false;

        coins -= cost;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        coinsText.text = "Coins: " + coins.ToString();
        cpsText.text = "CPS: " + coinsPerSecond.ToString("0.##");
    }

    public int GetDamageUpgradeCost()
    {
        // cost = baseCost * growth^level
        float costF = damageBaseCost * Mathf.Pow(damageCostGrowth, damageLevel);
        int cost = Mathf.RoundToInt(costF);
        return Mathf.Max(1, cost);
    }

    public bool TryBuyDamageUpgrade()
    {
        int cost = GetDamageUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        damageLevel += 1;
        damagePerShot += 1;
        return true;
    }

    public int GetCpsUpgradeCost()
    {
        float costF = cpsBaseCost * Mathf.Pow(cpsCostGrowth, cpsLevel);
        int cost = Mathf.RoundToInt(costF);
        return Mathf.Max(1, cost);
    }

    public bool TryBuyCpsUpgrade()
    {
        int cost = GetCpsUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        cpsLevel += 1;
        coinsPerSecond += cpsGainPerLevel;
        return true;
    }

    public int GetTurretCost()
    {
        float costF = turretBaseCost * Mathf.Pow(turretCostGrowth, turretCount);
        int cost = Mathf.RoundToInt(costF);
        return Mathf.Max(1, cost);
    }

    public bool TryBuyTurret()
    {
        int cost = GetTurretCost();
        if (!SpendCoins(cost))
            return false;

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

        Turret turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity, castleTransform);
    }
}