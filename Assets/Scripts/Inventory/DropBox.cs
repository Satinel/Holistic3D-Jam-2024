using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;

    [SerializeField] bool _playerProperty, _tradeBox;
    [SerializeField] DropBox _linkedBox, _partnerBox;

    public DropBox LinkedBox => _linkedBox;

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
        if(_tradeBox)
        {
            foreach(var tradedItem in _items)
            {
                Item item = tradedItem.GetComponent<Item>();
                item.SetPlayerProperty(!_playerProperty);
                item.SendToDropBox(_partnerBox);
            }

            _items.Clear();
            _totalValue = 0;
        }
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(_tradeBox)
        {
            foreach(var cancelledItem in _items)
            {
                Item item = cancelledItem.GetComponent<Item>();
                item.ResetPosition(_linkedBox);
            }

            _items.Clear();
            _totalValue = 0;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item droppedItem = eventData.pointerDrag.GetComponent<Item>();

        if(!droppedItem) { return; }

        if(_playerProperty != droppedItem.PlayerProperty) { return; }

        droppedItem.SetEndPosition(this, Input.mousePosition);
        
        if(!_tradeBox)
        {
            droppedItem.SetStartPosition();
        }
    }

    public void AddItem(GameObject item)
    {
        _items.Add(item);
        ItemScriptableObject itemSO = item.GetComponent<Item>().ItemSO;
        _totalValue += itemSO.BaseValue;

        if(_tradeBox)
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

        if(_tradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }
}
