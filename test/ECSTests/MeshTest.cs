using Bulldog.ECS;
using Bulldog.ECS.Componets;
using Xunit;

namespace Bulldog.test.ECSTests;

public class MeshTest
{
    private readonly Mesh _sut;

    public MeshTest()
    {
        _sut = new Mesh();
    }

    [Fact]
    public void MakeMeshComponent()
    {
    }
}