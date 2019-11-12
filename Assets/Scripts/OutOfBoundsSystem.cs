using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public class OutOfBoundsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref OutOfBoundsStruct bounds, ref Translation trans) =>
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
        });
    }
}
