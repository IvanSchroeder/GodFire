using UnityEngine;

public interface IBurnable {
    BurnableTrigger BurnableTrigger { get; set; }
    ParticleSystem BurnEffect { get; set; }
    ParticleSystem SparkEffect { get; set; }

    bool IsBurning { get; set; }
    bool IsBurntOut { get; set; }
    bool BurnsInstantly { get; set; }
    float BurnHealth { get; set; }
    float BurnAmount { get; set; }
    float BurnRate { get; set; }
    float AdditionalFuelAmount { get; set; }

    public float GetFuelAmount();
    public void StartBurn(float deltaTime);
    public void EndBurn();
    public void CompleteBurning();
}
