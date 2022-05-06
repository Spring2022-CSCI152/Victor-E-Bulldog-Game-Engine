using System.Numerics;
using Bulldog.ECS;
using Bulldog.ECS.Components;
using Bulldog.ECS.Systems;
using Bulldog.Renderer;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using Bulldog.Utils;
using Mesh = Bulldog.core.Mesh;
using Texture = Bulldog.Renderer.Texture;

namespace Bulldog.Core
{
    class Program
    {
        private static IWindow _window;
        private static GL _gl;
        private static IKeyboard primaryKeyboard;
        private const int Width = 800;
        private const int Height = 700;
        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;
        private static Texture _texture;
        private static Shader _shader;
        // mesh
        private static ObjLoader _myObj;

        private const string VertShaderSourcePath = "../../../src/Core/shader.vert";
        private const string FragShaderSourcePath = "../../../src/Core/shader.frag";
        private const string TexturePath = "../../../src/Scene/uv-test.png";
        private const string ObjPath = "../../../src/Scene/suzanne.obj";

        //Setup the camera's location, directions, and movement speed
        private static Camera Camera;

        private static World World;
        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;

        private static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";
            options.PreferredDepthBufferBits = 24;
            _window = Window.Create(options);

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;

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
            
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
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
            // create _vao to store buffers
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);
            // tell _vao how data is organized inside of _vbo
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
            _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 0, verts.Length);
            _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 0, verts.Length + txcds.Length);
            
            Console.WriteLine("Buffers done.");
            World = new World();
            World.AddSystem(new RenderSystem());
            World.CreateEntity("E1",new MeshComponent()
            {
                Vao = _vao, Texture = _texture, Shader = _shader, _gl = _gl, Indices = _myObj.Indices
            });
            Camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, Width / Height);
            
        }
        

        private static void OnRender(double obj)  //draw each frame
        {
            unsafe
            {
                _gl.Enable(EnableCap.DepthTest);

                _vao.Bind();
                _shader.Use();
                _texture.Bind();
                
                _shader.SetUniform("uTexture0", 0);
                _gl.DrawElements(PrimitiveType.Triangles, (uint) _myObj.Indices.Length, DrawElementsType.UnsignedInt, null);
                
                var difference = (float) (_window.Time * 100);

                _shader.SetUniform("uModel", Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(25f)));
                _shader.SetUniform("uView", Camera.GetViewMatrix());
                _shader.SetUniform("uProjection", Camera.GetProjectionMatrix());

                //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
                //_gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }

        private static void OnUpdate(double obj)
        {
           
        }

        private static void OnClose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
        }
        
        private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default) { LastMousePosition = position; }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                Camera.ModifyDirection(xOffset, yOffset);
            }
        }

        private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            Camera.ModifyZoom(scrollWheel.Y);
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