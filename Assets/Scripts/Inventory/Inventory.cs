using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // public static Action<bool, ItemScriptableObject, int> OnItemAmountChanged;
    // public static Action<bool, Dictionary<Currency, int>> OnMoneyAmountChanged;
    public static Action<bool> OnInventoryLoaded;

    // public Dictionary<ItemScriptableObject, int> Items { get; private set; } = new();
    public Dictionary<Currency, int> Wallet { get; private set; } = new();

    // public int TotalFunds => GetTotalMoney();

    [SerializeField] bool _isPlayer = false;
    [SerializeField] List<int> _startingCoins = new();
    [SerializeField] List<ItemScriptableObject> _coins = new();
    [SerializeField] List<ItemScriptableObject> _startingItems = new();

    [SerializeField] DropBox _dropBox, _coinBox;
    [SerializeField] Item _itemPrefab;

    public DropBox CoinBox => _coinBox;

    // void OnEnable()
    // {
    //     TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
    // }

    // void OnDisable()
    // {
    //     TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
    // }

    public void Start()
    {
        if(_isPlayer)
        {
            ShowInventory(Customer.Type.None);
        }
    }

    public void ShowInventory(Customer.Type customerType)
    {
        int currencies = Enum.GetNames(typeof(Currency)).Length;
        
        for(int i = 0; i < currencies; i++)
        {
            if(_startingCoins.Count < i + 1) { break; }

            Wallet.Add((Currency)i, _startingCoins[i]);

            for (int j = 0; j < _startingCoins[i]; j++)
            {
                Item newCoin = Instantiate(_itemPrefab, _coinBox.transform.position, Quaternion.identity, _coinBox.transform);
                newCoin.SetUpMoney(_coins[i], _isPlayer, _coinBox, (Currency)i, customerType);
            }
        }
        // OnMoneyAmountChanged?.Invoke(_isPlayer, Wallet);

        foreach(ItemScriptableObject startingItem in _startingItems)
        {
            Item newItem = Instantiate(_itemPrefab, _dropBox.transform.position, Quaternion.identity, _dropBox.transform);
            newItem.SetUp(startingItem, _isPlayer, _dropBox, customerType);
            // AddToItems(startingItem, 1);
        }

        OnInventoryLoaded?.Invoke(_isPlayer);
    }

    public void GenerateCopper(int amount, Customer.Type customerType)
    {
        for(int i = 0; i < amount; i++)
        {
            Item newCoin = Instantiate(_itemPrefab, _coinBox.transform.position, Quaternion.identity, _coinBox.transform);
            newCoin.SetUpMoney(_coins[0], _isPlayer, _coinBox, Currency.Copper, customerType);
        }

        _coinBox.PayInCopper(amount);
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
