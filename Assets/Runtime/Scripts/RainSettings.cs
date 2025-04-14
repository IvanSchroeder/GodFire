using System;
using UnityEngine;

namespace WorldTime {
    [CreateAssetMenu(fileName = "NewRainSettings", menuName = "Assets/Time Settings/Rain")]
    public class RainSettings : ScriptableObject {
        public string rainTypeName = "Rain";
        public string rainStartLogMessage = $"It started raining!";
        public string rainStopLogMessage = $"Raining stopped...";
        public AnimationCurve rainTimeRangeCurve;
        public AnimationCurve rainAmountCurve;
        public AnimationCurve rainSpeedCurve;
        public float baseRainTimeHours = 18;
        public float baseRainSpeed = 2;
        public float baseRainAmount = 100;
        public Color weatherColor;
        public AudioClip audioClip;
        [Range(1f, 10f)] public float rainChanceCutoff = 2f;
        [Range(1f, 5f)] public float fuelConsuptionMultiplier = 2f;
    }
}
