using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class TransformAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float rotSpeed;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TransformStruct
        {
            speed = rotSpeed
        });

        dstManager.AddComponentData(entity, new RotationEulerXYZ());
        dstManager.AddComponentData(entity, new InputStruct());
    }
}
