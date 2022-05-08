using Moq;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;

namespace Bulldog.test.RendererTests;
using Xunit;

public class ShaderTest
{
    private readonly Shader _sut;
    private GL _gl;

    public ShaderTest()
    {
        _sut = new Shader(null, "shader.vert","shader.frag");
        _gl = new GL(null);


    }

    [Fact(Skip = "Problems with the shader constructor and are not allowed to mock since data is 100% needed for the nested init functions and checking if the uniform actually changed")]
    public void TestSetUniform()
    {
        Assert.Null(_sut.GetGl());
        _sut.SetUniform("Test", 1);
    //    Assert.NotNull(_sut.GetGl().);
    }
    
    
}