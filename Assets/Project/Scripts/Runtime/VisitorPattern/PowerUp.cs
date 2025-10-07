using System;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "Assets/Data/Power Up")]
public class PowerUp : ScriptableObject, IVisitor {
    public int HealthBonus = 10;
    public int ManaBonus = 10;

    public void Visit(object o) {
        MethodInfo visitMethod = GetType().GetMethod("Visit", new Type[] { o.GetType() });

        if (visitMethod != null && visitMethod != GetType().GetMethod("Visit", new Type[] { typeof(object) })) {
            visitMethod.Invoke(this, new object[] { o });
        }else {
            DefaultVisit(o);
        }
    }

    void DefaultVisit(object o) {
        Debug.Log("PowerUp.DefaultVisit");
    }

    public void Visit(HealthComponent healthComponent) {
        // healthComponent.Health += HealthBonus;
        healthComponent.AddHealth(HealthBonus);
        Debug.Log("PowerUp.Visit(HealthComponent)");
    }

    public void Visit(ManaComponent manaComponent) {
        // manaComponent.Mana += ManaBonus;
        manaComponent.AddMana(ManaBonus);
        Debug.Log("PowerUp.Visit(ManaComponent)");
    }

    public void Visit<T>(T visitable) where T : Component, IVisitable {
        throw new NotImplementedException();
    }
}