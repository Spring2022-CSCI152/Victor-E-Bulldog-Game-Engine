using Silk.NET.OpenGL;
using Bulldog.ECS;
using Bulldog.ECS.Componets;
using System.Numerics;
using Bulldog.Utils;
using Silk.NET.Windowing;

namespace Bulldog.Renderer;


public class ObjRenderer
{
    private static GL Gl;
    private static BufferObject<float> Vbo;
    private static BufferObject<uint> Ebo;
    private static VertexArrayObject<float, uint> Vao;
    private static Texture Texture;
    private static Shader Shader;
    private static IWindow _window;
    
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

    public ObjRenderer()
    {
        /*
        Entity ws = entities.Find(WorldScene);
        Entity 
        Component thisMesh = var.GetComponent<Component>(mesh);
        */
        //store mesh, texture, and translation data internally
       
    }
    public static void Init()
    {
        Gl.Enable(EnableCap.DepthTest);
        Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        Vao.Bind();
        Texture.Bind();
        Shader.Use();
        Shader.SetUniform("uTexture0", 0);
    }
    
    public static void CreateObj(GL Gl, World ThisWorld, uint[] Indices, float[] Vertices, string VertShaderSourcePath, string FragShaderSourcePath, string TextureSourcePath)
    {
   
        Mesh ThisMesh = new Mesh();
        Entity ThisEntity = new Entity();

        Ebo = new BufferObject<uint>(Gl,Indices, BufferTargetARB.ElementArrayBuffer);
        Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
        ThisMesh.Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);
        
        ThisMesh.Shader = new Shader(Gl, VertShaderSourcePath, FragShaderSourcePath);
        ThisMesh.Texture = new Texture(Gl, TextureSourcePath);

        ThisEntity.AddComponent(ThisMesh);
        ThisWorld.Register(ThisEntity);

    }

    public static void PlaceCamera()
    {
        var view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraZoom), Width / Height, 0.1f, 100.0f);
        Shader.SetUniform("uView", view);
        Shader.SetUniform("uProjection", projection);
    }
    public static void DrawObj()
    {
        Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    /*
     public static void RotateObj()
    {
        //Use elapsed time to convert to radians to allow our cube to rotate over time
        var difference = (float) (_window.Time * 100);
        var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference));
        Shader.SetUniform("uModel", model);
    }
*/
    public static void Shutdown()
    {
        
    }

    public static void BeginScene()
    {
        //drawing everything on screen
    }

    public static void EndScene()
    {
        //swap and clear buffer
    }
        
        
    
}