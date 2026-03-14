using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    public GameManager game;

    [Header("Castle Damage UI")]
    public Button castleDamageButton;
    public TMP_Text castleDamageButtonText;

    [Header("Turret Damage UI")]
    public Button turretDamageButton;
    public TMP_Text turretDamageButtonText;

    [Header("CPS UI")]
    public Button cpsButton;
    public TMP_Text cpsButtonText;

    [Header("Turret Buy UI")]
    public Button turretButton;
    public TMP_Text turretButtonText;

    [Header("Moat UI")]
    public Button moatButton;
    public TMP_Text moatButtonText;

    [Header("Lava Damage UI")]
    public Button lavaDamageButton;
    public TMP_Text lavaDamageButtonText;

    [Header("Knight UI")]
    public Button knightButton;
    public TMP_Text knightButtonText;

    [Header("Idle Upgrade UI")]
    public Button coinIncomeUpgradeButton;
    public TMP_Text coinIncomeUpgradeText;

    public Button manaIncomeUpgradeButton;
    public TMP_Text manaIncomeUpgradeText;

    void Start()
    {
        game.OnUIDataChanged += Refresh;
        DefenderManager.Instance.OnDefendersChanged += Refresh;
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;

        Refresh();
    }

    void OnDestroy()
    {
        game.OnUIDataChanged -= Refresh;
        DefenderManager.Instance.OnDefendersChanged -= Refresh;
        ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
    }

    void OnResourceChanged(ResourceType type, double amount)
    {
        Refresh();
    }

    public void OnClick_BuyCastleDamage()
    {
        game.TryBuyCastleDamageUpgrade();
    }

    public void OnClick_BuyTurretDamage()
    {
        game.TryBuyTurretDamageUpgrade();
    }

    public void OnClick_BuyCps()
    {
        game.TryBuyCpsUpgrade();
    }

    public void OnClick_BuyTurret()
    {
        game.TryBuyTurret();
    }

    public void OnClick_MoatButton()
    {
        if (!game.moatPurchased)
            game.TryBuyMoat();
        else if (!game.lavaMoatPurchased)
            game.TryUpgradeToLavaMoat();
    }

    public void OnClick_BuyLavaDamage()
    {
        game.TryBuyLavaMoatDamageUpgrade();
    }

    public void OnClick_BuyKnight()
    {
        game.TryBuyKnight();
    }

    public void OnClick_BuyCoinIncomeUpgrade()
    {
        int index = UpgradeManager.Instance.GetNextAvailableUpgradeIndex(ResourceType.Coins);

        if (index >= 0)
            UpgradeManager.Instance.TryPurchaseUpgrade(index);
    }

    public void OnClick_BuyManaIncomeUpgrade()
    {
        int index = UpgradeManager.Instance.GetNextAvailableUpgradeIndex(ResourceType.Mana);

        if (index >= 0)
            UpgradeManager.Instance.TryPurchaseUpgrade(index);
    }

    public void Refresh()
    {
        RefreshCastleDamage();
        RefreshTurretDamage();
        RefreshCps();
        RefreshTurretBuy();
        RefreshMoat();
        RefreshLavaDamage();
        RefreshKnightBuy();

        RefreshIncomeUpgrade(ResourceType.Coins, coinIncomeUpgradeButton, coinIncomeUpgradeText);
        RefreshIncomeUpgrade(ResourceType.Mana, manaIncomeUpgradeButton, manaIncomeUpgradeText);
    }

    void RefreshCastleDamage()
    {
        double cost = game.GetCastleDamageUpgradeCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

        castleDamageButtonText.text =
            "Castle Damage +" +
            "\nCurrent: " + game.FormatNumber(game.castleDamage) +
            "\nNext: " + game.FormatNumber(game.castleDamage + 1) +
            "\nCost: " + game.FormatNumber(cost);

        castleDamageButton.interactable = !game.gameOver && coins >= cost;
    }

    void RefreshTurretDamage()
    {
        double cost = game.GetTurretDamageUpgradeCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

        turretDamageButtonText.text =
            "Turret Damage +" +
            "\nCurrent: " + game.FormatNumber(game.turretDamage) +
            "\nNext: " + game.FormatNumber(game.turretDamage + 1) +
            "\nCost: " + game.FormatNumber(cost);

        turretDamageButton.interactable = !game.gameOver && coins >= cost;
    }

    void RefreshCps()
    {
        double cost = game.GetCpsUpgradeCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

        cpsButtonText.text =
            "Coins Per Second +" +
            "\nCurrent: " + game.FormatNumber(game.coinsPerSecond) +
            "\nNext: " + game.FormatNumber(game.coinsPerSecond + game.cpsGainPerLevel) +
            "\nCost: " + game.FormatNumber(cost);

        cpsButton.interactable = !game.gameOver && coins >= cost;
    }

    void RefreshTurretBuy()
    {
        double cost = game.GetTurretCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        int turretCount = DefenderManager.Instance.GetCount(DefenderType.Turret);

        turretButtonText.text =
            "Buy Turret" +
            "\nOwned: " + turretCount +
            "\nDamage: " + game.FormatNumber(game.turretDamage) +
            "\nCost: " + game.FormatNumber(cost);

        turretButton.interactable = !game.gameOver && coins >= cost;
    }

    void RefreshMoat()
    {
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

        if (!game.moatPurchased)
        {
            moatButtonText.text =
                "Buy Moat" +
                "\nCost: " + game.FormatNumber(game.moatCost);

            moatButton.interactable = !game.gameOver && coins >= game.moatCost;
        }
        else if (!game.lavaMoatPurchased)
        {
            moatButtonText.text =
                "Unlock Lava Moat" +
                "\nCost: " + game.FormatNumber(game.lavaMoatUnlockCost);

            moatButton.interactable = !game.gameOver && coins >= game.lavaMoatUnlockCost;
        }
        else
        {
            moatButtonText.text = "Lava Moat\nUnlocked";
            moatButton.interactable = false;
        }
    }

    void RefreshLavaDamage()
    {
        double cost = game.GetLavaMoatUpgradeCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

        if (!game.lavaMoatPurchased)
        {
            lavaDamageButtonText.text =
                "Lava DPS Up" +
                "\nUnlock Lava First";
        }
        else
        {
            double nextValue = game.lavaMoatDps * game.lavaMoatGrowth;

            lavaDamageButtonText.text =
                "Lava DPS Up" +
                "\nCurrent: " + game.FormatNumber(game.lavaMoatDps) +
                "\nNext: " + game.FormatNumber(nextValue) +
                "\nCost: " + game.FormatNumber(cost);
        }

        lavaDamageButton.interactable =
            !game.gameOver &&
            game.lavaMoatPurchased &&
            coins >= cost;
    }

    void RefreshKnightBuy()
    {
        double cost = game.GetKnightCost();
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        int knightCount = DefenderManager.Instance.GetCount(DefenderType.Knight);

        knightButtonText.text =
            "Buy Knight" +
            "\nOwned: " + knightCount +
            "\nDamage: " + game.FormatNumber(game.knightDamage) +
            "\nCost: " + game.FormatNumber(cost);

        knightButton.interactable = !game.gameOver && coins >= cost;
    }

    void RefreshIncomeUpgrade(ResourceType targetType, Button button, TMP_Text text)
{
    Upgrade upgrade = UpgradeManager.Instance.GetNextAvailableUpgrade(targetType);
    double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);

    if (upgrade == null)
    {
        text.text =
            targetType + " Income" +
            "\nAll Tiers Purchased";

        button.interactable = false;
        return;
    }

    double cost = upgrade.GetCost();

    text.text =
        upgrade.upgradeName +
        "\nTier: " + upgrade.tier +
        "\nMultiplier: x" + upgrade.effect.multiplier.ToString("0.##") +
        "\nState: " + upgrade.state +
        "\nCost: " + game.FormatNumber(cost);

    button.interactable =
        !game.gameOver &&
        upgrade.state == UpgradeState.Available &&
        coins >= cost;
}
}