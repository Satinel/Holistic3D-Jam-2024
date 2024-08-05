using UnityEngine;
using System;

public class TradingSystem : MonoBehaviour
{
    public static Action OnTradeCompleted;
    public static Action OnTradeCancelled;
    public static Action<Customer> OnNewCustomer;
    public static Action OnOpenToPublic;
    
    int _playerValue, _compValue;


    [SerializeField] GameObject _openButton;
    Customer _currentCustomer;
    bool _isOpen;
    
    public const int CopperValue = 1, SilverValue = 10, GoldValue = 100, PlatinumValue = 1000;

    public Customer.Type TradeType => _currentCustomer.CustomerType; // This MIGHT be useful?

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.T))
    //     {
    //         NewCustomer(_testCustomer);
    //     }
    //     if(Input.GetKeyDown(KeyCode.X))
    //     {
    //         NoCustomer();
    //     }
    // }

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
        Town.OnNextCustomer += Town_OnNextCustomer;
        Town.OnNoCustomers += Town_OnNoCustomers;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        Town.OnNextCustomer -= Town_OnNextCustomer;
        Town.OnNoCustomers -= Town_OnNoCustomers;
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

    void Town_OnNextCustomer(Customer customer)
    {
        NewCustomer(customer);
    }

    void Town_OnNoCustomers()
    {
        NoCustomer();
        // TODO Handle no more customers
    }

    void NewCustomer(Customer customer)
    {
        _currentCustomer = customer;
        OnNewCustomer?.Invoke(customer);
        Debug.Log(_currentCustomer.name); // TODO DELETE

        switch(_currentCustomer.CustomerType)
        {
            case Customer.Type.Buy:
                // TODO Pick a random item customer wants to buy from player inventory and put into player tradebox
                break;
            case Customer.Type.Sell:
                // TODO Pick a random item customer wants to sell from customer inventory and put into customer tradebox
                break;
            case Customer.Type.Barter:
                // TODO Nothing?
                break;
            case Customer.Type.None:
                // TODO Nothing!(?)
                break;
            default:
                break;
        }
    }

    void NoCustomer()
    {
        _currentCustomer = null;
        OnNewCustomer?.Invoke(null);
    }

    public bool MakeOffer()
    {
        int offer = _compValue - _playerValue;

        if(offer < 0) { return true; }

        return offer <= _currentCustomer.Tolerance;
    }

    public void AttemptTrade() // Used for UI Button
    {
        if(!_currentCustomer) { return; }

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

        if(_isOpen)
        {
            OnOpenToPublic?.Invoke();
        }
    }

    void ProcessRejection()
    {
        _currentCustomer.ReduceStrikes(1); // TODO? Some formula to change this amount based on variables (or not do it at all)
        // TODO Failure Message/Trade Rejected (possibly alter customer Tolerance) (possibly alter player Reputation)
    }

    public void CancelTrade() // Used for UI Button
    {
        OnTradeCancelled?.Invoke();
    }

    public void OpenToPublic() // Used for UI Button
    {
        _isOpen = true;
        _openButton.SetActive(false);
        OnOpenToPublic?.Invoke();
    }
}
