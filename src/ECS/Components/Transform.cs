using System.Numerics;

namespace Bulldog.ECS.Components;

public class Transform : Component
{
    public Vector3 Position { get; set; } = new(0, 0, 0);
    public float Scale { get; set; } = 1f;
    public Quaternion Rotation{ get;set; }  = Quaternion.Identity;
}