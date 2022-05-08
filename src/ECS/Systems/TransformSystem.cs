using System.Numerics;
using Bulldog.ECS.Components;

namespace Bulldog.ECS.Systems;

public class TransformSystem : BaseSystem
{
    public TransformSystem() : base(new[] { "TransformComponent" }) {}

    private void SetCurrentEntity(Entity entity)
    {
        CurrentEntity = entity;
    }

    public override void Update(Vector3 dt)
    {
        var transform = CurrentEntity.GetComponent<TransformComponent>();
        transform.Position += dt;
    }
}