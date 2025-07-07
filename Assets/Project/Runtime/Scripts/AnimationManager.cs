using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour {
    Animator animator;

    #region Animation Hashes and Durations

    static readonly int IdleHash = Animator.StringToHash("Entity_Idle");
    static readonly int WalkHash = Animator.StringToHash("Entity_Walk");
    static readonly int RunHash = Animator.StringToHash("Entity_Run");
    // etc...
    const float CROSSFADE_DURATION = 0.1f;

    readonly Dictionary<int, float> AnimationDurationDictionary = new() {
        { IdleHash, 0.53f },
        { WalkHash, 0.53f },
        { RunHash, 0.53f }
    };

    #endregion

    void Awake() => animator = GetComponent<Animator>();

    public float Idle() => PlayAnimation(IdleHash);
    public float Walk() => PlayAnimation(WalkHash);
    public float Run() => PlayAnimation(RunHash);

    float PlayAnimation(int animationHash) {
        animator.CrossFade(animationHash, CROSSFADE_DURATION);
        return AnimationDurationDictionary[animationHash];
    }
}
