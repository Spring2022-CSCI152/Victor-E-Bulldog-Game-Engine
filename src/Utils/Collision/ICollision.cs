using System.Numerics;

namespace Bulldog.Utils.Collision;

public interface ICollision
{
    bool CheckCollision(IBox a, IBox b);
    Vector3 CalculateMinSeparation(IBox a, IBox b);
}