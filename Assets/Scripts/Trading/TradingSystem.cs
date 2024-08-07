using UnityEngine;
using System;

public class TradingSystem : MonoBehaviour
{
    public static Action OnIncorrectChange;
    public static Action<bool, int> OnOfferAccepted;
    public static Action OnOfferRejected;
    public static Action OnTradeCompleted;
    public static Action OnTradeCancelled;
    public static Action<Customer> OnNewCustomer;
    public static Action OnOpenToPublic;
    public static Action OnBuyCustomer;
    public static Action OnSellCustomer;
    public static Action<int> OnOfferValueChanged;
    public static Action<Customer.Type, int> OnChangeGiven;
    public static Action OnResetTrade;
    public static Action OnResetBarter;
    
    int _playerValue, _compValue, _offerValue;

    int _offer;
    int _basePrice;

    [SerializeField] GameObject _openButton;
    Customer _currentCustomer;
    
    public const int CopperValue = 1, SilverValue = 10, GoldValue = 100, PlatinumValue = 1000;


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
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        Town.OnNextCustomer += Town_OnNextCustomer;
        Town.OnNoCustomers += Town_OnNoCustomers;
        Inventory.OnInventoryLoaded += Inventory_OnInventoryLoaded;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
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

    void DropBox_OnItemPicked(Item item)
    {
        _basePrice = item.ItemSO.BaseValue;
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
        float rake = 0;

        if(_currentCustomer.CustomerType == Customer.Type.Buy)
        {
            if(_offerValue > _currentCustomer.GetTotalFunds())
            {
                return false; // TODO(?) Invoke a sad message from customer that they can't afford this
            }

            _offer = _offerValue - _basePrice;

            if(_offer <= 0) { return true; } // TODO(?) Invoke a pleased message from customer that they got a good deal

            rake = 1 - ((float)_basePrice / _offerValue);
        }

        if(_currentCustomer.CustomerType == Customer.Type.Sell)
        {
            if(_offerValue <= 0) { return false; } // TODO(?) Invoke a snide message from customer that player should offer something

            _offer = _basePrice - _offerValue;

            if(_offer <= 0) { return true; } // TODO(?) Invoke a pleased message from customer that they got a good deal

            rake = 1 - ((float)_offerValue / _basePrice);
        }

        return rake <= _currentCustomer.Tolerance;
    }

    bool OfferChange(out int offer) // TODO Go over this thoroughly at some point
    {
        offer = _compValue - _playerValue;
        
        if(offer < 0) { return true; } // Player gave too much change (TODO: mention this in OnChangeGiven<int> action)
        if(offer == 0) { return true; } // Player gave exact change (TODO: mention this in OnChangeGiven<int> action and +player rep)
        if(_playerValue <= 0) { return false; } // Should not happen
        if(_compValue <= 0) { return true; } // Also should not happen but we don't want to divide by zero

        float rake = 1 - ((float)_playerValue / _compValue);

        return rake <= _currentCustomer.Tolerance;
    }

    void ProcessTrade()
    {
        // TODO UI/VFX/SFX (include profit/loss and if correct change)
        OnTradeCompleted?.Invoke(); // TODO(?) Increase player reputation
    }

    void ProcessRejection()
    {
        _currentCustomer.ReduceStrikes(1); // TODO? Some formula to change this amount based on variables
        // TODO? alter customer Tolerance and/or alter player Reputation

        if(_currentCustomer.Strikes <= 0)
        {
            // TODO alter player reputation and display angry customer message (plus SFX maybe)
            CancelTrade();
        }
    }

    public void Haggle()
    {
        if(!_currentCustomer) { return; }

        if(MakeOffer())
        {
            bool buying = _currentCustomer.CustomerType == Customer.Type.Buy;
            OnOfferAccepted?.Invoke(buying, _offer);
        }
        else
        {
            OnOfferRejected?.Invoke();
            ProcessRejection();
        }
    }

    public void AttemptTrade() // Used for UI Button
    {
        if(!_currentCustomer) { return; }

        if(OfferChange(out int offer))
        {
            OnChangeGiven?.Invoke(_currentCustomer.CustomerType, offer);
            ProcessTrade();
        }
        else
        {
            OnIncorrectChange?.Invoke();
            ProcessRejection();
        }
    }

    public void RepeatCustomer() // Used for UI Button
    {
        HandleCustomerType();
    }


    public void CancelTrade() // Used for UI Button
    {
        _basePrice = 0;
        _offerValue = 0;
        OnOfferValueChanged?.Invoke(_offerValue);
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(true);
        NoCustomer();
    }

    public void OpenToPublic() // Used for UI Button
    {
        _basePrice = 0;
        _offerValue = 0;
        OnOfferValueChanged?.Invoke(_offerValue);
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(false);
        OnOpenToPublic?.Invoke();
    }

    public void ResetTradeBoxes() // Used for UI Button
    {
        if(!_currentCustomer || _currentCustomer.CustomerType == Customer.Type.Barter)
        {
            OnResetBarter?.Invoke();
        }
        else
        {
            OnResetTrade?.Invoke();
        }
    }

    public void ChangeCopper(bool increase) // UI Button
    {
        if(increase)
        {
            _offerValue += CopperValue;
        }
        else
        {
            _offerValue -= CopperValue;
        }

        if(_offerValue < 0)
        {
            _offerValue = 0;
        }
        if(_offerValue > 9999)
        {
            _offerValue = 9999;
        }

        OnOfferValueChanged?.Invoke(_offerValue);
    }

    public void ChangeSilver(bool increase) // UI Button
    {
        if(increase)
        {
            _offerValue += SilverValue;
        }
        else
        {
            _offerValue -= SilverValue;
        }

        if(_offerValue < 0)
        {
            _offerValue = 0;
        }
        if(_offerValue > 9999)
        {
            _offerValue = 9999;
        }

        OnOfferValueChanged?.Invoke(_offerValue);
    }

    public void ChangeGold(bool increase) // UI Button
    {
        if(increase)
        {
            _offerValue += GoldValue;
        }
        else
        {
            _offerValue -= GoldValue;
        }

        if(_offerValue < 0)
        {
            _offerValue = 0;
        }
        if(_offerValue > 9999)
        {
            _offerValue = 9999;
        }

        OnOfferValueChanged?.Invoke(_offerValue);
    }

    public void ChangePlatinum(bool increase) // UI Button
    {
        if(increase)
        {
            _offerValue += PlatinumValue;
        }
        else
        {
            _offerValue -= PlatinumValue;
        }

        if(_offerValue < 0)
        {
            _offerValue = 0;
        }
        if(_offerValue > 9999)
        {
            _offerValue = 9999;
        }

        OnOfferValueChanged?.Invoke(_offerValue);
    }
}
