using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [field:SerializeField] public bool PlayerProperty { get; private set; }

    [SerializeField] ItemScriptableObject _itemSO;

    [SerializeField] Image _itemImage, _raycastImage;
    [SerializeField] Color _draggingColor, _defaultColor;
    
    int _baseValue;

    Vector2 _endPositon;
    DropBox _currentBox;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _endPositon = transform.position;
        _raycastImage.raycastTarget = false;
        _raycastImage.color = _draggingColor;
        _currentBox.RemoveValue(_baseValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = _endPositon;
        _raycastImage.raycastTarget = true;
        _raycastImage.color = _defaultColor;
    }

    public void SetUp(ItemScriptableObject itemSO, bool playerProperty)
    {
        _itemSO = itemSO;
        _baseValue = _itemSO.BaseValue;
        _itemImage.sprite = _itemSO.ItemSprite;
        SetPlayerProperty(playerProperty);
    }

    public void SetEndPosition(DropBox newBox, Vector2 newPosition)
    {
        _endPositon = newPosition;
        _currentBox = newBox;
        _currentBox.AddValue(_baseValue);
    }

    private void SetPlayerProperty(bool playerProperty)
    {
        PlayerProperty = playerProperty;
    }
}
