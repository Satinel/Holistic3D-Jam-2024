using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoinSend : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] DropBox _coinBox;
    [SerializeField] Button _increaseButton;
    [SerializeField] Currency _currency;
    [SerializeField] AudioSource _audioSource;

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

            if(_audioSource)
            {
                _audioSource.Play();
            }
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            _coinBox.RetrieveItem(_currency);

            if(_audioSource)
            {
                _audioSource.Play();
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(_coinBox)
            {
                _coinBox.SendXChildItems(transform, _sendAmount);

                if(_audioSource)
                {
                    _audioSource.Play();
                }
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
