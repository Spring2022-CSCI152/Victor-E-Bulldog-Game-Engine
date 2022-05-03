namespace Bulldog.ECS
{
    public abstract class Component : ECSObject
    {
        public new string Name { get => base.Name; }

        public Component() { }
        
    }
}