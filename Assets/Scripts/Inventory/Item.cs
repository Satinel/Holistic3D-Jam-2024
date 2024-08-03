using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [field:SerializeField] public bool PlayerProperty { get; private set; }

    [field:SerializeField] public ItemScriptableObject ItemSO { get; private set; }

    [SerializeField] Image _itemImage, _raycastImage;
    [SerializeField] Color _draggingColor, _defaultColor;
    
    int _baseValue;

    Vector2 _startPosition;
    Vector2 _endPositon;
    DropBox _currentBox;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _endPositon = transform.position;
        _raycastImage.raycastTarget = false;
        _raycastImage.color = _draggingColor;
        _currentBox.RemoveItem(gameObject);
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
        ItemSO = itemSO;
        _baseValue = ItemSO.BaseValue;
        _itemImage.sprite = ItemSO.ItemSprite;
        SetPlayerProperty(playerProperty);
        SetStartPosition();
    }
    
    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = _startPosition;
    }

    public void SetEndPosition(DropBox newBox, Vector2 newPosition)
    {
        _endPositon = newPosition;
        if(_currentBox != newBox)
        _currentBox = newBox;
        _currentBox.AddItem(gameObject);
    }

    public void SetPlayerProperty(bool playerProperty)
    {
        PlayerProperty = playerProperty;
    }
}
