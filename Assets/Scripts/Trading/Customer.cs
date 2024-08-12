using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;
    [SerializeField] Dialogue _dialogue;

    [field:SerializeField] public string Name { get; private set; } = string.Empty;
    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField][field:Range(0, 1f)] public float Tolerance { get; private set; }
    [field:SerializeField] public int MaxStrikes { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }
    [field:SerializeField] public int Opinion { get; private set; }

    public bool MaxTradesReached => _currentTrades >= _maxTrades;
    public bool MaxStrikesReached => Strikes >= MaxStrikes;

    [SerializeField] SpriteRenderer _spriteRender;
    [SerializeField] List<GameObject> _angerMarks;

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
            _dialogue.SetActiveDialogue(true);
            Strikes = 0;
            _currentTrades = 0;
            _spriteRender.enabled = true;

            _inventory.ShowInventory(customer.CustomerType);
        }
        else
        {
            foreach(GameObject mark in _angerMarks)
            {
                mark.SetActive(false);
            }
            _spriteRender.enabled = false;
            _isActiveCustomer = false;
            _dialogue.SetActiveDialogue(false);
        }
        
        
    }

    void TradingSystem_OnExchangeCurrency(int amount, Currency currency)
    {
        if(_isActiveCustomer && CustomerType == Type.Bank)
        {
            int coinAmount = 0;
            switch(currency)
            {
                case Currency.Copper:
                    coinAmount = Mathf.FloorToInt((float)amount / TradingSystem.CopperValue);
                    break;
                case Currency.Silver:
                    coinAmount = Mathf.FloorToInt((float)amount / TradingSystem.SilverValue);
                    break;
                case Currency.Gold:
                    coinAmount = Mathf.FloorToInt((float)amount / TradingSystem.GoldValue);
                    break;
                case Currency.Platinum:
                    coinAmount = Mathf.FloorToInt((float)amount / TradingSystem.PlatinumValue);
                    break;
            }
            _inventory.GenerateCoins(coinAmount, CustomerType, currency);
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
        if(CustomerType == Type.Bank) { return; }

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

        if(CustomerType != Type.Bank)
        {
            _currentTrades++;
            
            if(MaxTradesReached)
            {
                // TODO display customer parting message
            }
        }
    }

    void DropBox_OnBuyPriceSet(int cost)
    {
        if(!_isActiveCustomer) { return; }

        if(cost > GetTotalFunds())
        { 
            cost = GetTotalFunds(); // Customer will give the sum totality of their coin in the hopes that it's good enough
        }
        
        _inventory.CoinBox.Pay(cost);
    }

    void ChangeOpinion(int change)
    {
        Opinion += change;
    }

    public int GetTotalFunds()
    {
        return _inventory.CoinBox.GetTrueValue();
    }

    public void IncreaseStrikes()
    {
        if(CustomerType == Type.Bank) { return; }

        Strikes++;
        
        // TODO SFX

        if(Strikes <= _angerMarks.Count)
        {
            _angerMarks[Strikes -1].SetActive(true);
        }
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
