using UnityEngine;
using System;
using System.Collections.Generic;

public class TradingSystem : MonoBehaviour
{
    public static Action<Dictionary<ItemScriptableObject, int>, Dictionary<Currency, int>> OnTradeCompleted; 

}
