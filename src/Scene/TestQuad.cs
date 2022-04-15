namespace Bulldog.Scene;

public class TestQuad
{
    //Vertex data, uploaded to the VBO.
    public static readonly float[] Vertices =
    {
        //X     Y     Z
        0.5f,  0.5f, 0.0f,
        0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.5f
    };

    public static readonly float[] TexCoords =
    {
        //U  V
        1.0f, 0.0f,
        1.0f, 1.0f,
        0.0f, 1.0f,
        0.0f, 0.0f
    };

    public static readonly float[] InterleavedVertices =
    {
        //X    Y      Z     U   V
        0.5f,  0.5f, 0.0f, 1f, 0f,
        0.5f, -0.5f, 0.0f, 1f, 1f,
        -0.5f, -0.5f, 0.0f, 0f, 1f,
        -0.5f,  0.5f, 0.5f, 0f, 0f
    };
    
    //Index data, uploaded to the EBO.
    public static readonly uint[] Indices =
    {
        0, 1, 3,
        1, 2, 3
    };
}