using UnityEngine;
// Example Visitable:

public class HealthComponent : MonoBehaviour, IVisitable {
    [SerializeField] int health = 100;

    public void Accept(IVisitor visitor) {
        visitor.Visit(this);
        Debug.Log("HealthComponent.Accept");
    }

    public void AddHealth(int healthAmount) {
        health += healthAmount;
    }
}
