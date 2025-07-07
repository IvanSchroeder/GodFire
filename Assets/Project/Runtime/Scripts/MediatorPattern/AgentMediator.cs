// using UnityServiceLocator;
using UnityEngine;

public class AgentMediator : Mediator<Agent> {
    // void Awake() => ServiceLocator.Global.Register(this as Mediator<Agent>);
    protected override bool MediatorConditionMet(Agent target) => target.Status == AgentStatus.Active;

    protected override void OnRegistered(Agent entity) {
        Debug.Log($"{entity.name} registered");
        Broadcast(entity, new MessagePayload.Builder(entity).WithContent("Registered").Build());
    }

    protected override void OnDeregistered(Agent entity) {
        Debug.Log($"{entity.name} Deregistered");
        Broadcast(entity, new MessagePayload.Builder(entity).WithContent("Deregistered").Build());
    }

    // Add additional logic here
}