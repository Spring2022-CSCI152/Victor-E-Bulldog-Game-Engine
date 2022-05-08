using System.Numerics;

namespace Bulldog.ECS.Components;

public class TransformComponent : Components.Component
{
    public Vector3 Position;
    public float Scale;
    public Quaternion Rotation;
}