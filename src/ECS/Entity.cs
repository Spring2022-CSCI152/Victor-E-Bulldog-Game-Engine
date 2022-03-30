using System;

namespace Bulldog.ECS;

public class Entity
{

    private Guid Id { get; }
    public List<Component> ComponentPool { get; }

    public void AddComponent(Component component)
    {
        ComponentPool.Add(component);
        component.EntityId = this.Id;
    }
    public T GetComponent<T>() where T : Component
    {
        return ComponentPool.Where(component => component.GetType().Equals(typeof(T))).Cast<T>().FirstOrDefault();
    }
    

    
}