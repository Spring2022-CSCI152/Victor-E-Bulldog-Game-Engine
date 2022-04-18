using Assimp;
using Bulldog.Renderer;
using Bulldog.Utils;
using Silk.NET.OpenGL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;
using Texture = Bulldog.Renderer.Texture;
// -> --> ==> *** == <=> != !!! 
namespace Bulldog.core;

public class Mesh
{
    private ObjLoader _myObj;
    private float[] verts;
    private float[] txcds;
    private float[] norms;
    
    private List<Texture> Textures = new List<Texture>();

    // ReSharper disable once InconsistentNaming
    private static VertexArrayObject<float, uint> Vao;
    private static BufferObject<float> Vbo;
    private static BufferObject<uint> Ebo;
    private readonly GL _gl;

    public Mesh(GL gl, string path)
    {
        _gl = gl;
        _myObj = new ObjLoader(path);
        verts = _myObj.Vertices;
        txcds = _myObj.TexCoords;
        norms = _myObj.Normals;
        // now that we have all the required data, set the vertex buffers and its attribute pointers.
        Init();
    }

    public Mesh(GL gl, ObjLoader path, Texture textureIn)
    {
        _gl = gl;
        _myObj = path;
        verts = path.Vertices;
        txcds = path.TexCoords;
        norms = path.Normals;
        Textures.Add(textureIn);
        // now that we have all the required data, set the vertex buffers and its attribute pointers.
        Init();
    }

    public void Draw(Renderer.Shader shader, Texture texture)
    {
        unsafe
        {
            Vao.Bind();
            shader.Use();
            
            //Setting a uniform.
            //Bind a texture and and set the uTexture0 to use texture0.
            texture.Bind();
            shader.SetUniform("uTexture0", 0);
            
            //Draw the geometry.
            //_gl.DrawElements(PrimitiveType.Triangles, (uint) TestQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
            _gl.DrawElements(PrimitiveType.Triangles, (uint) _myObj.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }


    private void Init()
    {
        var bufferSize = (nuint) (verts.Length + txcds.Length + norms.Length);
        // create an empty buffer of proper size
        Vbo = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, bufferSize, null);
        // populate buffer with data
        Vbo.SetSubData(0, verts);
        Vbo.SetSubData(verts.Length, txcds);
        Vbo.SetSubData(verts.Length + txcds.Length, norms);
        // create index buffer
        Ebo = new BufferObject<uint>(_gl, _myObj.Indices, BufferTargetARB.ElementArrayBuffer);
        // create _vao to store buffers
        Vao = new VertexArrayObject<float, uint>(_gl, Vbo, Ebo);
        // tell _vao how data is organized inside of _vbo
        Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
        Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 0, verts.Length);
        Vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 0, verts.Length + txcds.Length);
    }

    public void Dispose()
    {
        Vbo.Dispose();
        Vbo.Dispose();
        Ebo.Dispose();
        _myObj.Dispose();
    }
}