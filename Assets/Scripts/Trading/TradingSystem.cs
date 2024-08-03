using UnityEngine;
using System;
using System.Collections.Generic;

public class TradingSystem : MonoBehaviour
{
    public static Action<Dictionary<ItemScriptableObject, int>, Dictionary<Currency, int>> OnTradeCompleted;
    public static Action OnTradeCancelled;

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

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
    }

    void DropBox_OnTradeBoxValueChanged(bool playerProperty, int value)
    {
        // TODO Balance Scales?
    }
}
