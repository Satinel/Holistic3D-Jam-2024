using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tradeBoxText, _payText;
    [SerializeField] TextMeshProUGUI _compTradeBoxText;
    [SerializeField] TextMeshProUGUI _profitText, _changeText, _offerText, _tradeTypeText, _customerNameText;
    [SerializeField] Image _customerImage, _itemImage;
    [SerializeField] GameObject _setPriceButton, _resetTradeButton, _payTextParent;
    [SerializeField] GameObject _resultWindow, _rejectionWindow, _noItemsWindow, _setPriceWindow, _incorrectChangeWindow, _customerName, _noCustomersWindow;

    int _playerValue, _compValue;
    bool _showTradeNumbers = false;

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
        DropBox.OnNoItems += DropBox_OnNoItems;
        TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged += TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven += TradingSystem_OnChangeGiven;
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        DropBox.OnTradeResults += DropBox_OnTradeResults;
        DropBox.OnBuyPriceSet += DropBox_OnBuyPriceSet;
        DropBox.OnSellPriceSet += DropBox_OnSellPriceSet;
        Town.OnNoCustomers += Town_OnNoCustomers;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        DropBox.OnNoItems -= DropBox_OnNoItems;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged -= TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven -= TradingSystem_OnChangeGiven;
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        DropBox.OnTradeResults -= DropBox_OnTradeResults;
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

    void TradingSystem_OnIncorrectChange()
    {
        _incorrectChangeWindow.SetActive(true);
    }

    void TradingSystem_OnOfferAccepted(bool isBuying, int offer)
    {
        _setPriceButton.SetActive(false);
        CloseSetPrice();
    }

    void TradingSystem_OnOfferRejected()
    {
        _rejectionWindow.SetActive(true);
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!customer)
        {
            _showTradeNumbers = false;
            _resetTradeButton.SetActive(false);
            _customerImage.enabled = false;
            _customerName.SetActive(false);
            _tradeTypeText.text = string.Empty;
            return;
        }

        _resetTradeButton.SetActive(true);
        _customerImage.sprite = customer.Sprite;
        _customerImage.enabled = true;
        _customerName.SetActive(true);
        _customerNameText.text = customer.Name;
        _changeText.text = string.Empty;
        if(customer.CustomerType == Customer.Type.Buy)
        {
            _showTradeNumbers = false;
            _tradeTypeText.text = "Sell Offer";
        }
        else if(customer.CustomerType == Customer.Type.Sell)
        {
            _showTradeNumbers = false;
            _tradeTypeText.text = "Buy Offer";
        }
        else
        {
            _showTradeNumbers = true;
        }
        // TODO Set a text field based on a string supplied by customer as a greeting
    }

    void TradingSystem_OnOfferValueChanged(int value)
    {
        _offerText.text = value.ToString("D4");
    }

    void TradingSystem_OnTradeCancelled()
    {
        CloseRejection();
        CloseResults();
        CloseNoItems();
        CloseSetPrice();
        CloseChange();
        _setPriceButton.SetActive(false);
        _resetTradeButton.SetActive(false);
        _tradeBoxText.text = string.Empty;
        _compTradeBoxText.text = string.Empty;
        _payTextParent.SetActive(false);
    }

    void TradingSystem_OnChangeGiven(Customer.Type customerType, int change)
    {
        if(customerType == Customer.Type.Buy)
        {
            if(change < 0)
            {
                _changeText.text = $"Gave {-change:N0} Extra Change!";
                // TODO Player Rep+
            }
            else if(change == 0)
            {
                _changeText.text = $"Gave Exact Change!";
                // TODO Player Rep++
            }
            else
            {
                _changeText.text = $"Stole {change:N0} Change!";
                // TODO Player Rep--
            }
        }
        if(customerType == Customer.Type.Sell)
        {
            if(change < 0)
            {
                _changeText.text = $"Overpaid by {-change:N0}!";
                // TODO Player Rep+
            }
            else if(change == 0)
            {
                _changeText.text = $"Perfect Payment!";
                // TODO Player Rep++
            }
            else
            {
                _changeText.text = $"Underpaid by {change:N0}!";
                // TODO Player Rep--
            }
        }
        _tradeBoxText.text = string.Empty;
        _compTradeBoxText.text = string.Empty;
        _payTextParent.SetActive(false);
    }

    void DropBox_OnItemPicked(Item item)
    {
        // TODO (Somewhere) Customer message re: item they want to buy/sell
        _setPriceButton.SetActive(true);
        _itemImage.enabled = true;
        _itemImage.sprite = item.ItemSO.ItemSprite;
    }

    void DropBox_OnTradeResults(bool isPlayer, int value)
    {
        if(isPlayer)
        {
            _playerValue = value;
        }
        else
        {
            _compValue = value;
        }

        _resultWindow.SetActive(true);
        _profitText.text = $"Profit: {_compValue - _playerValue:N0}";
    }

    void DropBox_OnBuyPriceSet(int totalValue)
    {
        _payTextParent.SetActive(true);
        _payText.text = $"Customer Pays: {totalValue:N0}₡";
    }

    void DropBox_OnSellPriceSet(int totalValue)
    {
        _payTextParent.SetActive(true);
        _payText.text = _payText.text = $"You Pay: {totalValue:N0}₡";
    }

    void Town_OnNoCustomers()
    {
        _noCustomersWindow.SetActive(true);
    }

    public void CloseResults() // UI Button
    {
        _resultWindow.SetActive(false);
    }

    public void CloseRejection() // UI Button
    {
        _rejectionWindow.SetActive(false);
    }

    public void CloseNoItems() // UI Button
    {
        _noItemsWindow.SetActive(false);
    }

    public void CloseSetPrice() // UI Button
    {
        _itemImage.enabled = false;
        _setPriceWindow.SetActive(false);
    }

    public void CloseChange() // UI Button
    {
        _incorrectChangeWindow.SetActive(false);
    }

    public void OpenSetPrice() // UI Button
    {
        _setPriceWindow.SetActive(true);
    }

    public void CloseNoCustomersWindow() // UI Button
    {
        _noCustomersWindow.SetActive(false);
    }
}
