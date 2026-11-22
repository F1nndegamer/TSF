using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemRow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button RowButton;

    private ShopData data;
    private Shop ownerShop;

    public void Setup(ShopData shopData, Shop shop)
    {
        data = shopData;
        ownerShop = shop;
        icon.sprite = data.sellsSeeds ? data.plantData.seeds.seedinventorySprite : data.plantData.inventorySprite;
        nameText.text = string.IsNullOrEmpty(data.itemName) ? (data.sellsSeeds ? data.plantData.seeds.SeedName : data.plantData.plantName) : data.itemName;
        int buyPrice = data.sellsSeeds ? data.plantData.seeds.buyPrice : data.plantData.buyPrice;
        int sellPrice = data.sellsSeeds ? data.plantData.seeds.sellPrice : data.plantData.sellPrice;
        int price = ownerShop.IsSellingMode ? sellPrice : buyPrice;
        priceText.text = price + "g";

        RowButton.onClick.RemoveAllListeners();
        RowButton.onClick.AddListener(() => ownerShop.SelectItem(data));
    }
}