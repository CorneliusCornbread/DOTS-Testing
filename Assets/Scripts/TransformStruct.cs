using Unity.Entities;

public struct TransformStruct : IComponentData
{
    public float transformX;
    public float transformY;
    public float transformZ;

    public float rotX;
    public float rotY;
    public float rotZ;
}
