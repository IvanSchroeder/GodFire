using System;
using UnityUtilities;
using UnityEngine;

namespace UnityUtilities {
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; set; }
        public bool IsRunning { get; protected set; }

        protected float initialTime;
        
        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0, 1);
        
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        public Action OnTimerResume = delegate { };
        public Action OnTimerPause = delegate { };

        public TimerManager ControllingTimerManager;

        protected Timer(float value, ref TimerManager timerManager) {
            initialTime = value;
            IsRunning = false;
            ControllingTimerManager = timerManager;
        }

        public void Start() {
            CurrentTime = initialTime;
            if (!IsRunning) {
                IsRunning = true;
                ControllingTimerManager?.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning) {
                IsRunning = false;
                ControllingTimerManager?.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }
        
        // public abstract void Tick(float deltaTime);
        public abstract void Tick(float deltaTime);
        public abstract bool IsFinished { get; }

        public float GetTime() => CurrentTime;
        
        public void Resume() {
            IsRunning = true;
            OnTimerResume?.Invoke();
        }

        public void Pause() {
            IsRunning = false;
            OnTimerPause?.Invoke();
        }

        public virtual void Reset() {
            Pause();
            CurrentTime = initialTime;
        }
        public virtual void Reset(float newTime) {
            initialTime = newTime;
            Reset();
        }
        
        public virtual void Restart() {
            Reset();
            Start();
        }
        public virtual void Restart(float newTime) {
            Reset(newTime);
            Start();
        }

        public virtual void AddTime(float timeToAdd) => CurrentTime += timeToAdd;
        public virtual void RemoveTime(float timeToRemove) => CurrentTime -= timeToRemove;
        public virtual void MultiplyTime(float timeToMult) => CurrentTime *= timeToMult;
        public virtual void DivideTime(float timeToCut) => CurrentTime /= timeToCut;

        bool disposed;

        ~Timer() {
            Dispose(false);
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                ControllingTimerManager?.DeregisterTimer(this);
            }

            disposed = true;
        }
    }
    
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer {
        public override bool IsFinished => CurrentTime <= 0;

        public CountdownTimer(float value, ref TimerManager timerManager) : base(value, ref timerManager) { }

        public override void Tick(float deltaTime) {
            if (IsRunning && CurrentTime > 0) {
                RemoveTime(deltaTime);
            }
            
            if (IsRunning && CurrentTime <= 0) {
                Stop();
            }

            CurrentTime.Clamp(0f, initialTime);
        }
    }
    
    /// <summary>
    /// Timer that counts up from zero to infinity.  Great for measuring durations.
    /// </summary>
    public class StopwatchTimer : Timer {
        public StopwatchTimer(ref TimerManager timerManager) : base(0, ref timerManager) { }

        public override void Tick(float deltaTime) {
            if (IsRunning) {
                AddTime(deltaTime);
            }
        }
        
        public override bool IsFinished => throw new NotImplementedException();
        
        public override void Reset() {
            base.Reset(0);
        }
    }

    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : Timer {
        public int TicksPerSecond { get; private set; }

        public Action OnTick = delegate { };
        
        float timeThreshold;

        public FrequencyTimer(int ticksPerSecond, ref TimerManager timerManager) : base(0, ref timerManager) {
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick(float deltaTime) {
            if (IsRunning && CurrentTime >= timeThreshold) {
                RemoveTime(timeThreshold);
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < timeThreshold) {
                AddTime(deltaTime);
            }
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset() {
            CurrentTime = 0;
        }
        
        public void Reset(int newTicksPerSecond) {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }
        
        void CalculateTimeThreshold(int ticksPerSecond) {
            TicksPerSecond = ticksPerSecond;
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}

