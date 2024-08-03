using UnityEngine;
using UnityEngine.EventSystems;

public class DropBox : MonoBehaviour, IDropHandler
{
    [SerializeField] bool _playerProperty;

    int _totalValue;

    public void OnDrop(PointerEventData eventData)
    {
        Item droppedItem = eventData.pointerDrag.GetComponent<Item>();

        if(!droppedItem) { return; }
        if(_playerProperty != droppedItem.PlayerProperty) { return; }

        droppedItem.SetEndPosition(this, Input.mousePosition);
    }

    public void AddValue(int value)
    {
        _totalValue += value;
        Debug.Log(_totalValue);
    }

    public void RemoveValue(int value)
    {
        _totalValue -= value;
        Debug.Log(_totalValue);
    }
}
