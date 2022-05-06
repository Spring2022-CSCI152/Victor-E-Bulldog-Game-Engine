using Bulldog.ECS.Components;
using Silk.NET.OpenGL;

namespace Bulldog.ECS.Systems;

public class RenderSystem : BaseSystem
{
    public RenderSystem() : base(new[] { "MeshComponent" }) {}

    public void SetCurrentEntity(Entity entity)
    {
        CurrentEntity = entity;
    }
    public override void Draw()
    {
        unsafe
        {
            var temp = CurrentEntity.GetComponent<MeshComponent>();

            temp.Vao.Bind();
            temp.Shader.Use();
            //Setting a uniform.
            //Bind a texture and and set the uTexture0 to use texture0.
            temp.Texture.Bind();
            temp.Shader.SetUniform("uTexture0", 0);
            temp._gl.DrawElements(PrimitiveType.Triangles, (uint) temp.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }
}