using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Data used for limiting and objects Y position by resetting it to a given position
/// </summary>
public struct OutOfBoundsStruct : IComponentData
{
    /// <summary>
    /// The position we will reset to if the min or max y is exceeded
    /// </summary>
    public float3 resetPos;

    /// <summary>
    /// The minimum Y before reset
    /// </summary>
    public float minY;

    /// <summary>
    /// The maxmimum Y before reset;
    /// </summary>
    public float maxY;
}
