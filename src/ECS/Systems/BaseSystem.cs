namespace Bulldog.ECS.Systems;

public class BaseSystem<T> where T : Component
{
    public static List<T> Components = new List<T>();

    public static void Register(T component)
    {
        Components.Add(component);
    }

    public static void Update(float deltaTime)
    {
        foreach ( T component in Components)
        {
            component.Update(deltaTime);
        }
    }
    
}