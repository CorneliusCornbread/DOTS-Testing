using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;


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
    private struct TransformJob : IJobForEach<TransformStruct, PhysicsVelocity, Translation, InputStruct>
    {
        public float deltaTime;

        public void Execute(ref TransformStruct data, ref PhysicsVelocity rb, ref Translation pos, ref InputStruct input)
        {
            //euler.Value += data.rotX * deltaTime;

            //euler.Value.x += data.speed * input.mouseX * deltaTime;
            //euler.Value.z += data.speed * input.mouseY * deltaTime;

            if (input.shift)
            {
                pos.Value.z += data.speed * input.horizontal * deltaTime * 5;
                pos.Value.x -= data.speed * input.vertical * deltaTime * 5;
            }

            else
            {
                float horiz = data.speed * input.horizontal * deltaTime * 10;
                float vert = data.speed * input.vertical * deltaTime * 10;

                rb.Linear.x = horiz;
            }

            pos.Value.y += data.speed * input.alt * deltaTime * 2;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TransformJob job = new TransformJob { deltaTime = Time.deltaTime };
        return job.Schedule(this, inputDeps);
    }
}
