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
            rotX = rotSpeed, 
            rotY = rotSpeed, 
            rotZ = rotSpeed,

            transformX = 0,
            transformY = 0,
            transformZ = 0
        });

        dstManager.AddComponentData(entity, new RotationEulerXYZ());
    }
}
