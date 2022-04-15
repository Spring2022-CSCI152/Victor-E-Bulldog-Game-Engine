// using Bulldog.Renderer;
// using Silk.NET.Input;
// using Silk.NET.Maths;
// using Silk.NET.OpenGL;
// using Silk.NET.Windowing;
//
// using Bulldog.Scene;
// using Bulldog.Utils;
// using Shader = Bulldog.Renderer.Shader;
// using Texture = Bulldog.Renderer.Texture;
//
// namespace Bulldog.Core
// {
//     class Program
//     {
//         private static IWindow _window;
//         private static GL _gl;
//
//         private const string VertShaderSourcePath = "../../../src/Renderer/Shaders/shader2.vert";
//         private const string FragShaderSourcePath = "../../../src/Renderer/Shaders/shader2.frag";
//         private const string TexturePath = "../../../src/Scene/uv-test.png";
//         private const string ObjPath = "../../../src/Scene/suzanne.obj";
//
//         private static BufferObject<float> _vbo;
//         private static BufferObject<uint> _ebo;
//         private static VertexArrayObject<float, uint> _vao;
//         private static Shader _shader;
//         //Create a texture object
//         private static Texture _texture;
//         // mesh
//         private static ObjLoader _myObj;
//
//         private static void Main()
//         {
//             //Create a window.
//             var options = WindowOptions.Default;
//             options.Size = new Vector2D<int>(800, 600);
//             options.Title = "LearnOpenGL with Silk.NET";
//
//             _window = Window.Create(options);
//
//             //Assign events.
//             _window.Load += OnLoad;
//             _window.Update += OnUpdate;
//             _window.Render += OnRender;
//
//             //Run the window.
//             _window.Run();
//         }
//
//
//         
//         
//         private static void OnLoad()
//         {
//             //Set-up input context.
//             IInputContext input = _window.CreateInput();
//
//             foreach (var keyboard in input.Keyboards)
//             {
//                 keyboard.KeyDown += KeyDown;
//             }
//             
//             //Getting the opengl api for drawing to the screen.
//             _gl = GL.GetApi(_window);
//             
//             //Creating a shader.
//             Console.WriteLine("Compiling shaders...");
//             _shader = new Shader(_gl, VertShaderSourcePath, FragShaderSourcePath);
//             Console.WriteLine("Shaders Done.");
//             
//             //Load texture
//             _texture = new Texture(_gl, TexturePath);
//             
//             // load obj
//             _myObj = new ObjLoader(ObjPath);
//             
//             // create buffers
//             Console.WriteLine("Creating buffers...");
//             var verts = _myObj.Vertices;
//             var txcds = _myObj.TexCoords;
//             var norms = _myObj.Normals;
//             var bufferSize = (nuint) (verts.Length + txcds.Length + norms.Length);
//             // create an empty buffer of proper size
//             _vbo = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, bufferSize, null);
//             // populate buffer with data
//             _vbo.SetSubData(0, verts);
//             _vbo.SetSubData(verts.Length, txcds);
//             _vbo.SetSubData(verts.Length + txcds.Length, norms);
//             // create index buffer
//             _ebo = new BufferObject<uint>(_gl, _myObj.Indices, BufferTargetARB.ElementArrayBuffer);
//             // create vao to store buffers
//             _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);
//             // tell vao how data is organized inside of vbo
//             _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
//             _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 0, verts.Length);
//             _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 0, verts.Length + txcds.Length);
//             
//             Console.WriteLine("Buffers done.");
//         }
//
//
//         private static unsafe void OnRender(double obj)
//         {
//             //Here all rendering should be done.
//             //Clear the color channel.
//             _gl.Clear((uint) ClearBufferMask.ColorBufferBit);
//
//             //Bind the geometry and shader.
//             _vao.Bind();
//             _shader.Use();
//             
//             //Setting a uniform.
//             //Bind a texture and and set the uTexture0 to use texture0.
//             _texture.Bind(TextureUnit.Texture0);
//             _shader.SetUniform("uTexture0", 0);
//             
//             //Draw the geometry.
//             //_gl.DrawElements(PrimitiveType.Triangles, (uint) TestQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
//             _gl.DrawElements(PrimitiveType.Triangles, (uint) _myObj.Indices.Length, DrawElementsType.UnsignedInt, null);
//         }
//
//         
//         
//         
//         private static void OnUpdate(double obj)
//         {
//             //Here all updates to the program should be done.
//         }
//
//         
//         
//         
//         private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
//         {
//             //Check to close the window on escape.
//             if (arg2 == Key.Escape)
//             {
//                 _window.Close();
//             }
//         }
//     }
// }

using System.Numerics;
using Silk.NET;
using Bulldog.Renderer;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using System.Drawing;
using Bulldog.Utils;
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
        private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        private static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private static Vector3 CameraUp = Vector3.UnitY;
        private static Vector3 CameraDirection = Vector3.Zero;
        private static float CameraYaw = -90f;
        private static float CameraPitch = 0f;
        private static float CameraZoom = 45f;

        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;

        private static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";
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
            
        }

        private static void OnRender(double obj)  //draw each frame
        {
            unsafe
            {
                _gl.Enable(EnableCap.DepthTest);
                _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

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
                //Use elapsed time to convert to radians to allow our cube to rotate over time
                var difference = (float) (_window.Time * 100);

                var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference));
                var view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraZoom), Width / Height, 0.1f, 100.0f);

                _shader.SetUniform("uModel", model);
                _shader.SetUniform("uView", view);
                _shader.SetUniform("uProjection", projection);

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