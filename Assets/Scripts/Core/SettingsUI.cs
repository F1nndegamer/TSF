using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI instance;

    public GameObject settingsPanel;
    public GameObject controlsTabContent;
    public GameObject generalTabContent;
    public Button controlsTabButton;
    public Button generalTabButton;
    public Image controlsTabImage;
    public Image generalTabImage;
    public Sprite activeTabsprite;
    public Sprite inactiveTabSprite;
    public Button closeButton;

    public Button resetDefaultButton;
    public InputRebind[] hotkeyRows;

    public Button resumeButton;
    public Button saveGameButton;
    public Button fullscreenToggleButton;
    public TextMeshProUGUI fullscreenButtonText;

    public Slider musicSlider;
    public Slider sfxSlider;
    public Button quitButton;
    public TextMeshProUGUI statusText;

    public bool pauseTimeWhenOpen = true;

    private const string MusicVolKey = "MusicVolume";
    private const string SfxVolKey = "SfxVolume";
    private const string FullscreenKey = "FullscreenEnabled";
    [ContextMenu("Get hotkeys")]
    private void FindHotKeyRows()
    {
        hotkeyRows = GetComponentsInChildren<InputRebind>(includeInactive: true);
    }
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (controlsTabButton != null)
            controlsTabButton.onClick.AddListener(() => SwitchTab(showGeneral: false));
        if (generalTabButton != null)
            generalTabButton.onClick.AddListener(() => SwitchTab(showGeneral: true));
        if (closeButton != null)
            closeButton.onClick.AddListener(() => SetSettingsOpen(false));
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => SetSettingsOpen(false));
        if (resetDefaultButton != null)
            resetDefaultButton.onClick.AddListener(ResetAllHotKeys);
        if (saveGameButton != null)
            saveGameButton.onClick.AddListener(OnSaveGameClicked);
        if (fullscreenToggleButton != null)
            fullscreenToggleButton.onClick.AddListener(ToggleFullScreen);
        if (quitButton != null)

            quitButton.onClick.AddListener(QuitGame);

        LoadGeneralSettings();

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
        if (InputManager.instance != null)
        {
            InputManager.instance.Close += HandleEscapePressed;
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (InputManager.instance != null)
            InputManager.instance.Close -= HandleEscapePressed;
        if (pauseTimeWhenOpen)
            Time.timeScale = 1f;
    }
    private void HandleEscapePressed()
    {
        //So set false when: its active, set active if gamemanager.isinvopen = false
        if (settingsPanel == null) return;

        if (settingsPanel.activeSelf)
        {
            SetSettingsOpen(false);
            return;
        }

        if (!GameManager.IsInventoryOpen)
        {
            SetSettingsOpen(true);
        }
    }
    public void ToggleSettings()
    {
        if (settingsPanel == null) return;
        SetSettingsOpen(!settingsPanel.activeSelf);
    }
    public void SetSettingsOpen(bool open)
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(open);
        GameManager.IsInventoryOpen = open;

        MouseToGrid.Instance.ChangeCursor(open);

        if (pauseTimeWhenOpen)
            Time.timeScale = open ? 0f : 1f;

        statusText.text = "";
        if (open)
        {
            SwitchTab(showGeneral: false);
            RefreshAllHotkeys();
            RefreshFullscreenLabel();
        }
    }

    public void SwitchTab(bool showGeneral)
    {
        controlsTabContent.SetActive(!showGeneral);
        generalTabContent.SetActive(showGeneral);
        controlsTabImage.sprite = showGeneral ? inactiveTabSprite : activeTabsprite;
        generalTabImage.sprite = showGeneral ? activeTabsprite : inactiveTabSprite;
        if (!showGeneral)
            RefreshAllHotkeys();
    }
    public void RefreshAllHotkeys()
    {
        if (hotkeyRows == null)
            return;
        foreach (InputRebind row in hotkeyRows)
        {
            if (row != null)
                row.RefreshUI();
        }
    }
    public void ResetAllHotKeys()
    {
        InputManager.instance.ResetBinding();
        RefreshAllHotkeys();
        SetStatus("Controlls reset to default");
    }
    public void LoadGeneralSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicVolKey, 0.8f);
        float savedSFX = PlayerPrefs.GetFloat(SfxVolKey, 0.8f);
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        musicSlider.SetValueWithoutNotify(savedMusic);
        sfxSlider.SetValueWithoutNotify(savedSFX);

        Screen.fullScreen = savedFullscreen;
        RefreshFullscreenLabel();
    }
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MusicVolKey, value);
        PlayerPrefs.Save();
    }
    private void OnSfxVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(SfxVolKey, value);
        PlayerPrefs.Save();
    }
    private void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetInt(FullscreenKey, Screen.fullScreen ? 1 : 0);
        PlayerPrefs.Save();
        RefreshFullscreenLabel();
    }
    private void RefreshFullscreenLabel()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Fullscreen: ON" : "Fullscreen: OFF";
    }
    private void OnSaveGameClicked()
    {
        SetStatus("Save system soon");
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    private void SetStatus(string message)
    {
        statusText.text = message;
    }
}
