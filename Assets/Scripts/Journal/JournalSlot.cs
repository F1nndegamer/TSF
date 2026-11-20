using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class JournalSlot : MonoBehaviour
{
    public Image backgroundImage;
    public Image plantIcon;
    public Button button;

    public void Setup(PlantData plant, bool isDiscovered, bool isSelected, Sprite normalbg, Sprite selectedBg, System.Action onClick)
    {
        backgroundImage.sprite = isSelected ? selectedBg : normalbg;
        if(plant != null && plant.inventorySprite != null)
        {
            plantIcon.enabled = true;
            plantIcon.sprite = plant.inventorySprite;


            plantIcon.color = isDiscovered ? Color.white : new Color(0.18f, 0.11f, 0.08f, 0.85f);
        }
        else
        {
            plantIcon.enabled = false;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}