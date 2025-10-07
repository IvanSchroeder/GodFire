using UnityEngine;

public class Pickup : MonoBehaviour {
    public PowerUp PowerUp;

    void OnTriggerEnter(Collider other) {
        var visitable = other.GetComponent<IVisitable>();

        if (visitable != null) {
            visitable.Accept(PowerUp);
            Destroy(gameObject);
        }
    }
}

// public interface IVisitor<T> where T : IVisitable {
//     void Visit(T Component);
// }

// public class MyAdvancedVisitor : MonoBehaviour, IVisitor {
//     private Dictionary<Type, object> Visitors = new();

//     public void RegiserVisitor<T>(IVisitor<T> visitor) {
//         Visitors[typeof(T)] = visitor;
//     }

//     public void Visit<T>(T visitable) where T : Component {
//         // this is a bit naive, you could easily add support for inheritance
//         // e.g. a IVisitor<BaseType> should be callable with a DerivedType
//         if (!Visitors.TryGetValue(typeof(T), out var boxedVisitor)) {
//             return;
//         }

//         if (!(boxedVisitor is IVisitor<T> concreteVisitor)) {
//             // or throw or log error whatever
//             return;
//         }

//         concreteVisitor.Visit(visitable);
//     }
// }