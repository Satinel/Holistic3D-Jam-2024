using UnityEngine;
using System;

public class TradingSystem : MonoBehaviour
{
    public static Action OnOfferAccepted;
    public static Action OnOfferRejected;
    public static Action OnTradeCancelled;
    public static Action<Customer> OnNewCustomer;
    public static Action OnOpenToPublic;
    public static Action OnBuyCustomer;
    public static Action OnSellCustomer;
    
    int _playerValue, _compValue;

    [SerializeField] GameObject _openButton;
    Customer _currentCustomer;
    
    public const int CopperValue = 1, SilverValue = 10, GoldValue = 100, PlatinumValue = 1000;

    public Customer.Type TradeType => _currentCustomer.CustomerType; // This MIGHT be useful?

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.T))
    //     {

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
        Inventory.OnInventoryLoaded += Inventory_OnInventoryLoaded;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        Town.OnNextCustomer -= Town_OnNextCustomer;
        Town.OnNoCustomers -= Town_OnNoCustomers;
        Inventory.OnInventoryLoaded -= Inventory_OnInventoryLoaded;
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
    }

    void Inventory_OnInventoryLoaded(bool isPlayer)
    {
        if(isPlayer) { return; }

        HandleCustomerType();
    }

    void HandleCustomerType()
    {
        switch(_currentCustomer.CustomerType)
        {
            case Customer.Type.Buy:
                BuyingCustomer();
                break;
            case Customer.Type.Sell:
                SellingCustomer();
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

    void BuyingCustomer()
    {
        OnBuyCustomer?.Invoke();
        // TODO Prompt for player to set a price
    }

    void SellingCustomer()
    {
        OnSellCustomer?.Invoke();
        // TODO Prompt for player to set a price
    }

    void NoCustomer()
    {
        _currentCustomer = null;
        OnNewCustomer?.Invoke(null);
    }

    bool MakeOffer()
    {
        if(_playerValue <= 0) { return false; } // TODO(?) Invoke a snide message from customer that player should offer something
        if(_compValue <= 0) { return true; } // TODO(?) Invoke message thanking player for free gift

        int offer = _compValue - _playerValue;

        if(offer <= 0) { return true; } // TODO(?) Invoke a smug message from customer that they got a great deal

        float rake = 1 - ((float)_playerValue / _compValue);

        return rake <= _currentCustomer.Tolerance;
    }

    void ProcessTrade()
    {
        // TODO UI/VFX/SFX (include profit/loss and if correct change)
        OnOfferAccepted?.Invoke();
    }

    void ProcessRejection()
    {
        _currentCustomer.ReduceStrikes(1); // TODO? Some formula to change this amount based on variables
        OnOfferRejected?.Invoke();
        // TODO? alter customer Tolerance and/or alter player Reputation

        if(_currentCustomer.Strikes <= 0)
        {
            // TODO alter player reputation and display angry customer message (plus SFX maybe)
            CancelTrade();
        }
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

    public void RepeatCustomer() // Used for UI Button
    {
        HandleCustomerType();
    }


    public void CancelTrade() // Used for UI Button
    {
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(true);
        NoCustomer();
    }

    public void OpenToPublic() // Used for UI Button
    {
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(false);
        OnOpenToPublic?.Invoke();
    }
}
