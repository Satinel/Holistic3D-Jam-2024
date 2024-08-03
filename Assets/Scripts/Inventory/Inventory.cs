using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Action<bool, ItemScriptableObject, int> OnItemAmountChanged;
    public static Action<bool, Dictionary<Currency, int>> OnMoneyAmountChanged;

    public Dictionary<ItemScriptableObject, int> ItemDictionary { get; private set; } = new Dictionary<ItemScriptableObject, int>();
    public Dictionary<Currency, int> Wallet { get; private set; } = new Dictionary<Currency, int>();

    [SerializeField] bool _isPlayer = false;
    [SerializeField] List<int> _startingCoins = new List<int>();

    void OnEnable()
    {
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
    }

    void OnDisable()
    {
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
    }

    void Start()
    {
        int currencies = Enum.GetNames(typeof(Currency)).Length;
        
        for(int i = 0; i < currencies; i++)
        {
            if(_startingCoins.Count < i + 1) { break; }

            Wallet.Add((Currency)i, _startingCoins[i]);
        }
        OnMoneyAmountChanged?.Invoke(_isPlayer, Wallet);
    }

    void TradingSystem_OnTradeCompleted(Dictionary<ItemScriptableObject, int> items, Dictionary<Currency, int> money)
    {
        if(money.Count > 0)
        {
            ChangeMoneyTotal(money);
        }

        if(items.Count <= 0) { return; } // If there is some sort of deal which only involves money??

        foreach(var item in items) // Even though items and money could be coded the same, I like differentiating them like this
        {
            ItemScriptableObject itemKey = item.Key;
            int itemAmount = item.Value;
            if(itemAmount > 0)
            {
                if(_isPlayer)
                {
                    AddToItems(itemKey, itemAmount);
                }
                else
                {
                    RemoveFromItems(itemKey, -itemAmount);
                }
            }
            else if(itemAmount < 0)
            {
                if(_isPlayer)
                {
                    RemoveFromItems(itemKey, itemAmount);
                }
                else
                {
                    AddToItems(itemKey, -itemAmount);
                }
            }
        }
    }

    void AddToItems(ItemScriptableObject item, int amount)
    {
        if(ItemDictionary.ContainsKey(item))
        {
            ItemDictionary[item] += amount;
        }
        else
        {
            ItemDictionary.Add(item, amount);
        }
        OnItemAmountChanged?.Invoke(_isPlayer, item, ItemDictionary[item]);
    }

    void RemoveFromItems(ItemScriptableObject item, int amount)
    {
        if(!ItemDictionary.ContainsKey(item)) { return; } // Ideally it should be impossible for this to happen

        ItemDictionary[item] += amount; // Note: This is a negative number so += is the correct way to handle it
        
        if(ItemDictionary[item] >= 0)
        {
            ItemDictionary.Remove(item);
        }
        OnItemAmountChanged?.Invoke(_isPlayer, item, ItemDictionary[item]);
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
                if(!Wallet.ContainsKey(key))
                {
                    Wallet.Add(key, 0);
                }
                Wallet[key] += amount;
            }
            else if(amount < 0)
            {
                if(!Wallet.ContainsKey(key)) { return; } // Ideally it should ALSO be impossible for THIS to happen

                Wallet[key] += amount; // += with a negative amount
                if(Wallet[key] < 0)
                {
                    Wallet[key] = 0;
                }
            }
        }
        
        OnMoneyAmountChanged?.Invoke(_isPlayer, Wallet);
    }
}
