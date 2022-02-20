namespace Bulldog.Renderer.Shaders;

public class FragmentShader
{
    //Fragment shaders are run on each fragment/pixel of the geometry.
    public static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";
}