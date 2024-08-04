using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;

    [SerializeField] Transform _copperParent, _silverParent, _goldParent, _platinumParent;

    [SerializeField] bool _playerProperty, _isTradeBox, _isCoinBox;
    [SerializeField] DropBox _tradeBox, _coinBox, _inventoryBox, _partnerBox, _partnerCoinBox;

    public DropBox TradeBox => _tradeBox;
    public DropBox CoinBox => _coinBox;
    public DropBox InventoryBox => _inventoryBox;
    public Transform CopperParent => _copperParent;
    public Transform SilverParent => _silverParent;
    public Transform GoldParent => _goldParent;
    public Transform PlatinumParent => _platinumParent;

    List<GameObject> _items = new();
    int _totalValue;

    void OnEnable()
    {
        TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
    }

    void OnDisable()
    {
        TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
    }

    void TradingSystem_OnTradeCompleted()
    {
        if(_isTradeBox)
        {
            foreach(var tradedItem in _items)
            {
                Item item = tradedItem.GetComponent<Item>();
                item.SetInTrade(false);
                item.SetPlayerProperty(!_playerProperty);
                if(item.ItemSO.Type == Type.Coin)
                {
                    item.SendToCoinBox(_partnerCoinBox);
                }
                else
                {
                    item.SendToDropBox(_partnerBox);
                }
            }
            _items.Clear();
            _totalValue = 0;
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(_isTradeBox)
        {
            foreach(var cancelledItem in _items)
            {
                Item item = cancelledItem.GetComponent<Item>();
                item.SetInTrade(false);
                if(item.ItemSO.Type == Type.Coin)
                {
                    item.SendToCoinBox(_coinBox);
                }
                else
                {
                    item.SendToDropBox(_inventoryBox);
                }
            }
            _items.Clear();
            _totalValue = 0;
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item droppedItem = eventData.pointerDrag.GetComponent<Item>();

        if(!droppedItem) { return; }

        if(_playerProperty != droppedItem.PlayerProperty) { return; }

        if(_isTradeBox)
        {
            droppedItem.SetCurrentBox(this);
            droppedItem.SetInTrade(true);
            return;
        }
    }

    public void AddItem(GameObject item)
    {
        _items.Add(item);
        ItemScriptableObject itemSO = item.GetComponent<Item>().ItemSO;
        _totalValue += itemSO.BaseValue;

        if(_isTradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }

    public void RemoveItem(GameObject item)
    {
        if(_items.Contains(item))
        {
            _items.Remove(item);
            ItemScriptableObject itemSO = item.GetComponent<Item>().ItemSO;
            _totalValue -= itemSO.BaseValue;
        }

        if(_isTradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }
}
