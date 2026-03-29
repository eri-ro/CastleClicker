using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One shop button for a single upgrade type (text, cost, and buy action).
public class UpgradeButton : MonoBehaviour
{
    public Button button;
    public TMP_Text labelText;
    public UpgradeEffectType effectType;

    void Awake()
    {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(OnClick);
    }

    public void Refresh()
    {
        UpgradeManager up = UpgradeManager.Instance;
        if (!ShouldShowButton(up))
        {
            SetButtonActive(false);
            return;
        }

        SetButtonActive(true);

        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        bool gameOver = CastleManager.Instance.gameOver;

        switch (effectType)
        {
            case UpgradeEffectType.CastleDamage:
                RefreshDamageUpgrade("Castle Damage", up.GetCastleDamage(), 1, up.GetCastleDamageUpgradeCost(), coins, gameOver);
                break;
            case UpgradeEffectType.CastleCannonFireRate:
                RefreshCannonUpgrade(up, "Cannon Fire Rate", "Faster fire rate", up.GetCannonFireRateCost(), coins, gameOver);
                break;
            case UpgradeEffectType.CastleCannonMultiShot:
                RefreshCannonOneShot(up, "Cannon Multi-Shot", "Fires 3 shots", up.GetCannonMultiShotCost(), up.HasCannonMultiShot(), coins, gameOver);
                break;
            case UpgradeEffectType.CastleCannonPiercing:
                RefreshCannonOneShot(up, "Cannon Piercing", "Shots pierce 2 enemies", up.GetCannonPiercingCost(), up.HasCannonPiercing(), coins, gameOver);
                break;
            case UpgradeEffectType.CastleCannonSplash:
                RefreshCannonUpgrade(up, "Cannon Splash", "Splash damage", up.GetCannonSplashCost(), coins, gameOver);
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
                break;
            case UpgradeEffectType.MoatMonsterDamage:
                {
                    Upgrade u = up.GetUpgradeByEffectType(effectType);
                    bool available = u != null && u.state == UpgradeState.Available;
                    double current = up.GetMoatMonsterDamage();
                    double cost = up.GetMoatMonsterDamageUpgradeCost();
                    labelText.text =
                        "Moat Monster Damage +" +
                        "\nCurrent: " + FormatUtils.FormatNumber(current) +
                        "\nNext: " + FormatUtils.FormatNumber(up.GetMoatMonsterDamageNextPreview()) +
                        "\nCost: " + FormatUtils.FormatNumber(cost);
                    if (!available)
                        labelText.text += "\n(Unlock prerequisite)";
                    button.interactable = !gameOver && available && coins >= cost;
                }
                break;
            case UpgradeEffectType.BuyMoatMonster:
                {
                    int owned = DefenderManager.Instance.GetCount(DefenderType.MoatMonster);
                    bool atMax = owned >= GameManager.MaxMoatMonsters;
                    labelText.text = atMax
                        ? "Buy Moat Monster\nOwned: " + GameManager.MaxMoatMonsters + " / " + GameManager.MaxMoatMonsters + " (Max)"
                        : "Buy Moat Monster" +
                          "\nOwned: " + owned + " / " + GameManager.MaxMoatMonsters +
                          "\nDamage: " + FormatUtils.FormatNumber(up.GetMoatMonsterDamage()) +
                          "\nCost: " + FormatUtils.FormatNumber(up.GetMoatMonsterCost());
                    button.interactable = !gameOver && !atMax && coins >= up.GetMoatMonsterCost();
                }
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

    void RefreshCannonUpgrade(UpgradeManager up, string name, string desc, double cost, double coins, bool gameOver)
    {
        Upgrade u = up.GetUpgradeByEffectType(effectType);
        bool available = u?.state == UpgradeState.Available;
        int level = u != null ? u.level : 0;
        labelText.text = $"{name}\n{desc}";
        if (level > 0)
            labelText.text += $"\nLevel: {level}";
        labelText.text += $"\nCost: {FormatUtils.FormatNumber(cost)}";
        if (!available)
            labelText.text += "\n(Unlock prerequisite)";
        button.interactable = !gameOver && available && coins >= cost;
    }

    void RefreshCannonOneShot(UpgradeManager up, string name, string desc, double cost, bool purchased, double coins, bool gameOver)
    {
        Upgrade u = up.GetUpgradeByEffectType(effectType);
        bool available = u != null && u.state == UpgradeState.Available;
        if (purchased)
        {
            labelText.text = $"{name}\nPurchased";
            button.interactable = false;
        }
        else
        {
            labelText.text = $"{name}\n{desc}\nCost: {FormatUtils.FormatNumber(cost)}";
            if (!available)
                labelText.text += "\n(Unlock prerequisite)";
            button.interactable = !gameOver && available && coins >= cost;
        }
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
            double current = up.GetLavaMoatDps();
            double next = current * up.lavaMoatGrowth;
            double cost = up.GetLavaMoatUpgradeCost();
            labelText.text =
                "Lava Moat DPS" +
                "\nCurrent: " + FormatUtils.FormatNumber(current) +
                "\nNext: " + FormatUtils.FormatNumber(next) +
                "\nCost: " + FormatUtils.FormatNumber(cost);
            button.interactable = !gameOver && coins >= cost;
        }
    }

    public void OnClick()
    {
        if (effectType == UpgradeEffectType.BuyMoat)
        {
            UpgradeManager um = UpgradeManager.Instance;
            if (!um.IsMoatPurchased())
                um.TryBuyMoat();
            else if (!um.IsLavaMoatPurchased())
                um.TryUpgradeToLavaMoat();
            else
                um.TryPurchaseUpgradeByEffectType(UpgradeEffectType.LavaMoatDps);
            return;
        }

        UpgradeManager.Instance.TryPurchaseUpgradeByEffectType(effectType);
    }

    void SetButtonActive(bool visible)
    {
        if (gameObject.activeSelf != visible)
            gameObject.SetActive(visible);
    }

    bool ShouldShowButton(UpgradeManager up)
    {
        switch (effectType)
        {
            case UpgradeEffectType.TurretDamage:
                return up.GetTurretCount() >= 1;
            case UpgradeEffectType.LavaMoatDps:
                return false;
            case UpgradeEffectType.MoatMonsterDamage:
                return DefenderManager.Instance.GetCount(DefenderType.MoatMonster) >= 1;
            case UpgradeEffectType.BuyMoatMonster:
                return up.IsMoatPurchased();
            case UpgradeEffectType.CastleCannonFireRate:
            case UpgradeEffectType.CastleCannonMultiShot:
            case UpgradeEffectType.CastleCannonPiercing:
            case UpgradeEffectType.CastleCannonSplash:
                return up.IsPrerequisiteSatisfied(effectType);
            default:
                return true;
        }
    }
}
