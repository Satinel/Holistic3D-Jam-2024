using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;
    public static Action OnTradeResults;
    public static Action OnNoItems;
    public static Action<Item, bool> OnItemPicked;
    public static Action<int> OnBuyPriceSet;
    public static Action<int> OnSellPriceSet;
    public static Action OnTradeBoxProcessed;

    [SerializeField] Transform _copperParent, _silverParent, _goldParent, _platinumParent;
    [SerializeField] TextMeshProUGUI _copperText, _silverText, _goldText, _platinumText;

    [SerializeField] bool _playerProperty, _isTradeBox, _isCoinBox;
    [SerializeField] DropBox _tradeBox, _coinBox, _inventoryBox, _partnerBox, _partnerCoinBox;

    public bool IsTradeBox => _isTradeBox;

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
        Inventory.OnInventoryStart += Inventory_OnInventoryStart;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnBarterAccepted += TradingSystem_OnBarterAccepted;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer += TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer += TradingSystem_OnSellCustomer;
        TradingSystem.OnResetTrade += TradingSystem_OnResetTrade;
        TradingSystem.OnResetBarter += TradingSystem_OnResetBarter;
        TradingSystem.OnStrikeOut += TradingSystem_OnStrikeOut;
    }

    void OnDisable()
    {
        Inventory.OnInventoryStart -= Inventory_OnInventoryStart;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnBarterAccepted -= TradingSystem_OnBarterAccepted;
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnBuyCustomer -= TradingSystem_OnBuyCustomer;
        TradingSystem.OnSellCustomer -= TradingSystem_OnSellCustomer;
        TradingSystem.OnResetTrade -= TradingSystem_OnResetTrade;
        TradingSystem.OnResetBarter -= TradingSystem_OnResetBarter;
        TradingSystem.OnStrikeOut -= TradingSystem_OnStrikeOut;
    }

    void Inventory_OnInventoryStart(Inventory inventory, bool isPlayer)
    {
        if(isPlayer)
        {
            if(_playerProperty)
            {
                if(_isCoinBox)
                {
                    inventory.SetCoinBox(this);
                    return;
                }
                if(!_isTradeBox)
                {
                    inventory.SetDropBox(this);
                    return;
                }
            }
        }
        else
        {
            if(!_playerProperty)
            {
                if(_isCoinBox)
                {
                    inventory.SetCoinBox(this);
                    return;
                }
                if(!_isTradeBox)
                {
                    inventory.SetDropBox(this);
                    return;
                }
            }
        }
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
                OnSellPriceSet?.Invoke(_totalValue);
            }
        }
        OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
    }

    void TradingSystem_OnBarterAccepted(int offer)
    {
        if(!_isTradeBox) { return; }
        if(_playerProperty) { return; }

        if(offer < 0)
        {
            OnBuyPriceSet?.Invoke(-offer);
        }
    }

    void TradingSystem_OnTradeCompleted(Customer currentCustomer)
    {
        if(_isTradeBox)
        {
            if(currentCustomer.CustomerType != Customer.Type.Bank)
            {
                OnTradeResults?.Invoke();
            }

            if(_playerProperty)
            {
                currentCustomer.AddToInventory(_items); // Items get flipped to belong to currentCustomer in the foreach below
                // TODO(?) Remove from player's Inventory?
            }
            else
            {
                currentCustomer.RemoveFromInventory(_items); // Items get flipped to belong to player in the foreach blow
                // TODO(?) Add to player's Inventory?
            }

            foreach(var tradedItem in _items)
            {
                Item item = tradedItem.GetComponent<Item>();
                item.SetInTrade(false);
                item.SetPlayerProperty(!_playerProperty);
                if(item.ItemSO.ItemType == ItemType.Coin)
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
        Invoke(nameof(SetCoinTexts), 0.01f);

        if(_isTradeBox && _playerProperty)
        {
            OnTradeBoxProcessed?.Invoke();
        }
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
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
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
                if(item.ItemSO.ItemType == ItemType.Coin)
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
        Invoke(nameof(SetCoinTexts), 0.01f);
    }

    public void TutorialClear()
    {
        foreach(GameObject item in _items)
        {
            Destroy(item);
        }
        _items.Clear();
        _totalValue = 0;
    }

    void TradingSystem_OnBuyCustomer(bool isTutorial)
    {
        if(_isCoinBox || _isTradeBox) { return; }

        if(!_playerProperty) { return; }

        if(isTutorial)
        {
            Item item = _items[0].GetComponent<Item>();

            OnItemPicked?.Invoke(item, _playerProperty);
            _items.Remove(item.gameObject);
            item.SendToDropBox(_tradeBox);
            return;
        }
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
                        if(item.ItemSO.ItemType == ItemType.Coin)
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
        Invoke(nameof(SetCoinTexts), 0.01f);
    }

    void TradingSystem_OnResetBarter()
    {
        ResetTradeBox();
    }

    void ResetTradeBox()
    {
        if(_isTradeBox)
        {
            List<GameObject> tempList = new();

            foreach(var itemGO in _items)
            {
                Item item = itemGO.GetComponent<Item>();
                item.SetInTrade(false);
                if (item.ItemSO.ItemType == ItemType.Coin)
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
        Invoke(nameof(SetCoinTexts), 0.01f);
    }

    void TradingSystem_OnStrikeOut(Customer customer)
    {
        ResetTradeBox();
    }

    void PickRandomItem()
    {
        if(_items.Count < 1)
        {
            OnNoItems?.Invoke();
            return;
        }

        Item randomItem = _items[UnityEngine.Random.Range(0, _items.Count)].GetComponent<Item>();

        OnItemPicked?.Invoke(randomItem, _playerProperty);
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
        
        Invoke(nameof(SetCoinTexts), 0.01f);
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
        
        Invoke(nameof(SetCoinTexts), 0.01f);
    }

    void SetCoinTexts()
    {
        if(!_copperText) { return; }
        if(!_isCoinBox && !_isTradeBox) { return; }

        if(_copperParent.childCount <= 0)
        {
            _copperText.text = string.Empty;
        }
        else
        {
            _copperText.text = _copperParent.childCount.ToString();
        }
        if(_silverParent.childCount <= 0)
        {
            _silverText.text = string.Empty;
        }
        else
        {
            _silverText.text = _silverParent.childCount.ToString();
        }
        if(_goldParent.childCount <= 0)
        {
            _goldText.text = string.Empty;
        }
        else
        {
            _goldText.text = _goldParent.childCount.ToString();
        }
        if(_platinumParent.childCount <= 0)
        {
            _platinumText.text = string.Empty;
        }
        else
        {
            _platinumText.text = _platinumParent.childCount.ToString();
        }
    }

    public void SendChildItem(Transform parent) // UI Button (Note this only works for currency but could be applied to stackable items with Item.SortCoins and UI reworking)
    {
        if(parent.childCount > 0)
        {
            Item item = parent.GetChild(0).GetComponent<Item>();
            RemoveItem(item.gameObject);
            item.SendToDropBox(_tradeBox);
            item.SetInTrade(true);
            AddToSentItems(item);
            item.SortCoins(_tradeBox);
        }
    }

    public void SendXChildItems(Transform parent, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if(parent.childCount > 0)
            {
                Item item = parent.GetChild(0).GetComponent<Item>();
                RemoveItem(item.gameObject);
                item.SendToDropBox(_tradeBox);
                item.SetInTrade(true);
                AddToSentItems(item);
                item.SortCoins(_tradeBox);
            }
            else
            {
                break;
            }
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

    public void RetrieveCoinStack(Transform parent)
    {
        if(_isTradeBox)
        {
            if(_playerProperty)
            {
                foreach(var coin in parent.GetComponentsInChildren<Item>())
                {
                    if(coin.ItemSO.ItemType == ItemType.Coin)
                    {
                        coin.SendToCoinBox(_coinBox);
                        _coinBox.RemoveFromSentItems(coin);
                        RemoveItem(coin.gameObject);
                    }
                }
                SetCoinTexts();
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

        while(cost >= TradingSystem.PlatinumValue && _platinumParent.childCount > 0)
        {
            Item platCoin = _platinumParent.GetChild(0).GetComponent<Item>();
            RemoveItem(platCoin.gameObject);
            platCoin.SendToDropBox(_tradeBox);
            platCoin.SortCoins(_tradeBox);

            cost -= TradingSystem.PlatinumValue;
        }

        if(_silverParent.childCount * TradingSystem.SilverValue >= cost)
        {
            while(cost > 0 && _silverParent.childCount > 0)
            {
                Item silverCoin = _silverParent.GetChild(0).GetComponent<Item>();
                RemoveItem(silverCoin.gameObject);
                silverCoin.SendToDropBox(_tradeBox);
                silverCoin.SortCoins(_tradeBox);

                cost -= TradingSystem.SilverValue;
            }
        }

        if(_goldParent.childCount * TradingSystem.GoldValue >= cost)
        {
            while(cost > 0 && _goldParent.childCount > 0)
            {
                Item goldCoin = _goldParent.GetChild(0).GetComponent<Item>();
                RemoveItem(goldCoin.gameObject);
                goldCoin.SendToDropBox(_tradeBox);
                goldCoin.SortCoins(_tradeBox);

                cost -= TradingSystem.GoldValue;
            }
        }

        if(_copperParent.childCount * TradingSystem.CopperValue >= cost)
        {
            while(cost > 0 && _copperParent.childCount > 0)
            {
                Item copperCoin = _copperParent.GetChild(0).GetComponent<Item>();
                RemoveItem(copperCoin.gameObject);
                copperCoin.SendToDropBox(_tradeBox);
                copperCoin.SortCoins(_tradeBox);

                cost -= TradingSystem.CopperValue;
            }
        }

        while(cost > 0 && _platinumParent.childCount > 0)
        {
            Item platCoin = _platinumParent.GetChild(0).GetComponent<Item>();
            RemoveItem(platCoin.gameObject);
            platCoin.SendToDropBox(_tradeBox);
            platCoin.SortCoins(_tradeBox);

            cost -= TradingSystem.PlatinumValue;
        }

        while(cost > 0 && _goldParent.childCount > 0)
        {
            Item goldCoin = _goldParent.GetChild(0).GetComponent<Item>();
            RemoveItem(goldCoin.gameObject);
            goldCoin.SendToDropBox(_tradeBox);
            goldCoin.SortCoins(_tradeBox);

            cost -= TradingSystem.GoldValue;
        }

        while(cost > 0 && _silverParent.childCount > 0)
        {
            Item silverCoin = _silverParent.GetChild(0).GetComponent<Item>();
            RemoveItem(silverCoin.gameObject);
            silverCoin.SendToDropBox(_tradeBox);
            silverCoin.SortCoins(_tradeBox);

            cost -= TradingSystem.SilverValue;
        }

        while(cost > 0 && _copperParent.childCount > 0)
        {
            Item copperCoin = _copperParent.GetChild(0).GetComponent<Item>();
            RemoveItem(copperCoin.gameObject);
            copperCoin.SendToDropBox(_tradeBox);
            copperCoin.SortCoins(_tradeBox);

            cost -= TradingSystem.CopperValue;
        }
    }

    public void CurrencyExchange(int amount, Currency currency)
    {
        int cost;

        switch(currency)
        {
            case Currency.Copper:
                cost = amount * TradingSystem.CopperValue;
                while(cost > 0 && _copperParent.childCount > 0)
                {
                    Item copperCoin = _copperParent.GetChild(0).GetComponent<Item>();
                    RemoveItem(copperCoin.gameObject);
                    copperCoin.SendToDropBox(_tradeBox);
                    copperCoin.SortCoins(_tradeBox);

                    cost -= TradingSystem.CopperValue;
                }
                break;
            case Currency.Silver:
                cost = amount * TradingSystem.SilverValue;
                while(cost > 0 && _silverParent.childCount > 0)
                {
                    Item silverCoin = _silverParent.GetChild(0).GetComponent<Item>();
                    RemoveItem(silverCoin.gameObject);
                    silverCoin.SendToDropBox(_tradeBox);
                    silverCoin.SortCoins(_tradeBox);

                    cost -= TradingSystem.SilverValue;
                }
                break;
            case Currency.Gold:
                cost = amount * TradingSystem.GoldValue;
                while(cost > 0 && _goldParent.childCount > 0)
                {
                    Item goldCoin = _goldParent.GetChild(0).GetComponent<Item>();
                    RemoveItem(goldCoin.gameObject);
                    goldCoin.SendToDropBox(_tradeBox);
                    goldCoin.SortCoins(_tradeBox);

                    cost -= TradingSystem.GoldValue;
                }
                break;
            case Currency.Platinum:
                cost = amount * TradingSystem.PlatinumValue;
                while(cost > 0 && _platinumParent.childCount > 0)
                {
                    Item platinumCoin = _platinumParent.GetChild(0).GetComponent<Item>();
                    RemoveItem(platinumCoin.gameObject);
                    platinumCoin.SendToDropBox(_tradeBox);
                    platinumCoin.SortCoins(_tradeBox);

                    cost -= TradingSystem.PlatinumValue;
                }
                break;
        }

    }

    public void TransferAllMoney()
    {
        if(_isCoinBox && !_playerProperty)
        {
            foreach(var coin in _items)
            {
                Item item = coin.GetComponent<Item>();
                item.SetInTrade(false);
                item.SetPlayerProperty(!_playerProperty);
                if(item.ItemSO.ItemType == ItemType.Coin)
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
        }
    }
}
