using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.UIElements;

public enum UIPanel
{
    None,
    Inventory,
    Settings,
    Journal,
    Shop,
    Experiment,
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject inventoryPanel;
    public GameObject settingsPanel;
    public GameObject journalPanel;
    public GameObject shopPanel;
    public GameObject experimentpanel;

    public bool pauseTimeWhenSettings = true;

    public UIPanel currentPanel { get; private set; } = UIPanel.None;

    void Awake()
    {
        instance = this;
        CloseAllPanels();
    }
    void Start()
    {
        InputManager.instance.Close += HandleEscape;
    }
    void OnDestroy()
    {
        InputManager.instance.Close += HandleEscape;
        Time.timeScale = 1f;
    }
    private GameObject GetPanel(UIPanel panel)
    {
        return panel switch
        {
            UIPanel.Inventory => inventoryPanel,
            UIPanel.Settings => settingsPanel,
            UIPanel.Journal => journalPanel,
            UIPanel.Shop => shopPanel,
            UIPanel.Experiment => experimentpanel,
            _ => null
        };
    }
    public bool IsOpen(UIPanel panel)
    {
        return currentPanel == panel;
    }
    public bool HasOpenPanel()
    {
        return currentPanel != UIPanel.None;
    }
    public bool OpenPanel(UIPanel panel)
    {
        GameObject target = GetPanel(panel);

        if (target == null || panel == UIPanel.None)
            return false;
        if (currentPanel == panel)
            return true;

        CloseCurrentPanel();

        target.SetActive(true);
        currentPanel = panel;

        UpdateUIState();

        return true;
    }
    public void ClosePanel(UIPanel panel)
    {
        if (currentPanel != panel)
            return;
        CloseCurrentPanel();
    }
    public bool TogglePanel(UIPanel panel)
    {
        if (currentPanel == panel)
        {
            CloseCurrentPanel();
            return false;
        }
        return OpenPanel(panel);
    }
    public void CloseCurrentPanel()
    {
        if (currentPanel == UIPanel.None)
            return;
        GameObject panel = GetPanel(currentPanel);

        if (panel != null)
            panel.SetActive(false);

        currentPanel = UIPanel.None;
        UpdateUIState();
    }
    private void HandleEscape()
    {
        if (HasOpenPanel())
            CloseCurrentPanel();
    }
    private void CloseAllPanels()
    {
        inventoryPanel.SetActive(false);
        settingsPanel.SetActive(false);
        journalPanel.SetActive(false);
        shopPanel.SetActive(false);
        experimentpanel.SetActive(false);

        currentPanel = UIPanel.None;
        UpdateUIState();
    }
    private void UpdateUIState()
    {
        bool isOpen = HasOpenPanel();
        MouseToGrid.Instance.ChangeCursor(isOpen);
        if (pauseTimeWhenSettings)
            Time.timeScale = currentPanel != UIPanel.None ? 0f : 1f;
    }
}