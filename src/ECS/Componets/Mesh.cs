using Bulldog.Renderer;

namespace Bulldog.ECS.Componets;

public struct Mesh
{
    public VertexArrayObject<float, uint> Vao;
    public Texture Texture;
    public Shader Shader;
}