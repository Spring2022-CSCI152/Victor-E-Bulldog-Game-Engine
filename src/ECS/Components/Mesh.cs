using Bulldog.Renderer;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using Texture = Bulldog.Renderer.Texture;

namespace Bulldog.ECS.Components
{
    public class MeshComponent : Component
    {
        public VertexArrayObject<float, uint> Vao;
        public float[] Indices;
        public Texture Texture;
        public Shader Shader;
        public GL Gl;
    }
}