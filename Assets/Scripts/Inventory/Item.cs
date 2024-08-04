using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler // TODO? Different logic for InputButton.Right
{
    public static Action<Item> OnAnyItemClicked; // TODO Subscribe to this and update some info tab/window/tooltip

    [field:SerializeField] public bool PlayerProperty { get; private set; }

    [field:SerializeField] public ItemScriptableObject ItemSO { get; private set; }

    [SerializeField] Image _itemImage, _raycastImage;
    [SerializeField] Color _selectedColor, _defaultColor;
    [SerializeField] Canvas _parentCanvas;
    
    Vector2 _startPosition;
    Vector2 _endPositon;
    DropBox _currentBox;

    void OnEnable()
    {
        OnAnyItemClicked += Item_OnAnyItemClicked;
    }

    void OnDisable()
    {
        OnAnyItemClicked += Item_OnAnyItemClicked;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        OnAnyItemClicked?.Invoke(this);

        _endPositon = transform.position;
        _raycastImage.raycastTarget = false;
        _currentBox.RemoveItem(gameObject);
        transform.SetParent(_parentCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        transform.position = _endPositon;
        _raycastImage.raycastTarget = true;
        transform.SetParent(_currentBox.transform, true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        OnAnyItemClicked?.Invoke(this);
        
        SendToDropBox(_currentBox.LinkedBox);
    }

    public void SetUp(ItemScriptableObject itemSO, bool playerProperty, DropBox dropBox)
    {
        ItemSO = itemSO;
        gameObject.name = ItemSO.ItemName;
        _itemImage.sprite = ItemSO.ItemSprite;
        SetPlayerProperty(playerProperty);
        SetStartPosition();
        _currentBox = dropBox;
        _currentBox.AddItem(gameObject);
        _parentCanvas = GetComponentInParent<Canvas>();
    }
    
    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    public void SetEndPosition(DropBox newBox, Vector2 newPosition)
    {
        _endPositon = newPosition;
        _currentBox = newBox;
        _currentBox.AddItem(gameObject);
    }

    public void SendToDropBox(DropBox inventoryBox)
    {
        _currentBox = inventoryBox;
        _currentBox.AddItem(gameObject);
        transform.position = _currentBox.transform.position;
        transform.SetParent(_currentBox.transform, true);
        SetStartPosition();
    }

    public void ResetPosition(DropBox inventoryBox)
    {
        _currentBox = inventoryBox;
        _currentBox.AddItem(gameObject);
        transform.position = _startPosition;
        transform.SetParent(_currentBox.transform, true);
        SetStartPosition();
    }

    public void SetPlayerProperty(bool playerProperty)
    {
        PlayerProperty = playerProperty;
    }

    void Item_OnAnyItemClicked(Item selectedItem)
    {
        if(selectedItem != this)
        {
            _raycastImage.color = _defaultColor;
        }
        else
        {
            _raycastImage.color = _selectedColor;
        }
    }
}
