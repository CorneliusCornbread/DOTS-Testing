using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem)), UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
public class FollowControllerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref FollowControllerStruct camInfo, ref Translation camTransform, ref Rotation camRot, ref InputStruct input) => 
        {
            float3 targetPos = new float3();
            float4 targetRot = new float4();

            Entities.ForEach((ref PhysicsControllerStruct playerInfo, ref Translation playerTransform, ref Rotation playerRot) =>
            {
                targetPos = playerTransform.Value;
                targetRot = playerRot.Value.value;
            });

            targetPos.y += .5f;

            camTransform.Value = targetPos;

            camInfo.verticalRot -= input.mouseY * 2;// * Time.deltaTime;
            camInfo.verticalRot = Mathf.Clamp(camInfo.verticalRot, -90, 90); //Clamps the camera so you can't turn into an owl and look all the way up and behind you

            camInfo.targetEuler.y = FromQ(targetRot).y;
            camInfo.targetEuler.z = 0;
            camInfo.targetEuler.x = camInfo.verticalRot;

            camRot.Value = ToQ(camInfo.targetEuler);
        });
    }

    //Taken from "Vlad" on stack overflow: https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
    /// <summary>
    /// Converts euler angles to quaternion
    /// </summary>
    /// <param name="v">Euler</param>
    /// <returns></returns>
    public static float4 ToQ(float3 v)
    {
        return ToQ(v.y, v.x, v.z);
    }

    //Taken from "Vlad" on stack overflow: https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
    /// <summary>
    /// Converts euler angles to quaternion
    /// </summary>
    /// <param name="yaw">y</param>
    /// <param name="pitch">x</param>
    /// <param name="roll">z</param>
    /// <returns></returns>
    public static float4 ToQ(float yaw, float pitch, float roll)
    {
        yaw *= Mathf.Deg2Rad;
        pitch *= Mathf.Deg2Rad;
        roll *= Mathf.Deg2Rad;
        float rollOver2 = roll * 0.5f;
        float sinRollOver2 = (float)System.Math.Sin(rollOver2);
        float cosRollOver2 = (float)System.Math.Cos(rollOver2);
        float pitchOver2 = pitch * 0.5f;
        float sinPitchOver2 = (float)System.Math.Sin(pitchOver2);
        float cosPitchOver2 = (float)System.Math.Cos(pitchOver2);
        float yawOver2 = yaw * 0.5f;
        float sinYawOver2 = (float)System.Math.Sin(yawOver2);
        float cosYawOver2 = (float)System.Math.Cos(yawOver2);
        float4 result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }

    //Taken from "Vlad" on stack overflow: https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
    /// <summary>
    /// Converts quaternion to euler angles
    /// </summary>
    /// <param name="q2">Quaternion</param>
    /// <returns></returns>
    public static float3 FromQ(float4 q2)
    {
        float4 q = new float4(q2.w, q2.z, q2.x, q2.y);
        float3 pitchYawRoll;
        pitchYawRoll.y = (float)System.Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
        pitchYawRoll.x = (float)System.Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
        pitchYawRoll.z = (float)System.Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
        return new float3(pitchYawRoll.x * Mathf.Rad2Deg, pitchYawRoll.y * Mathf.Rad2Deg, pitchYawRoll.z * Mathf.Rad2Deg);
    }
}
