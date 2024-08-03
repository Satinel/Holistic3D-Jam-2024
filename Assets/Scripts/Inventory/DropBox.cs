using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    public static Action<bool, int> OnTradeBoxValueChanged;

    [SerializeField] bool _playerProperty, _tradeBox;

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

    void TradingSystem_OnTradeCompleted(Dictionary<ItemScriptableObject, int> ignore, Dictionary<Currency, int> money)
    {
        if(_tradeBox)
        {
            foreach(var tradedItem in _items)
            {
                Item item = tradedItem.GetComponent<Item>();
                item.SetPlayerProperty(!_playerProperty);
                _totalValue -= item.ItemSO.BaseValue;
            }

            _items.Clear();
        }
    }

    void TradingSystem_OnTradeCancelled()
    {
        if(_tradeBox)
        {
            foreach(var cancelledItem in _items)
            {
                Item item = cancelledItem.GetComponent<Item>();
                item.ResetPosition();
                _totalValue -= item.ItemSO.BaseValue; // TODO Deal with adding values to (primarily the player's) inventory box
            }

            _items.Clear();
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
        }

        ItemScriptableObject itemSO = item.GetComponent<Item>().ItemSO;

        _totalValue -= itemSO.BaseValue;
        if(_tradeBox)
        {
            OnTradeBoxValueChanged?.Invoke(_playerProperty, _totalValue);
        }
    }
}
