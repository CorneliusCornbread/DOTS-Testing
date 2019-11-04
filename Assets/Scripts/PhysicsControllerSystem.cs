using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Systems;

//[BurstCompile]
public class PhysicsControllerSystem : JobComponentSystem
{
    private EntityQuery q;

    public struct MoveJob : IJobChunk
    {
        public float deltaTime;
        public ArchetypeChunkComponentType<PhysicsVelocity> physVType;
        public ArchetypeChunkComponentType<PhysicsMass> physMassType;
        public ArchetypeChunkComponentType<InputStruct> inputType;
        public ArchetypeChunkComponentType<PhysicsControllerStruct> pControlType;
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
                inputStructs[i] = inp;
                controllers[i] = controllerStruct;
                toWorlds[i] = toWorld;
                rotations[i] = rot;
                translations[i] = trans;
            }
        }

        public void Move(ref PhysicsVelocity rb, ref PhysicsMass mass, ref InputStruct input, ref PhysicsControllerStruct playerData, ref LocalToWorld toWorld, ref Rotation rot, ref Translation trans)
        {
            float3 targetVelocity = new float3();
            float speed = 1000;
            float gravity = 1;

            float maxVelocityChange = 10;

            //Set target to input velocity
            targetVelocity.x = input.horizontal;
            targetVelocity.z = input.vertical;            

            //Calculate how fast we should be moving
            targetVelocity = TransformDirection(toWorld.Value, targetVelocity); //Change from local space to world space
            targetVelocity *= speed * deltaTime;

            //Apply a force that attempts to reach our target velocity
            float3 velocity = rb.Linear;
            float3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = math.clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = math.clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity; //If we are't wall running or climbing a ladder apply gravity to the player

            //Mouse movement
            rb.Angular.y = -input.mouseX * 2; //* deltaTime;

            mass.InverseInertia[0] = 0;
            mass.InverseInertia[2] = 0;

            float3 pos = trans.Value;
            float3 target = pos;
            target.y -= 2;

            RaycastInput rInput = new RaycastInput()
            {
                Start = pos,
                End = target,
                Filter = new CollisionFilter()
            };

            BuildPhysicsWorld physWorld = World.Active.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physWorld.PhysicsWorld.CollisionWorld;

            collisionWorld.CastRay(rInput, out Unity.Physics.RaycastHit hit);

            Debug.Log(hit.ToString());


            rb.Linear += velocityChange;
        }
    }
    
    public static float3 TransformDirection(float4x4 a, float3 b)
    {
        return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
    }
    
    protected override void OnCreate()
    {
        q = GetEntityQuery
            (
            typeof(PhysicsVelocity),
            typeof(PhysicsMass),
            typeof(InputStruct),
            typeof(PhysicsControllerStruct),
            typeof(LocalToWorld),
            typeof(Rotation),
            typeof(Translation),
            typeof(PhysicsCollider)
            );
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Setup job
        MoveJob job = new MoveJob
        {
            physVType = GetArchetypeChunkComponentType<PhysicsVelocity>(),
            physMassType = GetArchetypeChunkComponentType<PhysicsMass>(),
            inputType = GetArchetypeChunkComponentType<InputStruct>(),
            pControlType = GetArchetypeChunkComponentType<PhysicsControllerStruct>(),
            toWorldType = GetArchetypeChunkComponentType<LocalToWorld>(),
            rotType = GetArchetypeChunkComponentType<Rotation>(),
            transType = GetArchetypeChunkComponentType<Translation>(),
            deltaTime = Time.deltaTime
        };

        return job.Schedule(q, inputDeps);
    }
}