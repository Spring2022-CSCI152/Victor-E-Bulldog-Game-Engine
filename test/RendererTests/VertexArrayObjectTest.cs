using Bulldog.Renderer;
using Moq;
using Silk.NET.OpenGL;
using Buffer = System.Buffer;

namespace Bulldog.test.RendererTests;
using Xunit;

public class VertexArrayObjectTest
{
    private readonly VertexArrayObject<float, uint> _sut;
    private GL _gl;
    private readonly BufferObject<float> vbo;
    private readonly BufferObject<uint> ebo;
  
    public VertexArrayObjectTest()
    {
       // vbo = new Mock<BufferObject<float>>();
       // ebo = new BufferObject<uint>()
       // _sut = new VertexArrayObject<float, uint>(null,vbo, ebo);
        _gl = new GL(null);
    }

    [Fact]
    public void MakeVbo()
    {

    }


};
