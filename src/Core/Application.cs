using Bulldog.Renderer;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Bulldog.Scene;
using Bulldog.Utils;
using Shader = Bulldog.Renderer.Shader;
using Texture = Bulldog.Renderer.Texture;

namespace Bulldog.Core
{
    class Program
    {
        private static IWindow _window;
        private static GL _gl;

        private const string VertShaderSourcePath = "../../../src/Renderer/Shaders/shader.vert";
        private const string FragShaderSourcePath = "../../../src/Renderer/Shaders/shader.frag";
        private const string TexturePath = "../../../src/Scene/TestTexture.jpg";

        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;
        private static Shader _shader;
        //Create a texture object
        private static Texture _texture;

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


        
        
        private static void OnLoad()
        {
            //Set-up input context.
            IInputContext input = _window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            
            //Getting the opengl api for drawing to the screen.
            _gl = GL.GetApi(_window);
            
            //Creating buffers
            Console.WriteLine("Creating buffers...");
            //Initializing a vertex buffer that holds the vertex data.
            _vbo = new BufferObject<float>(_gl, TestQuad.Vertices, BufferTargetARB.ArrayBuffer);
            //Initializing a element buffer that holds the index data.
            _ebo = new BufferObject<uint>(_gl, TestQuad.Indices, BufferTargetARB.ElementArrayBuffer);
            //Creating a vertex array.
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);
            //Telling the VAO object how to lay out the attribute pointers
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
            Console.WriteLine("Buffers done.");
            
            //Creating a shader.
            Console.WriteLine("Compiling shaders...");
            _shader = new Shader(_gl, VertShaderSourcePath, FragShaderSourcePath);
            Console.WriteLine("Shaders Done.");
            
            //Load texture
            _texture = new Texture(_gl, TexturePath);
            
            // try loading
            ObjLoader myObj = new ObjLoader();
        }

        
        
        
        private static unsafe void OnRender(double obj)
        {
            //Here all rendering should be done.
            //Clear the color channel.
            _gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            //Bind the geometry and shader.
            _vao.Bind();
            _shader.Use();
            
            //Setting a uniform.
            //Bind a texture and and set the uTexture0 to use texture0.
            _texture.Bind(TextureUnit.Texture0);
            _shader.SetUniform("uTexture0", 0);

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