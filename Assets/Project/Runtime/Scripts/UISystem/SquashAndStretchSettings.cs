using UnityEngine;

[CreateAssetMenu(fileName = "SquashAndStretchSettings", menuName = "Data/Settings/Squash and Stretch")]
public class SquashAndStretchSettings : ScriptableObject {
    public float duration = 0.25f;
    public float initialScale = 1f;
    public float targetScale = 1.3f;
    public int vibrato = 10;
    public float elasticity = 1;
}
