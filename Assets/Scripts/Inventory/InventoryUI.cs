using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _copperText, _silverText, _goldText, _platinumText;
    [SerializeField] TextMeshProUGUI _compCopperText, _compSilverText, _compGoldText, _compPlatinumText;

    void OnEnable()
    {
        Inventory.OnItemAmountChanged += Inventory_OnItemAmountChanged;
        Inventory.OnMoneyAmountChanged += Inventory_OnMoneyAmountChanged;
    }

    void OnDisable()
    {
        Inventory.OnItemAmountChanged -= Inventory_OnItemAmountChanged;
        Inventory.OnMoneyAmountChanged -= Inventory_OnMoneyAmountChanged;
    }

    void Inventory_OnMoneyAmountChanged(bool isPlayer, Dictionary<Currency, int> money)
    {
        if(isPlayer)
        {
            if(money.ContainsKey(Currency.Copper))
            {
                _copperText.text = money[Currency.Copper].ToString("N0");
            }
            if(money.ContainsKey(Currency.Silver))
            {
                _silverText.text = money[Currency.Silver].ToString("N0");
            }
            if(money.ContainsKey(Currency.Gold))
            {
                _goldText.text = money[Currency.Gold].ToString("N0");
            }
            if(money.ContainsKey(Currency.Platinum))
            {
                _platinumText.text = money[Currency.Platinum].ToString("N0");
            }
        }
        else
        {
            if(money.ContainsKey(Currency.Copper))
            {
                _compCopperText.text = money[Currency.Copper].ToString("N0");
            }
            else
            {
                _compCopperText.text = "0";
            }
            if(money.ContainsKey(Currency.Silver))
            {
                _compSilverText.text = money[Currency.Silver].ToString("N0");
            }
            else
            {
                _compSilverText.text = "0";
            }
            if(money.ContainsKey(Currency.Gold))
            {
                _compGoldText.text = money[Currency.Gold].ToString("N0");
            }
            else
            {
                _compGoldText.text = "0";
            }
            if(money.ContainsKey(Currency.Platinum))
            {
                _compPlatinumText.text = money[Currency.Platinum].ToString("N0");
            }
            else
            {
                _compPlatinumText.text = "0";
            }
        }
    }

    void Inventory_OnItemAmountChanged(bool isPlayer, ItemScriptableObject item, int amount)
    {
        // TODO Add/Remove game objects to/from inventory
    }
}
