using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    public GameManager game;

    [Header("Effect-type upgrade buttons (assign in Inspector, set each UpgradeButton.effectType)")]
    public UpgradeButton[] upgradeButtons;

    [Header("CPS upgrade (ResourceManager)")]
    public CpsUpgradeButton cpsUpgradeButton;

    [Header("Income tier upgrades")]
    public IncomeUpgradeButton coinIncomeUpgradeButton;
    public IncomeUpgradeButton manaIncomeUpgradeButton;

    void Start()
    {
        game.OnUIDataChanged += Refresh;
        DefenderManager.Instance.OnDefendersChanged += Refresh;
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        Refresh();
    }

    void OnDestroy()
    {
        game.OnUIDataChanged -= Refresh;
        DefenderManager.Instance.OnDefendersChanged -= Refresh;
        ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
    }

    void OnResourceChanged(ResourceType type, double amount)
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (UpgradeButton ub in upgradeButtons)
            ub.Refresh();

        cpsUpgradeButton.Refresh();
        coinIncomeUpgradeButton.Refresh();
        manaIncomeUpgradeButton.Refresh();
    }
}
