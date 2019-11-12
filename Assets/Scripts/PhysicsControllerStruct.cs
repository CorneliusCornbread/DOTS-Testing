using Unity.Entities;

public struct PhysicsControllerStruct : IComponentData
{
    public uint playerID;
    public bool isGrounded;
    public int rbIndex;
}
