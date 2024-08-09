using UnityEngine;

public class BalanceScale : MonoBehaviour
{
    [SerializeField] float _minYHeight, _maxYHeight, _minZRotation, _maxZRotation;

    [SerializeField] Transform _compPan, _playerPan, _beam;

    int _basePrice, _offerValue, _compValue, _playerValue;

    Vector2 _compStartPosition, _playerStartPosition;
    Quaternion _beamStartRotation;

    Customer.Type _customerType;

    void Start()
    {
        _compStartPosition = _compPan.localPosition;
        _playerStartPosition = _playerPan.localPosition;
        _beamStartRotation = _beam.localRotation;
    }

    void OnEnable()
    {
        TradingSystem.OnBasePriceSet += TradingSystem_OnBasePriceSet;
        TradingSystem.OnOfferValueChanged += TradingSystem_OnOfferValueChanged;
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
    }

    void OnDisable()
    {
        TradingSystem.OnBasePriceSet -= TradingSystem_OnBasePriceSet;
        TradingSystem.OnOfferValueChanged -= TradingSystem_OnOfferValueChanged;
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
    }

    void TradingSystem_OnBasePriceSet(int basePrice, Customer.Type customerType)
    {
        _basePrice = basePrice;
        _customerType = customerType;
    }

    void TradingSystem_OnOfferValueChanged(int offerValue)
    {
        _offerValue = offerValue;

        if(_customerType == Customer.Type.Buy)
        {
            CalculateOffer(_offerValue, _basePrice);
        }
        else if(_customerType == Customer.Type.Sell)
        {
            CalculateOffer(_basePrice, _offerValue);
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
        if(_customerType == Customer.Type.Buy)
        {
            CalculateOffer(_offerValue, _basePrice);
            return;
        }
        if(_customerType == Customer.Type.Sell)
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

            compHeight = offerNormalized * _minYHeight;
            playerHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _maxZRotation;
        }
        else
        {
            if(_playerValue == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)_compValue / _playerValue);
            playerHeight = offerNormalized * _minYHeight;
            compHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _minZRotation;
        }

        AdjustScales(compHeight, playerHeight, beamRotation);
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

            compHeight = offerNormalized * _minYHeight;
            playerHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _maxZRotation;
        }
        else
        {
            if(offerValue == 0) { return; } // No divide by zero

            float offerNormalized = 1 - ((float)baseprice / offerValue);
            playerHeight = offerNormalized * _minYHeight;
            compHeight = offerNormalized * _maxYHeight;
            beamRotation = offerNormalized * _minZRotation;
        }

        AdjustScales(compHeight, playerHeight, beamRotation);
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
    }
}
