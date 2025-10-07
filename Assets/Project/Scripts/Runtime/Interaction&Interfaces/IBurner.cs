using UnityEngine;

public interface IBurner {
    BurnerTrigger BurnerTrigger { get; set; }
    void OnItemEntered<T>(Item item) where T : IBurnable;
    void OnItemBurn();
    void OnItemExited<T>(Item item) where T : IBurnable;
}
