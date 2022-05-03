using System.Numerics;
using Bulldog.Core;
using Bulldog.Utils;
using Silk.NET.OpenGL;

namespace Bulldog.Renderer;

public class Renderer
{
    private void CullFrustum(Camera camera){}
    private void CullOccluded(){}

    private void DrawDeferred(GL gl,float difference,float height, float width,Camera camera, Shader geometryPassShader)
    {
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, GBuffer.FBO);
        var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference));
        var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.Up);
        // ReSharper disable once PossibleLossOfFraction
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), width / height, 0.1f, 100.0f);
        geometryPassShader.Use();
        geometryPassShader.SetUniform("model", model);
        geometryPassShader.SetUniform("view", view);
        geometryPassShader.SetUniform("projection", projection);

        //We're drawing with just vertices and no incidences, and it takes 36 vertices to have a six-sided textured cube
        gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        gl.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
    private void DrawForward(){}
    
    private void CalculateLightingDeferred(){}
    private void CalculateLightingForward(){}

    private void CalculateShadowsDeferred(){}
    private void CalculateShadowForward(){}
    
    public void DrawAll(){}
    
}