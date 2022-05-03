using Bulldog.Renderer;

namespace Bulldog.ECS.Components;

public class Mesh : Component
{
    public VertexArrayObject<float, uint> Vao;
    public Texture Texture;
    public Shader Shader;
}