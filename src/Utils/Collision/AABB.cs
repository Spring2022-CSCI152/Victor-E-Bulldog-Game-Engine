using System.Numerics;
using Silk.NET.Maths;

namespace Bulldog.Utils.Collision
{
	public class AABB : IBox
	{
		public Vector3 Max;
		public Vector3 Min;

		public AABB(float xMin, float yMin, float zMin, float zMax, float xMax, float yMax)
		{
			Max = new Vector3(xMax, yMax, zMax);
			Min = new Vector3(xMin, yMin, zMin);
		}

		public AABB()
		{
			Max = new Vector3(0);
			Min = new Vector3(0);
		}

		public float Width()
		{
			return Math.Abs(Max.X - Min.X);
		}
		public float Height()
		{
			return Math.Abs(Max.Y - Min.Y);
		}
		public float Depth()
		{
			return Math.Abs(Max.Z - Min.Z);
		}

		public Vector3 Origin()
		{
			return (new Vector3( (Min.X + Width() / 2), Min.Y + Depth() / 2,  Min.Z + Height() / 2  ));
		}
		
		Vector3 IBox.Origin => Origin();
		float IBox.HalfWith => Width();
		public Vector3 GetMin()
		{
			return Min;
		}

		public Vector3 GetMax()
		{
			return Max;
		}

		protected bool Equals(AABB other)
		{
			return Max.Equals(other.Max) && Max.Equals(other.Max);
		}

		public bool Equals(IBox? other)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AABB) obj);
		}

		public void UpdateMin(Vector3 localMin) {
			Min = Vector3.Min(Min, localMin);
		}

		public void UpdateMax(Vector3 localMax) {
			Max = Vector3.Max(Max, localMax);
		}

		public Vector3 Center() {
			return (Min + Max) / 2f;
		}

		public Vector3 Diff() {
			return Max - Min;
		}

		internal void ExpandToFit(AABB b) {
			if (b.Min.X < this.Min.X) { this.Min.X = b.Min.X; }            
			if (b.Min.Y < this.Min.Y) { this.Min.Y = b.Min.Y; }            
			if (b.Min.Z < this.Min.Z) { this.Min.Z = b.Min.Z; }
            
			if (b.Max.X > this.Max.X) { this.Max.X = b.Max.X; }
			if (b.Max.Y > this.Max.Y) { this.Max.Y = b.Max.Y; }
			if (b.Max.Z > this.Max.Z) { this.Max.Z = b.Max.Z; }                        
		}

		public AABB ExpandedBy(AABB b) {
			AABB newbox = this;
			if (b.Min.X < newbox.Min.X) { newbox.Min.X = b.Min.X; }            
			if (b.Min.Y < newbox.Min.Y) { newbox.Min.Y = b.Min.Y; }            
			if (b.Min.Z < newbox.Min.Z) { newbox.Min.Z = b.Min.Z; }
            
			if (b.Max.X > newbox.Max.X) { newbox.Max.X = b.Max.X; }
			if (b.Max.Y > newbox.Max.Y) { newbox.Max.Y = b.Max.Y; }
			if (b.Max.Z > newbox.Max.Z) { newbox.Max.Z = b.Max.Z; }   

			return newbox;
		}

		public void ExpandBy(AABB b) 
		{
			var ret = this.ExpandedBy (b);
			this.Max = ret.Max;
			this.Min = ret.Min;
		}

		public static AABB FromSphere(Vector3 pos, float radius) {
			AABB box = new AABB();
			box.Min.X = pos.X - radius;
			box.Max.X = pos.X + radius;
			box.Min.Y = pos.Y - radius;
			box.Max.Y = pos.Y + radius;
			box.Min.Z = pos.Z - radius;
			box.Max.Z = pos.Z + radius;

			return box;
		}

		
		public override int GetHashCode()
		{
			return HashCode.Combine(Max, Min);
		}
	}
}