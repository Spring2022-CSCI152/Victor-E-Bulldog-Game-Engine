using System.Data;

namespace Bulldog.ECS;

public class Component
{
    public Guid EntityId { get; set; }
    public virtual void Update(float deltaTime){}
}