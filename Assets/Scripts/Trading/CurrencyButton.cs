using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrencyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] DropBox _coinBox;
    [SerializeField] Button _increaseButton, _decreaseButton;
    [SerializeField] Currency _currency;

    [SerializeField] int _sendAmount;

    bool _isOver, _isCustomer;

    void OnEnable()
    {
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
    }

    void OnDisable()
    {
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        _isOver = false;
    }

    void Update()
    {
        if(!_isCustomer) { return; }
        if(!_isOver) { return; }

        if(Input.mouseScrollDelta.y > 0)
        {
            if(!_increaseButton) { return; }

            _increaseButton.onClick.Invoke();
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            if(_decreaseButton && !_coinBox)
            {
                _decreaseButton.onClick.Invoke();
            }
            else if(_coinBox)
            {
                _coinBox.RetrieveItem(_currency);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(_coinBox)
            {
                _coinBox.SendXChildItems(transform, _sendAmount);
            }
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

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(!customer)
        {
            _isCustomer = false;
            return;
        }

        if(customer.CustomerType == Customer.Type.None)
        {
            _isCustomer = false;
        }
        else
        {
            _isCustomer = true;
        }

    }
}
