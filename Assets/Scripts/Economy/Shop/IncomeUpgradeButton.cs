using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shop button for coin or mana passive income tiers (next tier to buy).
public class IncomeUpgradeButton : MonoBehaviour
{
    public Button button;
    public TMP_Text labelText;
    public ResourceType resourceType;

    public void Refresh()
    {
        Upgrade upgrade = UpgradeManager.Instance.GetNextAvailableUpgrade(resourceType);
        double coins = ResourceManager.Instance.GetResource(ResourceType.Coins);
        bool gameOver = CastleManager.Instance.gameOver;

        if (upgrade == null)
        {
            labelText.text = resourceType + " Income\nAll tiers purchased";
            button.interactable = false;
            return;
        }

        double cost = upgrade.GetCost();
        labelText.text =
            upgrade.upgradeName +
            "\nTier: " + upgrade.tier +
            "\nMultiplier: x" + upgrade.effect.multiplier.ToString("0.##") +
            "\nCost: " + FormatUtils.FormatNumber(cost);

        button.interactable = !gameOver && upgrade.state == UpgradeState.Available && coins >= cost;
    }

    public void OnClick()
    {
        int index = UpgradeManager.Instance.GetNextAvailableUpgradeIndex(resourceType);
        if (index >= 0)
            UpgradeManager.Instance.TryPurchaseUpgrade(index);
    }
}
