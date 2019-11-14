using Unity.Entities;

public struct PhysicsControllerStruct : IComponentData
{
    public uint playerID;
    public bool isGrounded;
    public float timeSinceLastJump;
    public int rbIndex;
}
