using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject _openingSplash, _clickOverlay;
    [SerializeField] GameObject _speechWindow, _scalesParent;
    [SerializeField] GameObject _playerStockBox, _playerCoinBox, _playerTradeBox;
    [SerializeField] GameObject _compStockBox, _compCoinBox, _compTradeBox;
    [SerializeField] GameObject _buttonsParent;
    [SerializeField] TextMeshProUGUI _text, _button1Text, _button2Text, _button3Text;
    [SerializeField] TradingSystem _tradingSystem;
    [SerializeField] Customer[] _mentors;
    [SerializeField] GameObject _musicPlayer;
    
    Player _player;

    bool _acceptThePremise;

    void Awake()
    {
        _player = FindAnyObjectByType<Player>();
    }

    public void ClickToBegin() // UI Button
    {
        _clickOverlay.SetActive(false);
        _musicPlayer.SetActive(true);
        _buttonsParent.SetActive(true);
    }

    public void Button1()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
        }
    }

    public void Button2()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
        }
    }

    public void Button3()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
        }
    }

    public void AcceptThePremise()
    {
        _text.text = "Whatever your personal circumstances, ";
    }
}
