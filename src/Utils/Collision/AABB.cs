using Silk.NET.Maths;

namespace Bulldog.Utils.Collision
{
	public class AABB : IBox
	{
		private Vector3D<float> _max;
		private Vector3D<float> _min;

		public AABB(float xMin, float yMin, float zMin, float zMax, float xMax, float yMax)
		{
			_max = new Vector3D<float>(xMax, yMax, zMax);
			_min = new Vector3D<float>(xMin, yMin, zMin);
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

		public Vector3D<float> Origin()
		{
			return (new Vector3D<float> ( (_min.X + Width() / 2), _min.Y + Depth() / 2,  _min.Z + Height() / 2  ));
		}


		Vector3D<float> IBox.Origin => Origin();
		float IBox.halfWith => Width();

	}
}