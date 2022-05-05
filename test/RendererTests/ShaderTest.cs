using Moq;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;

namespace Bulldog.test.RendererTests;
using Xunit;

public class ShaderTest
{
    private readonly Mock<Shader> _sut;

    public ShaderTest()
    {
        _sut = new Mock<Shader>();
    }

    [Fact]
    public void TestSetUniform()
    {
        
    }
}