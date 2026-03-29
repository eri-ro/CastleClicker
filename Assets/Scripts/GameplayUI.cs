using System.Text;
using TMPro;
using UnityEngine;

// HUD: coins, mana, castle HP, defender summary.
public class GameplayUI : MonoBehaviour
{
    [Header("Resources")]
    public TMP_Text coinsText;
    public TMP_Text cpsText;
    public TMP_Text manaText;

    [Header("Castle")]
    public TMP_Text castleHealthText;

    [Header("Defenders")]
    public TMP_Text defenderText;

    void Start()
    {
        GameManager.Instance.OnUIDataChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnUIDataChanged -= Refresh;
    }

    public void Refresh()
    {
        coinsText.text = "Coins:\n" + FormatUtils.FormatNumber(ResourceManager.Instance.GetResource(ResourceType.Coins));
        cpsText.text = "CPS:\n" + FormatUtils.FormatNumber(ResourceManager.Instance.GetCoinsPerSecond());
        manaText.text = "Mana:\n" + FormatUtils.FormatNumber(ResourceManager.Instance.GetResource(ResourceType.Mana));
        castleHealthText.text =
            "Castle HP:\n" + CastleManager.Instance.currentCastleHealth + " / " + CastleManager.Instance.maxCastleHealth;

        int cannon = DefenderManager.Instance.GetCount(DefenderType.CastleCannon);
        int turrets = DefenderManager.Instance.GetCount(DefenderType.Turret);
        int moat = DefenderManager.Instance.GetCount(DefenderType.Moat);
        int moatMonsters = DefenderManager.Instance.GetCount(DefenderType.MoatMonster);
        double cannonDmg = DefenderManager.Instance.GetDamage(DefenderType.CastleCannon);
        double turretDmg = DefenderManager.Instance.GetDamage(DefenderType.Turret);
        double moatDmg = DefenderManager.Instance.GetDamage(DefenderType.Moat);
        double moatMonsterDmg = DefenderManager.Instance.GetDamage(DefenderType.MoatMonster);

        var sb = new StringBuilder();
        sb.AppendLine("Defenders");
        sb.Append("Cannon: ").Append(cannon).Append(" | Dmg: ").AppendLine(FormatUtils.FormatNumber(cannonDmg));
        if (turrets > 0)
            sb.Append("Turrets: ").Append(turrets).Append(" | Dmg: ").AppendLine(FormatUtils.FormatNumber(turretDmg));
        if (moat > 0)
            sb.Append("Moat: ").Append(moat).Append(" | DPS: ").AppendLine(FormatUtils.FormatNumber(moatDmg));
        if (moatMonsters > 0)
            sb.Append("Moat monsters: ").Append(moatMonsters).Append(" | Dmg: ").AppendLine(FormatUtils.FormatNumber(moatMonsterDmg));
        defenderText.text = sb.ToString().TrimEnd();
    }
}

// Shared number text for HUD, shop, waves, and stats (small values as integers; huge ones in scientific notation).
public static class FormatUtils
{
    public static string FormatNumber(double value)
    {
        if (value < 100000)
            return Mathf.FloorToInt((float)value).ToString();

        return value.ToString("0.###e0");
    }
}
