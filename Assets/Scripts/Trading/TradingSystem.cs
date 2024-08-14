using UnityEngine;
using System;

public class TradingSystem : MonoBehaviour
{
    public static Action OnIncorrectChange;
    public static Action<bool, int> OnOfferAccepted;
    public static Action<int> OnBarterAccepted;
    public static Action OnOfferRejected;
    public static Action<Customer> OnTradeCompleted;
    public static Action OnTradeCancelled;
    public static Action<Customer> OnNewCustomer;
    public static Action OnOpenToPublic;
    public static Action OnBuyCustomer;
    public static Action OnSellCustomer;
    public static Action<int> OnOfferValueChanged;
    public static Action<int> OnBasePriceSet;
    public static Action<Customer.Type, int> OnChangeGiven;
    public static Action OnResetTrade;
    public static Action OnResetBarter;
    public static Action<int, Currency> OnExchangeCurrency;
    public static Action<Customer> OnStrikeOut;
    public static Action OnBarterTooHigh;
    public static Action<bool> OnStoleChange;
    public static Action OnPoorCustomer;
    public static Action<Customer> OnFinishWithCustomer;
    
    int _playerValue, _compValue, _offerValue;

    int _offer;
    int _basePrice;

    [SerializeField] GameObject _openButton, _bankButton, _exchangeButtons, _goodbyeButton, _haggleButton, _activeTradeButtons, _completeTradeButton, _barterOfferButton, _greetingButton;
    [SerializeField] Customer _bank;
    Customer _currentCustomer;
    
    public const int CopperValue = 1, SilverValue = 10, GoldValue = 100, PlatinumValue = 1000;


    void Awake()
    {
        _bankButton.SetActive(false);
        _openButton.SetActive(false);
    }

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

    void DropBox_OnItemPicked(Item item, bool isPlayer)
    {
        _haggleButton.SetActive(true);
        _basePrice = item.ItemSO.BaseValue;
        OnBasePriceSet?.Invoke(_basePrice);
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
        _activeTradeButtons.SetActive(true);
        OnNewCustomer?.Invoke(customer);
    }

    void Inventory_OnInventoryLoaded(bool isPlayer)
    {
        if(isPlayer) { return; }

        _greetingButton.SetActive(true); // TODO? One day have many buttons for different dialogue responses
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
                BarterCustomer();
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
        // TODO Prompt for player to set a price (Set Haggle button active and deactivate offer button?) <- already done! Make Haggle Button more obvious
    }

    void SellingCustomer()
    {
        OnSellCustomer?.Invoke();
        // TODO Prompt for player to set a price (Set Haggle button active and deactivate offer button?) <- already done! Make Haggle Button more obvious
    }

    void BarterCustomer()
    {
        _barterOfferButton.SetActive(true);
    }

    void NoCustomer()
    {
        _currentCustomer = null;
        OnNewCustomer?.Invoke(null);
        _activeTradeButtons.SetActive(false);
        _bankButton.SetActive(true);
        _exchangeButtons.SetActive(false);
    }

