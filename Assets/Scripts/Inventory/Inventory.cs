using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Action<bool> OnInventoryLoaded;
    public static Action<Inventory, bool> OnInventoryStart;
    public static Action OnPlayerMoneyLoaded;
    public static Action OnPlayerStockLoaded;

    [SerializeField] bool _isPlayer = false;
    [SerializeField] List<int> _startingCoins = new();
    [SerializeField] List<ItemScriptableObject> _coins = new();
    [SerializeField] List<ItemScriptableObject> _startingItems = new();
    [SerializeField] Item _itemPrefab, _coinPrefab;

    DropBox _dropBox, _coinBox;

    public DropBox CoinBox => _coinBox;
    public DropBox StockBox => _dropBox;

    public void Start()
    {
        OnInventoryStart?.Invoke(this, _isPlayer);
    }

    public void SetCoinBox(DropBox coinBox)
    {
        _coinBox = coinBox;

        if(_isPlayer)
        {
            LoadMoney(Customer.Type.None);
            OnPlayerMoneyLoaded?.Invoke();
        }
    }

    public void SetDropBox(DropBox dropBox)
    {
        _dropBox = dropBox;

        if(_isPlayer)
        {
            LoadStock(Customer.Type.None);
            OnPlayerStockLoaded?.Invoke();
        }
    }

    public void ShowInventory(Customer.Type customerType)
    {
        LoadMoney(customerType);

        LoadStock(customerType);

        OnInventoryLoaded?.Invoke(_isPlayer);
    }

    public void LoadMoney(Customer.Type customerType)
    {
        int currencies = Enum.GetNames(typeof(Currency)).Length;

        for(int i = 0; i < currencies; i++)
        {
            if (_startingCoins.Count < i + 1) { break; }

            for(int j = 0; j < _startingCoins[i]; j++)
            {
                Item newCoin = Instantiate(_coinPrefab, _coinBox.transform.position, Quaternion.identity, _coinBox.transform);
                newCoin.SetUpMoney(_coins[i], _isPlayer, _coinBox, (Currency)i, customerType);
            }            
        }
    }

    public void LoadStock(Customer.Type customerType)
    {
        foreach(ItemScriptableObject startingItem in _startingItems)
        {
            Item newItem = Instantiate(_itemPrefab, _dropBox.transform.position, Quaternion.identity, _dropBox.transform);
            newItem.SetUp(startingItem, _isPlayer, _dropBox, customerType);
        }
    }

    public void GenerateCoins(int amount, Customer.Type customerType, Currency currency)
    {
        for(int i = 0; i < amount; i++)
        {
            Item newCoin = Instantiate(_coinPrefab, _coinBox.transform.position, Quaternion.identity, _coinBox.transform);
            newCoin.SetUpMoney(_coins[(int)currency], _isPlayer, _coinBox, currency, customerType);
        }

        _coinBox.CurrencyExchange(amount, currency);
    }

    public void CoinsForDebt(int amount, Customer.Type customerType, Currency currency)
    {
        for(int i = 0; i < amount; i++)
        {
            Item newCoin = Instantiate(_coinPrefab, _coinBox.transform.position, Quaternion.identity, _coinBox.transform);
            newCoin.SetUpMoney(_coins[(int)currency], _isPlayer, _coinBox, currency, customerType);
        }
    }

    public void AddItems(List<GameObject> items)
    {
        foreach(GameObject itemPrefab in items)
        {
            ItemScriptableObject itemSO = itemPrefab.GetComponent<Item>().ItemSO;

            if(itemSO.ItemType != ItemType.Coin)
            {
                _startingItems.Add(itemSO);
            }
        }
    }

    public void Remove(List<GameObject> items)
    {
        foreach(GameObject itemPrefab in items)
        {
            ItemScriptableObject itemSO = itemPrefab.GetComponent<Item>().ItemSO;

            if(itemSO.ItemType != ItemType.Coin)
            {
                if(_startingItems.Contains(itemSO))
                {
                    _startingItems.Remove(itemSO);
                }
            }
        }
    }

    public void ClearInventory()
    {
        if(!_isPlayer)
        {
            _dropBox.TutorialClear();
            _coinBox.TutorialClear();
        }
    }

    public void CreateDebt()
    {
        _coinBox.TransferAllMoney();
    }

    // public int GetTotalMoney()
    // {
    //     int total = 0;

    //     foreach(var currency in Wallet)
    //     {
    //         switch(currency.Key)
    //         {
    //             case Currency.Copper:
    //                 total += currency.Value * TradingSystem.CopperValue;
    //                 break;
    //             case Currency.Silver:
    //                 total += currency.Value * TradingSystem.SilverValue;
    //                 break;
    //             case Currency.Gold:
    //                 total += currency.Value * TradingSystem.GoldValue;
    //                 break;
    //             case Currency.Platinum:
    //                 total += currency.Value * TradingSystem.PlatinumValue;
    //                 break;
    //         }
    //     }

    //     return total;
    // }

    // void TradingSystem_OnTradeCompleted(Dictionary<ItemScriptableObject, int> items, Dictionary<Currency, int> money)
    // {
    //     if(money.Count > 0)
    //     {
    //         ChangeMoneyTotal(money);
    //     }

    //     if(items.Count <= 0) { return; } // If there is some sort of deal which only involves money??

    //     foreach(var item in items) // Even though items and money could be coded the same, I like differentiating them like this
    //     {
    //         ItemScriptableObject itemKey = item.Key;
    //         int itemAmount = item.Value;
    //         if(itemAmount > 0)
    //         {
    //             if(_isPlayer)
    //             {
    //                 AddToItems(itemKey, itemAmount);
    //             }
    //             else
    //             {
    //                 RemoveFromItems(itemKey, -itemAmount);
    //             }
    //         }
    //         else if(itemAmount < 0)
    //         {
    //             if(_isPlayer)
    //             {
    //                 RemoveFromItems(itemKey, itemAmount);
    //             }
    //             else
    //             {
    //                 AddToItems(itemKey, -itemAmount);
    //             }
    //         }
    //     }
    // }

    // void AddToItems(ItemScriptableObject item, int amount)
    // {
    //     if(Items.ContainsKey(item))
    //     {
    //         Items[item] += amount;
    //     }
    //     else
    //     {
    //         Items.Add(item, amount);
    //     }
    //     OnItemAmountChanged?.Invoke(_isPlayer, item, Items[item]);
    // }

    // void RemoveFromItems(ItemScriptableObject item, int amount)
    // {
    //     if(!Items.ContainsKey(item)) { return; } // Ideally it should be impossible for this to happen

    //     Items[item] += amount; // Note: This is a negative number so += is the correct way to handle it

    //     if(Items[item] >= 0)
    //     {
    //         Items.Remove(item);
    //     }
    //     OnItemAmountChanged?.Invoke(_isPlayer, item, Items[item]);
    // }

    // void ChangeMoneyTotal(Dictionary<Currency, int> money)
    // {
    //     foreach(var coinType in money)
    //     {
    //         Currency key = coinType.Key;
    //         int amount = coinType.Value;
    //         if(!_isPlayer)
    //         {
    //             amount *= -1; // Whatever the value is, we want the opposite sign for the non-player
    //         }
    //         if(coinType.Value > 0)
    //         {
    //             if(!Wallet.ContainsKey(key))
    //             {
    //                 Wallet.Add(key, 0);
    //             }
    //             Wallet[key] += amount;
    //         }
    //         else if(amount < 0)
    //         {
    //             if(!Wallet.ContainsKey(key)) { return; } // Ideally it should ALSO be impossible for THIS to happen

    //             Wallet[key] += amount; // += with a negative amount
    //             if(Wallet[key] < 0)
    //             {
    //                 Wallet[key] = 0;
    //             }
    //         }
    //     }
    //     OnMoneyAmountChanged?.Invoke(_isPlayer, Wallet);
    // }
}
