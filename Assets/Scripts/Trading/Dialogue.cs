using UnityEngine;
using System;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public static Action<string> OnLineSpoken;

    [SerializeField] List<string> _greetings = new();
    [SerializeField] List<string> _askToBuy = new();
    [SerializeField] List<string> _offerToSell = new();
    [SerializeField] List<string> _acceptPrice = new();
    [SerializeField] List<string> _rejectPrice = new();
    [SerializeField] List<string> _completeDeal = new();
    [SerializeField] List<string> _rejectDeal = new();
    [SerializeField] List<string> _gainStrike = new();
    [SerializeField] List<string> _maxStrikes = new();
    [SerializeField] List<string> _maxTrades = new();
    [SerializeField] List<string> _madeProfit = new();
    [SerializeField] List<string> _lostChange = new();
    [SerializeField] List<string> _payAllMoney = new();
    [SerializeField] List<string> _departures = new();

    bool _isActive = false;

    void OnEnable()
    {
        Inventory.OnInventoryLoaded += Inventory_OnInventoryLoaded;
    }

    void OnDisable()
    {
        Inventory.OnInventoryLoaded -= Inventory_OnInventoryLoaded;
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

    void SpeakLine(List<string> interaction)
    {
        if(!_isActive) { return; }

        string line = interaction[UnityEngine.Random.Range(0, interaction.Count)];
        OnLineSpoken?.Invoke(line);
    }
}
