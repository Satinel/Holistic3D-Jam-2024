using UnityEngine;

public class BalanceScale : MonoBehaviour
{
    [SerializeField] float _minYHeight, _maxYHeight, _minZRotation, _maxZRotation;

    [SerializeField] Transform _compPan, _playerPan, _beam;

    int _compValue, _playerValue;

    Vector2 _compStartPosition, _playerStartPosition;
    Quaternion _beamStartRotation;

    void Start()
    {
        _compStartPosition = _compPan.localPosition;
        _playerStartPosition = _playerPan.localPosition;
        _beamStartRotation = _beam.localRotation;
    }

    void OnEnable()
    {
        DropBox.OnTradeBoxValueChanged += DropBox_OnTradeBoxValueChanged;
    }

    void OnDisable()
    {
        DropBox.OnTradeBoxValueChanged -= DropBox_OnTradeBoxValueChanged;
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

    void CalculateWeights() // SOMEDAY: Figure out why this still works when the if checks return early, also how did I manage to create this?
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
