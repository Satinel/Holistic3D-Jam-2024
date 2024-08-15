using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickedItemDisplay : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] TextMeshProUGUI _itemNameText;
    [SerializeField] GameObject _itemNameParent;
    [SerializeField] AudioSource _audioSource;

    // Currently there's no good place to put this image that isn't going to be confusing so the original function is disabled

    void Start()
    {
        // TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        Item.OnAnyItemClicked += Item_OnAnyItemClicked;
        _itemImage.enabled = false;
        _itemNameParent.SetActive(false);
    }

    void OnDestroy()
    {
        // TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        Item.OnAnyItemClicked -= Item_OnAnyItemClicked;
    }

    // void TradingSystem_OnTradeCompleted(Customer customer)
    // {
    //     _itemImage.enabled = false;
    //     _itemNameParent.SetActive(false);
    // }

    void Item_OnAnyItemClicked(Item item)
    {
        if(item.IsMoney)
        {
            _audioSource.Play();
        }
        // else
        // {
        //     _itemImage.sprite = item.ItemSO.ItemSprite;
        //     _itemNameText.text = item.ItemSO.ItemName;
        //     gameObject.SetActive(true);
        //     _itemImage.enabled = true;
        //     _itemNameParent.SetActive(true);
        // }
    }
}
