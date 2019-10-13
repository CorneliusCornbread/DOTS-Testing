using Unity.Transforms;
using Unity.Entities;
using UnityEngine;

public class PhysicsControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InputStruct());
        dstManager.AddComponentData(entity, new PhysicsControllerStruct());
    }
}
