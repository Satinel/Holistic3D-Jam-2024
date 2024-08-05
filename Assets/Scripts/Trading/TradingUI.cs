using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _copperText, _silverText, _goldText, _platinumText, _tradeBoxText;
    [SerializeField] TextMeshProUGUI _compCopperText, _compSilverText, _compGoldText, _compPlatinumText, _compTradeBoxText;
    [SerializeField] Image _customerImage;

    int _copperTotal, _silverTotal, _goldTotal, _platinumTotal;
    int _compCopperTotal, _compSilverTotal, _compGoldTotal, _compPlatinumTotal;

    void OnEnable()
    {
        // Inventory.OnItemAmountChanged += Inventory_OnItemAmountChanged;
        Inventory.OnMoneyAmountChanged += Inventory_OnMoneyAmountChanged;
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
        DropBox.OnCoinAdded += DropBox_OnCoinAdded;
        DropBox.OnCoinRemoved += DropBox_OnCoinRemoved;
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
    }

    void OnDisable()
    {
        // Inventory.OnItemAmountChanged -= Inventory_OnItemAmountChanged;
        Inventory.OnMoneyAmountChanged -= Inventory_OnMoneyAmountChanged;
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
        DropBox.OnCoinAdded -= DropBox_OnCoinAdded;
        DropBox.OnCoinRemoved -= DropBox_OnCoinRemoved;
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
    }

    void Inventory_OnMoneyAmountChanged(bool isPlayer, Dictionary<Currency, int> money)
    {
        if(isPlayer)
        {
            if(money.ContainsKey(Currency.Copper))
            {
                _copperTotal = money[Currency.Copper];
                _copperText.text = _copperTotal.ToString("N0");
            }
            if(money.ContainsKey(Currency.Silver))
            {
                _silverTotal = money[Currency.Silver];
                _silverText.text = _silverTotal.ToString("N0");
            }
            if(money.ContainsKey(Currency.Gold))
            {
                _goldTotal = money[Currency.Gold];
                _goldText.text = _goldTotal.ToString("N0");
            }
            if(money.ContainsKey(Currency.Platinum))
            {
                _platinumTotal = money[Currency.Platinum];
                _platinumText.text = _platinumTotal.ToString("N0");
            }
        }
        else
        {
            if(money.ContainsKey(Currency.Copper))
            {
                _compCopperTotal = money[Currency.Copper];
                _compCopperText.text = money[Currency.Copper].ToString("N0");
            }
            else
            {
                _compCopperText.text = "0";
            }
            if(money.ContainsKey(Currency.Silver))
            {
                _compSilverTotal = money[Currency.Silver];
                _compSilverText.text = money[Currency.Silver].ToString("N0");
            }
            else
            {
                _compSilverText.text = "0";
            }
            if(money.ContainsKey(Currency.Gold))
            {
                _compGoldTotal = money[Currency.Gold];
                _compGoldText.text = money[Currency.Gold].ToString("N0");
            }
            else
            {
                _compGoldText.text = "0";
            }
            if(money.ContainsKey(Currency.Platinum))
            {
                _compPlatinumTotal = money[Currency.Platinum];
                _compPlatinumText.text = money[Currency.Platinum].ToString("N0");
            }
            else
            {
                _compPlatinumText.text = "0";
            }
        }
    }

    // void Inventory_OnItemAmountChanged(bool isPlayer, ItemScriptableObject item, int amount)
    // {
        
    // }

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

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!customer)
        {
            _customerImage.enabled = false;
            return;
        }

        _customerImage.sprite = customer.Sprite;
        _customerImage.enabled = true;
        // TODO Set a text field based on a string supplied by customer
    }
}
