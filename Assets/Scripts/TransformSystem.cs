using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;

public class TransformSystem : JobComponentSystem
{
 
    /* Component system implementation
    [BurstCompile]
    protected override void OnUpdate()
    {
        
        Entities.ForEach((ref TransformStruct data, ref RotationEulerXYZ euler, ref Translation pos) =>
        {
            euler.Value += data.rotX * Time.deltaTime;

            pos.Value.y = 4;

            //scale.Value += Time.deltaTime;
        });
    }*/

    
    //Job Component System implementation
    [BurstCompile]
    private struct TransformJob : IJobForEach<TransformStruct, RotationEulerXYZ, Translation>
    {
        public float deltaTime;

        public void Execute(ref TransformStruct data, ref RotationEulerXYZ euler, ref Translation pos)
        {
            euler.Value += data.rotX * deltaTime;

            pos.Value.y = 4;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TransformJob job = new TransformJob { deltaTime = Time.deltaTime };
        return job.Schedule(this, inputDeps);
    }
}
