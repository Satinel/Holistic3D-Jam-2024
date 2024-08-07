using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;
    public static Action<bool, Currency> OnCoinAdded;
    public static Action<bool, Currency> OnCoinRemoved;
    public static Action<bool, int> OnTradeResults;
    public static Action OnNoItems;
    public static Action<Item> OnItemPicked;
    public static Action<int> OnBuyPriceSet;

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
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer += TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer += TradingSystem_OnSellCustomer;
    }

    void OnDisable()
    {
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer -= TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer -= TradingSystem_OnSellCustomer;
    }

    void TradingSystem_OnOfferAccepted(bool isBuying, int value)
    {
        if(!_isTradeBox) { return; }
        
        if(isBuying)
        {
            if(_playerProperty)
            {
                _totalValue += value;
                OnBuyPriceSet?.Invoke(_totalValue);
            }
        }
        else
        {
            if(!_playerProperty)
            {
                _totalValue -= value;
            }
        }
        OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
    }

    void TradingSystem_OnTradeCompleted()
    {
        if(_isTradeBox)
        {
            int trueValue = GetTrueValue();

            OnTradeResults?.Invoke(_playerProperty, trueValue);

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
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(!_playerProperty)
        {
            foreach(GameObject customerItem in _items)
            {
                Destroy(customerItem);
            }
            _items.Clear();
            _totalValue = 0;
        }

        if(_isTradeBox && _playerProperty)
        {
            foreach(var playerItem in _items)
            {
                Item item = playerItem.GetComponent<Item>();
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
        if(_items.Count < 1)
        {
            OnNoItems?.Invoke();
            return;
        }

        Item randomItem = _items[UnityEngine.Random.Range(0, _items.Count)].GetComponent<Item>();

        OnItemPicked?.Invoke(randomItem);
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
    
    public int GetTrueValue()
    {
        int tValue = 0;

        foreach(GameObject item in _items)
        {
            tValue += item.GetComponent<Item>().ItemSO.BaseValue;
        }

        return tValue;
    }

    public void Pay(int price)
    {
        int cost = price;

        while(cost > 0 && _platinumParent.childCount > 0)
        {
            Item platCoin = _platinumParent.GetChild(0).GetComponent<Item>();
            RemoveItem(platCoin.gameObject);
            platCoin.SendToDropBox(_tradeBox);

            cost -= TradingSystem.PlatinumValue;
        }

        while(cost > 0 && _goldParent.childCount > 0)
        {
            Item goldCoin = _goldParent.GetChild(0).GetComponent<Item>();
            RemoveItem(goldCoin.gameObject);
            goldCoin.SendToDropBox(_tradeBox);

            cost -= TradingSystem.GoldValue;
        }

        while(cost > 0 && _silverParent.childCount > 0)
        {
            Item silverCoin = _silverParent.GetChild(0).GetComponent<Item>();
            RemoveItem(silverCoin.gameObject);
            silverCoin.SendToDropBox(_tradeBox);

            cost -= TradingSystem.SilverValue;
        }

        while(cost > 0 && _copperParent.childCount > 0)
        {
            Item copperCoin = _copperParent.GetChild(0).GetComponent<Item>();
            RemoveItem(copperCoin.gameObject);
            copperCoin.SendToDropBox(_tradeBox);

            cost -= TradingSystem.CopperValue;
        }
    }
}
