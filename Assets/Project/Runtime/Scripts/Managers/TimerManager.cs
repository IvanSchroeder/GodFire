using System.Collections.Generic;
using UnityUtilities;

public static class TimerManager {
    static readonly List<Timer> timersList = new();

    public static void RegisterTimer(Timer timer) => timersList.Add(timer);
    public static void DeregisterTimer(Timer timer) => timersList.Remove(timer);

    public static void UpdateTimers() {
        // foreach (var timer in new List<Timer>(timersList)) {
        //     timer.Tick();
        // }
    }

    public static void Clear() => timersList.Clear();
}
