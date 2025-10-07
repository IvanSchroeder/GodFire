using System;
using UnityEngine;
using UnityUtilities;
using DG.Tweening;
using System.Collections.Generic;

public class SquashAndStretchController : MonoBehaviour {
    [Flags]
    public enum SquashStretchAxis {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("References")]
    [Space(2f)]
    [SerializeField] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField] private bool canBeOverwritten;

    [Space(5f)]
    [Header("Animation Settings")]
    [Space(2f)]
    // [SerializeField] private bool resetAfterAnimation = true;
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float resetScaleDuration = 0.5f;

    [Space(5f)]
    [Header("Looping Settings")]
    [Space(2f)]
    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Vector3 _initialScaleVector;
    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake() {
        if (transformToAffect.IsNull()) transformToAffect = transform;

        _initialScaleVector = transformToAffect.localScale;
    }

    public void PlaySquashAndStretch(SquashAndStretchSettings settings) {
        if (looping && !canBeOverwritten) return;

        switch (settings.animationType) {
            case SquashAndStretchSettings.AnimationType.Scale:
                ScaleToTarget(settings);
                break;
            case SquashAndStretchSettings.AnimationType.Punch:
                PunchToTarget(settings);
                break;
        }
    }

    public async void PunchToTarget(SquashAndStretchSettings settings) {
        ResetToInitialScale(true);

        await transformToAffect.DOScale(
            new Vector3(
                affectX ? settings.targetScale : 1,
                affectY ? settings.targetScale : 1,
                affectZ ? settings.targetScale : 1
            ),
            settings.duration).SetEase(Ease.Linear).IsComplete();
        await transformToAffect.DOScale(
            new Vector3(
                affectX ? initialScale : 1,
                affectY ? initialScale : 1,
                affectZ ? initialScale : 1
            ),
            settings.duration).SetEase(Ease.OutExpo).IsComplete();
    }

    public void ScaleToTarget(SquashAndStretchSettings settings) {
        ResetToInitialScale(true);
        transformToAffect.DOScale(settings.targetScale, settings.duration).SetEase(Ease.OutExpo);
    }

    public void ResetToInitialScale(bool instant = false) {
        transformToAffect.DOComplete();
        
        if (instant)
            transformToAffect.localScale = _initialScaleVector;
        else
            transformToAffect.DOScale(initialScale, resetScaleDuration).SetEase(Ease.OutExpo);
    }
}