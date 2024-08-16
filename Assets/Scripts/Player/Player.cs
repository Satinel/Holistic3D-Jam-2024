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

    [SerializeField] TextMeshProUGUI _netWorthText, _debtText, _finalResultsText;
    [SerializeField] GameObject _netWorthParent, _debtParent;
    [SerializeField] Inventory _inventory;

    int _preTradeWorth, _preTradeRep;

    bool _moneyLoaded;
    bool _stockLoaded;
    bool _isTutorial;

    void Awake()
    {
        if(!_inventory)
        {
            _inventory = GetComponent<Inventory>();
        }
    }

    void OnEnable()
    {
        Inventory.OnPlayerMoneyLoaded += Inventory_PlayerMoneyLoaded;
        Inventory.OnPlayerStockLoaded += Inventory_PlayerStockLoaded;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        DropBox.OnNetWorthReady += DropBox_OnNetWorthReady;
        TradingSystem.OnFinishWithCustomer += TradingSystem_OnFinishWithCustomer;
        Town.OnNoCustomers += Town_OnNoCustomers;
    }

    void OnDisable()
    {
        Inventory.OnPlayerMoneyLoaded -= Inventory_PlayerMoneyLoaded;
        Inventory.OnPlayerStockLoaded -= Inventory_PlayerStockLoaded;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        DropBox.OnNetWorthReady -= DropBox_OnNetWorthReady;
        TradingSystem.OnFinishWithCustomer -= TradingSystem_OnFinishWithCustomer;
        Town.OnNoCustomers -= Town_OnNoCustomers;
    }

    public void EnableInventory()
    {
        _inventory.enabled = true;
    }

    void Inventory_PlayerMoneyLoaded()
    {
        _moneyLoaded = true;

        if(_stockLoaded)
        {
            SetNetWorthText();
        }
    }

    void Inventory_PlayerStockLoaded()
    {
        _stockLoaded = true;

        if(_moneyLoaded)
        {
            SetNetWorthText();
        }
    }

    public void SetNetWorthText()
    {
        CalculateNetWorth();
        _netWorthText.text = $"Net Worth\n{NetWorth:N0}";
        _netWorthParent.SetActive(true);
    }

    void CalculateNetWorth()
    {
        TotalStockValue = _inventory.StockBox.GetTrueValue();
        TotalCoinValue = _inventory.CoinBox.GetTrueValue();

        NetWorth = TotalStockValue + TotalCoinValue;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        _preTradeWorth = NetWorth;
        _preTradeRep = Reputation;
    }

    void DropBox_OnNetWorthReady()
    {
        SetNetWorthText();
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

        CalculateNetWorth();

        if(_isTutorial) { return; }

        int profit = NetWorth - _preTradeWorth;
        TotalProfits += profit;

        OnProfitCalculated?.Invoke(profit, Reputation - _preTradeRep);
    }

    void Town_OnNoCustomers()
    {
        _finalResultsText.text = $"Total Profits Earned: {TotalProfits:N0}\nFinal Networth: {NetWorth:N0}\nReputation: {Reputation}";
    }

    public void SetDebt(int debt)
    {
        Debt = debt;
        _debtText.text = $"Debt\n{Debt:N0}";
        _debtParent.SetActive(true);
        SetNetWorthText();
    }

    public void FinishTutorial()
    {
        _isTutorial = false;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            SetDebt(0);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            Reputation++;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            Reputation--;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(_inventory.CoinBox.GetTrueValue());
            Debug.Log(_inventory.StockBox.GetTrueValue());
        }
    }
}
