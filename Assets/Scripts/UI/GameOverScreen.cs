using TMPro;
using UnityEngine;
using UnityEngine.UI;

// End-of-run stats and buttons after the castle falls.
[RequireComponent(typeof(CanvasGroup))]
public class GameOverScreen : MonoBehaviour
{
    [Header("Stats")]
    public TMP_Text enemiesKilledText;
    public TMP_Text totalDamageText;
    public TMP_Text coinsSpentText;
    public TMP_Text timeSurvivedText;

    [Header("Stats (extra)")]
    public TMP_Text manaEarnedText;

    [Header("Panel")]
    public GameObject statsPanel;

    [Header("Buttons")]
    [Tooltip("Returns to main menu (hides game over, shows main menu background).")]
    public Button mainMenuButton;
    [Tooltip("Closes the application.")]
    public Button exitButton;

    [Header("References")]
    public MainMenuUI mainMenuUI;

    void Start()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.interactable = true;
        statsPanel.SetActive(true);
        Refresh();
    }

    void OnMainMenuClicked()
    {
        mainMenuUI.OnGameOverMainMenuClicked();
    }

    void OnExitClicked()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        statsPanel.SetActive(false);
    }

    public void Refresh()
    {
        GameStatsTracker stats = GameStatsTracker.Instance;

        enemiesKilledText.text = "Enemies Killed:\n" + stats.EnemiesKilled;

        totalDamageText.text = "Total Damage Dealt:\n" + FormatUtils.FormatNumber(stats.TotalDamageDealt);

        coinsSpentText.text = "Coins Spent:\n" + FormatUtils.FormatNumber(stats.TotalCoinsSpent);

        timeSurvivedText.text = "Time Survived:\n" + stats.FormatTimeSurvived();

        manaEarnedText.text = "Mana Earned:\n" + FormatUtils.FormatNumber(ResourceManager.Instance.GetResource(ResourceType.Mana));
    }
}
