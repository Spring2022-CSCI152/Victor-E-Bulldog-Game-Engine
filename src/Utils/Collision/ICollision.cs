using System.Numerics;

namespace Bulldog.Utils.Collision;

public interface ICollision
{
    bool IntersectsAABB(IBox a, IBox b);
    public bool IntersectsSphere(IBox A, float radius);
    Vector3 CalculateMinSeparation(IBox a, IBox b);
}