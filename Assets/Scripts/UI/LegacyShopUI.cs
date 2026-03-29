using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Between-run meta shop: spend mana on permanent bonuses.
public class LegacyShopUI : MonoBehaviour
{
    [Header("References")]
    public MainMenuUI mainMenuUI;

    [Header("Display")]
    public TMP_Text totalManaText;

    [Header("Starting Coins Upgrade")]
    public Button startingCoinsButton;
    public TMP_Text startingCoinsLabel;

    [Header("Starting Mana/sec Upgrade")]
    public Button startingManaPerSecButton;
    public TMP_Text startingManaPerSecLabel;

    [Header("Starting Castle Health Upgrade")]
    public Button startingCastleHealthButton;
    public TMP_Text startingCastleHealthLabel;

    [Header("Defender Damage Upgrade")]
    public Button defenderDamageButton;
    public TMP_Text defenderDamageLabel;

    [Header("Starting CPS Upgrade")]
    public Button startingCpsButton;
    public TMP_Text startingCpsLabel;

    [Header("Enemy Coin Drop Upgrade")]
    public Button enemyCoinDropButton;
    public TMP_Text enemyCoinDropLabel;

    [Header("Wave Reward Upgrade")]
    public Button waveRewardButton;
    public TMP_Text waveRewardLabel;

    [Header("Upgrade Discount")]
    public Button upgradeDiscountButton;
    public TMP_Text upgradeDiscountLabel;

    [Header("Close")]
    public Button closeButton;

    void Start()
    {
        startingCoinsButton.onClick.AddListener(OnBuyStartingCoins);
        startingManaPerSecButton.onClick.AddListener(OnBuyStartingManaPerSec);
        startingCastleHealthButton.onClick.AddListener(OnBuyStartingCastleHealth);
        defenderDamageButton.onClick.AddListener(OnBuyDefenderDamage);
        startingCpsButton.onClick.AddListener(OnBuyStartingCps);
        enemyCoinDropButton.onClick.AddListener(OnBuyEnemyCoinDrop);
        waveRewardButton.onClick.AddListener(OnBuyWaveReward);
        upgradeDiscountButton.onClick.AddListener(OnBuyUpgradeDiscount);
        closeButton.onClick.AddListener(OnClose);

        LegacyManager.Instance.OnLegacyDataChanged += Refresh;

        ApplyLegacyShopLabelStyle();

        Refresh();
    }

    void OnDestroy()
    {
        LegacyManager.Instance.OnLegacyDataChanged -= Refresh;
    }

    void OnBuyStartingCoins()
    {
        if (LegacyManager.Instance.TryBuyStartingCoins())
            Refresh();
    }

    void OnBuyStartingManaPerSec()
    {
        if (LegacyManager.Instance.TryBuyStartingManaPerSec())
            Refresh();
    }

    void OnBuyStartingCastleHealth()
    {
        if (LegacyManager.Instance.TryBuyStartingCastleHealth())
            Refresh();
    }

    void OnBuyDefenderDamage()
    {
        if (LegacyManager.Instance.TryBuyDefenderDamage())
            Refresh();
    }

    void OnBuyStartingCps()
    {
        if (LegacyManager.Instance.TryBuyStartingCps())
            Refresh();
    }

    void OnBuyEnemyCoinDrop()
    {
        if (LegacyManager.Instance.TryBuyEnemyCoinDrop())
            Refresh();
    }

    void OnBuyWaveReward()
    {
        if (LegacyManager.Instance.TryBuyWaveReward())
            Refresh();
    }

    void OnBuyUpgradeDiscount()
    {
        if (LegacyManager.Instance.TryBuyUpgradeDiscount())
            Refresh();
    }

    void OnClose()
    {
        mainMenuUI.OnLegacyShopClosed();
    }

