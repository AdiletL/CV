using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerMove", menuName = "SO/PlayerMove", order = 51)]
public class SO_PlayerMove : SO_CharacterMove
{
    [field: SerializeField, Space(5), Header("AnimationClips")] 
    public AnimationClip RunClip { get; private set; }
    
    [field: SerializeField] public float RunSpeed { get; private set; }

    [field: SerializeField] public float RotateSpeed { get; private set; }
}
