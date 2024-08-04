using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Action<bool, ItemScriptableObject, int> OnItemAmountChanged;
    public static Action<bool, Dictionary<Currency, int>> OnMoneyAmountChanged;

    Dictionary<ItemScriptableObject, int> _itemDictionary = new();
    Dictionary<Currency, int> _wallet = new();

    [SerializeField] bool _isPlayer = false;
    [SerializeField] List<int> _startingCoins = new();
    [SerializeField] List<ItemScriptableObject> _startingItems = new();

    [SerializeField] DropBox _dropBox;
    [SerializeField] Item _itemPrefab;

    // void OnEnable()
    // {
    //     TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
    // }

    // void OnDisable()
    // {
    //     TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
    // }

    void Start()
    {
        int currencies = Enum.GetNames(typeof(Currency)).Length;
        
        for(int i = 0; i < currencies; i++)
        {
            if(_startingCoins.Count < i + 1) { break; }

            _wallet.Add((Currency)i, _startingCoins[i]);
        }
        OnMoneyAmountChanged?.Invoke(_isPlayer, _wallet);

        foreach(ItemScriptableObject startingItem in _startingItems)
        {
            Item newItem = Instantiate(_itemPrefab, _dropBox.transform.position, Quaternion.identity, _dropBox.transform);
            newItem.SetUp(startingItem, _isPlayer, _dropBox);
            AddToItems(startingItem, 1);
        }
    }

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

    void AddToItems(ItemScriptableObject item, int amount)
    {
        if(_itemDictionary.ContainsKey(item))
        {
            _itemDictionary[item] += amount;
        }
        else
        {
            _itemDictionary.Add(item, amount);
        }
        OnItemAmountChanged?.Invoke(_isPlayer, item, _itemDictionary[item]);
    }

    void RemoveFromItems(ItemScriptableObject item, int amount)
    {
        if(!_itemDictionary.ContainsKey(item)) { return; } // Ideally it should be impossible for this to happen

        _itemDictionary[item] += amount; // Note: This is a negative number so += is the correct way to handle it
        
        if(_itemDictionary[item] >= 0)
        {
            _itemDictionary.Remove(item);
        }
        OnItemAmountChanged?.Invoke(_isPlayer, item, _itemDictionary[item]);
    }

    void ChangeMoneyTotal(Dictionary<Currency, int> money)
    {
        foreach(var coinType in money)
        {
            Currency key = coinType.Key;
            int amount = coinType.Value;
            if(!_isPlayer)
            {
                amount *= -1; // Whatever the value is, we want the opposite sign for the non-player
            }
            if(coinType.Value > 0)
            {
                if(!_wallet.ContainsKey(key))
                {
                    _wallet.Add(key, 0);
                }
                _wallet[key] += amount;
            }
            else if(amount < 0)
            {
                if(!_wallet.ContainsKey(key)) { return; } // Ideally it should ALSO be impossible for THIS to happen

                _wallet[key] += amount; // += with a negative amount
                if(_wallet[key] < 0)
                {
                    _wallet[key] = 0;
                }
            }
        }
        OnMoneyAmountChanged?.Invoke(_isPlayer, _wallet);
    }
}
