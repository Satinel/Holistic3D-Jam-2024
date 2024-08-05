using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Inventory _inventory;

    [field:SerializeField] public Type CustomerType { get; private set; }
    [field:SerializeField] public int Tolerance { get; private set; }
    [field:SerializeField] public int Strikes { get; private set; }

    public int TotalFunds => _inventory.TotalFunds;

    public enum Type
    {
        None,
        Buy,
        Sell,
        Barter
    }

    public void ReduceStrikes(int amount)
    {
        Strikes -= amount;

        // TODO VFX (probably adding pieces of an anger emoji)

        if(Strikes <= 0)
        {
            // TODO Customer stops trading
        }
    }
}