    bool MakeOffer()
    {
        float rake = 0;

        if(_currentCustomer.CustomerType == Customer.Type.Buy)
        {
            if(_offerValue > _currentCustomer.GetTotalFunds())
            {
                OnPoorCustomer?.Invoke();
                return true;
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
        
        if(offer < 0) { return true; } // Player gave too much change
        if(offer == 0) { return true; } // Player gave exact change
        if(_playerValue <= 0) { return false; } // Should not happen
        if(_compValue <= 0) { return true; } // Also should not happen but we don't want to divide by zero

        float rake = 1 - ((float)_playerValue / _compValue);

        return rake <= _currentCustomer.Tolerance;
    }


    void ProcessTrade()
    {
        _completeTradeButton.SetActive(false);

        // TODO UI/VFX/SFX (include profit/loss and if correct change)
        
        OnTradeCompleted?.Invoke(_currentCustomer);

        if(_currentCustomer.CustomerType == Customer.Type.Bank)
        {
            _exchangeButtons.SetActive(true);
        }
    }

    void ProcessRejection()
    {
        _currentCustomer.IncreaseStrikes();
        
        // TODO? alter customer Tolerance

        if(_currentCustomer.Strikes >= _currentCustomer.MaxStrikes)
        {
            OnStrikeOut?.Invoke(_currentCustomer);
            FinishWithCustomer();
        }
    }

    public void AttemptBarter() // Used for UI Button
    {
        if(_compValue == 0 && _playerValue == 0) { return; }
        
        int offer = _compValue - _playerValue;
        
        if(offer <= 0)
        {
            if(_currentCustomer.GetTotalFunds() >= offer)
            {
                OnBarterAccepted?.Invoke(offer); // Player gave higher or equal value which customer will pay
                _completeTradeButton.SetActive(true);
                _barterOfferButton.SetActive(false);
            }
            else
            {
                OnBarterTooHigh?.Invoke();
            }
        }
        else
        {
            if(OfferChange(out int o))
            {
                OnBarterAccepted?.Invoke(o);
                _completeTradeButton.SetActive(true);
                _barterOfferButton.SetActive(false);
            }
            else
            {
                OnOfferRejected?.Invoke();
                // ProcessRejection(); For now you don't get strikes with bad barter offers but your reputation will plummet
            }
        }
    }

    public void Haggle() // Used for UI Button
    {
        if(!_currentCustomer) { return; }

        if(MakeOffer())
        {
            bool buying = _currentCustomer.CustomerType == Customer.Type.Buy;
            _haggleButton.SetActive(false);
            OnOfferAccepted?.Invoke(buying, _offer);
            _completeTradeButton.SetActive(true);
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
            OnStoleChange?.Invoke(offer > 0);
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
        if(_currentCustomer.MaxTradesReached)
        {
            FinishWithCustomer();
            return;
        }

        HandleCustomerType();
    }

    public void FinishWithCustomer()
    {
        if(_currentCustomer)
        {
            OnFinishWithCustomer?.Invoke(_currentCustomer);
        }
        _exchangeButtons.SetActive(false);
        _barterOfferButton.SetActive(false);
        _activeTradeButtons.SetActive(false);
        _haggleButton.SetActive(false);
        _completeTradeButton.SetActive(false);
        _basePrice = 0;
        OnBasePriceSet?.Invoke(_basePrice);
        _offerValue = 0;
        OnOfferValueChanged?.Invoke(_offerValue);
        _goodbyeButton.SetActive(true);
    }

    public void CancelTrade() // Used for UI Button
    {
        _activeTradeButtons.SetActive(false);
        _haggleButton.SetActive(false);
        _basePrice = 0;
        OnBasePriceSet?.Invoke(_basePrice);
        _offerValue = 0;
        OnOfferValueChanged?.Invoke(_offerValue);
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(true);
        _bankButton.SetActive(true);
        _goodbyeButton.SetActive(false);
        NoCustomer();
    }

    public void OpenToPublic() // Used for UI Button
    {
        _basePrice = 0;
        OnBasePriceSet?.Invoke(_basePrice);
        _offerValue = 0;
        OnOfferValueChanged?.Invoke(_offerValue);
        OnTradeCancelled?.Invoke();
        _openButton.SetActive(false);
        _bankButton.SetActive(false);
        _exchangeButtons.SetActive(false);
        OnOpenToPublic?.Invoke();
    }

    public void ResetTradeBoxes() // Used for UI Button
    {
        if(!_currentCustomer || _currentCustomer.CustomerType == Customer.Type.Barter || _currentCustomer.CustomerType == Customer.Type.Bank)
        {
            OnResetBarter?.Invoke();
            if(_currentCustomer.CustomerType == Customer.Type.Bank)
            {
                _exchangeButtons.SetActive(true);
            }
            if(_currentCustomer.CustomerType == Customer.Type.Barter)
            {
                _barterOfferButton.SetActive(true);
                _completeTradeButton.SetActive(false);
            }
        }
        else
        {
            OnResetTrade?.Invoke();
        }
    }

    public void GoToBank() // Used for UI Button
    {
        NewCustomer(_bank);
        _openButton.SetActive(false);
        _bankButton.SetActive(false);
        _exchangeButtons.SetActive(true);
        _completeTradeButton.SetActive(false);
    }

    public void ExchangeCurrency(int currency) // Used for UI Button
    {
        _exchangeButtons.SetActive(false);
        OnExchangeCurrency?.Invoke(_playerValue, (Currency)currency);
        _completeTradeButton.SetActive(true);
    }

    public void GiveAGoodGreeting() // Used for UI Button
    {
        _greetingButton.SetActive(false);
        HandleCustomerType();
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
