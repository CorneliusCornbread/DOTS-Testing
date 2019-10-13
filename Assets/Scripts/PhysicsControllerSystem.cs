using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class PhysicsControllerSystem : ComponentSystem
{
    public struct MoveJob : IJobForEach<PhysicsVelocity, InputStruct, PhysicsControllerStruct>
    {
        public float deltaTime;

        public void Execute(ref PhysicsVelocity rb, ref InputStruct input, ref PhysicsControllerStruct playerData)
        {
            Vector3 targetVelocity = new Vector3();
            float speed = 1000;
            float gravity = 2;
            float mass = 3;

            float maxVelocityChange = 10;

            targetVelocity.z = input.horizontal;
            targetVelocity.x = -input.vertical;

            //Debug.Log(targetVelocity.x);

            // Calculate how fast we should be moving
            //targetVelocity = transform.TransformDirection(targetVelocity); //change from local space to world space
            targetVelocity *= speed * deltaTime;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.Linear;
            Vector3 velocityChange = (targetVelocity - velocity);
            //velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            //velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity * mass; //If we are't wall running or climbing a ladder apply gravity to the player

            Debug.Log(velocityChange.x);

            rb.Linear.x += velocityChange.x;
            rb.Linear.y += velocityChange.y;
            rb.Linear.z += velocityChange.z;
        }
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity rb, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld, ref Rotation rot) =>
        {
            float3 targetVelocity = new float3();
            float speed = 1000;
            float gravity = 2;
            float mass = 3;

            float maxVelocityChange = 10;

            targetVelocity.z = input.horizontal;
            targetVelocity.x = -input.vertical;

            //Debug.Log(rot.Value.value.w);


            // Calculate how fast we should be moving
            //targetVelocity = transform.TransformDirection(targetVelocity); //change from local space to world space
            targetVelocity = Rotate(toWorld.Value, targetVelocity);
            targetVelocity *= speed * Time.deltaTime;

            // Apply a force that attempts to reach our target velocity
            float3 velocity = rb.Linear;
            float3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = math.clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = math.clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity * mass; //If we are't wall running or climbing a ladder apply gravity to the player

            rb.Linear += velocityChange;
            rb.Angular = new float3(0, 0, 0);

            rot.Value.value = new float4(0, 0, 0, rot.Value.value.w);
        });
    }

    [BurstCompile]
    public static float3 Rotate(float4x4 a, float3 b)
    {
        return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
    }
}
