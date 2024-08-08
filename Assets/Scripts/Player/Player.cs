using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field:SerializeField] public int Reputation { get; private set; }

    void OnEnable()
    {
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnStrikeOut += TradingSystem_OnStrikeOut;
    }

    void OnDisable()
    {
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnStrikeOut -= TradingSystem_OnStrikeOut;
    }

    void TradingSystem_OnTradeCompleted(Customer customer)
    {
        if(customer.CustomerType == Customer.Type.Bank) { return; }
        if(customer.CustomerType == Customer.Type.Barter) { return; }

        Reputation += customer.Opinion - customer.Strikes;
    }

    void TradingSystem_OnStrikeOut(Customer customer)
    {
        Reputation += customer.Opinion - customer.MaxStrikes;
    }
}
