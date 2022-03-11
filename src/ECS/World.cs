namespace Bulldog.ECS;
public static class World
{
    private static readonly List<Entity> Entities = new();

    public static void Register(Entity entity)
    {
        Entities.Add(entity);
    }





}