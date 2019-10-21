using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem)), UpdateBefore(typeof(TransformSystemGroup))]
public class FollowControllerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref FollowControllerStruct camInfo, ref Translation camTransform, ref Rotation camRot) => 
        {
            Debug.Log("Start");

            float3 targetPos = new float3();
            float4 targetRot = new float4();

            Entities.ForEach((ref PhysicsControllerStruct playerInfo, ref Translation playerTransform, ref Rotation playerRot) =>
            {
                Debug.Log("Start2");
                targetPos = playerTransform.Value;
                targetRot = playerRot.Value.value;
            });


            camTransform.Value = targetPos;
            camTransform.Value.y += .5f;
            camRot.Value.value.x = targetRot.x;
            camRot.Value.value.y = targetRot.y;
            camRot.Value.value.z = targetRot.z;
            camRot.Value.value.w = targetRot.w;
        });
    }
}
