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

            camInfo.verticalRot -= Input.GetAxis("Mouse Y") * 2f * Time.deltaTime;
            camInfo.verticalRot = Mathf.Clamp(camInfo.verticalRot, -90, 90); //Clamps the camera so you can't turn into an owl and look all the way up and behind you
            camRot.Value.value = targetRot;

            //camRot.Value.value.y = camInfo.verticalRot;
        });
    }
}
