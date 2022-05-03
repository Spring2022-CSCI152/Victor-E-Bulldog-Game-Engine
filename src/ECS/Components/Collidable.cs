using Bulldog.Utils.Collision;

namespace Bulldog.ECS.Components;

public class Collidable : Component
{
    public AABB Aabb;
    public Collision Collision;
}