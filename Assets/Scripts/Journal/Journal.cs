using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    public static Journal instance;

    public List<PlantData> allCrops = new();

    public GameObject journalPanel;
    public Transform gridParent;
    public GameObject journalSlotPrefab;
    public TextMeshProUGUI progressText;
    public Button closeButton;

    public Sprite normalSlotSprite;
    public Sprite selectedSlotSprite;

    public Image previewIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descriptionText;

    public GameObject discoveryPopup;
    public Image popupIcon;
    public TextMeshProUGUI popupText;

    private HashSet<PlantData> discoveredCrops = new();
    private PlantData selectedCrop;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(() => SetJournalOpen(false));
        if (journalPanel != null)
            journalPanel.SetActive(false);
        if (discoveryPopup != null)
            discoveryPopup.SetActive(false);
    }
    public void ToggleJournal()
    {
        if(UIManager.instance.TogglePanel(UIPanel.Journal))
        {
            if(selectedCrop == null && allCrops.Count > 0)
                selectedCrop = allCrops[0];
            RefreshJournalUI();
        }
    }
    public void SetJournalOpen(bool open)
    {
        if (open)
        {
            if (!UIManager.instance.OpenPanel(UIPanel.Journal))
                return;
            if(selectedCrop == null && allCrops.Count > 0)
                selectedCrop = allCrops[0];
            RefreshJournalUI();
        }
        else
        {
            UIManager.instance.ClosePanel(UIPanel.Journal);
        }
    }
    public bool isDiscovered(PlantData data)
    {
        return data != null && discoveredCrops.Contains(data);
    }
    public void DiscoverCrop(PlantData plata, bool showPopup = true)
    {
        if (plata == null || discoveredCrops.Contains(plata))
            return;

        discoveredCrops.Add(plata);
        if (showPopup && discoveryPopup != null)
        {
            StartCoroutine(ShowDiscovery(plata));
        }
        if (journalPanel != null && journalPanel.activeSelf)
        {
            RefreshJournalUI();
        }
    }
    private System.Collections.IEnumerator ShowDiscovery(PlantData data)
    {
        discoveryPopup.SetActive(true);
        if (popupIcon != null) popupIcon.sprite = data.inventorySprite;
        if (popupText != null) popupText.text = "New Entry: " + data.plantName;

        yield return new WaitForSeconds(3f);
        discoveryPopup.SetActive(false);
    }
    public void RefreshJournalUI()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        if (progressText != null)
            progressText.text = discoveredCrops.Count + "/" + allCrops.Count;
        foreach (PlantData crop in allCrops)
        {
            GameObject slotObj = Instantiate(journalSlotPrefab, gridParent);
            JournalSlot slot = slotObj.GetComponent<JournalSlot>();
            bool unlocked = isDiscovered(crop);
            bool isSelected = (crop == selectedCrop);

            slot.Setup(crop, unlocked, isSelected, normalSlotSprite, selectedSlotSprite, () =>
            {
                selectedCrop = crop;
                RefreshJournalUI();
            });
        }
        RefreshdetailPane();
    }
    private void RefreshdetailPane()
    {
        if (selectedCrop == null)
            return;
        bool unlocked = isDiscovered(selectedCrop);

        previewIcon.enabled = true;
        previewIcon.sprite = selectedCrop.inventorySprite;
        if (unlocked)
        {
            previewIcon.color = Color.white;
            titleText.text = selectedCrop.plantName;
            float growTime = selectedCrop.seeds != null ? selectedCrop.seeds.growthTime : 0f;
            statsText.text = "Grow: " + growTime + "s   |   Yield: x" + selectedCrop.harvestYield + "   |   Sell: " + selectedCrop.sellPrice + "g";
            descriptionText.text = selectedCrop.plantDetails;
        }
        else
        {
            previewIcon.color = new Color(0.18f, 0.11f, 0.08f, 0.85f);
            titleText.text = "???";
            statsText.text = "Grow: ??? | Yield: ???";
            descriptionText.text = "Undiscovered plant. Plant more or Experiment :eyes:";
        }
    }
}