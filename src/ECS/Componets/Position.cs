using Bulldog.ECS.Systems;
using Silk.NET.Maths;
namespace Bulldog.ECS.Componets;

public class Position : Component
{
    public Vector3D<float> Loc   { get; set; }
}