    public void Refresh()
    {
        LegacyManager lm = LegacyManager.Instance;
        double mana = lm.TotalMana;

        totalManaText.text = "Total Mana:\n" + FormatUtils.FormatNumber(mana);

        double costCoins = lm.GetStartingCoinsCost();
        startingCoinsLabel.text = $"Starting Coins +{lm.startingCoinsBonusPerLevel}\n" +
            $"Level: {lm.LegacyStartingCoinsLevel} | Next: {FormatUtils.FormatNumber(costCoins)} mana";
        startingCoinsButton.interactable = mana >= costCoins;

        double costManaSec = lm.GetStartingManaPerSecCost();
        startingManaPerSecLabel.text = $"Starting Mana/sec +{lm.startingManaPerSecBonusPerLevel}\n" +
            $"Level: {lm.LegacyStartingManaPerSecLevel} | Next: {FormatUtils.FormatNumber(costManaSec)} mana";
        startingManaPerSecButton.interactable = mana >= costManaSec;

        double costCastle = lm.GetStartingCastleHealthCost();
        startingCastleHealthLabel.text = $"Starting Castle HP +{lm.startingCastleHealthBonusPerLevel}\n" +
            $"Level: {lm.LegacyStartingCastleHealthLevel} | Next: {FormatUtils.FormatNumber(costCastle)} mana";
        startingCastleHealthButton.interactable = mana >= costCastle;

        double costDef = lm.GetDefenderDamageCost();
        int pctDef = Mathf.RoundToInt((float)(lm.defenderDamageBonusPerLevel * 100));
        defenderDamageLabel.text = $"Defender Damage +{pctDef}%\n" +
            $"Level: {lm.LegacyDefenderDamageLevel} | Next: {FormatUtils.FormatNumber(costDef)} mana";
        defenderDamageButton.interactable = mana >= costDef;

        double costCps = lm.GetStartingCpsCost();
        startingCpsLabel.text = $"Starting CPS +{lm.startingCpsBonusPerLevel}\n" +
            $"Level: {lm.LegacyStartingCpsLevel} | Next: {FormatUtils.FormatNumber(costCps)} mana";
        startingCpsButton.interactable = mana >= costCps;

        double costDrop = lm.GetEnemyCoinDropCost();
        int pctDrop = Mathf.RoundToInt((float)(lm.enemyCoinDropBonusPerLevel * 100));
        enemyCoinDropLabel.text = $"Enemy Coin Drop +{pctDrop}%\n" +
            $"Level: {lm.LegacyEnemyCoinDropLevel} | Next: {FormatUtils.FormatNumber(costDrop)} mana";
        enemyCoinDropButton.interactable = mana >= costDrop;

        double costWave = lm.GetWaveRewardCost();
        int pctWave = Mathf.RoundToInt((float)(lm.waveRewardBonusPerLevel * 100));
        waveRewardLabel.text = $"Wave Reward +{pctWave}%\n" +
            $"Level: {lm.LegacyWaveRewardLevel} | Next: {FormatUtils.FormatNumber(costWave)} mana";
        waveRewardButton.interactable = mana >= costWave;

        double costDisc = lm.GetUpgradeDiscountCost();
        int pctDisc = Mathf.RoundToInt((float)(lm.upgradeDiscountPerLevel * 100));
        upgradeDiscountLabel.text = $"Upgrade Cost -{pctDisc}%\n" +
            $"Level: {lm.LegacyUpgradeDiscountLevel} | Next: {FormatUtils.FormatNumber(costDisc)} mana";
        upgradeDiscountButton.interactable = mana >= costDisc;
    }

    void OnEnable()
    {
        Refresh();
    }

    void ApplyLegacyShopLabelStyle()
    {
        void Style(TMP_Text t)
        {
            t.textWrappingMode = TextWrappingModes.Normal;
            t.fontSizeMin = 11f;
            t.fontSizeMax = 24f;
            t.enableAutoSizing = true;
            t.fontSize = 24f;
        }

        Style(startingCoinsLabel);
        Style(startingManaPerSecLabel);
        Style(startingCastleHealthLabel);
        Style(defenderDamageLabel);
        Style(startingCpsLabel);
        Style(enemyCoinDropLabel);
        Style(waveRewardLabel);
        Style(upgradeDiscountLabel);
    }
}
