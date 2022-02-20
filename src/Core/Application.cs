using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Bulldog.Renderer.Shaders;
using Bulldog.Scene;

namespace Bulldog.Core
{
    class Program
    {
        private static IWindow _window;
        private static GL _gl;

        private static uint _vbo;
        private static uint _ebo;
        private static uint _vao;
        private static uint _shader;

        private static void Main()
        {
            //Create a window.
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";

            _window = Window.Create(options);

            //Assign events.
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;

            //Run the window.
            _window.Run();
        }


        private static unsafe void OnLoad()
        {
            //Set-up input context.
            IInputContext input = _window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }



            
            //Getting the opengl api for drawing to the screen.
            _gl = GL.GetApi(_window);
            
            //Creating a vertex array.
            Console.WriteLine("Creating buffers...");
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            //Initializing a vertex buffer that holds the vertex data.
            _vbo = _gl.GenBuffer(); //Creating the buffer.
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo); //Binding the buffer.
            fixed (void* v = &TestQuad.Vertices[0])
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (TestQuad.Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            _ebo = _gl.GenBuffer(); //Creating the buffer.
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo); //Binding the buffer.
            fixed (void* i = &TestQuad.Indices[0])
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (TestQuad.Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
            }
            Console.WriteLine("Buffers done.");

            //Creating a vertex shader.
            Console.WriteLine("Compiling shaders...");
            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, VertexShader.VertexShaderSource);
            _gl.CompileShader(vertexShader);

            //Checking the shader for compilation errors.
            string infoLog = _gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
            }

            //Creating a fragment shader.
            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, FragmentShader.FragmentShaderSource);
            _gl.CompileShader(fragmentShader);

            //Checking the shader for compilation errors.
            infoLog = _gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling fragment shader {infoLog}");
            }

            //Combining the shaders under one shader program.
            _shader = _gl.CreateProgram();
            _gl.AttachShader(_shader, vertexShader);
            _gl.AttachShader(_shader, fragmentShader);
            _gl.LinkProgram(_shader);

            //Checking the linking for errors.
            _gl.GetProgram(_shader, GLEnum.LinkStatus, out var status);
            Console.WriteLine(status == 0 ? $"Error linking shader {_gl.GetProgramInfoLog(_shader)}" : "Shaders done.");

            //Delete the no longer useful individual shaders;
            _gl.DetachShader(_shader, vertexShader);
            _gl.DetachShader(_shader, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            //Tell opengl how to give the data to the shaders.
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            _gl.EnableVertexAttribArray(0);
        }

        private static unsafe void OnRender(double obj)
        {
            //Here all rendering should be done.
            //Clear the color channel.
            _gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            //Bind the geometry and shader.
            _gl.BindVertexArray(_vao);
            _gl.UseProgram(_shader);

            //Draw the geometry.
            _gl.DrawElements(PrimitiveType.Triangles, (uint) TestQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        private static void OnUpdate(double obj)
        {
            //Here all updates to the program should be done.
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            //Check to close the window on escape.
            if (arg2 == Key.Escape)
            {
                _window.Close();
            }
        }
    }
}