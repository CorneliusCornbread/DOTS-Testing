using Unity.Entities;
using Unity.Mathematics;

public struct InputStruct : IComponentData
{
    public float2 moveRaw;
    public float2 move;

    public float alt;

    public float mouseX;
    public float mouseY;

    public bool shift;
    public bool jump;
}
