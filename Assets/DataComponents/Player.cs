using Unity.Entities;
[GenerateAuthoringComponent]
public struct Player : IComponentData {
    public bool isGrounded;
    public int jumpStrength;
    public float movementSpeed;
}