using Bulldog.Renderer;
using Bulldog.Utils;
using Silk.NET.OpenGL;
using Shader = Bulldog.Renderer.Shader;
using Texture = Bulldog.Renderer.Texture;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;
using Vector3D = Assimp.Vector3D;

namespace Bulldog.Core;

public class Model : IDisposable
{
    // defines non-index buffer types
    enum BufferType
    {
        VertexBuffer = 0,
        TexCoordBuffer = 1,
        NormalBuffer = 2,
        NumBuffers = 3
    }
    
    // GL context
    private readonly GL _gl;
    
    // loads model data
    private readonly ModelLoader _model;
    
    // creates model texture
    private readonly Texture _texture;
    
    // model buffers
    private VertexArrayObject<float, uint> _vao;
    // private List<BufferObject<float>> _bufferList;
    private BufferObject<float> _vertBuffer;
    private BufferObject<float> _normBuffer;
    private BufferObject<float> _txcdBuffer;
    private BufferObject<uint> _indexBuffer;
    
    // Model bounding box AABB
    public readonly Vector3D MinVertex;
    public readonly Vector3D MaxVertex;
    
    
    
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="gl">OpenGL context.</param>
    /// <param name="modelPath">File path to 3D model file.</param>
    /// <param name="texturePath">File path to texture image file.</param>
    public Model(GL gl, string modelPath, string texturePath)
    {
        // set OpenGL context
        _gl = gl;
        // create texture
        _texture = new Texture(_gl, texturePath);
        // load model data
        _model = new ModelLoader(modelPath);
        // set bound box
        MaxVertex = _model.MaxVertex;
        MinVertex = _model.MinVertex;
        // populate buffers with model data
        InitBuffers();
    }

    /// <summary>
    /// Initializes VBOs/VAO with model data.
    /// </summary>
    private void InitBuffers()
    {
        // populate vertex buffer
        _vertBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_model.Vertices.Length,
            _model.Vertices
        );
        
        // populate texCoord buffer
        _txcdBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_model.TexCoords.Length,
            _model.TexCoords
        );
        
        // populate normal buffer
        _normBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_model.Normals.Length,
            _model.Normals
        );
        
        // populate index buffer
        _indexBuffer = new BufferObject<uint>(
            _gl,
            BufferTargetARB.ElementArrayBuffer,
            (nuint)_model.Indices.Length,
            _model.Indices
        );

        // create/configure vao
        // we're using a separate vbo for each attribute
        // NOTE: '3' is hard-coded because assimp exclusively uses Vector3D, which has 3 components.
        //       Also, strides and offsets are 0 because we aren't interleaving vertex types
        _vao = new VertexArrayObject<float, uint>(_gl);
        
        // vertex
        _vertBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.VertexBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // tex-coord
        _txcdBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.TexCoordBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // normal
        _normBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.NormalBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // index
        _indexBuffer.Bind();
        
        // prevent outside modification
        _vao.Unbind();
    }

    /// <summary>
    /// Renders the model to the screen.
    /// </summary>
    /// <param name="shader">Shader to use when rendering.</param>
    /// <param name="callback">Optional callback function to run before rendering. Usually used for setting uniforms.</param>
    public unsafe void Render(Shader shader, Action<Shader> callback = null)
    {
        _vao.Bind();
        shader.Use();
        
        // use texture
        _texture.Bind(TextureUnit.Texture0);
        shader.SetUniform("uTexture0", 0);
        
        // perform callback
        callback?.Invoke(shader);
        
        // render every mesh in model
        foreach (var mesh in _model.MeshDataList)
        {
            _gl.DrawElementsBaseVertex(
                PrimitiveType.Triangles,
                mesh.IndexCount,
                GLEnum.UnsignedInt,
                (void*)(sizeof(uint) * mesh.BaseIndex),
                (int)mesh.BaseVertex
            );
        }
        
        // prevent outside modification
        _vao.Unbind();
    }

    /// <summary>
    /// Frees buffer data responsibly
    /// </summary>
    public void Dispose()
    {
        _vertBuffer.Dispose();
        _normBuffer.Dispose();
        _txcdBuffer.Dispose();
        _indexBuffer.Dispose();
        _vao.Dispose();
        _texture.Dispose();
    }
}
