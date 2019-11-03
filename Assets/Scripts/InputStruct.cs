using Unity.Entities;

public struct InputStruct : IComponentData
{
    public float horizontal;
    public float vertical;
    public float alt;

    public float mouseX;
    public float mouseY;

    public bool shift;
    public bool jump;
}
