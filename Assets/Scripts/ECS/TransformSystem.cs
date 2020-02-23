using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;


public class TransformSystem : ComponentSystem
{
    //Component System implementation
    [BurstCompile]
    protected override void OnUpdate()
    {
        
        Entities.ForEach((ref TransformStruct data, ref InputStruct input, ref PhysicsVelocity rb) =>
        {
            //euler.Value += data.rotX * deltaTime;

            //euler.Value.x += data.speed * input.mouseX * deltaTime;
            //euler.Value.z += data.speed * input.mouseY * deltaTime;
            float deltaTime = Time.DeltaTime;

            if (input.shift)
            {
                float horiz = data.speed * input.move.x * deltaTime * 500;
                float vert = data.speed * input.move.y * deltaTime * 500;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            else
            {
                float horiz = data.speed * input.move.x * deltaTime * 100;
                float vert = data.speed * input.move.y * deltaTime * 100;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            if (input.alt == 0)
                return;

            const float max = 10;

            if (rb.Linear.y < max || rb.Linear.y > -max)
            {
                float alt = data.speed * input.alt * deltaTime * 100;
                if (alt > 0 && rb.Linear.y + alt > max)
                {
                    float target = max - rb.Linear.y;
                    if (target < 0)
                        return;
                    else
                        alt = target;

                }

                else if (alt < 0 && rb.Linear.y + alt < -max)
                {
                    float target = (-max - rb.Linear.y);
                    if (target > 0)
                        return;
                    else
                        alt = target;
                }

                alt = Mathf.Clamp(alt, -max - 1, max + 1);

                rb.Linear.y += alt;
            }
        });
    }//

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
                float horiz = data.speed * input.move.x * deltaTime * 500;
                float vert = data.speed * input.move.y * deltaTime * 500;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            else
            {
                float horiz = data.speed * input.move.x * deltaTime * 100;
                float vert = data.speed * input.move.y * deltaTime * 100;

                rb.Linear.z = horiz;
                rb.Linear.x = -vert;
            }

            if (input.alt == 0)
                return;

            const float max = 10;

            if (rb.Linear.y < max || rb.Linear.y > -max)
            {
                float alt = data.speed * input.alt * deltaTime * 100;
                if (alt > 0 && rb.Linear.y + alt > max)
                {
                    float target = max - rb.Linear.y;
                    if (target < 0)
                        return;
                    else
                        alt = target;

                }

                else if (alt < 0 && rb.Linear.y + alt < -max)
                {
                    float target = (-max - rb.Linear.y);
                    if (target > 0)
                        return;
                    else
                        alt = target;
                }

                alt = Mathf.Clamp(alt, -max - 1, max + 1);

                rb.Linear.y += alt;
            }
        }
    }

    /*
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TransformJob job = new TransformJob { deltaTime = Time.deltaTime };
        return job.Schedule(this, inputDeps);
    }*/
}
