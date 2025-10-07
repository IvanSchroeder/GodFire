using UnityEngine;

public class ManaComponent : MonoBehaviour, IVisitable {
    [SerializeField] int mana = 100;

    public void Accept(IVisitor visitor) {
        visitor.Visit(this);
        Debug.Log("ManaComponent.Accept");
    }

    public void AddMana(int manaAmount) {
        mana += manaAmount;
    }
}