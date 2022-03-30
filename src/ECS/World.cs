namespace Bulldog.ECS;
public class World
{
    public List <Entity> Entities = new();

    public void Register(Entity entity)
    {
        Entities.Add(entity);
    }





}