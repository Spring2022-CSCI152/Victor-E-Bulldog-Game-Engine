using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using Bulldog.Utils;

namespace Bulldog.Core;

internal static class Program
{
    // window and IO
    private static IWindow _window;
    private const int Width = 960;
    private const int Height = 720;
    private static IKeyboard primaryKeyboard;
    //Used to track change in mouse movement to allow for moving of the Camera
    private static Vector2 LastMousePosition;
    
    // OpenGL
    private static GL _gl;
    private static Shader _shader;
    
    // mesh
    private static Model _model;

    // file paths
    private const string VertShaderSourcePath = "../../../src/Core/shader.vert";
    private const string FragShaderSourcePath = "../../../src/Core/shader.frag";
    private const string TexturePath = "../../../src/Scene/uv-test.png";
    // private const string ObjPath = "../../../res/index-testing.obj";
    // private const string ObjPath = "../../../src/Scene/suzanne.obj";
    private const string ObjPath = "../../../res/CLASSROOM.obj";
    // private const string ObjPath = "../../../res/SuzanneTri.obj";
    // private const string ObjPath = "../../../res/CupOBJ/Cup.obj";

    // camera properties
    private static class CamProps
    {
        //Setup the camera's location, directions, and movement speed
        public static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        public static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        public static Vector3 CameraUp = Vector3.UnitY;
        public static Vector3 CameraDirection = Vector3.Zero;
        public static float CameraYaw = -90f;
        public static float CameraPitch = 0f;
        public static float CameraZoom = 45f;
        // For Camera Speed
        public static float CameraRotateSpeed = 20f;
        public static float CameraTranslateSpeed = 10f;
    }
    
    
    
    
    /// <summary>
    /// Main Function.
    /// </summary>
    private static void Main()
    {
        // initialize window
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Funtoosh Victor E Bulldog";
        _window = Window.Create(options);

        // initialize window callback functions
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClose;

        // main loop
        _window.Run();
    }

    /// <summary>
    /// Window load callback function. Runs on window open. Used to initialize application.
    /// </summary>
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
        
        // load model
        _model = new Model(_gl, ObjPath, TexturePath);
    }
    
    /// <summary>
    /// Window render callback function. Runs every frame. Used for drawing images to screen.
    /// </summary>
    /// <param name="obj">???</param>
    private static void OnRender(double obj)  //draw each frame
    {
        // OpenGL frame setup
        _gl.Enable(EnableCap.DepthTest);
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        // render model
        _model.Render(_shader, UpdateShader);
    }

    /// <summary>
    /// Sets uniforms related to position, translation, and perspective
    /// </summary>
    /// <param name="shader">Shader whose uniforms will be modified</param>
    private static void UpdateShader(Shader shader)
    {
        var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(CamProps.CameraYaw)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(CamProps.CameraPitch));
        var view = Matrix4x4.CreateLookAt(CamProps.CameraPosition, CamProps.CameraPosition + CamProps.CameraFront, CamProps.CameraUp);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CamProps.CameraZoom), Width / Height, 0.1f, 200.0f);

        shader.SetUniform("uModel", model);
        shader.SetUniform("uView", view);
        shader.SetUniform("uProjection", projection);
    }

    /// <summary>
    /// Window update callback function. Runs every frame. Used to execute game logic.
    /// </summary>
    /// <param name="obj">???</param>
    private static void OnUpdate(double obj)
    {
        
    }

    /// <summary>
    /// Window close callback function. Runs when window closes. Used to free application resources.
    /// </summary>
    private static void OnClose()
    {
        _shader.Dispose();
        _model.Dispose();
    }
    
    // IO
    
    private static void KeyDown(IKeyboard arg1, Key key, int arg3)
    {
        //Check to close the window on escape.
        if (key == Key.Escape)
        {
            _window.Close();
        }

        switch (key)
        {
            case Key.W:
                CamProps.CameraPitch += CamProps.CameraRotateSpeed;
                break;
            case Key.S:
                CamProps.CameraPitch -= CamProps.CameraRotateSpeed;
                break;
            case Key.A:
                CamProps.CameraYaw += CamProps.CameraRotateSpeed;
                break;
            case Key.D:
                CamProps.CameraYaw -= CamProps.CameraRotateSpeed;
                break;
            case Key.F:
                CamProps.CameraPosition.Z += CamProps.CameraTranslateSpeed;
                break;
            case Key.R:
                CamProps.CameraPosition.Z -= CamProps.CameraTranslateSpeed;
                break;
        }
    }

    private static void WhileKeyDown()
    {
        
    }
}
