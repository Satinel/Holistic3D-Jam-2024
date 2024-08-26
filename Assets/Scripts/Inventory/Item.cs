using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler
{
    public static Action OnCoinClicked;
    public static Action<Item> OnItemPointedAt;
    public static Action<bool> OnItemDrag;

    [field:SerializeField] public bool PlayerProperty { get; private set; }
    [field:SerializeField] public bool IsMoney { get; private set; }
    [field:SerializeField] public ItemScriptableObject ItemSO { get; private set; }
    
    public Currency CurrencyType { get;  private set; }

    [SerializeField] Image _itemImage, _raycastImage;
    [SerializeField] Color _selectedColor, _defaultColor;
    [SerializeField] Canvas _parentCanvas;
    
    DropBox _currentBox;
    bool _inTrade, _barterLocked;
    Vector2 _startPosition;
    Customer.Type _customerType;

    public DropBox CurrentBox => _currentBox;

    void OnEnable()
    {
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnBarterAccepted += TradingSystem_OnBarterAccepted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnResetBarter += TradingSystem_OnResetBarter;
    }

    void OnDisable()
    {
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnBarterAccepted -= TradingSystem_OnBarterAccepted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnResetBarter -= TradingSystem_OnResetBarter;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!PlayerProperty) { return; }

        if(!customer)
        {
            _customerType = Customer.Type.None;
            return;
        }

        _customerType = customer.CustomerType;
    }

    void TradingSystem_OnBarterAccepted(int offer)
    {
        _barterLocked = true;
    }

    void TradingSystem_OnTradeCancelled()
    {
        _barterLocked = false;
    }

    void TradingSystem_OnResetBarter()
    {
        _barterLocked = false;
    }

    void TradingSystem_OnTradeCompleted(Customer customer)
    {
        _barterLocked = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            eventData.pointerDrag = null;
            return;
        }

        switch(_customerType)
        {
            case Customer.Type.Buy:
                if(PlayerProperty && !IsMoney) // Cannot add extra items to sell to customer
                {
                    eventData.pointerDrag = null;
                    return;
                }
                if(!PlayerProperty) // Cannot move customer items
                {
                    eventData.pointerDrag = null;
                    return;
                }
                break;
            case Customer.Type.Sell:
                if(!PlayerProperty) // Cannot move customer items
                {
                    eventData.pointerDrag = null;
                    return;
                }
                // if(!IsMoney)
                // {
                //     eventData.pointerDrag = null;
                //     return;
                // }
                break;
            case Customer.Type.Barter:
                if(_barterLocked && !IsMoney)
                {
                    eventData.pointerDrag = null; // Cannot add/remove items when barter offer accepted (only give change)
                    return;
                }
                if(!PlayerProperty && IsMoney) // Can move customer items but not their money
                {
                    eventData.pointerDrag = null;
                    return;
                }
                break;
            case Customer.Type.Bank:
                if(!PlayerProperty) // Cannot move customer items (but there shouldn't be any)
                {
                    eventData.pointerDrag = null;
                    return;
                }
                if(!IsMoney) // Cannot move non-money player items
                {
                    eventData.pointerDrag = null;
                    return;
                }
                break;
            case Customer.Type.None:
                eventData.pointerDrag = null; // There is no customer so don't move items
                return;
            default:
                return; // NO DRAGGING ITEMS WITHOUT A CUSTOMER!
        }

        _raycastImage.raycastTarget = false;
        _currentBox.RemoveItem(gameObject);
        transform.SetParent(_parentCanvas.transform, true);
        _inTrade = false;
        OnItemDrag?.Invoke(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        _raycastImage.raycastTarget = true;
        transform.SetParent(_currentBox.transform, true);
        transform.position = _currentBox.transform.position;
        OnItemDrag?.Invoke(false);
        
        if(_inTrade)
        {
            if(IsMoney)
            {
                SortCoins(_currentBox);
            }
            return;
        }

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

        switch(_customerType)
        {
            case Customer.Type.Buy:
                if(!PlayerProperty) { return; } // Can only move player items
                if(PlayerProperty && !IsMoney) { return; } // Cannot add extra items to be sold
                break;
            case Customer.Type.Sell:
                if(!PlayerProperty) { return; } // Can only move player items
                // if(!IsMoney) { return; }
                break;
            case Customer.Type.Bank:
                if(PlayerProperty && !IsMoney) { return; } // Can only move player money
                if(!PlayerProperty) { return; } // Cannot move customer items
                break;
            case Customer.Type.Barter:
                if(_barterLocked && !IsMoney) { return; } // Cannot alter items after offer accepted
                if(!PlayerProperty && IsMoney) { return; } // Cannot move customer's money
                break;
            case Customer.Type.None:
                return; // No customer means no moving items!
            default:
                return; // Cannot move ANYTHING because it messes stuff up!
        }
        
        if(IsMoney)
        {
            OnCoinClicked?.Invoke();
            
            if(!_inTrade)
            {
                _currentBox.AddToSentItems(this);
                _currentBox.RemoveItem(gameObject);
                SendToDropBox(_currentBox.TradeBox);
                SortCoins(_currentBox);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnItemPointedAt?.Invoke(this);
    }

    public void SetInTrade(bool inTrade)
    {
        _inTrade = inTrade;
    }

    public void SetUp(ItemScriptableObject itemSO, bool playerProperty, DropBox dropBox, Customer.Type customerType)
    {
        ItemSO = itemSO;
        gameObject.name = ItemSO.ItemName;
        // _itemImage.sprite = ItemSO.ItemSprite;
        _raycastImage.sprite = ItemSO.ItemSprite;
        SetPlayerProperty(playerProperty);
        SetStartPosition();
        _currentBox = dropBox;
        _currentBox.AddItem(gameObject);
        _parentCanvas = GetComponentInParent<Canvas>();
        _customerType = customerType;
    }

    public void SetUpMoney(ItemScriptableObject itemSO, bool playerProperty, DropBox coinBox, Currency coinType, Customer.Type customerType)
    {
        IsMoney = true;
        CurrencyType = coinType;
        SetUp(itemSO, playerProperty, coinBox, customerType);

        SortCoins(coinBox);
    }

    public void SortCoins(DropBox coinBox)
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
        if(coinBox.IsTradeBox)
        {
            _inTrade = true;
        }
        else
        {
            _inTrade = false;
        }
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
        _currentBox.RemoveFromSentItems(this);
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

    // void Item_OnAnyItemClicked(Item selectedItem)
    // {
    //     if(selectedItem != this)
    //     {
    //         _raycastImage.color = _defaultColor;
    //     }
    //     else
    //     {
    //         _raycastImage.color = _selectedColor;
    //     }
    // }
}
