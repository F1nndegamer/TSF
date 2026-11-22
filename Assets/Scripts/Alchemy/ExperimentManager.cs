using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager instance;

    public List<Recipe> allRecipes = new();

    public GameObject experimentPanel;
    public GameObject experimentWs;
    public GameObject recipeWs;

    public Button experimentTab;
    public Button recipeTab;
    public Image experimentTabImage;
    public Image recipeTabImage;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public Button closeExperiment;

    public Transform pickerGridParent;
    public GameObject pickerSlotPrefab;

    public PlantData slotA;
    public PlantData slotB;
    public Image slotAIcon;
    public Image SlotBIcon;
    public Button slotAButton;
    public Button slotBButton;
    public Image resultPreviewIcon;
    public Sprite unknownResultSprite;
    public TextMeshProUGUI feedbackText;
    public Button combineButton;

    public Transform recipeListParent;
    public GameObject recipeRowPrefab;

    private HashSet<Recipe> unlockedRecipes = new();
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb.tabKey.wasPressedThisFrame)
        {
            ToggleExperiment();
        }
    }
    void Start()
    {
        if (combineButton != null)
            combineButton.onClick.AddListener(TryCombine);

        if (closeExperiment != null)
            closeExperiment.onClick.AddListener(() => SetExperimentOpen(false));

        if (experimentTab != null)
            experimentTab.onClick.AddListener(() => SwitchSubTab(showRecipeBook: false));

        if (recipeTab != null)
            recipeTab.onClick.AddListener(() => SwitchSubTab(showRecipeBook: true));
        if (slotAButton != null)
            slotAButton.onClick.AddListener(() => ClearSlot(isSlotA: true));
        if (slotBButton != null)
            slotBButton.onClick.AddListener(() => ClearSlot(isSlotA: false));
        if (experimentPanel != null) experimentPanel.SetActive(false);

        RefreshSlotsUI();
    }
    public void ToggleExperiment()
    {
        SetExperimentOpen(!experimentPanel.activeSelf);
    }
    public void SetExperimentOpen(bool open)
    {
        if (experimentPanel == null) return;
        experimentPanel.SetActive(open);
        GameManager.IsInventoryOpen = open;
        MouseToGrid.Instance.ChangeCursor(open);
        if (open)
        {
            SwitchSubTab(showRecipeBook: false);
            RebuildCropPicker();
            RefreshSlotsUI();
        }
    }
    public void SwitchSubTab(bool showRecipeBook)
    {
        if (experimentWs != null)
            experimentWs.SetActive(!showRecipeBook);
        if (recipeWs != null)
            recipeWs.SetActive(showRecipeBook);
        if (experimentTabImage != null)
            experimentTabImage.sprite = showRecipeBook ? inactiveSprite : activeSprite;
        if (recipeTabImage != null)
            recipeTabImage.sprite = showRecipeBook ? activeSprite : inactiveSprite;

        if (showRecipeBook)
            RebuildRecipeBook();
        else
            RebuildCropPicker();
    }
    public void RebuildCropPicker()
    {
        for (int i = pickerGridParent.childCount - 1; i >= 0; i--)
        {
            GameObject oldChild = pickerGridParent.GetChild(i).gameObject;
            oldChild.SetActive(false);
            Destroy(oldChild);
        }

        Dictionary<PlantData, int> ownedPlants = Inventory.instance.GetAllPlants();
        foreach (var kvp in ownedPlants)
        {
            PlantData plant = kvp.Key;
            int totalOwned = kvp.Value;
            if (plant == null || totalOwned <= 0)
                continue;

            int stagedCount = 0;
            if (slotA == plant) stagedCount++;
            if (slotB == plant) stagedCount++;

            int available = totalOwned - stagedCount;

            if (available <= 0)
                continue;

            GameObject slotObj = Instantiate(pickerSlotPrefab, pickerGridParent);
            InventoryItem itemUI = slotObj.GetComponent<InventoryItem>();
            if (itemUI != null)
            {
                itemUI.Sprite.sprite = plant.inventorySprite;
                itemUI.countText.text = available.ToString();
            }
            Button btn = slotObj.GetComponent<Button>();
            if (btn != null)
            {
                PlantData captureplant = plant;
                btn.onClick.AddListener(() => OnPickerCropClicked(captureplant));
            }
        }
    }
    private void OnPickerCropClicked(PlantData plata)
    {
        if (slotA == null)
            slotA = plata;
        else if (slotB == null)
            slotB = plata;
        else
            slotB = plata;

        if (resultPreviewIcon != null)
        {
            resultPreviewIcon.sprite = unknownResultSprite;
            resultPreviewIcon.enabled = unknownResultSprite != null;
        }
        RefreshSlotsUI();
        RebuildCropPicker();
    }
    public void ClearSlot(bool isSlotA)
    {
        if (isSlotA)
            slotA = null;
        else
            slotB = null;

        if (resultPreviewIcon != null)
        {
            resultPreviewIcon.sprite = unknownResultSprite;
            resultPreviewIcon.enabled = unknownResultSprite != null;
        }
        RefreshSlotsUI();
        RebuildCropPicker();
    }
    public void RefreshSlotsUI()
    {
        if (slotAIcon != null)
        {
            slotAIcon.enabled = slotA != null && slotA.inventorySprite != null;
            slotAIcon.color = slotAIcon.enabled ? Color.white : Color.clear;
            if (slotAIcon.enabled) slotAIcon.sprite = slotA.inventorySprite;
        }
        if (SlotBIcon != null)
        {
            SlotBIcon.enabled = slotB != null && slotB.inventorySprite != null;
            SlotBIcon.color = SlotBIcon.enabled ? Color.white : Color.clear;
            if (SlotBIcon.enabled) SlotBIcon.sprite = slotB.inventorySprite;
        }
        combineButton.interactable = (slotA != null && slotB != null);
    }
    public void TryCombine()
    {
        if (slotAIcon == null || SlotBIcon == null)
        {
            SetFeedback("Select 2 crops to experiment");
            return;
        }
        int countA = Inventory.instance.TryGetPlant(slotA);
        int countB = (slotA == slotB) ? countA : Inventory.instance.TryGetPlant(slotB);

        bool hasEnough = (slotA == slotB) ? (countA >= 2) : (countA >= 1 && countB >= 1);
        if (!hasEnough)
        {
            SetFeedback("You dont have enough in your inventory");
            slotA = null;
            slotB = null;
            RefreshSlotsUI();
            RebuildCropPicker();
            return;
        }
        PlantData usedA = slotA;
        PlantData usedB = slotB;

        Inventory.instance.RemovePlant(usedA, 1);
        Inventory.instance.RemovePlant(usedB, 1);

        Recipe matched = FindMatchingRecipe(usedA, usedB);
        if (matched == null)
        {
            SetFeedback(GetHelpfulFailureFeedback(usedB, usedB));
            RefreshSlotsUI();
            RebuildCropPicker();
            return;
        }

        PlantData resultPlant = matched.resultType == ExperimentResultType.NewSpecies ? matched.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(matched);

        if (resultPlant == null)
        {
            SetFeedback("Uhh Dev f*cked up");
            RefreshSlotsUI();
            RebuildCropPicker();
            return;
        }
        bool isNewRecipe = unlockedRecipes.Add(matched);
        Inventory.instance.AddSeeds(resultPlant, 1);

        Journal.instance.DiscoverCrop(resultPlant);

        resultPreviewIcon.enabled = true;
        resultPreviewIcon.sprite = resultPlant.inventorySprite;
        string Label = matched.resultType == ExperimentResultType.NewSpecies ? "New Species" : "New Type";
        SetFeedback(isNewRecipe ? "Success! Unlocked " + Label + ": " + resultPlant.plantName : "Combined again into " + resultPlant.plantName);
        RefreshSlotsUI();
        RebuildCropPicker();
    }
    private Recipe FindMatchingRecipe(PlantData a, PlantData b)
    {
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe != null && recipe.Matches(a, b))
                return recipe;
        }
        return null;
    }
    private string GetHelpfulFailureFeedback(PlantData a, PlantData b)
    {
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null) continue;
            if (recipe.IngredientA == a)
                return "No reaction yet.. But there is potential";
            if (recipe.IngredientB == b)
                return "No reaction yet.. But there is some potential in b";
        }
        if (a == b)
            return "Paring these plants didnt do anything.";
        return "These plants didnt combine into anything, try more!";
    }
    public bool CraftFromRecipeBook(Recipe recipe)
    {
        if (recipe == null || !unlockedRecipes.Contains(recipe))
            return false;
        if (recipe.IngredientA != null && Inventory.instance.TryGetPlant(recipe.IngredientA) < recipe.craftCropCostA)
            return false;
        if (recipe.IngredientB != null && Inventory.instance.TryGetPlant(recipe.IngredientB) < recipe.craftCostCostB)
            return false;
        PlantData output = recipe.resultType == ExperimentResultType.NewSpecies ? recipe.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(recipe);

        if (output == null) return false;

        if (recipe.IngredientA != null && recipe.craftCropCostA > 0)
            Inventory.instance.RemovePlant(recipe.IngredientA, recipe.craftCropCostA);
        if (recipe.IngredientB != null && recipe.craftCostCostB > 0)
            Inventory.instance.RemovePlant(recipe.IngredientB, recipe.craftCostCostB);

        Inventory.instance.AddPlant(output, Mathf.Max(1, recipe.craftOutputAmount));
        return true;
    }
    public void RebuildRecipeBook()
    {
        if (recipeListParent == null || recipeRowPrefab == null)
            return;

        for (int i = recipeListParent.childCount - 1; i >= 0; i--)
        {
            GameObject oldRow = recipeListParent.GetChild(i).gameObject;
            oldRow.SetActive(false);
            Destroy(recipeListParent.GetChild(i).gameObject);
        }
        foreach (Recipe recipe in unlockedRecipes)
        {
            GameObject row = Instantiate(recipeRowPrefab, recipeListParent);
            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            Button btn = row.GetComponentInChildren<Button>();
            Debug.Log(btn == null);
            Debug.Log(label == null);
            string costStr = recipe.craftCropCostA + "x " + (recipe.IngredientA ? recipe.IngredientA.plantName : "None") + " + " +
                            recipe.craftCostCostB + "x " + (recipe.IngredientB ? recipe.IngredientB.plantName : "None");

            PlantData result = recipe.resultType == ExperimentResultType.NewSpecies ? recipe.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(recipe);
            label.text = result.plantName + costStr;
            if (btn != null)
            {
                Recipe captured = recipe;
                btn.onClick.AddListener(() =>
                {
                    CraftFromRecipeBook(captured);
                    RebuildRecipeBook();
                });
            }
        }
    }
    private void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }
}