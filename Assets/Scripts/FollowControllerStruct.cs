using Unity.Entities;
using Unity.Mathematics;

public struct FollowControllerStruct : IComponentData
{
    public uint assignedPlayer;
    public float verticalRot;
    public float3 targetEuler;
}
