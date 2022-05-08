using Bulldog.ECS.Components;
using Silk.NET.OpenGL;
namespace Bulldog.ECS.Systems;

public class MeshSystem : BaseSystem
{
    public MeshSystem() : base(new[] { "MeshComponent" }) {}

    private void SetCurrentEntity(Entity entity)
    {
        CurrentEntity = entity;
    }
    public override void Draw()
    {
        unsafe
        {
            var mesh = CurrentEntity.GetComponent<MeshComponent>();

            mesh.Vao.Bind();
            mesh.Shader.Use();
            //Setting a uniform.
            //Bind a texture and and set the uTexture0 to use texture0.
            mesh.Texture.Bind();
            mesh.Shader.SetUniform("uTexture0", 0);
            if (CurrentEntity.HasComponent<TransformComponent>())
            {
                //TODO: set VMP Matraices before drawing mesh to screen
            }
            mesh.Gl.DrawElements(PrimitiveType.Triangles, (uint) mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }
}