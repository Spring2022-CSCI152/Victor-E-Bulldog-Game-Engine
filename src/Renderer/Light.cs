using System.ComponentModel;
using System.Numerics;
using Bulldog.Core;

namespace Bulldog.Renderer;

public class Light
{
    private LightingType Type { get; }
    private Vector3 Position { get; }
    private Vector3 Direction { get; }

    public Light(LightingType lt, Vector3 position, Vector3 direction)
    {
        Type = lt;
        Position = position;
        if (lt == LightingType.Point)
            throw new Exception($"{lt} Should not have Directionality");
        Direction = direction;
    }
    
    public Light(LightingType lt, Vector3 position)
    {
        Type = lt;
        Position = position;
        if (lt < LightingType.Directional)
            throw new Exception($"{lt} Should have Directionality");
    }
}