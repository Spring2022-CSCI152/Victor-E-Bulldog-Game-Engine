using System.Numerics;

namespace Bulldog.Core;

public class Camera
{
    public Vector3 Position;
    public Vector3 Front;
    public Vector3 Up;
    public Vector3 Direction;
    public float Yaw;
    public float Pitch;
    public float Zoom = 45f;

    public Camera()
    {
        Position = new Vector3(0.0f, 0.0f, 3.0f);
        Front = new Vector3(0.0f, 0.0f, -1.0f);
        Up = Vector3.UnitY;
        Direction = Vector3.Zero;
        Yaw = -90f;
        Pitch = 0f;
        Zoom = 45f;
    }
}