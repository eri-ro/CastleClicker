using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Opens/closes the shop panel and refreshes all shop labels when resources or defenders change.
public class ShopUI : MonoBehaviour
{
    [Header("References")]
    public GameManager game;

    [Header("Shop Panel")]
    [Tooltip("The panel containing all upgrade buttons. Hidden by default, shown when Shop button is clicked.")]
    public GameObject shopPanel;
    [Tooltip("Button on the gameplay screen that opens/closes the shop panel.")]
    public Button shopButton;

    [Header("CPS upgrade (ResourceManager)")]
    public CpsUpgradeButton cpsUpgradeButton;

    [Header("Income tier upgrades")]
    public IncomeUpgradeButton coinIncomeUpgradeButton;
    public IncomeUpgradeButton manaIncomeUpgradeButton;

    void Awake()
    {
        shopButton.onClick.AddListener(ToggleShop);
    }

    void Start()
    {
        shopPanel.SetActive(false);
        ApplyLabelStyleToShopButtons();

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

    void OnResourceChanged(ResourceType type, double amount) => Refresh();

    public void ToggleShop()
    {
        bool willShow = !shopPanel.activeSelf;
        shopPanel.SetActive(willShow);

        if (willShow)
            shopPanel.transform.SetAsLastSibling();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void Refresh()
    {
        foreach (UpgradeButton ub in shopPanel.GetComponentsInChildren<UpgradeButton>(true))
            ub.Refresh();

        cpsUpgradeButton.Refresh();
        coinIncomeUpgradeButton.Refresh();
        manaIncomeUpgradeButton.Refresh();
    }

    static void ApplyLabelStyle(TMP_Text t)
    {
        t.textWrappingMode = TextWrappingModes.Normal;
        t.fontSizeMin = 11f;
        t.fontSizeMax = 24f;
        t.enableAutoSizing = true;
        t.fontSize = 24f;
    }

    void ApplyLabelStyleToShopButtons()
    {
        foreach (UpgradeButton ub in shopPanel.GetComponentsInChildren<UpgradeButton>(true))
            ApplyLabelStyle(ub.labelText);

        ApplyLabelStyle(cpsUpgradeButton.labelText);
        ApplyLabelStyle(coinIncomeUpgradeButton.labelText);
        ApplyLabelStyle(manaIncomeUpgradeButton.labelText);
    }
}
