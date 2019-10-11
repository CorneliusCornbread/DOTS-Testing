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
    private struct TransformJob : IJobForEach<TransformStruct, PhysicsVelocity, InputStruct>
    {
        public float deltaTime;

        public void Execute(ref TransformStruct data, ref PhysicsVelocity rb, ref InputStruct input)
        {
            //euler.Value += data.rotX * deltaTime;

            //euler.Value.x += data.speed * input.mouseX * deltaTime;
            //euler.Value.z += data.speed * input.mouseY * deltaTime;

            if (input.shift)
            {
                float horiz = data.speed * input.horizontal * deltaTime * 500;
                float vert = data.speed * input.vertical * deltaTime * 500;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            else
            {
                float horiz = data.speed * input.horizontal * deltaTime * 100;
                float vert = data.speed * input.vertical * deltaTime * 100;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            float max = 20;

            Debug.Log(rb.Linear.y);

            if (rb.Linear.y < max || rb.Linear.y > -max)
            {
                float alt = data.speed * input.alt * deltaTime * 100;

                if (rb.Linear.y + alt > max)
                    alt = max - rb.Linear.y;

                else if (rb.Linear.y + alt < -max)
                    alt = -max - rb.Linear.y;

                alt = Mathf.Clamp(alt, -max - 1, max + 1);

                rb.Linear.y += alt;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TransformJob job = new TransformJob { deltaTime = Time.deltaTime };
        return job.Schedule(this, inputDeps);
    }
}
