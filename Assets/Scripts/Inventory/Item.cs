using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler // TODO? Different logic for InputButton.Right
{
    public static Action<Item> OnAnyItemClicked; // TODO Subscribe to this and update some info tab/window/tooltip

    [field:SerializeField] public bool PlayerProperty { get; private set; }
    [field:SerializeField] public bool IsMoney { get; private set; }
    [field:SerializeField] public ItemScriptableObject ItemSO { get; private set; }
    
    public Currency CurrencyType { get;  private set; }

    [SerializeField] Image _itemImage, _raycastImage;
    [SerializeField] Color _selectedColor, _defaultColor;
    [SerializeField] Canvas _parentCanvas;
    
    DropBox _currentBox;
    bool _inTrade;
    Vector2 _startPosition;

    void OnEnable()
    {
        OnAnyItemClicked += Item_OnAnyItemClicked;
    }

    void OnDisable()
    {
        OnAnyItemClicked += Item_OnAnyItemClicked;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!PlayerProperty && IsMoney) { return; }
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        OnAnyItemClicked?.Invoke(this);

        _raycastImage.raycastTarget = false;
        _currentBox.RemoveItem(gameObject);
        transform.SetParent(_parentCanvas.transform, true);
        _inTrade = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!PlayerProperty && IsMoney) { return; }
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!PlayerProperty && IsMoney) { return; }
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        _raycastImage.raycastTarget = true;
        transform.SetParent(_currentBox.transform, true);
        transform.position = _currentBox.transform.position;
        
        if(_inTrade) { return; }

        if(IsMoney)
        {
            SendToCoinBox(_currentBox.CoinBox);
        }
        else
        {
            SendToDropBox(_currentBox.InventoryBox);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }
        if(!PlayerProperty && IsMoney) { return; }

        OnAnyItemClicked?.Invoke(this);
        
        if(IsMoney)
        {
            if(!_inTrade)
            {
                _currentBox.RemoveItem(gameObject);
                SendToDropBox(_currentBox.TradeBox);
                _inTrade = true;
            }
            else
            {
                _currentBox.RemoveItem(gameObject);
                SendToCoinBox(_currentBox.CoinBox);
                _inTrade = false;
            }
        }
        else
        {
            if(!_inTrade)
            {
                _currentBox.RemoveItem(gameObject);
                SendToDropBox(_currentBox.TradeBox);
                _inTrade = true;
            }
            else
            {
                _currentBox.RemoveItem(gameObject);
                SendToDropBox(_currentBox.InventoryBox);
                _inTrade = false;
            }
        }
    }

    public void SetInTrade(bool inTrade)
    {
        _inTrade = inTrade;
    }

    public void SetUp(ItemScriptableObject itemSO, bool playerProperty, DropBox dropBox)
    {
        ItemSO = itemSO;
        gameObject.name = ItemSO.ItemName;
        _itemImage.sprite = ItemSO.ItemSprite;
        SetPlayerProperty(playerProperty);
        SetStartPosition();
        _currentBox = dropBox;
        _currentBox.AddItem(gameObject);
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public void SetUpMoney(ItemScriptableObject itemSO, bool playerProperty, DropBox coinBox, Currency coinType)
    {
        IsMoney = true;
        CurrencyType = coinType;
        SetUp(itemSO, playerProperty, coinBox);

        SortCoins(coinBox);
    }

    void SortCoins(DropBox coinBox)
    {
        switch(CurrencyType)
        {
            case Currency.Copper:
                transform.SetParent(coinBox.CopperParent, true);
                transform.position = coinBox.CopperParent.position;
                SetStartPosition();
                break;
            case Currency.Silver:
                transform.SetParent(coinBox.SilverParent, true);
                transform.position = coinBox.SilverParent.position;
                SetStartPosition();
                break;
            case Currency.Gold:
                transform.SetParent(coinBox.GoldParent, true);
                transform.position = coinBox.GoldParent.position;
                SetStartPosition();
                break;
            case Currency.Platinum:
                transform.SetParent(coinBox.PlatinumParent, true);
                transform.position = coinBox.PlatinumParent.position;
                SetStartPosition();
                break;
        }
        _inTrade = false;
    }

    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    public void SetCurrentBox(DropBox newBox)
    {
        _currentBox = newBox;
        _currentBox.AddItem(gameObject);
    }

    public void SendToDropBox(DropBox inventoryBox)
    {
        _currentBox = inventoryBox;
        _currentBox.AddItem(gameObject);
        transform.position = _currentBox.transform.position;
        transform.SetParent(_currentBox.transform, true);
        SetStartPosition();
    }

    public void SendToCoinBox(DropBox coinBox)
    {
        _currentBox = coinBox;
        _currentBox.AddItem(gameObject);
        SortCoins(coinBox);
    }

    public void ResetPosition(DropBox inventoryBox)
    {
        _currentBox = inventoryBox;
        _currentBox.AddItem(gameObject);
        transform.position = _startPosition;
        transform.SetParent(_currentBox.transform, true);
        SetStartPosition();
    }

    public void SetPlayerProperty(bool playerProperty)
    {
        PlayerProperty = playerProperty;
    }

    void Item_OnAnyItemClicked(Item selectedItem)
    {
        if(selectedItem != this)
        {
            _raycastImage.color = _defaultColor;
        }
        else
        {
            _raycastImage.color = _selectedColor;
        }
    }
}
