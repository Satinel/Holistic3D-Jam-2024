using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;
    public static Action<bool, Currency> OnCoinAdded;
    public static Action<bool, Currency> OnCoinRemoved;

    [SerializeField] Transform _copperParent, _silverParent, _goldParent, _platinumParent;

    [SerializeField] bool _playerProperty, _isTradeBox, _isCoinBox;
    [SerializeField] DropBox _tradeBox, _coinBox, _inventoryBox, _partnerBox, _partnerCoinBox;

    public DropBox TradeBox => _tradeBox;
    public DropBox CoinBox => _coinBox;
    public DropBox InventoryBox => _inventoryBox;
    public Transform CopperParent => _copperParent;
    public Transform SilverParent => _silverParent;
    public Transform GoldParent => _goldParent;
    public Transform PlatinumParent => _platinumParent;

    List<GameObject> _items = new();
    int _totalValue;

    void OnEnable()
    {
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer += TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer += TradingSystem_OnSellCustomer;
    }

    void OnDisable()
    {
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer -= TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer -= TradingSystem_OnSellCustomer;
    }

    void TradingSystem_OnTradeCompleted()
    {
        if(_isTradeBox)
        {
            foreach(var tradedItem in _items)
            {
                Item item = tradedItem.GetComponent<Item>();
                item.SetInTrade(false);
                item.SetPlayerProperty(!_playerProperty);
                if(item.ItemSO.Type == Type.Coin)
                {
                    item.SendToCoinBox(_partnerCoinBox);
                }
                else
                {
                    item.SendToDropBox(_partnerBox);
                }
            }
            _items.Clear();
            _totalValue = 0;
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }

        if(!_playerProperty)
        {
            foreach(GameObject customerItem in _items)
            {
                customerItem.SetActive(false);
            }
            _items.Clear();
            _totalValue = 0;
        }
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(_isTradeBox)
        {
            foreach(var cancelledItem in _items)
            {
                Item item = cancelledItem.GetComponent<Item>();
                item.SetInTrade(false);
                if(item.ItemSO.Type == Type.Coin)
                {
                    item.SendToCoinBox(_coinBox);
                }
                else
                {
                    item.SendToDropBox(_inventoryBox);
                }
            }
            _items.Clear();
            _totalValue = 0;
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }

    void TradingSystem_OnBuyCustomer()
    {
        if (_isCoinBox || _isTradeBox) { return; }

        if (!_playerProperty) { return; }

        PickRandomItem();
    }

    void TradingSystem_OnSellCustomer()
    {
        if(_isCoinBox || _isTradeBox) { return; }

        if(_playerProperty) { return; }

        PickRandomItem();
    }

    void PickRandomItem()
    {
        Item randomItem = _items[UnityEngine.Random.Range(0, _items.Count)].GetComponent<Item>();

        _items.Remove(randomItem.gameObject);
        randomItem.SendToDropBox(_tradeBox);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item droppedItem = eventData.pointerDrag.GetComponent<Item>();

        if(!droppedItem) { return; }

        if(_playerProperty != droppedItem.PlayerProperty) { return; }

        if(_isTradeBox)
        {
            droppedItem.SetCurrentBox(this);
            droppedItem.SetInTrade(true);
            return;
        }
    }

    public void AddItem(GameObject itemPrefab)
    {
        _items.Add(itemPrefab);
        ItemScriptableObject itemSO = itemPrefab.GetComponent<Item>().ItemSO;
        _totalValue += itemSO.BaseValue;

        if(_isTradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
        if(_isCoinBox)
        {
            Item item = itemPrefab.GetComponent<Item>();
            OnCoinAdded(_playerProperty, item.CurrencyType);
        }
    }

    public void RemoveItem(GameObject itemPrefab)
    {
        if(_items.Contains(itemPrefab))
        {
            _items.Remove(itemPrefab);
            ItemScriptableObject itemSO = itemPrefab.GetComponent<Item>().ItemSO;
            _totalValue -= itemSO.BaseValue;
        }

        if(_isTradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
        if(_isCoinBox)
        {
            Item item = itemPrefab.GetComponent<Item>();
            OnCoinRemoved(_playerProperty, item.CurrencyType);
        }
    }
}
