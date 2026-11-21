using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ExperimentResultType
{
    NewSpecies,
    NewTypeVarient
}
public enum VarientModifier
{
    SwithGrooth,
    BountifulYield,
    HighYield
}

[CreateAssetMenu(fileName = "New recipe", menuName = "Farming/Experiment", order = 2)]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public ExperimentResultType resultType = ExperimentResultType.NewTypeVarient;

    public PlantData IngredientA;
    public PlantData IngredientB;

    public string clueHint = "Hint or smt idk yet";

    public PlantData resultingSpecies;
    public PlantData basePlantvarient;

    public VarientModifier modtype = VarientModifier.SwithGrooth;
    public string VarientPrefix = "Swift";
    public Color varientTint = new Color(0.85f, 1.0f, 0.75f, 1f);
    [Range(0.25f, 1f)] public float growthMultipler = 0.75f;
    public int bonusYield;
    public float sellPriceMultiplier = 1.35f;

    public int craftCropCostA;
    public int craftCostCostB;
    public int craftOutputAmount = 2;

    public bool Matches(PlantData a, PlantData b)
    {
        return (a == IngredientA && b == IngredientB) || (a == IngredientB && a == IngredientA);
    }
}