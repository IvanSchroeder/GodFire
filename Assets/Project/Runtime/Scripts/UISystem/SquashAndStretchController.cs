using System;
using UnityEngine;
using UnityUtilities;
using DG.Tweening;

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
    [SerializeField] private bool playOnStart;

    [Space(5f)]
    [Header("Animation Settings")]
    [Space(2f)]
    [SerializeField] private SquashAndStretchSettings squashAndStretchSettings;
    [SerializeField] private bool resetAfterAnimation = true;

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

    private void Start() {
        if (playOnStart) {
            PlaySquashAndStretch();
        }
    }

    public void PlaySquashAndStretch() {
        if (looping && !canBeOverwritten) return;

        PunchToTarget();
    }

    public void PunchToTarget() {
        transformToAffect.DOComplete();
        transformToAffect.DOPunchScale(
            _initialScaleVector.MultiplyBy(new Vector3(
                affectX ? squashAndStretchSettings.targetScale : 1,
                affectY ? squashAndStretchSettings.targetScale : 1,
                affectZ ? squashAndStretchSettings.targetScale : 1
            )),
            squashAndStretchSettings.duration,
            squashAndStretchSettings.vibrato,
            squashAndStretchSettings.elasticity).SetEase(Ease.OutExpo);
    }

    public void ScaleToTarget() {
        transformToAffect.DOComplete();
        transformToAffect.DOScale(squashAndStretchSettings.targetScale, squashAndStretchSettings.duration).SetEase(Ease.OutExpo);
    }

    public void ResetToInitialScale() {
        transformToAffect.DOComplete();
        transformToAffect.DOScale(squashAndStretchSettings.initialScale, squashAndStretchSettings.duration).SetEase(Ease.OutExpo);
    }
}