using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class PhysicsControllerSystem : JobComponentSystem
{
    private EntityQuery q;

    //[BurstCompile]
    public struct MoveJob : IJobChunk
    {
        public float deltaTime;
        public ArchetypeChunkComponentType<PhysicsVelocity> physVType;
        public ArchetypeChunkComponentType<InputStruct> inputType;
        public ArchetypeChunkComponentType<PhysicsControllerStruct> pControlType;
        public ArchetypeChunkComponentType<LocalToWorld> toWorldType;
        public ArchetypeChunkComponentType<Rotation> rotType;
        public ArchetypeChunkComponentType<Translation> transType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            try
            {
                NativeArray<PhysicsVelocity> physVel = chunk.GetNativeArray(physVType);
                NativeArray<InputStruct> inputStructs = chunk.GetNativeArray(inputType);
                NativeArray<PhysicsControllerStruct> controllers = chunk.GetNativeArray(pControlType);
                NativeArray<LocalToWorld> toWorlds = chunk.GetNativeArray(toWorldType);
                NativeArray<Rotation> rotations = chunk.GetNativeArray(rotType);
                NativeArray<Translation> translations = chunk.GetNativeArray(transType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    //physVel[i] = new PhysicsVelocity();
                    //inputStructs[i] = new InputStruct();
                    //controllers[i] = new PhysicsControllerStruct();
                    //toWorlds[i] = new LocalToWorld();
                    //rotations[i] = new Rotation();
                    //translations[i] = new Translation();

                    PhysicsVelocity rb = physVel[i];
                    InputStruct inp = inputStructs[i];
                    PhysicsControllerStruct controllerStruct = controllers[i];
                    LocalToWorld toWorld = toWorlds[i];
                    //Rotation rot = rotations[i];
                    //Translation trans = translations[i];


                    Execute(ref rb, ref inp, ref controllerStruct, ref toWorld);
                }
            }
            catch (System.Exception e)
            {

                Debug.LogError(e);
            }
        }

        public void Execute(ref PhysicsVelocity rb, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld)
        {
            Debug.Log("execute");

            float3 targetVelocity = new float3();
            float speed = 1000;
            float gravity = 1;

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
            velocityChange.y = -gravity; //If we are't wall running or climbing a ladder apply gravity to the player

            rb.Linear += velocityChange;
            Debug.Log("execute end");
        }
    }

    /*
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
    }*/

    [BurstCompile]
    public static float3 Rotate(float4x4 a, float3 b)
    {
        return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
    }


    protected override void OnCreate()
    {
        q = GetEntityQuery
            (
            typeof(PhysicsVelocity),
            typeof(InputStruct),
            typeof(PhysicsControllerStruct),
            typeof(LocalToWorld),
            typeof(Rotation),
            typeof(Translation)
            );
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MoveJob job = new MoveJob
        {
            deltaTime = Time.deltaTime,
            physVType = GetArchetypeChunkComponentType<PhysicsVelocity>(),
            inputType = GetArchetypeChunkComponentType<InputStruct>(),
            pControlType = GetArchetypeChunkComponentType<PhysicsControllerStruct>(),
            toWorldType = GetArchetypeChunkComponentType<LocalToWorld>(),
            rotType = GetArchetypeChunkComponentType<Rotation>(),
            transType = GetArchetypeChunkComponentType<Translation>()
        };

        return job.Schedule(q, inputDeps);
    }
}