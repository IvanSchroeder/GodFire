using System;
using UnityEngine;

public class Agent : MonoBehaviour, IVisitable {
    // IGoapMultithread goapSystem;
    Mediator<Agent> mediator;
    // AgentMotor motor;

    public AgentStatus Status { get; set; } = AgentStatus.Active;

    void Start() {
        // goapSystem = ServiceLocator.For(this).Get<GoapFactory<Agent>>().CreateMultithread(this);
        // mediator = ServiceLocalt.For(this).Get<Mediator<Agent>>();
        // motor = GetComponent<AgentMotor>();

        mediator.Register(this);
    }

    void OnDestroy() => mediator.Deregister(this);

    void Update() {
        // motor.Move(goapSystem.Update(Time.deltaTime));
    }

    void Send(IVisitor message) => mediator.Broadcast(this, message);
    void Send(IVisitor message, Func<Agent, bool> predicate) => mediator.Broadcast(this, message, predicate);

    const float k_radius = 5.0f;
    Func<Agent, bool> IsNearby => target => Vector3.Distance(transform.position, target.transform.position) <= k_radius;

    public void Accept(IVisitor message) => message.Visit(this);

    // public void PlanActions(AgentAction goal) => goapSystem.PlanActions(goal);
}

public enum AgentStatus {
    Active,
    Rest
}
