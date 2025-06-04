using System.Collections.Generic;
using UnityUtilities;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.ComponentModel;

[Serializable]
public class TimerManager {
    public List<Timer> TimersList = new();

    public void RegisterTimer(Timer timer) => TimersList?.Add(timer);
    public void DeregisterTimer(Timer timer) => TimersList?.Remove(timer);

    public void UpdateTimers(float deltaTime) {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Tick(deltaTime);
        }
    }

    public void StartAllTimers() {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Start();
        }
    }

    public void StopAllTimers() {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Stop();
        }
    }

    public void ResumeAllTimers() {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Resume();
        }
    }

    public void PauseAllTimers() {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Pause();
        }
    }

    public void DisposeTimers() {
        foreach (var timer in new List<Timer>(TimersList)) {
            timer.Dispose();
        }

        Clear();
    }

    public void Clear() => TimersList.Clear();
}
