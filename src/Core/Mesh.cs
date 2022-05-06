using Bulldog.Renderer;
using Bulldog.Utils;
using Silk.NET.OpenGL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;
using Texture = Bulldog.Renderer.Texture;
// -> --> ==> >- *** == <=> !!! != !!! <=> == *** -< <== <-- <- \\
namespace Bulldog.core;

public class Mesh
{
    private readonly float[] _verts;
    private readonly float[] _txcds;
    private readonly float[] _norms;
    private readonly uint[] _indices;
    private List<Texture> _textures;

    // ReSharper disable once InconsistentNaming
    private static VertexArrayObject<float, uint> Vao;
    private static BufferObject<float> Vbo;
    private static BufferObject<uint> Ebo;
    private readonly GL _gl;

    public Mesh(GL gl, string path, string textureIn)
    {
        var myObj = new ObjLoader(path);
        var myTexture = new Texture(gl, textureIn);
        _gl = gl;
        _verts = myObj.Vertices;
        _txcds = myObj.TexCoords;
        _norms = myObj.Normals;
        _indices = myObj.Indices; ;
        _textures.Add(myTexture);
        // now that we have all the required data, set the vertex buffers and its attribute pointers.
        Init();
        myObj.Dispose();
        myTexture.Dispose();
    }

    public Mesh(GL gl, ObjLoader path, Texture textureIn)
    {
        _gl = gl;
        _verts = path.Vertices;
        _txcds = path.TexCoords;
        _norms = path.Normals;
        _indices = path.Indices;
        _textures = new List<Texture>();
        _textures.Add(textureIn);
        // now that we have all the required data, set the vertex buffers and its attribute pointers.
        Init();
    }

    public void Draw(Renderer.Shader shader, Texture textureIn)
    {
        unsafe
        {
            Vao.Bind();
            shader.Use();
            
            //Setting a uniform.
            //Bind a texture and and set the uTexture0 to use texture0.
            textureIn.Bind();
            shader.SetUniform("uTexture0", 0);
            
            //Draw the geometry.
            //_gl.DrawElements(PrimitiveType.Triangles, (uint) TestQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
            _gl.DrawElements(PrimitiveType.Triangles, (uint) _indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }


    private void Init()
    {
        var bufferSize = (nuint) (_verts.Length + _txcds.Length + _norms.Length);
        // create an empty buffer of proper size
        Vbo = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, bufferSize, null);
        // populate buffer with data
        Vbo.SetSubData(0, _verts);
        Vbo.SetSubData(_verts.Length, _txcds);
        Vbo.SetSubData(_verts.Length + _txcds.Length, _norms);
        // create index buffer
        Ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);
        // create _vao to store buffers
        Vao = new VertexArrayObject<float, uint>(_gl, Vbo, Ebo);
        // tell _vao how data is organized inside of _vbo
        Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
        Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 0, _verts.Length);
        Vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 0, _verts.Length + _txcds.Length);
    }

    public void Dispose()
    {
        Vbo.Dispose();
        Vbo.Dispose();
        Ebo.Dispose();
    }
}