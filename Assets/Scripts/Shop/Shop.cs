using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class Shop : MonoBehaviour
{
    public static Shop instance;

    public GameObject shopPanel;
    public Transform listContentParent;
    public GameObject shopRowPrefab;
    public List<ShopData> shopitems = new();

    public TextMeshProUGUI moneyText;
    public Button closeButton;
    public Button buyTabButton;
    public Button sellTabButton;

    public Image previewIcon;
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailCatagoryText;
    public TextMeshProUGUI detailDescriptionText;
    public TextMeshProUGUI detailUnitPriceText;
    public TextMeshProUGUI QuantityText;
    public TextMeshProUGUI totalPriceText;
    public Button minusButton;
    public Button plusButton;
    public Button confirmButton;

    public TextMeshProUGUI confirmButtonLabel;
    public bool IsSellingMode { get; private set; } = false;

    private ShopData SelectedItem;
    private int Quantity = 1;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GameManager.Money = 1000;
        RefreshMoneyUI();
        Inventory.instance.RefreshMoney();
        if (closeButton != null)
            closeButton.onClick.AddListener(() => SetShopOpen(false));
        if (buyTabButton != null)
            buyTabButton.onClick.AddListener(() => SetMode(false));
        if (sellTabButton != null)
            sellTabButton.onClick.AddListener(() => SetMode(true));
        if (minusButton != null)
            minusButton.onClick.AddListener(() => ChangeQuantity(-1));
        if (plusButton != null)
            plusButton.onClick.AddListener(() => ChangeQuantity(1));
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmTransaction);
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
    public void ToggleShop()
    {
        SetShopOpen(!shopPanel.activeSelf);
    }
    public void SetShopOpen(bool open)
    {
        shopPanel.SetActive(open);
        GameManager.IsInventoryOpen = open;
        MouseToGrid.Instance.ChangeCursor(open);
        if (open)
        {
            Quantity = 1;
            RefreshMoneyUI();
            RebuildList();
            if (shopitems.Count > 0)
                SelectItem(shopitems[0]);
            else
                ClearDetails();
        }
    }
    public void SetMode(bool sellMode)
    {
        IsSellingMode = sellMode;
        Quantity = 1;
        RebuildList();
        if (SelectedItem != null)
            SelectItem(SelectedItem);
    }
    public void RebuildList()
    {
        foreach (Transform child in listContentParent)
        {
            Destroy(child.gameObject);
        }
        foreach (ShopData item in shopitems)
        {
            GameObject rowObj = Instantiate(shopRowPrefab, listContentParent);
            ShopItemRow row = rowObj.GetComponent<ShopItemRow>();
            if (row != null)
            {
                row.Setup(item, this);
            }
        }
    }
    public void SelectItem(ShopData item)
    {
        SelectedItem = item;
        Quantity = Mathf.Clamp(Quantity, 1, GetMaxAffordableorOwned());
        RefreshDetails();
    }
    public void ChangeQuantity(int delta)
    {
        if (SelectedItem == null)
            return;
        int maxAllowed = Mathf.Max(1, GetMaxAffordableorOwned());
        Quantity = Mathf.Clamp(Quantity + delta, 1, maxAllowed);
        RefreshDetails();
    }
    private int GetMaxAffordableorOwned()
    {
        if (SelectedItem == null || SelectedItem.plantData == null)
            return 1;
        if (!IsSellingMode)
        {
            if (SelectedItem.buyPrice <= 0) return 99;
            return Mathf.Max(1, GameManager.Money / SelectedItem.buyPrice);
        }
        else
        {
            int owned = SelectedItem.sellsSeeds ? Inventory.instance.TryGetSeeds(SelectedItem.plantData) : Inventory.instance.TryGetPlant(SelectedItem.plantData);
            return Mathf.Max(1, owned);
        }
    }
    private void RefreshDetails()
    {
        if (SelectedItem == null || SelectedItem.plantData == null)
        {
            ClearDetails();
            return;
        }
        PlantData pData = SelectedItem.plantData;
        bool isSeed = SelectedItem.sellsSeeds;

        previewIcon.enabled = true;
        previewIcon.sprite = isSeed ? pData.seeds.seedinventorySprite : pData.inventorySprite;

        detailTitleText.text = string.IsNullOrEmpty(SelectedItem.itemName) ? (isSeed ? pData.seeds.SeedName : pData.plantName) : SelectedItem.itemName;

        detailCatagoryText.text = isSeed ? "Seed" : "Crop";

        if (!string.IsNullOrEmpty(SelectedItem.customDescripton))
            detailDescriptionText.text = SelectedItem.customDescripton;
        else
            detailCatagoryText.text = isSeed ? pData.seeds.seedDetails : pData.plantDetails;

        int unitPrice = IsSellingMode ? SelectedItem.sellPrice : SelectedItem.buyPrice;
        int totalPrice = unitPrice * Quantity;

        detailUnitPriceText.text = unitPrice + "g each";
        QuantityText.text = Quantity.ToString();
        totalPriceText.text = "Total: " + totalPrice + "g";
        confirmButtonLabel.text = IsSellingMode ? "SELL" : "BUY";
        if (!IsSellingMode)
        {
            confirmButton.interactable = GameManager.Money >= totalPrice && unitPrice >= 0;
        }
        else
        {
            int owned = isSeed ? Inventory.instance.TryGetSeeds(pData) : Inventory.instance.TryGetPlant(pData);
            confirmButton.interactable = owned >= Quantity && Quantity > 0;
        }
    }
    private void ClearDetails()
    {
        previewIcon.enabled = false;
        detailCatagoryText.text = "";
        detailTitleText.text = "";
        detailDescriptionText.text = "";
        detailUnitPriceText.text = "";
        QuantityText.text = "0";
        totalPriceText.text = "";
        confirmButton.interactable = false;
    }
    public void ConfirmTransaction()
    {
        if (SelectedItem == null || SelectedItem.plantData == null)
            return;

        PlantData pData = SelectedItem.plantData;
        bool isSeed = SelectedItem.sellsSeeds;
        if (!IsSellingMode)
        {
            int cost = SelectedItem.buyPrice * Quantity;
            if (GameManager.Money < cost)
                return;
            GameManager.Money -= cost;
            if (isSeed)
                Inventory.instance.AddSeeds(pData, Quantity);
            else
                Inventory.instance.AddPlant(pData, Quantity);
        }
        else
        {
            int owned = isSeed ? Inventory.instance.TryGetSeeds(pData) : Inventory.instance.TryGetPlant(pData);
            if (owned < Quantity)
                return;
            if (isSeed)
                Inventory.instance.RemoveSeeds(pData, Quantity);
            else
                Inventory.instance.RemovePlant(pData, Quantity);
            GameManager.Money += SelectedItem.sellPrice * Quantity;
        }
        RefreshMoneyUI();
        RefreshDetails();
        if (GameManager.IsInventoryOpen && Inventory.instance.inventoryPanel.activeSelf)
        {
            Inventory.instance.InitialiseInv();
        }
    }
    public void RefreshMoneyUI()
    {
        string formatted = GameManager.Money.ToString("D3") + "g";
        if (moneyText != null)
            moneyText.text = formatted;
        if (moneyText != null)
        {
            moneyText.text = formatted;
        }
    }
}
