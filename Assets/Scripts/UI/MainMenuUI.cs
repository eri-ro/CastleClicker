using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Main menu, play, pause, game over flow, and legacy shop entry.
public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject legacyShopPanel;
    [Tooltip("Root of game over UI. Use a sibling of mainMenuPanel/gameplayPanel so it draws on top when shown.")]
    public GameObject gameOverPanel;

    [Header("Backgrounds")]
    [Tooltip("Hide this when Legacy Shop opens so the shop background shows instead.")]
    public GameObject mainMenuBackground;

    [Header("Main Menu")]
    public Button playButton;
    public Button legacyShopButton;
    public Button exitButton;
    public TMP_Text totalManaText;
    public TMP_Text titleText;

    [Header("Game Over")]
    public TMP_Text gameOverSubtitleText;
    public GameOverScreen gameOverScreen;

    [Header("Pause (ESC during gameplay)")]
    public GameObject pausePanel;
    public Button returnToMainMenuButton;

    [Header("Shop")]
    public ShopUI shopUI;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        legacyShopButton.onClick.AddListener(OnLegacyShopClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        returnToMainMenuButton.onClick.AddListener(OnReturnToMainMenuFromPauseClicked);

        GameManager.Instance.mainMenuUI = this;
        LegacyManager.Instance.OnLegacyDataChanged += Refresh;

        ShowMainMenu();
        gameOverPanel.SetActive(false);
        gameOverScreen.Hide();
        pausePanel.SetActive(false);
        Refresh();
    }

    public void OnEscapePressedDuringGameplay()
    {
        bool show = !pausePanel.activeSelf;
        pausePanel.SetActive(show);
        if (show)
            pausePanel.transform.SetAsLastSibling();
    }

    void OnReturnToMainMenuFromPauseClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void ApplyReturnToMainMenuUI()
    {
        shopUI.CloseShop();
        if (legacyShopPanel.activeSelf)
            OnLegacyShopClosed();
        pausePanel.SetActive(false);
        ShowMainMenu();
        gameOverPanel.SetActive(false);
        gameOverScreen.Hide();
        titleText.text = "Castle Clicker";
        gameOverSubtitleText.gameObject.SetActive(false);
        mainMenuBackground.SetActive(true);
        Refresh();
    }

    void OnDestroy()
    {
        LegacyManager.Instance.OnLegacyDataChanged -= Refresh;
    }

    public void ShowGameOverScreen()
    {
        gameplayPanel.SetActive(false);
        legacyShopPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        mainMenuBackground.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverScreen.transform.SetAsLastSibling();
        gameOverScreen.mainMenuUI = this;
        gameOverScreen.Show();
        titleText.text = "Game Over";
        gameOverSubtitleText.gameObject.SetActive(true);
        double manaEarned = ResourceManager.Instance.GetResource(ResourceType.Mana);
        gameOverSubtitleText.text = "Mana earned this run:\n" + FormatUtils.FormatNumber(manaEarned);
        Refresh();
    }

    public void OnGameOverMainMenuClicked()
    {
        gameOverScreen.Hide();
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        mainMenuBackground.SetActive(true);
        titleText.text = "Castle Clicker";
        gameOverSubtitleText.gameObject.SetActive(false);
        Refresh();
    }

    void OnExitClicked()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    void OnPlayClicked()
    {
        if (legacyShopPanel.activeSelf)
            OnLegacyShopClosed();

        shopUI.CloseShop();

        ShowGameplay();
        titleText.text = "Castle Clicker";
        gameOverSubtitleText.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        gameOverScreen.Hide();

        GameManager.Instance.StartNewGame();
    }

    void OnLegacyShopClicked()
    {
        legacyShopPanel.SetActive(true);
        legacyShopPanel.transform.SetAsLastSibling();
        mainMenuBackground.SetActive(false);
    }

    public void OnLegacyShopClosed()
    {
        legacyShopPanel.SetActive(false);
        mainMenuBackground.SetActive(true);
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        legacyShopPanel.SetActive(false);
    }

    void ShowGameplay()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void Refresh()
    {
        totalManaText.text = "Total Mana:\n" + FormatUtils.FormatNumber(LegacyManager.Instance.TotalMana);
    }
}
