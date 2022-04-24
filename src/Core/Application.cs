using System.Numerics;
using Bulldog.core;
using Bulldog.Utils;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Shader = Bulldog.Renderer.Shader;
using Texture = Bulldog.Renderer.Texture;

namespace Bulldog.Core
{
    class Program
    {
        private static IWindow Window;
        private static GL Gl;
        private static IKeyboard PrimaryKeyboard;
        private const int Width = 800;
        private const int Height = 700;
        private static Texture Texture;
        private static Shader Shader;
        // mesh
        private static ObjLoader MyObj;
        private static Mesh MyMesh;

        private const string VertShaderSourcePath = "../../../src/Core/shader.vert";
        private const string FragShaderSourcePath = "../../../src/Core/shader.frag";
        private const string TexturePath = "../../../src/Scene/uv-test.png";
        private const string ObjPath = "../../../src/Scene/suzanne.obj";

        //Setup the camera's location, directions, and movement speed
        private static Camera Camera;

        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;

        private static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";
            options.PreferredDepthBufferBits = 24;
            Window = Silk.NET.Windowing.Window.Create(options);

            Window.Load += OnLoad;
            Window.Update += OnUpdate;
            Window.Render += OnRender;
            Window.Closing += OnClose;

            Window.Run();
        }
        
        private static void OnLoad()
        {
            //Set-up input context.
            IInputContext input = Window.CreateInput();

            foreach (var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += KeyDown;
            }
            
            //Getting the opengl api for drawing to the screen.
            Gl = GL.GetApi(Window);
            Camera = new Camera();
            
            //Creating a shader.
            Console.WriteLine("Compiling shaders...");
            Shader = new Shader(Gl, "shader.vert", "shader.frag");
            Console.WriteLine("Shaders Done.");
            
            //Load texture
            Texture = new Texture(Gl, TexturePath);
            
            // load obj
            MyObj = new ObjLoader(ObjPath);
            MyMesh = new Mesh(Gl, MyObj, Texture);
            
            // create buffers
            Console.WriteLine("Buffers done.");
            
        }

        private static void OnRender(double obj)  //draw each frame
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            //Bind the geometry and shader.
            MyMesh.Draw(Shader);
            //Use elapsed time to convert to radians to allow our cube to rotate over time
            var difference = (float) (Window.Time * 100);

            //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
            //_gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        private static void OnUpdate(double obj)
        {
           
        }

        private static void OnClose()
        {
            MyMesh.Dispose();
        }
        
        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            //Check to close the window on escape.
            if (arg2 == Key.Escape)
            {
                Window.Close();
            }
        }
    }
}