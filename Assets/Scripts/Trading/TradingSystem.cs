using UnityEngine;
using System;
using System.Collections.Generic;

public class TradingSystem : MonoBehaviour
{
    public static Action<Dictionary<ItemScriptableObject, int>, Dictionary<Currency, int>> OnTradeCompleted; 

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Dictionary<ItemScriptableObject, int> itemT = new();

            Dictionary<Currency, int> moneyT = new()
            {
                { Currency.Copper, 5 },
                { Currency.Silver, 1 },
                { Currency.Platinum, 8 }
            };

            OnTradeCompleted?.Invoke(itemT, moneyT);
        }
    }
}
