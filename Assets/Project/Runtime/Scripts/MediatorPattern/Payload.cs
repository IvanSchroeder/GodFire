using UnityEngine;

public abstract class Payload<TData> : IVisitor {
    public abstract TData Content { get; set; }
    public abstract void Visit<T>(T visitable) where T : Component, IVisitable;
}

public class MessagePayload : Payload<string> {
    public Agent Source { get; set; }
    public override string Content { get; set; }

    private MessagePayload() {}

    public override void Visit<T>(T visitable) {
        Debug.Log($"{visitable.name} recieved message from {Source.name}: {Content}");
        // Execute login on visitable here
    }

    public class Builder {
        MessagePayload payload = new MessagePayload();

        public Builder(Agent source) => payload.Source = source;

        public Builder WithContent(string content) {
            payload.Content = content;
            return this;
        }

        public MessagePayload Build() => payload;
    }
}
