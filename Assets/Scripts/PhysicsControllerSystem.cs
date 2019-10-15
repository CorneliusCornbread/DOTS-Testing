using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class PhysicsControllerSystem : ComponentSystem
{
    [BurstCompile]
    public struct MoveJob : IJobForEach<PhysicsVelocity, InputStruct, PhysicsControllerStruct, LocalToWorld, RotationEulerXYZ>
    {
        public float deltaTime;

        public void Execute(ref PhysicsVelocity rb, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld, ref RotationEulerXYZ rot)
        {
            float3 targetVelocity = new float3();
            float speed = 1000;
            float gravity = 1;
            float mass = 1;

            float maxVelocityChange = 10;

            targetVelocity.z = input.horizontal;
            targetVelocity.x = -input.vertical;

            //Debug.Log(rot.Value.value.w);


            // Calculate how fast we should be moving
            //targetVelocity = transform.TransformDirection(targetVelocity); //change from local space to world space
            targetVelocity = Rotate(toWorld.Value, targetVelocity); //Change from local space to world space
            targetVelocity *= speed * deltaTime;

            // Apply a force that attempts to reach our target velocity
            float3 velocity = rb.Linear;
            float3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = math.clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = math.clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity * mass; //If we are't wall running or climbing a ladder apply gravity to the player

            rb.Linear += velocityChange;

            rot.Value = new float3(0, 0, 0);
            /*
            float baseSpd = 2;
            float maxSpd = 4;
            float mult = 5;

            if (rot.Value.value.x > 0)
            {
                rb.Angular.x = math.clamp(rot.Value.value.x * mult, baseSpd, maxSpd);
            }

            else if (rot.Value.value.x < 0)
            {
                rb.Angular.x = rb.Angular.x = math.clamp(rot.Value.value.x * mult, -baseSpd, -maxSpd);
            }

            else
            {
                rb.Angular.x = 0;
            }

            if (rot.Value.value.y > 0)
            {
                rb.Angular.y = math.clamp(rot.Value.value.y * mult, baseSpd, maxSpd);
            }

            else if (rot.Value.value.y < 0)
            {
                rb.Angular.y = math.clamp(rot.Value.value.y * mult, -baseSpd, -maxSpd);
            }

            else
            {
                rb.Angular.y = 0;
            }

            if (rot.Value.value.z > 0)
            {
                rb.Angular.z = math.clamp(rot.Value.value.z * mult, baseSpd, maxSpd);
            }

            else if (rot.Value.value.z < 0)
            {
                rb.Angular.z = math.clamp(rot.Value.value.z * mult, -baseSpd, -maxSpd);
            }

            else
            {
                rb.Angular.z = 0;
            }
            */

            //rot.Value.value = new float4(0, 0, 0, rot.Value.value.w);
        }
    }

    
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity rb, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld, ref RotationEulerXYZ rot) =>
        {
            float3 targetVelocity = new float3();
            float speed = 1000;
            float gravity = 1;
            float mass = 1;

            float maxVelocityChange = 10;

            targetVelocity.z = input.horizontal;
            targetVelocity.x = -input.vertical;

            //Debug.Log(rot.Value.value.w);


            // Calculate how fast we should be moving
            //targetVelocity = transform.TransformDirection(targetVelocity); //change from local space to world space
            targetVelocity = Rotate(toWorld.Value, targetVelocity); //Change from local space to world space
            targetVelocity *= speed * Time.deltaTime;

            // Apply a force that attempts to reach our target velocity
            float3 velocity = rb.Linear;
            float3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = math.clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = math.clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity * mass; //If we are't wall running or climbing a ladder apply gravity to the player

            rb.Linear += velocityChange;

            rot.Value = new float3(0, 0, 0);
        });
    }

    [BurstCompile]
    public static float3 Rotate(float4x4 a, float3 b)
    {
        return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
    }

    /*
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MoveJob job = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(this, inputDeps);
    }*/
}
