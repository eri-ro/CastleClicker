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
    }

    void Update()
    {
        Refresh();
    }
}