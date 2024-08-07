using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;

    [field:SerializeField] public string Name { get; private set; } = string.Empty;
    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField][field:Range(0, 1f)] public float Tolerance { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }
    
    bool _isActiveCustomer;

    public enum Type
    {
        None,
        Buy,
        Sell,
        Barter
    }

    void OnEnable()
    {
        TradingSystem.OnNewCustomer += TradingSystem_OnNewCustomer;
        DropBox.OnBuyPriceSet += DropBox_OnBuyPriceSet;
    }

    void OnDisable()
    {
        TradingSystem.OnNewCustomer -= TradingSystem_OnNewCustomer;
        DropBox.OnBuyPriceSet -= DropBox_OnBuyPriceSet;
    }

    void TradingSystem_OnNewCustomer(Customer customer)
    {
        if(customer == this)
        {
            _isActiveCustomer = true;
            _inventory.ShowInventory(customer.CustomerType);
        }
        else
        {
            _isActiveCustomer = false;
        }
    }

    void DropBox_OnBuyPriceSet(int cost)
    {
        if(!_isActiveCustomer) { return; }
        
        _inventory.CoinBox.Pay(cost); // Note: It SHOULD NOT be possible for the price to be higher than the customer's funds due to check in TradingSystem
    }

    public int GetTotalFunds()
    {
        return _inventory.CoinBox.GetTrueValue();
    }

    public void ReduceStrikes(int amount)
    {
        Strikes -= amount;

        // TODO VFX (probably adding pieces of an anger emoji)
    }
}
