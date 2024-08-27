using UnityEngine;
using System;
using System.Collections.Generic;

public class Town : MonoBehaviour
{
    public static Action<Customer> OnNextCustomer;
    public static Action OnNoCustomers;
    public static Action<int> OnCustomerCountChange;

    List<Customer> _customers = new();

    void OnEnable()
    {
        TradingSystem.OnOpenToPublic += TradingSystem_OnOpenToPublic;
    }

    void OnDisable()
    {
        TradingSystem.OnOpenToPublic -= TradingSystem_OnOpenToPublic;
    }
    
    void Start()
    {
        foreach(Customer customer in GetComponentsInChildren<Customer>())
        {
            _customers.Add(customer);
        }
    }

    void TradingSystem_OnOpenToPublic()
    {
        if(!gameObject.activeSelf) { return; }

        if(_customers.Count > 0)
        {
            Customer nextCustomer = _customers[UnityEngine.Random.Range(0, _customers.Count)];

            OnNextCustomer?.Invoke(nextCustomer);
            _customers.Remove(nextCustomer);
            OnCustomerCountChange?.Invoke(_customers.Count);
        }
        else
        {
            OnNoCustomers?.Invoke();
        }
    }
}
