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

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        game.OnUIDataChanged += Refresh;
        DefenderManager.Instance.OnDefendersChanged += Refresh;
    }

    void OnDisable()
    {
        game.OnUIDataChanged -= Refresh;
        DefenderManager.Instance.OnDefendersChanged -= Refresh;
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

    public void Refresh()
    {

        RefreshCastleDamage();
        RefreshTurretDamage();
        RefreshCps();
        RefreshTurretBuy();
        RefreshMoat();
        RefreshLavaDamage();
    }

    void RefreshCastleDamage()
    {
        double cost = game.GetCastleDamageUpgradeCost();

        castleDamageButtonText.text =
           "Castle Damage +" +
           "\nDamage: " + game.FormatNumber(game.castleDamage) +
           "\nCost: " + game.FormatNumber(cost);

        castleDamageButton.interactable = game.coins >= cost;
    }

    void RefreshTurretDamage()
    {
        double cost = game.GetTurretDamageUpgradeCost();

        turretDamageButtonText.text =
            "Turret Damage +" +
            "\nDamage: " + game.FormatNumber(game.turretDamage) +
            "\nCost: " + game.FormatNumber(cost);

        turretDamageButton.interactable = game.coins >= cost;
    }

    void RefreshCps()
    {
        double cost = game.GetCpsUpgradeCost();

        cpsButtonText.text =
            "+ " + game.FormatNumber(game.cpsGainPerLevel) + " CPS" +
            "\nCurrent: " + game.FormatNumber(game.coinsPerSecond) +
            "\nCost: " + game.FormatNumber(cost);

        cpsButton.interactable = game.coins >= cost;
    }

    void RefreshTurretBuy()
    {
        double cost = game.GetTurretCost();

        int turretCount = 0;
        turretCount = DefenderManager.Instance.GetCount(DefenderType.Turret);

        turretButtonText.text =
            "Buy Turret" +
            "\nOwned: " + turretCount +
            "\nCost: " + game.FormatNumber(cost);

       turretButton.interactable = game.coins >= cost;
    }

    void RefreshMoat()
    {
        if (!game.moatPurchased)
        {
            moatButtonText.text =
                "Buy Moat" +
                "\nCost: " + game.FormatNumber(game.moatCost);
        }
        else if (!game.lavaMoatPurchased)
        {
            moatButtonText.text =
                "Unlock Lava Moat" +
                "\nCost: " + game.FormatNumber(game.lavaMoatUnlockCost);
        }
        else
        {
            moatButtonText.text = "Lava Moat\nUnlocked";
        }

        if (!game.moatPurchased)
        {
                moatButton.interactable = game.coins >= game.moatCost;
        }
        else if (!game.lavaMoatPurchased)
        {
                moatButton.interactable = game.coins >= game.lavaMoatUnlockCost;
        }
        else
        {
                moatButton.interactable = false;
        }
    }

    void RefreshLavaDamage()
    {
        double cost = game.GetLavaMoatUpgradeCost();

        if (!game.lavaMoatPurchased)
        {
            lavaDamageButtonText.text = "Lava DPS Up\nUnlock Lava First";
        }
        else
        {
            lavaDamageButtonText.text =
                "Lava DPS Up" +
                "\nCurrent: " + game.FormatNumber(game.lavaMoatDps) +
                "\nCost: " + game.FormatNumber(cost);
        }

        if (lavaDamageButton != null)
        {
            lavaDamageButton.interactable =
                !game.gameOver &&
                game.lavaMoatPurchased &&
                game.coins >= cost;
        }
    }
}