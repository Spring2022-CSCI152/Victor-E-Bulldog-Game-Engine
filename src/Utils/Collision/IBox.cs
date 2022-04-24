using Silk.NET.Maths;
namespace Bulldog.Utils.Collision
{
	using System;

	/// <summary>
	/// Represents a physical body in the world.
	/// </summary>
	public interface IBox
	{
		Vector3D<float> Origin { get; }
		float halfWith         { get; }
	}
}
