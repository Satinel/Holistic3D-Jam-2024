using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;

    [field:SerializeField] public string Name { get; private set; } = string.Empty;
    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField][field:Range(0, 1f)] public float Tolerance { get; private set; }
    [field:SerializeField] public int MaxStrikes { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }
    [field:SerializeField] public int Opinion { get; private set; }

    public bool MaxTradesReached => _currentTrades >= _maxTrades;

    [SerializeField] int _baseOpinionChange = 1;
    [SerializeField] int _maxTrades = 3;
    
    int _currentTrades;
    bool _isActiveCustomer;

    public enum Type
    {
        None,
        Buy,
        Sell,
        Barter,
        Bank
    }

    void OnEnable()
    {
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnExchangeCurrency += TradingSystem_OnExchangeCurrency;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        TradingSystem.OnChangeGiven += TradingSystem_OnChangeGiven;
        TradingSystem.OnStrikeOut += TradingSystem_OnStrikeOut;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        DropBox.OnBuyPriceSet += DropBox_OnBuyPriceSet;
    }

    void OnDisable()
    {
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnExchangeCurrency -= TradingSystem_OnExchangeCurrency;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        TradingSystem.OnChangeGiven -= TradingSystem_OnChangeGiven;
        TradingSystem.OnStrikeOut -= TradingSystem_OnStrikeOut;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        DropBox.OnBuyPriceSet -= DropBox_OnBuyPriceSet;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(customer == this)
        {
            _isActiveCustomer = true;
            Strikes = 0;
            _currentTrades = 0;

            _inventory.ShowInventory(customer.CustomerType);
        }
        else
        {
            _isActiveCustomer = false;
        }
    }

    void TradingSystem_OnExchangeCurrency(int amount)
    {
        if(_isActiveCustomer && CustomerType == Type.Bank)
        {
            _inventory.GenerateCopper(amount, CustomerType);
        }
    }

    void TradingSystem_OnOfferAccepted(bool isBuying, int offer)
    {
        if(!_isActiveCustomer) { return; }

        if(offer <= 0)
        {
            ChangeOpinion(_baseOpinionChange);
        }
    }

    void TradingSystem_OnOfferRejected()
    {
        if(!_isActiveCustomer) { return; }

        ChangeOpinion(-_baseOpinionChange);
    }

    void TradingSystem_OnIncorrectChange()
    {
        if(!_isActiveCustomer) { return; }

        ChangeOpinion(-_baseOpinionChange);
    }

    void TradingSystem_OnChangeGiven(Type type, int change)
    {
        if(!_isActiveCustomer) { return; }

        if(change <= 0)
        {
            ChangeOpinion(_baseOpinionChange * 2);
        }
        else if(change == 0)
        {
            ChangeOpinion(_baseOpinionChange);
        }
        else
        {
            ChangeOpinion(-_baseOpinionChange * 2);
        }
    }

    void TradingSystem_OnStrikeOut(Customer customer)
    {
        if(!_isActiveCustomer) { return; }

        // TODO display angry customer message (plus SFX maybe)
    }

    void TradingSystem_OnTradeCompleted(Customer customer)
    {
        if(!_isActiveCustomer) { return; }

        // TODO display some customer message
        _currentTrades++;
    }

    void DropBox_OnBuyPriceSet(int cost)
    {
        if(!_isActiveCustomer) { return; }
        
        _inventory.CoinBox.Pay(cost); // Note: It SHOULD NOT be possible for the price to be higher than the customer's funds due to check in TradingSystem
    }

    void ChangeOpinion(int change)
    {
        Opinion += change;
    }

    public int GetTotalFunds()
    {
        return _inventory.CoinBox.GetTrueValue();
    }

    public void IncreaseStrikes(int amount)
    {
        Strikes += amount;

        // TODO VFX (probably adding pieces of an anger emoji)
    }

    public void AddToInventory(List<GameObject> items)
    {
        _inventory.AddItems(items);
    }

    public void RemoveFromInventory(List<GameObject> items)
    {
        _inventory.Remove(items);
    }
}
