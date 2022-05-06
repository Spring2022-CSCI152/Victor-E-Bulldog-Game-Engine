using System.Numerics;

namespace Bulldog.Utils.Collision;

public class Collision : ICollision
{

    public bool IntersectsAABB(IBox a, IBox b)
    {
        return( (a.GetMin().X <= b.GetMax().X && a.GetMax().X >= b.GetMax().X) &&
                (a.GetMin().Y <= b.GetMax().Y && a.GetMax().Y >= b.GetMax().X) &&
                (a.GetMin().Z <= b.GetMax().Z && a.GetMax().Z >= b.GetMax().Z)
            );

    }

    public bool IntersectsSphere(IBox A, float radius) {
        if ( 
            (A.Origin.X + radius < A.GetMin().X) ||
            (A.Origin.Y + radius < A.GetMin().Y) ||
            (A.Origin.Z + radius < A.GetMin().Z) ||
            (A.Origin.X - radius > A.GetMax().X) ||
            (A.Origin.Y - radius > A.GetMax().Y) ||
            (A.Origin.Z - radius > A.GetMax().Z)
        ) {
            return false;
        } else {
            return true;
        }
    }

    public Vector3 CalculateMinSeparation(IBox a, IBox b)
    {
        var x = Math.Min(Math.Abs(a.GetMin().X - b.GetMax().X), Math.Abs(a.GetMax().X - b.GetMin().X));
        var y = Math.Min(Math.Abs(a.GetMin().Y - b.GetMax().Y), Math.Abs(a.GetMax().Y - b.GetMin().Y));
        var z = Math.Min(Math.Abs(a.GetMin().Z - b.GetMax().Z), Math.Abs(a.GetMax().Z - b.GetMin().Z));
        
        return new Vector3(x, y, z);
    }
}