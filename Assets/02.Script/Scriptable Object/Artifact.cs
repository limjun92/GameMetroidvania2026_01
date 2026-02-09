using UnityEngine;

[CreateAssetMenu(menuName = "Artifact")]
public class Artifact : ScriptableObject
{
    public ArtifactType type;
    public string Name;
    public Sprite sprite;
}

public enum ArtifactType
{
    DoubleJump,
    Dash,
    WallJump
}
