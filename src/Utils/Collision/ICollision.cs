namespace Bulldog.Utils.Collision;

public interface ICollision
{
    IBox Box { get; }
    
    IBox Other { get; }
    
    IHit Hit { get; }
}