using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinRetrieve : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] DropBox _tradeBox;

    bool _isOver;

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(!_isOver) { return; }
            
            if(!_tradeBox) { return; }

            _tradeBox.RetrieveCoinStack(transform);
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
