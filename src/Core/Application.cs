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

        private const string VertShaderSourcePath = "../../../src/Renderer/Shaders/shader2.vert";
        private const string FragShaderSourcePath = "../../../src/Renderer/Shaders/shader2.frag";
        private const string TexturePath = "../../../src/Scene/uv-test.png";
        private const string ObjPath = "../../../src/Scene/suzanne.obj";

        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;
        private static Shader _shader;
        //Create a texture object
        private static Texture _texture;
        // mesh
        private static ObjLoader _myObj;

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

            foreach (var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += KeyDown;
            }
            
            //Getting the opengl api for drawing to the screen.
            _gl = GL.GetApi(_window);
            
            //Creating a shader.
            Console.WriteLine("Compiling shaders...");
            _shader = new Shader(_gl, VertShaderSourcePath, FragShaderSourcePath);
            Console.WriteLine("Shaders Done.");
            
            //Load texture
            _texture = new Texture(_gl, TexturePath);
            
            // load obj
            _myObj = new ObjLoader(ObjPath);
            
            // create buffers
            Console.WriteLine("Creating buffers...");
            var verts = _myObj.Vertices;
            var txcds = _myObj.TexCoords;
            var norms = _myObj.Normals;
            var bufferSize = (nuint) (verts.Length + txcds.Length + norms.Length);
            // create an empty buffer of proper size
            _vbo = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, bufferSize, null);
            // populate buffer with data
            _vbo.SetSubData(0, verts);
            _vbo.SetSubData(verts.Length, txcds);
            _vbo.SetSubData(verts.Length + txcds.Length, norms);
            // create index buffer
            _ebo = new BufferObject<uint>(_gl, _myObj.Indices, BufferTargetARB.ElementArrayBuffer);
            // create vao to store buffers
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);
            // tell vao how data is organized inside of vbo
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
            _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 0, verts.Length);
            _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 0, verts.Length + txcds.Length);
            
            Console.WriteLine("Buffers done.");
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
            //_gl.DrawElements(PrimitiveType.Triangles, (uint) TestQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
            _gl.DrawElements(PrimitiveType.Triangles, (uint) _myObj.Indices.Length, DrawElementsType.UnsignedInt, null);
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