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
    List<GameObject> _sentItems = new();
    int _totalValue;

    public List<GameObject> Items => _items;

    void OnEnable()
    {
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer += TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer += TradingSystem_OnSellCustomer;
        TradingSystem.OnResetTrade += TradingSystem_OnResetTrade;
        TradingSystem.OnResetBarter += TradingSystem_OnResetBarter;
    }

    void OnDisable()
    {
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer -= TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer -= TradingSystem_OnSellCustomer;
        TradingSystem.OnResetTrade -= TradingSystem_OnResetTrade;
        TradingSystem.OnResetBarter -= TradingSystem_OnResetBarter;
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

    void TradingSystem_OnTradeCompleted(Customer currentCustomer)
    {
        if(_isTradeBox)
        {
            int trueValue = GetTrueValue();

            OnTradeResults?.Invoke(_playerProperty, trueValue);

            if(_playerProperty)
            {
                currentCustomer.AddToInventory(_items); // Items get flipped to belong to currentCustomer in the foreach below
                // TODO(?) Remove from player's inventory?
            }
            else
            {
                currentCustomer.RemoveFromInventory(_items); // Items get flipped to belong to player in the foreach blow
                // TODO(?) Add to player's inventory?
            }
            

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
        _sentItems.Clear();
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(_isTradeBox && !_playerProperty)
        {
            foreach(var customerItem in _items)
            {
                Destroy(customerItem);
            }
            _items.Clear();
            _totalValue = 0;
        }

        if(!_isTradeBox && !_playerProperty)
        {
            foreach(var customerItem in _items)
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
        _sentItems.Clear();
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

    void TradingSystem_OnResetTrade()
    {
        if(_isTradeBox)
        {
            if(_playerProperty)
            {
                List<GameObject> tempList = new();

                foreach(var coin in _items)
                {
                    Item item = coin.GetComponent<Item>();
                    {
                        if(item.ItemSO.Type == Type.Coin)
                        {
                            item.SendToCoinBox(_coinBox);
                            _coinBox.RemoveFromSentItems(item);
                            tempList.Add(coin);
                        }
                    }
                }

                foreach(var removedCoin in tempList)
                {
                    RemoveItem(removedCoin);
                }
            }
        }
    }

    void TradingSystem_OnResetBarter()
    {
        if(_isTradeBox)
        {
            List<GameObject> tempList = new();

            foreach(var itemGO in _items)
            {
                Item item = itemGO.GetComponent<Item>();
                item.SetInTrade(false);
                if(item.ItemSO.Type == Type.Coin)
                {
                    item.SendToCoinBox(_coinBox);
                }
                else
                {
                    item.SendToDropBox(_inventoryBox);
                }
                tempList.Add(itemGO);
            }

            foreach(var removedItem in tempList)
            {
                RemoveItem(removedItem);
            }

            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
        _sentItems.Clear();
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
            if(droppedItem.PlayerProperty && droppedItem.IsMoney)
            {
                droppedItem.CurrentBox.AddToSentItems(droppedItem);
            }
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

    public void SendChildItem(Transform parent) // UI Button (Note this only works for currency but could be applied to stackable items with Item.cs and UI reworking)
    {
        if(parent.childCount > 0)
        {
            Item item = parent.GetChild(0).GetComponent<Item>();
            RemoveItem(item.gameObject);
            item.SendToDropBox(_tradeBox);
            item.SetInTrade(true);
            AddToSentItems(item);
        }
    }

    public void RetrieveItem(Currency currency) // UI Button (Note this only works for currency)
    {
        if(_sentItems.Count <= 0) { return; }

        foreach(GameObject itemGO in _sentItems)
        {
            Item item = itemGO.GetComponent<Item>();
            
            if(item.CurrencyType == currency)
            {
                _tradeBox.RemoveItem(itemGO);
                item.SendToCoinBox(this);
                item.SetInTrade(false);
                break;
            }
        }
    }

    public void AddToSentItems(Item item)
    {
        if(_sentItems.Contains(item.gameObject)) { return; }

        _sentItems.Add(item.gameObject);
    }

    public void RemoveFromSentItems(Item item)
    {
        if(_sentItems.Contains(item.gameObject))
        {
            _sentItems.Remove(item.gameObject);
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

    public void PayInCopper(int amount)
    {
        int cost = amount;

        while(cost > 0 && _copperParent.childCount > 0)
        {
            Item copperCoin = _copperParent.GetChild(0).GetComponent<Item>();
            RemoveItem(copperCoin.gameObject);
            copperCoin.SendToDropBox(_tradeBox);

            cost -= TradingSystem.CopperValue;
        }
    }
}
