using TMPro;
using UnityEngine;

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
        coinsText.text = "Coins: " + FormatUtils.FormatNumber(ResourceManager.Instance.GetResource(ResourceType.Coins));
        cpsText.text = "CPS: " + FormatUtils.FormatNumber(ResourceManager.Instance.GetCoinsPerSecond());
        manaText.text = "Mana: " + FormatUtils.FormatNumber(ResourceManager.Instance.GetResource(ResourceType.Mana));
        castleHealthText.text =
            "Castle HP: " + CastleManager.Instance.currentCastleHealth + " / " + CastleManager.Instance.maxCastleHealth;

        int cannon = DefenderManager.Instance.GetCount(DefenderType.CastleCannon);
        int turrets = DefenderManager.Instance.GetCount(DefenderType.Turret);
        int moat = DefenderManager.Instance.GetCount(DefenderType.Moat);
        int knights = DefenderManager.Instance.GetCount(DefenderType.Knight);
        double cannonDmg = DefenderManager.Instance.GetDamage(DefenderType.CastleCannon);
        double turretDmg = DefenderManager.Instance.GetDamage(DefenderType.Turret);
        double moatDmg = DefenderManager.Instance.GetDamage(DefenderType.Moat);
        double knightDmg = DefenderManager.Instance.GetDamage(DefenderType.Knight);
        defenderText.text =
            "Defenders" +
            "\nCannon: " + cannon + " | Dmg: " + FormatUtils.FormatNumber(cannonDmg) +
            "\nTurrets: " + turrets + " | Dmg: " + FormatUtils.FormatNumber(turretDmg) +
            "\nMoat: " + moat + " | DPS: " + FormatUtils.FormatNumber(moatDmg) +
            "\nKnights: " + knights + " | Dmg: " + FormatUtils.FormatNumber(knightDmg);
    }
}
