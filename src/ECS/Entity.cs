using System;
namespace Bulldog.ECS;

public class Entity
{
    private Guid Id { get; set; }
    private List<Component> _componentPool;

    public void AddComponent( Component component)
    {
        component.EntityId = this.Id;
    }
    
    public T GetComponent<T>() where T : Component
    {
        return _componentPool.Where(component => component.GetType() == typeof(T)).Cast<T>().FirstOrDefault();
    }
    public void Delete(){}
    
    
}