using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class TransformSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TransformStruct data, ref RotationEulerXYZ euler, ref Translation pos, ref Scale scale) =>
        {
            euler.Value.x += 10 * Time.deltaTime;
            euler.Value.y += 10 * Time.deltaTime;
            euler.Value.z += 10 * Time.deltaTime;

            //pos.Value.x += 10 * Time.deltaTime;
        });
    }
}
