using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _copperText, _silverText, _goldText, _platinumText, _tradeBoxText;
    [SerializeField] TextMeshProUGUI _compCopperText, _compSilverText, _compGoldText, _compPlatinumText, _compTradeBoxText;
    [SerializeField] TextMeshProUGUI _profitText, _changeText, _offerText, _tradeTypeText, _customerNameText;
    [SerializeField] Image _customerImage, _itemImage;
    [SerializeField] GameObject _resultWindow, _rejectionWindow, _noItemsWindow, _setPriceWindow, _incorrectChangeWindow, _customerName;

    int _copperTotal, _silverTotal, _goldTotal, _platinumTotal;
    int _compCopperTotal, _compSilverTotal, _compGoldTotal, _compPlatinumTotal;

    int _playerValue, _compValue;

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
        DropBox.OnCoinAdded += DropBox_OnCoinAdded;
        DropBox.OnCoinRemoved += DropBox_OnCoinRemoved;
        DropBox.OnNoItems += DropBox_OnNoItems;
        TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged += TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven += TradingSystem_OnChangeGiven;
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        DropBox.OnTradeResults += DropBox_OnTradeResults;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        DropBox.OnCoinAdded -= DropBox_OnCoinAdded;
        DropBox.OnCoinRemoved -= DropBox_OnCoinRemoved;
        DropBox.OnNoItems -= DropBox_OnNoItems;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnOfferValueChanged -= TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnChangeGiven -= TradingSystem_OnChangeGiven;
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        DropBox.OnTradeResults -= DropBox_OnTradeResults;
    }

    void DropBox_OnTradeBoxValueChanged(bool isPlayer, int value)
    {
        if(isPlayer)
        {
            _tradeBoxText.text = $"{value}";
        }
        else
        {
            _compTradeBoxText.text = $"{value}";
        }
    }

    void DropBox_OnCoinAdded(bool isPlayer, Currency currencyType)
    {
        switch (currencyType)
        {
            case Currency.Copper:
                if(isPlayer)
                {
                    _copperTotal++;
                    _copperText.text = _copperTotal.ToString("N0");
                }
                else
                {
                    _compCopperTotal++;
                    _compCopperText.text = _compCopperTotal.ToString("N0");
                }
                break;
            case Currency.Silver:
                if(isPlayer)
                {
                    _silverTotal++;
                    _silverText.text = _silverTotal.ToString("N0");
                }
                else
                {
                    _compSilverTotal++;
                    _compSilverText.text = _compSilverTotal.ToString("N0");
                }
                break;
            case Currency.Gold:
                if(isPlayer)
                {
                    _goldTotal++;
                    _goldText.text = _goldTotal.ToString("N0");
                }
                else
                {
                    _compGoldTotal++;
                    _compGoldText.text = _compGoldTotal.ToString("N0");
                }
                break;
            case Currency.Platinum:
                if(isPlayer)
                {
                    _platinumTotal++;
                    _platinumText.text = _platinumTotal.ToString("N0");
                }
                else
                {
                    _compPlatinumTotal++;
                    _compPlatinumText.text = _compPlatinumTotal.ToString("N0");
                }
                break;
        }
    }

    void DropBox_OnCoinRemoved(bool isPlayer, Currency currencyType)
    {
        switch(currencyType)
        {
            case Currency.Copper:
                if(isPlayer)
                {
                    _copperTotal--;
                    _copperText.text = _copperTotal.ToString("N0");
                }
                else
                {
                    _compCopperTotal--;
                    _compCopperText.text = _compCopperTotal.ToString("N0");
                }
                break;
            case Currency.Silver:
                if(isPlayer)
                {
                    _silverTotal--;
                    _silverText.text = _silverTotal.ToString("N0");
                }
                else
                {
                    _compSilverTotal--;
                    _compSilverText.text = _compSilverTotal.ToString("N0");
                }
                break;
            case Currency.Gold:
                if(isPlayer)
                {
                    _goldTotal--;
                    _goldText.text = _goldTotal.ToString("N0");
                }
                else
                {
                    _compGoldTotal--;
                    _compGoldText.text = _compGoldTotal.ToString("N0");
                }
                break;
            case Currency.Platinum:
                if(isPlayer)
                {
                    _platinumTotal--;
                    _platinumText.text = _platinumTotal.ToString("N0");
                }
                else
                {
                    _compPlatinumTotal--;
                    _compPlatinumText.text = _compPlatinumTotal.ToString("N0");
                }
                break;
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

    void TradingSystem_OnOfferRejected()
    {
        _rejectionWindow.SetActive(true);
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!customer)
        {
            _customerImage.enabled = false;
            _customerName.SetActive(false);
            _tradeTypeText.text = string.Empty;
            return;
        }

        _customerImage.sprite = customer.Sprite;
        _customerImage.enabled = true;
        _customerName.SetActive(true);
        _customerNameText.text = customer.Name;
        _changeText.text = string.Empty;
        if(customer.CustomerType == Customer.Type.Buy)
        {
            _tradeTypeText.text = "Sell Offer";
        }
        if(customer.CustomerType == Customer.Type.Sell)
        {
            _tradeTypeText.text = "Buy Offer";
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
        ResetCustomerCoins();
    }

    void ResetCustomerCoins()
    {
        _compCopperTotal = 0; _compSilverTotal = 0; _compGoldTotal = 0; _compPlatinumTotal = 0;
        _compCopperText.text = _compCopperTotal.ToString("N0");
        _compSilverText.text = _compSilverTotal.ToString("N0");
        _compGoldText.text = _compGoldTotal.ToString("N0");
        _compPlatinumText.text = _compPlatinumTotal.ToString("N0");
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
        
    }

    void DropBox_OnItemPicked(Item item)
    {
        _setPriceWindow.SetActive(true);
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
        _itemImage.enabled = true;
        _setPriceWindow.SetActive(true);
    }
}
