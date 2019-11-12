using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public class GroundCheckSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsControllerStruct player, ref Translation trans) =>
        {
            //Ground check
            float3 pos = trans.Value;
            float3 target = pos;
            target.y -= 1.15f;

            RaycastInput rInput = new RaycastInput()
            {
                Start = pos,
                End = target,
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1,
                    CollidesWith = 2,
                    GroupIndex = 1
                }
            };

            BuildPhysicsWorld physWorld = World.Active.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physWorld.PhysicsWorld.CollisionWorld;

            //Ground check raycast
            player.isGrounded = collisionWorld.CastRay(rInput, out Unity.Physics.RaycastHit hit);

            //Rigidbody hit
            player.rbIndex = hit.RigidBodyIndex;
        });
    }
}
