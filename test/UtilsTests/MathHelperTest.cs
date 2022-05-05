using Bulldog.Utils;

namespace Bulldog.test.UtilsTests;
using Xunit;

public class MathHelperTest
{
    [Theory]
    [InlineData(1.57079637, 90)]
    [InlineData(0, 0)]
    [InlineData(16.0570297, 920)]
    [InlineData(-1.57079637, -90)]
    public void TestDegreesToRadiansTheory(float expected, float degrees)
    {
        Assert.Equal(expected, MathHelper.DegreesToRadians(degrees));
    }
}