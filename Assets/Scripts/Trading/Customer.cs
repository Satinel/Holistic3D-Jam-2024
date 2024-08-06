using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;

    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField][field:Range(0, 1f)] public float Tolerance { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }

    public int TotalFunds => _inventory.TotalFunds;

    public enum Type
    {
        None,
        Buy,
        Sell,
        Barter
    }

    void OnEnable()
    {
        Town.OnNextCustomer += Town_OnNextCustomer;
    }

    void OnDisable()
    {
        Town.OnNextCustomer -= Town_OnNextCustomer;
    }

    void Town_OnNextCustomer(Customer customer)
    {
        if(customer == this)
        {
            _inventory.ShowInventory(customer.CustomerType);
        }
    }

    public void ReduceStrikes(int amount)
    {
        Strikes -= amount;

        // TODO VFX (probably adding pieces of an anger emoji)
    }
}
