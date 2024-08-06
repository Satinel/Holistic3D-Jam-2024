using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;

    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField][field:Range(0, 1f)] public float Tolerance { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }

    public int TotalFunds => _inventory.TotalFunds;
    
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
        
        if(cost > TotalFunds)
        {
            // TODO Handle customer not being able to afford item
        }
        else
        {
            _inventory.CoinBox.Pay(cost);
        }
    }

    public void ReduceStrikes(int amount)
    {
        Strikes -= amount;

        // TODO VFX (probably adding pieces of an anger emoji)
    }
}
