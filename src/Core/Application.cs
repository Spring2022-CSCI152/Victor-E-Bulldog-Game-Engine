using System.Numerics;
using Bulldog.ECS;
using Bulldog.Renderer;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using Bulldog.Utils;
using Texture = Bulldog.Renderer.Texture;

namespace Bulldog.Core
{
    class Program
    {
        private static World ThisWorld;
        
        private const string VertShaderSourcePath = "../../../src/Renderer/Shaders/shader.vert";
        private const string FragShaderSourcePath = "../../../src/Renderer/Shaders/shader.frag";
        private const string TextureSourcePath = "../../../src/Scene/TestTexture.jpg";
        private static IWindow _window;
        private static GL Gl;
        private static IKeyboard primaryKeyboard;
        private const int Width = 800;
        private const int Height = 700;
        

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
        
        private static readonly float[] Vertices =
        {
            //X    Y      Z     U   V
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f
        };
        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        
        

        private static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "Victor E. Bulldogs Cube!";
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
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            Gl = GL.GetApi(_window);
            ObjRenderer.CreateObj(Gl, ThisWorld, Indices, Vertices, VertShaderSourcePath, FragShaderSourcePath, TextureSourcePath);
        }

        private static void OnRender(double obj)  //draw each frame
        {
            
         /*   ThisWorld.Entities[0].ComponentPool[0].Vao.Bind();
            ThisWorld.Entities[0].ComponentPool[0].Texture.Bind();
            ThisWorld.Entities[0].ComponentPool[0].Shader.Bind();
            ThisWorld.Entities[0].ComponentPool[0].Shader.SetUniform("uTexture0", 0);*/
          

            //Use elapsed time to convert to radians to allow our cube to rotate over time
            

          /*Shader.SetUniform("uModel", model);
            Shader.SetUniform("uView", view);
            Shader.SetUniform("uProjection", projection);*/

            //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
           
        }

        private static void OnUpdate(double obj)
        {
           
        }

        private static void OnClose()
        {
           /* Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            */
         
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