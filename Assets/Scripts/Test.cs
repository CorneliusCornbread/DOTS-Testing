using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Entities;

public class Test : MonoBehaviour
{

    void Update()
    {
        float3 pos = transform.position;
        float3 target = pos;
        target.y -= 5;

        RaycastInput rInput = new RaycastInput()
        {
            Start = pos,
            End = target,
            Filter = new CollisionFilter()
            {
                BelongsTo = 1,
                CollidesWith = 1,
                GroupIndex = 1
            }
        };

        if (!rInput.Filter.IsValid)
        {
            Debug.Log("Filter isn't valid");
            return;
        }

        BuildPhysicsWorld physWorld = World.Active.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = physWorld.PhysicsWorld.CollisionWorld;

        collisionWorld.CastRay(rInput, out Unity.Physics.RaycastHit hit);

        Debug.Log(hit.Position);
    }
}
