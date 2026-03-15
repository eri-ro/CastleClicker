using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Button button;
    public TMP_Text labelText;
    public UpgradeEffectType effectType;

    public void Refresh()
    {
        var up = UpgradeManager.Instance;
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        bool gameOver = CastleManager.Instance.gameOver;

        switch (effectType)
        {
            case UpgradeEffectType.CastleDamage:
                RefreshDamageUpgrade("Castle Damage", up.GetCastleDamage(), 1, up.GetCastleDamageUpgradeCost(), coins, gameOver);
                break;
            case UpgradeEffectType.TurretDamage:
                RefreshDamageUpgrade("Turret Damage", up.GetTurretDamage(), 1, up.GetTurretDamageUpgradeCost(), coins, gameOver);
                break;
            case UpgradeEffectType.BuyTurret:
                int turrets = DefenderManager.Instance.GetCount(DefenderType.Turret);
                bool atMaxTurrets = turrets >= 16;
                labelText.text = atMaxTurrets
                    ? "Buy Turret\nOwned: 16 / 16 (Max)"
                    : "Buy Turret" +
                      "\nOwned: " + turrets + " / 16" +
                      "\nDamage: " + FormatUtils.FormatNumber(up.GetTurretDamage()) +
                      "\nCost: " + FormatUtils.FormatNumber(up.GetTurretCost());
                button.interactable = !gameOver && !atMaxTurrets && coins >= up.GetTurretCost();
                break;
            case UpgradeEffectType.BuyMoat:
                RefreshMoat(up, coins, gameOver);
                break;
            case UpgradeEffectType.LavaMoatDps:
                if (!up.IsLavaMoatPurchased())
                {
                    labelText.text = "Lava DPS Up\nUnlock Lava First";
                    button.interactable = false;
                }
                else
                {
                    double current = up.GetLavaMoatDps();
                    double next = current * up.lavaMoatGrowth;
                    double cost = up.GetLavaMoatUpgradeCost();
                    labelText.text =
                        "Lava DPS Up" +
                        "\nCurrent: " + FormatUtils.FormatNumber(current) +
                        "\nNext: " + FormatUtils.FormatNumber(next) +
                        "\nCost: " + FormatUtils.FormatNumber(cost);
                    button.interactable = !gameOver && coins >= cost;
                }
                break;
            case UpgradeEffectType.BuyKnight:
                labelText.text =
                    "Buy Knight" +
                    "\nOwned: " + DefenderManager.Instance.GetCount(DefenderType.Knight) +
                    "\nDamage: " + FormatUtils.FormatNumber(up.GetKnightDamage()) +
                    "\nCost: " + FormatUtils.FormatNumber(up.GetKnightCost());
                button.interactable = !gameOver && coins >= up.GetKnightCost();
                break;
            default:
                break;
        }
    }

    void RefreshDamageUpgrade(string name, double current, double perLevel, double cost, double coins, bool gameOver)
    {
        labelText.text =
            name + " +" +
            "\nCurrent: " + FormatUtils.FormatNumber(current) +
            "\nNext: " + FormatUtils.FormatNumber(current + perLevel) +
            "\nCost: " + FormatUtils.FormatNumber(cost);
        button.interactable = !gameOver && coins >= cost;
    }

    void RefreshMoat(UpgradeManager up, double coins, bool gameOver)
    {
        if (!up.IsMoatPurchased())
        {
            labelText.text = "Buy Moat\nCost: " + FormatUtils.FormatNumber(up.moatCost);
            button.interactable = !gameOver && coins >= up.moatCost;
        }
        else if (!up.IsLavaMoatPurchased())
        {
            labelText.text = "Unlock Lava Moat\nCost: " + FormatUtils.FormatNumber(up.lavaMoatUnlockCost);
            button.interactable = !gameOver && coins >= up.lavaMoatUnlockCost;
        }
        else
        {
            labelText.text = "Lava Moat\nUnlocked";
            button.interactable = false;
        }
    }

    public void OnClick()
    {
        if (effectType == UpgradeEffectType.BuyMoat)
        {
            if (!UpgradeManager.Instance.IsMoatPurchased())
                UpgradeManager.Instance.TryBuyMoat();
            else if (!UpgradeManager.Instance.IsLavaMoatPurchased())
                UpgradeManager.Instance.TryUpgradeToLavaMoat();
            return;
        }

        UpgradeManager.Instance.TryPurchaseUpgradeByEffectType(effectType);
    }
}
