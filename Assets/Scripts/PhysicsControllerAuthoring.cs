using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics;

public class PhysicsControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InputStruct());
        dstManager.AddComponentData(entity, new PhysicsControllerStruct());
        dstManager.AddComponentData(entity, new RotationEulerXYZ());

        OutOfBoundsStruct bounds = new OutOfBoundsStruct()
        {
            maxY = 1000,
            minY = -20,
            resetPos = transform.position
        };

        dstManager.AddComponentData(entity, bounds);
    }
}
