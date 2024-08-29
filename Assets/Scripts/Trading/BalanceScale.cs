using UnityEngine;

public class BalanceScale : MonoBehaviour
{
    [SerializeField] float _minYHeight, _maxYHeight, _minZRotation, _maxZRotation;

    [SerializeField] Transform _compPan, _playerPan, _beam;
    [SerializeField] GameObject _panWarning, _acceptMarker, _rejectMarker, _acceptIcon, _rejectIcon;

    // [SerializeField] bool _useTolerance; // For the time being I'd rather use the Marker system instead of this

    int _basePrice, _offerValue, _compValue, _playerValue;

    Vector2 _compStartPosition, _playerStartPosition;
    Quaternion _beamStartRotation;

    Customer _customer;

    void Start()
    {
        _compStartPosition = _compPan.localPosition;
        _playerStartPosition = _playerPan.localPosition;
        _beamStartRotation = _beam.localRotation;
    }

    void OnEnable()
    {
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        TradingSystem.OnBasePriceSet += TradingSystem_OnBasePriceSet;
        TradingSystem.OnOfferValueChanged += TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled += TradingSystem_OnTradeCancelled;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
    }

    void OnDisable()
    {
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        TradingSystem.OnBasePriceSet -= TradingSystem_OnBasePriceSet;
        TradingSystem.OnOfferValueChanged -= TradingSystem_OnOfferValueChanged;
        TradingSystem.OnTradeCancelled -= TradingSystem_OnTradeCancelled;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        _panWarning.SetActive(false);
        _acceptMarker.SetActive(false);
        _rejectMarker.SetActive(false);
        _acceptMarker.transform.localPosition = Vector2.zero;
        _rejectMarker.transform.localPosition = Vector2.zero;
        if(customer == null)
        {
            _customer = null;
        }

        _customer = customer;
    }

    void TradingSystem_OnBasePriceSet(int basePrice)
    {
        _basePrice = basePrice;
    }

    void TradingSystem_OnOfferValueChanged(int offerValue)
    {
        _offerValue = offerValue;

        if(!_customer) { return; }

        if(_customer.CustomerType == Customer.Type.Buy)
        {
            CalculateOffer(_offerValue, _basePrice);
        }
        else if(_customer.CustomerType == Customer.Type.Sell)
        {
            CalculateOffer(_basePrice, _offerValue);
        }
    }

    void TradingSystem_OnTradeCancelled()
    {
        _acceptMarker.SetActive(false);
        _rejectMarker.SetActive(false);
        _rejectIcon.SetActive(false);
        _acceptIcon.SetActive(false);
    }

    void TradingSystem_OnOfferRejected()
    {
        if(!_rejectMarker.activeSelf)
        {
            _rejectMarker.SetActive(true);
            _rejectMarker.transform.localPosition = new(0, _playerPan.localPosition.y);
            return;
        }

        if(_playerPan.localPosition.y < _rejectMarker.transform.localPosition.y)
        {
            _rejectMarker.transform.localPosition = new(0, _playerPan.localPosition.y);
        }
        
    }

    void TradingSystem_OnOfferAccepted(bool isBuying, int offer)
    {
        if(!_acceptMarker.activeSelf)
        {
            _acceptMarker.SetActive(true);
            _acceptMarker.transform.localPosition = new(0, _playerPan.localPosition.y);
            return;
        }

        if(_playerPan.localPosition.y > _acceptMarker.transform.localPosition.y)
        {
            _acceptMarker.transform.localPosition = new(0, _playerPan.localPosition.y);
        }
    }

    void DropBox_OnTradeBoxValueChanged(bool isPlayer, int value)
    {
        if(isPlayer)
        {
            _playerValue = value;
        }
        else
        {
            _compValue = value;
        }

        CalculateWeights();
    }

    public void OfferWindowOpened() // UI Button
    {
        if(_customer.CustomerType == Customer.Type.Buy)
        {
            CalculateOffer(_offerValue, _basePrice);
            return;
        }
        if(_customer.CustomerType == Customer.Type.Sell)
        {
            CalculateOffer(_basePrice, _offerValue);
            return;
        }
    }

    public void CalculateWeights() // UI Button
    {
        int offer = _compValue - _playerValue;
        
        if(offer == 0)
        {
            RestoreStartValues();
            return;
        }

        float compHeight;
        float playerHeight;
        float beamRotation;
        
        if(offer > 0)
        {
            if(_compValue == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)_playerValue / _compValue);

            // if(_useTolerance && offerNormalized <= _customer.Tolerance)
            // {
            //     RestoreStartValues();
            //     return;
            // }

            compHeight = offerNormalized * _minYHeight;
            playerHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _maxZRotation;
        }
        else
        {
            if(_playerValue == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)_compValue / _playerValue);
            
            // if(_useTolerance && offerNormalized <= _customer.Tolerance)
            // {
            //     RestoreStartValues();
            //     return;
            // }

            playerHeight = offerNormalized * _minYHeight;
            compHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _minZRotation;
        }

        AdjustScales(compHeight, playerHeight, beamRotation);
        CheckWarnings(offer);
    }

    void CalculateOffer(int baseprice, int offerValue) // SOMEDAY: Figure out why this still works when the if checks return early, also how did I manage to create this?
    {
        int offer = baseprice - offerValue;
        
        if(offer == 0)
        {
            RestoreStartValues();
            return;
        }

        float compHeight;
        float playerHeight;
        float beamRotation;
        
        if(offer > 0)
        {
            if(baseprice == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)offerValue / baseprice);
            // if(_useTolerance && offerNormalized <= _customer.Tolerance)
            // {
            //     RestoreStartValues();
            //     return;
            // }

            compHeight = offerNormalized * _minYHeight;
            playerHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _maxZRotation;
        }
        else
        {
            if(offerValue == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)baseprice / offerValue);

            // if(_useTolerance && offerNormalized <= _customer.Tolerance)
            // {
            //     RestoreStartValues();
            //     return;
            // }
            
            playerHeight = offerNormalized * _minYHeight;
            compHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _minZRotation;
        }

        AdjustScales(compHeight, playerHeight, beamRotation);
        CheckWarnings(offer);
    }

    void AdjustScales(float compHeight, float playerHeight, float beamRotation)
    {
        _playerPan.localPosition = new(_playerStartPosition.x, playerHeight);
        _compPan.localPosition = new(_compStartPosition.x, compHeight);
        _beam.localRotation = Quaternion.Euler(_beamStartRotation.x, _beamStartRotation.y, beamRotation);
    }

    void RestoreStartValues()
    {
        _compPan.localPosition = _compStartPosition;
        _playerPan.localPosition = _playerStartPosition;
        _beam.localRotation = _beamStartRotation;
        _panWarning.SetActive(false);
    }

    void CheckWarnings(int offer)
    {
        _panWarning.SetActive(false);
        _rejectIcon.SetActive(false);
        _acceptIcon.SetActive(false);

        if(!_customer) { return; }
        if(_customer.CustomerType == Customer.Type.Barter) { return; } // TODO (someday) only skip warning when barter not locked or something

        if(offer < 0)
        {
            _panWarning.SetActive(true);
            return;
        }

        if(_rejectMarker.activeSelf && _playerPan.localPosition.y >= _rejectMarker.transform.localPosition.y)
        {
            _rejectIcon.SetActive(true);
            return;
        }
        if(_acceptMarker.activeSelf && _playerPan.localPosition.y <= _acceptMarker.transform.localPosition.y)
        {
            _acceptIcon.SetActive(true);
            return;
        }
    }
}
