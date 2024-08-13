using UnityEngine;
using System;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public static Action<string> OnLineSpoken;

    [SerializeField] List<string> _greetings = new();
    [SerializeField] List<string> _noItems = new();
    [SerializeField] List<string> _askToBuy = new();
    [SerializeField] List<string> _offerToSell = new();
    [SerializeField] List<string> _acceptPrice = new();
    [SerializeField] List<string> _rejectPrice = new();
    [SerializeField] List<string> _completeDeal = new();
    [SerializeField] List<string> _rejectDeal = new();
    // [SerializeField] List<string> _gainStrike = new(); // Overwritten by rejecting prices and offers
    // [SerializeField] List<string> _maxStrikes = new(); // Overwritten by OnFinishWithCustomer
    // [SerializeField] List<string> _maxTrades = new(); // Overwritten by OnFinishWithCustomer
    [SerializeField] List<string> _priceTooHigh = new();
    [SerializeField] List<string> _lostChange = new();
    [SerializeField] List<string> _payAllMoney = new();
    [SerializeField] List<string> _departures = new();
    [SerializeField] List<string> _leaveAngry = new();

    bool _isActive = false, _stoleChange, _poorCustomer;

    void OnEnable()
    {
        Inventory.OnInventoryLoaded += Inventory_OnInventoryLoaded;
        DropBox.OnNoItems += DropBox_OnNoItems;
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnBarterAccepted += TradingSystem_OnBarterAccepted;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        // TradingSystem.OnStrikeOut += TradingSystem_OnStrikeOut; // Overwritten by OnFinishWithCustomer
        // TradingSystem.OnMaxTradesReached += TradingSystem_OnMaxTradesReached; // Overwritten by OnFinishWithCustomer
        TradingSystem.OnBarterTooHigh += TradingSystem_OnBarterTooHigh;
        TradingSystem.OnStoleChange += TradingSystem_OnStoleChange;
        TradingSystem.OnPoorCustomer += TradingSystem_OnPoorCustomer;
        TradingSystem.OnFinishWithCustomer += TradingSystem_OnFinishWithCustomer;
    }

    void OnDisable()
    {
        Inventory.OnInventoryLoaded -= Inventory_OnInventoryLoaded;
        DropBox.OnNoItems -= DropBox_OnNoItems;
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnBarterAccepted -= TradingSystem_OnBarterAccepted;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        // TradingSystem.OnStrikeOut -= TradingSystem_OnStrikeOut; // Overwritten by OnFinishWithCustomer
        // TradingSystem.OnMaxTradesReached -= TradingSystem_OnMaxTradesReached; // Overwritten by OnFinishWithCustomer
        TradingSystem.OnBarterTooHigh -= TradingSystem_OnBarterTooHigh;
        TradingSystem.OnStoleChange -= TradingSystem_OnStoleChange;
        TradingSystem.OnPoorCustomer -= TradingSystem_OnPoorCustomer;
        TradingSystem.OnFinishWithCustomer -= TradingSystem_OnFinishWithCustomer;
    }

    public void SetActiveDialogue(bool isActive)
    {
        _isActive = isActive;
    }

    void Inventory_OnInventoryLoaded(bool isPlayer)
    {
        if(isPlayer) { return; }

        SpeakLine(_greetings);
    }

    void DropBox_OnNoItems()
    {
        SpeakLine(_noItems);
    }

    void TradingSystem_OnOfferAccepted(bool isBuying, int offer)
    {
        if(_poorCustomer)
        {
            SpeakLine(_payAllMoney);
        }
        else
        {
            SpeakLine(_acceptPrice);
        }
    }

    void TradingSystem_OnBarterAccepted(int offer)
    {
        SpeakLine(_acceptPrice);
    }

    void DropBox_OnItemPicked(Item item, bool isPlayer)
    {
        if(isPlayer)
        {
            SpeakLine(_askToBuy);
        }
        else
        {
            SpeakLine(_offerToSell);
        }
    }

    void TradingSystem_OnOfferRejected()
    {
        SpeakLine(_rejectPrice);
    }

    void TradingSystem_OnTradeCompleted(Customer customer)
    {
        if(_stoleChange)
        {
            SpeakLine(_lostChange);
        }
        else
        {
            SpeakLine(_completeDeal);
        }
    }

    void TradingSystem_OnIncorrectChange()
    {
        SpeakLine(_rejectDeal);
    }

    // void TradingSystem_OnStrikeOut(Customer customer)
    // {
    //     SpeakLine(_maxStrikes);
    // }

    // void TradingSystem_OnMaxTradesReached()
    // {
    //     SpeakLine(_maxTrades);
    // }

    void TradingSystem_OnBarterTooHigh()
    {
        SpeakLine(_priceTooHigh);
    }

    void TradingSystem_OnStoleChange(bool stoleChange)
    {
        _stoleChange = stoleChange;
    }

    void TradingSystem_OnPoorCustomer()
    {
        _poorCustomer = true;
    }

    void TradingSystem_OnFinishWithCustomer(Customer customer)
    {
        _poorCustomer = false; // This should be fine...
        if(customer.MaxStrikesReached)
        {
            SpeakLine(_leaveAngry);
        }
        else
        {
            SpeakLine(_departures);
        }
    }

    void SpeakLine(List<string> interaction)
    {
        if(!_isActive) { return; }

        string line = interaction[UnityEngine.Random.Range(0, interaction.Count)];
        OnLineSpoken?.Invoke(line);
    }
}
