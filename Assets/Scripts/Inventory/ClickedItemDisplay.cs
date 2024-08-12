using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickedItemDisplay : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] TextMeshProUGUI _itemNameText;

    void Start()
    {
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        Item.OnAnyItemClicked += Item_OnAnyItemClicked;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        Item.OnAnyItemClicked -= Item_OnAnyItemClicked;
    }

    void TradingSystem_OnTradeCompleted(Customer customer)
    {
        gameObject.SetActive(false);
    }

    void Item_OnAnyItemClicked(Item item)
    {
        _itemImage.sprite = item.ItemSO.ItemSprite;
        _itemNameText.text = item.ItemSO.ItemName;
        gameObject.SetActive(true);
    }
}
