using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    public static Action<int, int> OnProfitCalculated;

    [field:SerializeField] public int Reputation { get; private set; }
    public int TotalStockValue { get; private set; }
    public int TotalCoinValue { get; private set; }
    public int NetWorth { get; private set; }
    public int Debt { get; private set; }
    public int TotalProfits { get; private set; }

    [SerializeField] TextMeshProUGUI _netWorthText, _debtText;

    [SerializeField] Inventory _inventory;

    int _preTradeWorth, _preTradeRep;

    void Awake()
    {
        if(!_inventory)
        {
            _inventory = GetComponent<Inventory>();
        }
    }

    void OnEnable()
    {
        Inventory.OnInventoryLoaded += Inventory_OnInventoryLoaded;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        DropBox.OnTradeBoxProcessed += DropBox_OnTradeBoxProcessed;
        TradingSystem.OnFinishWithCustomer += TradingSystem_OnFinishWithCustomer;
    }

    void OnDisable()
    {
        Inventory.OnInventoryLoaded -= Inventory_OnInventoryLoaded;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        DropBox.OnTradeBoxProcessed -= DropBox_OnTradeBoxProcessed;
        TradingSystem.OnFinishWithCustomer -= TradingSystem_OnFinishWithCustomer;
    }

    public void EnableInventory()
    {
        _inventory.enabled = true;
    }

    void Inventory_OnInventoryLoaded(bool isPlayer)
    {
        if (!isPlayer) { return; }

        CalculateNetWorth();
    }

    int CalculateNetWorth()
    {
        TotalStockValue = _inventory.StockBox.GetTrueValue();
        TotalCoinValue = _inventory.CoinBox.GetTrueValue();

        return TotalStockValue + TotalCoinValue;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        _preTradeWorth = CalculateNetWorth();
        _preTradeRep = Reputation;
    }

    void DropBox_OnTradeBoxProcessed()
    {
        _netWorthText.text = "Net Worth\n" + CalculateNetWorth().ToString("N0");
    }

    void TradingSystem_OnFinishWithCustomer(Customer customer)
    {
        if(customer.Opinion > 0)
        {
            Reputation++;
        }

        if(customer.Opinion < 0)
        {
            Reputation--;
        }

        int profit = CalculateNetWorth() - _preTradeWorth;

        TotalProfits += profit;

        OnProfitCalculated?.Invoke(profit, Reputation - _preTradeRep);
    }

    public void SetDebt(int debt)
    {
        Debt = debt;
        _debtText.text = Debt.ToString("N0");
    }
}
