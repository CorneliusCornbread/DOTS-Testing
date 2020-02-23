using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Extensions;

[BurstCompile]
public class PhysicsControllerSystem : JobComponentSystem
{
    private EntityQuery q;

    public struct MoveJob : IJobChunk
    {
        public float deltaTime;
        public ArchetypeChunkComponentType<PhysicsVelocity> physVType;
        public ArchetypeChunkComponentType<PhysicsMass> physMassType;
        [ReadOnly]
        public ArchetypeChunkComponentType<InputStruct> inputType;
        public ArchetypeChunkComponentType<PhysicsControllerStruct> pControlType;
        [ReadOnly]
        public ArchetypeChunkComponentType<LocalToWorld> toWorldType;
        public ArchetypeChunkComponentType<Rotation> rotType;
        public ArchetypeChunkComponentType<Translation> transType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            //Get arrays of objects that we'll be operating on
            NativeArray<PhysicsVelocity> physVel = chunk.GetNativeArray(physVType);
            NativeArray<PhysicsMass> physMass = chunk.GetNativeArray(physMassType);
            NativeArray<InputStruct> inputStructs = chunk.GetNativeArray(inputType);
            NativeArray<PhysicsControllerStruct> controllers = chunk.GetNativeArray(pControlType);
            NativeArray<LocalToWorld> toWorlds = chunk.GetNativeArray(toWorldType);
            NativeArray<Rotation> rotations = chunk.GetNativeArray(rotType);
            NativeArray<Translation> translations = chunk.GetNativeArray(transType);

            //Essentially IJobForEach
            for (int i = 0; i < chunk.Count; i++)
            {
                //Get data from arrays
                PhysicsVelocity rb = physVel[i];
                PhysicsMass pMass = physMass[i];
                InputStruct inp = inputStructs[i];
                PhysicsControllerStruct controllerStruct = controllers[i];
                LocalToWorld toWorld = toWorlds[i];
                Rotation rot = rotations[i];
                Translation trans = translations[i];

                //Do work on data
                Move(ref rb, ref pMass, ref inp, ref controllerStruct, ref toWorld, ref rot, ref trans);

                //Set data in array to data we changed
                physVel[i] = rb;
                physMass[i] = pMass;
                //inputStructs[i] = inp;
                controllers[i] = controllerStruct;
                //toWorlds[i] = toWorld;
                rotations[i] = rot;
                translations[i] = trans;
            }
        }

        public void Move(ref PhysicsVelocity rb, ref PhysicsMass mass, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld, ref Rotation rot, ref Translation trans)
        {
            float3 targetVelocity = new float3();
            float speed = 9;
            float gravity = 4;

            //Set target to input velocity
            targetVelocity.x = input.move.x;
            targetVelocity.z = input.move.y;

            //Calculate how fast we should be moving
            targetVelocity = TransformDirection(toWorld.Value, targetVelocity); //Change from local space to world space
            targetVelocity *= speed * deltaTime * 100;

            //Apply a force that attempts to reach our target velocity
            float3 velocityChange = (targetVelocity - rb.Linear);
            velocityChange.y = -gravity * deltaTime * 10; //Apply gravity to the player

            //Mouse movement
            rb.Angular.y = -input.mouseX * 2; //* deltaTime;

            mass.InverseInertia[0] = 0;
            mass.InverseInertia[2] = 0;

            if (playerData.isGrounded && input.jump && playerData.timeSinceLastJump > .1f)
            {
                velocityChange.y = 10;
                playerData.timeSinceLastJump = 0;
            }

            playerData.timeSinceLastJump += deltaTime;

            rb.ApplyLinearImpulse(mass, velocityChange);
        }
    }
    
    public static float3 TransformDirection(float4x4 a, float3 b)
    {
        return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
    }
    
    protected override void OnCreate()
    {
        var query = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<PhysicsVelocity>(),
                ComponentType.ReadWrite<PhysicsMass>(),
                ComponentType.ReadOnly<InputStruct>(),
                ComponentType.ReadWrite<PhysicsControllerStruct>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>(),
            }
        };

        q = GetEntityQuery(query);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Setup job
        MoveJob job = new MoveJob
        {
            physVType = GetArchetypeChunkComponentType<PhysicsVelocity>(false),
            physMassType = GetArchetypeChunkComponentType<PhysicsMass>(false),
            inputType = GetArchetypeChunkComponentType<InputStruct>(true),
            pControlType = GetArchetypeChunkComponentType<PhysicsControllerStruct>(false),
            toWorldType = GetArchetypeChunkComponentType<LocalToWorld>(true),
            rotType = GetArchetypeChunkComponentType<Rotation>(false),
            transType = GetArchetypeChunkComponentType<Translation>(false),
            deltaTime = Time.DeltaTime,
        };

        return job.Schedule(q, inputDeps);
    }
    
}