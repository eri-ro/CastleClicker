using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameManager game;
    public Button damageButton;
    public TMP_Text damageButtonText;
    public Button cpsButton;
    public TMP_Text cpsButtonText;
    public Button turretButton;
    public TMP_Text turretButtonText;
    public Button moatButton;
    public TMP_Text moatButtonText;

    void Start()
    {
        Refresh();
    }

    public void OnClick_BuyDamage()
    {
        bool bought = game.TryBuyDamageUpgrade();
        if (bought)
        {
            Refresh();
        }
        else
        {
            // error indicator or something
            Refresh(); // still refresh to keep cost accurate
        }
    }

    public void OnClick_BuyCps()
    {
        bool bought = game.TryBuyCpsUpgrade();
        Refresh();
    }

    public void OnClick_BuyTurret()
    {
        game.TryBuyTurret();
        Refresh();
    }

    public void OnClick_BuyMoat()
    {
        game.TryBuyMoat();
        Refresh();
    }

    public void OnClick_MoatButton()
    {
        if (!game.moatPurchased)
            game.TryBuyMoat();
        else
            game.TryUpgradeToLavaMoat();

        Refresh();
    }

    public void Refresh()
    {
        // Damage
        int dmgCost = game.GetDamageUpgradeCost();
        damageButtonText.text = "Damage +1\nCost: " + dmgCost;
        damageButton.interactable = (game.coins >= dmgCost);

        // CPS
        int cpsCost = game.GetCpsUpgradeCost();
        cpsButtonText.text = "+ " + game.cpsGainPerLevel.ToString("0.##") + " CPS\nCost: " + cpsCost;
        cpsButton.interactable = (game.coins >= cpsCost);

        // Turret
        int turretCost = game.GetTurretCost();
        turretButtonText.text = "Buy Turret\nCost: " + turretCost;
        turretButton.interactable = (game.coins >= turretCost);

        // Moat
        if (!game.moatPurchased)
            moatButtonText.text = "Buy Moat\nCost: " + game.moatCost;
        else if (!game.lavaMoatPurchased)
            moatButtonText.text = "Upgrade: Lava Moat\nCost: " + game.lavaMoatCost;
        else
            moatButtonText.text = "Lava Moat\nOwned";

        if (!game.moatPurchased)
            moatButton.interactable = (game.coins >= game.moatCost);
        else if (!game.lavaMoatPurchased)
            moatButton.interactable = (game.coins >= game.lavaMoatCost);
        else
            moatButton.interactable = false;        
    }

    void Update()
    {
        Refresh();
    }
}