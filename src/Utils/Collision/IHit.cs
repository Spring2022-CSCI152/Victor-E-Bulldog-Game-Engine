using System.Numerics;

namespace Bulldog.Utils.Collision;

public interface IHit
{
    IBox Box { get; }
    Vector3 Normal { get;  }
    
    

    bool IsNearest(IHit than, Vector3 from);
}