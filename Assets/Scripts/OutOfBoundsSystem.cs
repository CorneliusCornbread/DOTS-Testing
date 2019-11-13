using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
public class OutOfBoundsSystem : JobComponentSystem
{
    /// <summary>
    /// A job that iterates on everything with the components OutOfBoundsStruct and Translation.
    /// Will move object based on the given minY and maxY to the given respawn position.
    /// </summary>
    public struct BoundCheckJob : IJobForEach<OutOfBoundsStruct, Translation>
    {
        public void Execute([ReadOnly] ref OutOfBoundsStruct bounds, ref Translation trans)
        {
            if (trans.Value.y > bounds.maxY)
            {
                trans.Value = bounds.resetPos;
                BurstDebug.Log("Max bound limit exceeded on entity, resetting pos");
            }

            else if (trans.Value.y < bounds.minY)
            {
                trans.Value = bounds.resetPos;
                BurstDebug.Log("Min bound limit exceeded on entity, resetting pos");
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        BoundCheckJob j = new BoundCheckJob();
        return j.Schedule(this, inputDeps);
    }
}
