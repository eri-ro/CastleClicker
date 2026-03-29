using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shop button for the automatic coins-per-second upgrade.
public class CpsUpgradeButton : MonoBehaviour
{
    public Button button;
    public TMP_Text labelText;
    public GameManager game;

    public void Refresh()
    {
        ResourceManager rm = ResourceManager.Instance;
        double cost = rm.GetCpsUpgradeCost();
        double coins = rm.GetResource(ResourceType.Coins);
        double cps = rm.GetCoinsPerSecond();
        double cpsGain = rm.GetCpsGainPerLevel();
        bool gameOver = CastleManager.Instance.gameOver;

        labelText.text =
            "Coins Per Second\n" +
            "Current: " + FormatUtils.FormatNumber(cps) +
            "\nNext: " + FormatUtils.FormatNumber(cps + cpsGain) +
            "\nCost: " + FormatUtils.FormatNumber(cost);

        button.interactable = !gameOver && coins >= cost;
    }

    public void OnClick()
    {
        if (ResourceManager.Instance.TryBuyCpsUpgrade())
            game.NotifyUpgradeSystemChanged();
    }
}
