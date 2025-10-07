public interface IDestroyable {
    float MaxHealth { get; set; }
    
    void UpdateHealth(float health);
    void OnDestroyed();
}