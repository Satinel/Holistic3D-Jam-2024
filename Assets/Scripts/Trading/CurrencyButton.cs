using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrencyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button _increaseButton, _decreaseButton;

    bool _isOver;

    void OnDisable()
    {
        _isOver = false;
    }

    void Update()
    {
        if(_isOver && Input.mouseScrollDelta.y > 0)
        {
            _increaseButton.onClick.Invoke();
        }

        if(_isOver && Input.mouseScrollDelta.y < 0)
        {
            _decreaseButton.onClick.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isOver = false;
    }

}
