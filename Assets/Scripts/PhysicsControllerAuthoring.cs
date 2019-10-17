using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics;

public class PhysicsControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InputStruct());
        dstManager.AddComponentData(entity, new PhysicsControllerStruct());
        
    }
}
