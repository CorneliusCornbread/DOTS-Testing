using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class FollowControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FollowControllerStruct());
        //dstManager.AddComponentData(entity, new InputStruct());
    }
}
