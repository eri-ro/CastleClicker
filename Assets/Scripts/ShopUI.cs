using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
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

    public void OnClick_BuyCastleDamage()
    {
        game.TryBuyCastleDamageUpgrade();
        Refresh();
    }

    public void OnClick_BuyTurretDamage()
    {
        game.TryBuyTurretDamageUpgrade();
        Refresh();
    }

    public void OnClick_BuyCps()
    {
        game.TryBuyCpsUpgrade();
        Refresh();
    }

    public void OnClick_BuyTurret()
    {
        game.TryBuyTurret();
        Refresh();
    }

    public void OnClick_MoatButton()
    {
        if (!game.moatPurchased)
            game.TryBuyMoat();
        else if (!game.lavaMoatPurchased)
            game.TryUpgradeToLavaMoat();

        Refresh();
    }

    public void OnClick_BuyLavaDamage()
    {
        game.TryBuyLavaMoatDamageUpgrade();
        Refresh();
    }

    public void Refresh()
    {
        double castleCost = game.GetCastleDamageUpgradeCost();
        castleDamageButtonText.text = "Castle Damage +" + "\nCost: " + game.FormatNumber(castleCost);
        castleDamageButton.interactable = game.coins >= castleCost;

        double turretDmgCost = game.GetTurretDamageUpgradeCost();
        turretDamageButtonText.text = "Turret Damage +" + "\nCost: " + game.FormatNumber(turretDmgCost);
        turretDamageButton.interactable = game.coins >= turretDmgCost;

        double cpsCost = game.GetCpsUpgradeCost();
        cpsButtonText.text = "+ " + game.FormatNumber(game.cpsGainPerLevel) + " CPS\nCost: " + game.FormatNumber(cpsCost);
        cpsButton.interactable = game.coins >= cpsCost;

        double turretCost = game.GetTurretCost();
        turretButtonText.text = "Buy Turret\nCost: " + game.FormatNumber(turretCost);
        turretButton.interactable = game.coins >= turretCost;

        
        if (!game.moatPurchased)
            moatButtonText.text = "Buy Moat\nCost: " + game.FormatNumber(game.moatCost);
        else if (!game.lavaMoatPurchased)
            moatButtonText.text = "Unlock Lava Moat\nCost: " + game.FormatNumber(game.lavaMoatUnlockCost);
        else
            moatButtonText.text = "Lava Moat\nUnlocked";
        

        if (!game.moatPurchased)
            moatButton.interactable = game.coins >= game.moatCost;
        else if (!game.lavaMoatPurchased)
            moatButton.interactable = game.coins >= game.lavaMoatUnlockCost;
        else
            moatButton.interactable = false;

        double lavaCost = game.GetLavaMoatUpgradeCost();
        lavaDamageButtonText.text = "Lava DPS Up\nCost: " + game.FormatNumber(lavaCost);
        lavaDamageButton.interactable = game.lavaMoatPurchased && game.coins >= lavaCost;
    }

    void Update()
    {
        Refresh();
    }
}