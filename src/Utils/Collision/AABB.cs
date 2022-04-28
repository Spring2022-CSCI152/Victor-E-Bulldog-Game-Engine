using System.Numerics;
using Silk.NET.Maths;

namespace Bulldog.Utils.Collision
{
	public class AABB : IBox
	{
		private Vector3 _max;
		private Vector3 _min;

		public AABB(float xMin, float yMin, float zMin, float zMax, float xMax, float yMax)
		{
			_max = new Vector3(xMax, yMax, zMax);
			_min = new Vector3(xMin, yMin, zMin);
		}
		public float Width()
		{
			return Math.Abs(_max.X - _min.X);
		}
		public float Height()
		{
			return Math.Abs(_max.Y - _min.Y);
		}
		public float Depth()
		{
			return Math.Abs(_max.Z - _min.Z);
		}

		public Vector3 Origin()
		{
			return (new Vector3( (_min.X + Width() / 2), _min.Y + Depth() / 2,  _min.Z + Height() / 2  ));
		}
		
		Vector3 IBox.Origin => Origin();
		float IBox.HalfWith => Width();
		public Vector3 GetMin()
		{
			return _min;
		}

		public Vector3 GetMax()
		{
			return _max;
		}
	}
}