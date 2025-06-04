public interface IDestroyable {
    int CurrentHealth { get; set; }
    int MaxHealth { get; set; }
    
    void UpdateHealth(int health);
    void OnDestroyed();
}