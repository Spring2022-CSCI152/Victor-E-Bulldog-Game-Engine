namespace Bulldog.ECS
{
    public abstract class Component : ECSObject
    {
        public new string Name {set { Name = value;} get => base.Name;}

        public Component()
        {
        }

        public override string ToString()
        {
            return string.Format("Component<{0}:{1}>", Name, UID);
        }
    }
}