using System.Collections.Generic;
using UnityEngine;

public class ProcCrop : MonoBehaviour
{
    public static ProcCrop instance;
    private Dictionary<string, PlantData> generatedCache = new();
    void Awake()
    {
        instance = this;
    }
    public PlantData GetOrCreateVarient(Recipe recipe)
    {
        string key = recipe.name + "_" + recipe.basePlantvarient + "_" + recipe.modtype;
        if(generatedCache.TryGetValue(key, out PlantData cached) && cached != null)
            return cached;
        PlantData varient = ScriptableObject.Instantiate(recipe.basePlantvarient);
        varient.name = key;
        varient.plantName = string.IsNullOrEmpty(recipe.VarientPrefix) ? recipe.basePlantvarient.plantName + " Varient" : recipe.VarientPrefix + recipe.basePlantvarient.plantName;

        varient.HotBarSlot = -1;
        varient.location = Location.Inventory;

        switch(recipe.modtype)
        {
            case VarientModifier.SwithGrooth:
                if(varient.seeds != null)
                {
                    SeedData seedClone = ScriptableObject.Instantiate(varient.seeds);
                    seedClone.growthTime = Mathf.Max(1f, Mathf.Round(varient.seeds.growthTime * recipe.growthMultipler));
                    seedClone.HotBarSlot = -1;
                    seedClone.location = Location.Inventory;
                    varient.seeds = seedClone;
                }
                varient.plantDetails = "A quick varient of " + recipe.basePlantvarient.plantName;
                break;
            case VarientModifier.BountifulYield:
                varient.harvestYield += Mathf.Max(1, recipe.bonusYield);
                varient.plantDetails = "A more sustainable varient of " + recipe.basePlantvarient.plantName;
                break;
        }

        generatedCache[key] = varient;
        return varient;
    }    
}