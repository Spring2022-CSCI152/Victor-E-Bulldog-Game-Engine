using System.Numerics;
namespace Bulldog.Utils.Collision
{
	public interface IBox : IEquatable<IBox>
	{
		Vector3 Origin { get; }
		float HalfWith { get; }
		Vector3 GetMin();
		Vector3 GetMax();
	}
}
