using TMPro;
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
}