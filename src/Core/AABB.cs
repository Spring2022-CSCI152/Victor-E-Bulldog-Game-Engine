using Silk.NET.Maths;

namespace Bulldog.Core;

public class AABB
{
    public Vector3D<float> MinVertex { get; private set; }
    public Vector3D<float> MaxVertex { get; private set; }
}