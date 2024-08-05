using UnityEngine;
using System;

public class TradingSystem : MonoBehaviour
{
    public static Action OnTradeCompleted;
    public static Action OnTradeCancelled;
    public static Action<Customer.Type> OnNewCustomer;
    
    int _playerValue, _compValue;

    public static int CopperValue = 1, SilverValue = 10, GoldValue = 100, PlatinumValue = 1000;

    Customer _currentCustomer;
    [SerializeField] Customer _testCustomer;

    public Customer.Type TradeType => _currentCustomer.CustomerType; // This MIGHT be useful?

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            NewCustomer(_testCustomer);
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
        if(playerProperty)
        {
            _playerValue = value;
        }
        else
        {
            _compValue = value;
        }
    }

    void NewCustomer(Customer customer) // TODO Use this to start a transaction with a customer!
    {
        _currentCustomer = customer;
        OnNewCustomer?.Invoke(customer.CustomerType);
        Debug.Log(_currentCustomer.name); // TODO DELETE
    }

    void NoCustomer() // TODO Use this once all trading with a customer is complete!
    {
        _currentCustomer = null;
        OnNewCustomer?.Invoke(Customer.Type.None);
    }

    public bool MakeOffer()
    {
        int offer = _compValue - _playerValue;

        if(offer < 0) { return true; }

        return offer <= _currentCustomer.Tolerance;
    }

    public void AttemptTrade()
    {
        if(MakeOffer())
        {
            ProcessTrade();
        }
        else
        {
            ProcessRejection();
        }
    }

    void ProcessTrade()
    {
        // TODO UI/VFX/SFX (include profit/loss and if correct change)
        OnTradeCompleted?.Invoke();
    }

    void ProcessRejection()
    {
        _currentCustomer.ReduceStrikes(1); // TODO? Some formula to change this amount based on variables (or not do it at all)
        // TODO Failure Message/Trade Rejected (possibly alter customer Tolerance) (possibly alter player Reputation)
    }

    public void CancelTrade()
    {
        OnTradeCancelled?.Invoke();
    }
}
