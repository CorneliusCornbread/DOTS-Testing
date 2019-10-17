using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public struct FollowControllerStruct : IComponentData
{
    public uint assignedPlayer;
}
