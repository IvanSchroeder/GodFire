using System;
using ExtensionMethods;
using UnityEngine.AI;

namespace Utilities {
    public abstract class Timer {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; protected set; }
        
        public float Progress => Time / initialTime;
        
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) {
            initialTime = value;
            IsRunning = false;
        }

        public void Start() {
            Time = initialTime;
            if (!IsRunning) {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning) {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }
        
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Reset(bool pause = false);
        public void Restart() {
            Reset();
            Start();
        }
        
        public abstract void Tick(float deltaTime);

        public float GetTime() => Time;
    }
    
    public class CountdownTimer : Timer {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime) {
            if (IsRunning && Time > 0) {
                Time -= deltaTime;
                Time.Clamp(0f, initialTime);
            }
            
            if (IsRunning && Time <= 0) {
                Stop();
            }
        }
        
        public bool IsFinished => Time <= 0;
        
        public override void Reset(bool pause = false) {
            Time = initialTime;
            if (pause) Pause();
        }
        
        public void Reset(float newTime, bool pause = false) {
            initialTime = newTime;
            if (pause) Pause();
        }

        public void AddTime(float timeToAdd) => Time += timeToAdd;
        public void RemoveTime(float timeToRemove) => Time -= timeToRemove;
        public void MultiplyTime(float timeToMult) => Time *= timeToMult;
        public void DivideTime(float timeToCut) => Time /= timeToCut;
    }
    
    public class StopwatchTimer : Timer {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime) {
            if (IsRunning) {
                Time += deltaTime;
                Time.Clamp(0f, initialTime);
            }
        }
        
        public override void Reset(bool pause = false) {
            Time = 0;
            if (pause) Pause();
        }
    }
}

