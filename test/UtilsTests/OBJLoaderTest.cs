using Bulldog.Utils;
using Moq;
using Silk.NET.Maths;
using Vector3D = Assimp.Vector3D;

namespace Bulldog.test.UtilsTests;
using Xunit;

public class OBJLoaderTest
{
    private readonly List<Vector3D> _vl;
   // private readonly List<float> temp;
   // private readonly Action<Vector3D> _m;
    private readonly ObjLoader _sut;

    public OBJLoaderTest()
    {
       // _sut = new ObjLoader("path");
        _vl = new List<Vector3D>();
       // temp = new List<float>();
        //_m = new Action<Vector3D>(); jetbrains error https://youtrack.jetbrains.com/issue/RSRP-232809/False-error-Delegate-constructor-is-invoked-with-0-argument-s


    }
// all functions in this are private static, making this a strict utility class
    [Fact]
    public void TestExtractDataFromFloatVectorList()
    {
        
        
    }
}