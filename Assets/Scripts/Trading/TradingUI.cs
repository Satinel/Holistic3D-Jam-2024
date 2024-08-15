using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tradeBoxText, _payText;
    [SerializeField] TextMeshProUGUI _compTradeBoxText;
    [SerializeField] TextMeshProUGUI _profitText, _changeText, _offerText, _tradeTypeText, _tradeTypeText2, _customerNameText, _itemNameText;
    [SerializeField] Image _itemImage;
    [SerializeField] GameObject _payTextParent, _repeatCustomerButton, _tradeTypeText2Parent;
    [SerializeField] GameObject _resultWindow, _noItemsWindow, _setPriceWindow, _customerName, _noCustomersWindow, _itemNameWindow;

    // int _playerValue, _compValue;
    bool _showTradeNumbers = false;
    Customer _customer;

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
        DropBox.OnNoItems += DropBox_OnNoItems;
        // TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnResetBarter += TradingSystem_OnResetBarter;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged += TradingSystem_OnOfferValueChanged;
        TradingSystem.OnStrikeOut += TradingSystem_OnStrikeOut;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven += TradingSystem_OnChangeGiven;
        TradingSystem.OnFinishWithCustomer += TradingSystem_OnFinishWithCustomer;
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        DropBox.OnTradeResults += DropBox_OnTradeResults;
        Customer.OnMaxTradesReached += Customer_OnMaxTradesReached;
        Player.OnProfitCalculated += Player_OnProfitCalculated;
        DropBox.OnBuyPriceSet += DropBox_OnBuyPriceSet;
        DropBox.OnSellPriceSet += DropBox_OnSellPriceSet;
        Town.OnNoCustomers += Town_OnNoCustomers;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        DropBox.OnNoItems -= DropBox_OnNoItems;
        // TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnResetBarter += TradingSystem_OnResetBarter;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged -= TradingSystem_OnOfferValueChanged;
        TradingSystem.OnStrikeOut -= TradingSystem_OnStrikeOut;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven -= TradingSystem_OnChangeGiven;
        TradingSystem.OnFinishWithCustomer -= TradingSystem_OnFinishWithCustomer;
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        DropBox.OnTradeResults -= DropBox_OnTradeResults;
        Customer.OnMaxTradesReached -= Customer_OnMaxTradesReached;
        Player.OnProfitCalculated -= Player_OnProfitCalculated;
        DropBox.OnBuyPriceSet -= DropBox_OnBuyPriceSet;
        DropBox.OnSellPriceSet -= DropBox_OnSellPriceSet;
        Town.OnNoCustomers -= Town_OnNoCustomers;
    }

    void DropBox_OnTradeBoxValueChanged(bool isPlayer, int value)
    {
        if(!_showTradeNumbers) { return; }

        if(isPlayer)
        {
            _tradeBoxText.text = $"{value}";
        }
        else
        {
            _compTradeBoxText.text = $"{value}";
        }
    }

    void DropBox_OnNoItems()
    {
        _noItemsWindow.SetActive(true);
    }

    // void TradingSystem_OnIncorrectChange()
    // {
    //     _incorrectChangeWindow.SetActive(true);
    // }

    void TradingSystem_OnOfferAccepted(bool isBuying, int offer)
    {
        CloseSetPrice();
    }

    void TradingSystem_OnResetBarter()
    {
        _payTextParent.SetActive(false);
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!customer)
        {
            _customer = null;
            _customerName.SetActive(false);
            _tradeTypeText.text = string.Empty;
            _tradeTypeText2.text = string.Empty;
            _tradeTypeText2Parent.SetActive(false);
            SetShowTradeNumbers(false);
            return;
        }

        _customer = customer;
        _customerName.SetActive(true);
        _customerNameText.text = customer.Name;
        _changeText.text = string.Empty;
        if(customer.CustomerType == Customer.Type.Buy)
        {
            _tradeTypeText.text = "Sell Offer";
            _tradeTypeText2.text = "SELLING";
            _tradeTypeText2Parent.SetActive(true);
        }
        else if(customer.CustomerType == Customer.Type.Sell)
        {
            _tradeTypeText.text = "Buy Offer";
            _tradeTypeText2.text = "BUYING";
            _tradeTypeText2Parent.SetActive(true);
        }
        
        if(_customer.Tolerance <= 0 || _customer.IsTutorial)
        {
            SetShowTradeNumbers(true);
        }
    }

    void TradingSystem_OnOfferValueChanged(int value)
    {
        _offerText.text = value.ToString("D4");
    }

    void TradingSystem_OnStrikeOut(Customer customer)
    {
        _payTextParent.SetActive(false);
        CloseSetPrice();
    }

    void TradingSystem_OnTradeCancelled()
    {
        CloseResults();
        CloseNoItems();
        CloseSetPrice();
        DisableRepeatCustomerButton();
        _tradeBoxText.text = string.Empty;
        _compTradeBoxText.text = string.Empty;
        _payTextParent.SetActive(false);
        _itemImage.enabled = false;
        _itemNameWindow.SetActive(false);
        _itemNameText.text = string.Empty;
        _tradeTypeText2Parent.SetActive(false);
    }

    void TradingSystem_OnChangeGiven(Customer.Type customerType, int change)
    {
        // TODO This needs to be reworked because the values aren't always change relevant
        // if(customerType == Customer.Type.Buy)
        // {
        //     if(change < 0)
        //     {
        //         _changeText.text = $"Gave {-change:N0} Extra Change!";
        //         // TODO Player Rep+
        //     }
        //     else if(change == 0)
        //     {
        //         _changeText.text = $"Gave Exact Change!";
        //         // TODO Player Rep++
        //     }
        //     else
        //     {
        //         _changeText.text = $"Stole {change:N0} Change!";
        //         // TODO Player Rep--
        //     }
        // }
        // if(customerType == Customer.Type.Sell)
        // {
        //     if(change < 0)
        //     {
        //         _changeText.text = $"Overpaid by {-change:N0}!";
        //         // TODO Player Rep+
        //     }
        //     else if(change == 0)
        //     {
        //         _changeText.text = $"Perfect Payment!";
        //         // TODO Player Rep++
        //     }
        //     else
        //     {
        //         _changeText.text = $"Underpaid by {change:N0}!";
        //         // TODO Player Rep--
        //     }
        // }
        _tradeBoxText.text = string.Empty;
        _compTradeBoxText.text = string.Empty;
        _payTextParent.SetActive(false);
        _itemImage.enabled = false;
        _itemNameWindow.SetActive(false);
        _itemNameText.text = string.Empty;
        _tradeTypeText2Parent.SetActive(false);
    }

    void TradingSystem_OnFinishWithCustomer(Customer customer)
    {
        DisableRepeatCustomerButton();
    }

    void DropBox_OnItemPicked(Item item, bool isPlayer)
    {
        _tradeTypeText2Parent.SetActive(true);
        _itemImage.enabled = true;
        _itemImage.sprite = item.ItemSO.ItemSprite;
        _itemNameText.text = item.ItemSO.ItemName;
        _itemNameWindow.SetActive(true);
        _tradeTypeText2Parent.SetActive(true);
    }

    void DropBox_OnTradeResults()
    {
        // if(isPlayer)
        // {
        //     _playerValue = value;
        // }
        // else
        // {
        //     _compValue = value;
        // }

        // _resultWindow.SetActive(true);
        // _profitText.text = $"Profit: {_compValue - _playerValue:N0}";

        if(_customer)
        {
            if(!_customer.MaxTradesReached)
            {
                _repeatCustomerButton.SetActive(true);
            }
        }
    }

    void Customer_OnMaxTradesReached()
    {
        _repeatCustomerButton.SetActive(false);
    }

    void Player_OnProfitCalculated(int profit, int repChange)
    {
        if(_customer.IsTutorial) { return; }
        
        if(_customer.CustomerType == Customer.Type.Bank) { return; }
        
        _resultWindow.SetActive(true);
        if(profit >= 0)
        {
            _profitText.text = $"Profit: {profit:N0}";
        }
        else
        {
            _profitText.text = $"Losses: {profit:N0}";
        }

        if(repChange > 0)
        {
            _changeText.text = "Gained Reputation!";
        }
        else if(repChange < 0)
        {
            _changeText.text = "Lost Reputation...";
        }
        else
        {
            _changeText.text = "Reputation Unchanged.";
        }
    }

    void DropBox_OnBuyPriceSet(int totalValue)
    {
        _payTextParent.SetActive(true);
        _payText.text = $"Customer Pays: {totalValue:N0}₡";
    }

    void DropBox_OnSellPriceSet(int totalValue)
    {
        _payTextParent.SetActive(true);
        _payText.text = $"You Pay: {totalValue:N0}₡";
    }

    void Town_OnNoCustomers()
    {
        _noCustomersWindow.SetActive(true);
    }

    public void SetShowTradeNumbers(bool shouldShow)
    {
        _showTradeNumbers = shouldShow;
        _tradeBoxText.text = $"0";
        _compTradeBoxText.text = $"0";
    }

    public void CloseResults() // UI Button
    {
        _resultWindow.SetActive(false);
    }

    public void CloseNoItems() // UI Button
    {
        _noItemsWindow.SetActive(false);
    }

    public void CloseSetPrice() // UI Button
    {
        _setPriceWindow.SetActive(false);
    }

    public void OpenSetPrice() // UI Button
    {
        _setPriceWindow.SetActive(true);
    }

    public void CloseNoCustomersWindow() // UI Button
    {
        _noCustomersWindow.SetActive(false);
    }

    public void DisableRepeatCustomerButton() // UI Button
    {
        _repeatCustomerButton.SetActive(false);
    }
}
