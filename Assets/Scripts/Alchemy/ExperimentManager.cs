using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityPipeline.Microsoft.CodeAnalysis;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
public class ExperimentManager : MonoBehaviour {
    public static ExperimentManager instance;

    public List<Recipe> allRecipes = new();

    public GameObject experimentPanel;
    public PlantData slotA;
    public PlantData slotB;
    public Image slotAIcon;
    public Image SlotBIcon;
    public TextMeshProUGUI feedbackText;
    public Button combineButton;
    public Button closeExperiment;

    public GameObject recipeBookPanel;
    public Transform recipeListParent;
    public GameObject recipeRowPrefab;
    public Button closeRecipeBookButton;

    private HashSet<Recipe> unlockedRecipes = new();
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if(combineButton != null)
            combineButton.onClick.AddListener(TryCombine);
        if(closeExperiment != null)
            closeExperiment.onClick.AddListener(() => SetExperimentOpen(false));
        if(closeRecipeBookButton != null)
            closeRecipeBookButton.onClick.AddListener(() => SetRecipeBookOpen(false));
        if(experimentPanel != null) experimentPanel.SetActive(false);
        if(recipeBookPanel != null) recipeBookPanel.SetActive(false);

        RefreshSlotsUI();
    }
    public void SetExperimentOpen(bool open)
    {
        if(experimentPanel == null) return;
        experimentPanel.SetActive(open);
        GameManager.IsInventoryOpen = open;
        MouseToGrid.Instance.ChangeCursor(open);
        RefreshSlotsUI();
    }
    public void SetRecipeBookOpen(bool open)
    {
        if(recipeBookPanel == null) return;
        recipeBookPanel.SetActive(open);
        GameManager.IsInventoryOpen = open;
        MouseToGrid.Instance.ChangeCursor(open);
        if(open) RebuildRecipeBook();
    }
    public void SetIngredientA(PlantData plant)
    {
        slotA = plant;
        RefreshSlotsUI();
    }
    public void SetIngredientB(PlantData plata)
    {
        slotB = plata;
        RefreshSlotsUI();
    }
    public void RefreshSlotsUI()
    {
        if(slotAIcon != null)
        {
            slotAIcon.enabled = slotA != null && slotA.inventorySprite != null;
            if(slotAIcon.enabled) slotAIcon.sprite = slotA.inventorySprite;
        }
        if(SlotBIcon != null)
        {
            SlotBIcon.enabled = slotB != null && slotB.inventorySprite != null;
            if (SlotBIcon.enabled) SlotBIcon.sprite = slotB.inventorySprite;
        }
    }
    public void TryCombine()
    {
        if(slotAIcon == null || SlotBIcon == null)
        {
            SetFeedback("Select 2 crops to experiment");
            return;
        }
        int countA = Inventory.instance.TryGetPlant(slotA);
        int countB = (slotA == slotB) ? countA : Inventory.instance.TryGetPlant(slotB);

        bool hasEnough = (slotA == slotB) ? (countA >= 2) : (countA >= 1 && countB >= 1);
        if(!hasEnough)
        {
             SetFeedback("You dont have enough in your inventory");
             return;
        }

        Inventory.instance.RemovePlant(slotA, 1);
        Inventory.instance.RemovePlant(slotB, 1);

        Recipe matched = FindMatchingRecipe(slotA, slotB);
        if(matched == null)
        {
            string hint = GetHelpfulFailureFeedback(slotA, slotB);
            SetFeedback(hint);
            RefreshSlotsUI();
            return;
        }
        PlantData resultPlant = matched.resultType == ExperimentResultType.NewSpecies ? matched.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(matched);
        if(resultPlant == null)
        {
            SetFeedback("Uhh Dev f*cked up");
            RefreshSlotsUI();
            return;
        }
        bool isNewRecipe = unlockedRecipes.Add(matched);
        Inventory.instance.AddPlant(resultPlant, 1);

        Journal.instance.DiscoverCrop(resultPlant);

        string Label = matched.resultType == ExperimentResultType.NewSpecies ? "New Species" : "New Type";
        SetFeedback(isNewRecipe ? "Success! Unlocked " + Label + ": " + resultPlant.plantName : "Combined again into " + resultPlant.plantName);
        RefreshSlotsUI();
    }
    private Recipe FindMatchingRecipe(PlantData a, PlantData b)
    {
        foreach(Recipe recipe in allRecipes)
        {
            if(recipe != null && recipe.Matches(a, b))
                return recipe;
        }
        return null;
    }
    private string GetHelpfulFailureFeedback(PlantData a, PlantData b)
    {
        foreach(Recipe recipe in allRecipes)
        {
            if(recipe == null) continue;
            if(recipe.IngredientA == a)
                return "No reaction yet.. But there is potential";
            if(recipe.IngredientB == b)
                return "No reaction yet.. But there is some potential in b";
        }
        if(a == b)
            return "Paring these plants didnt do anything.";
        return "These plants didnt combine into anything, try more!";
    }
    public bool CraftFromRecipeBook(Recipe recipe)
    {
        if(recipe == null || !unlockedRecipes.Contains(recipe))
            return false;
        if(recipe.IngredientA != null && Inventory.instance.TryGetPlant(recipe.IngredientA) < recipe.craftCropCostA)
            return false;
        if (recipe.IngredientB != null && Inventory.instance.TryGetPlant(recipe.IngredientB) < recipe.craftCostCostB)
            return false;
        PlantData output = recipe.resultType == ExperimentResultType.NewSpecies ? recipe.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(recipe);
        
        if(output == null) return false;

        if(recipe.IngredientA != null && recipe.craftCropCostA > 0)
            Inventory.instance.RemovePlant(recipe.IngredientA, recipe.craftCropCostA);
        if (recipe.IngredientB != null && recipe.craftCostCostB > 0)
            Inventory.instance.RemovePlant(recipe.IngredientB, recipe.craftCostCostB);

        Inventory.instance.AddPlant(output, Mathf.Max(1, recipe.craftOutputAmount));
        return true;
    }
    public void RebuildRecipeBook()
    {
        if(recipeListParent == null || recipeRowPrefab == null)
            return;
        for(int i = recipeListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(recipeListParent.GetChild(i).gameObject);
        }
        foreach(Recipe recipe in unlockedRecipes)
        {
            GameObject row = Instantiate(recipeRowPrefab, recipeListParent);
            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            Button btn = row.GetComponentInChildren<Button>();

            string costStr = recipe.craftCropCostA + "x " + (recipe.IngredientA ? recipe.IngredientA.plantName : "None") + " + " +
                            recipe.craftCostCostB + "x " + (recipe.IngredientB? recipe.IngredientB.plantName : "None");
            PlantData result = recipe.resultType == ExperimentResultType.NewSpecies ? recipe.resultingSpecies : ProcCrop.instance.GetOrCreateVarient(recipe);
            label.text = result.plantName + costStr;
            if(btn != null)
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
        if(feedbackText != null)
            feedbackText.text = message;
    }
}