using Bulldog.Renderer;
using Silk.NET.OpenGL;
using Texture = Bulldog.Renderer.Texture;
using Assimp;
using Assimp.Configs;

namespace Bulldog.Core;

public class NuMesh
{
    // temporary containers
    private List<Vector3D> _vertexList = new List<Vector3D>();
    private List<Vector3D> _normalList = new List<Vector3D>();
    private List<Vector3D> _texCoordList = new List<Vector3D>();
    private List<uint> _indexList = new List<uint>();
    
    // model buffers
    private VertexArrayObject<float, uint> _vao;
    private BufferObject<float> _vertBuffer;
    private BufferObject<float> _normBuffer;
    private BufferObject<float> _txcdBuffer;
    private BufferObject<uint> _indexBuffer;
    
    enum BufferType
    {
        IndexBuffer = 0,
        VertexBuffer = 1,
        NormalBuffer = 2,
        TexCoordBuffer = 3,
        NumBuffers = 4
    }
    
    // class Mesh
    // {
    //     public uint NumIndices;
    //     public uint BaseVertex;
    //     public uint BaseIndex;
    //     
    //     public Texture _tex;
    // }

    public NuMesh(GL gl, string path)
    {
        // TODO: make LoadMeshes its own class
        LoadMeshes(path);
        InitBuffers(gl);
    }

    private void LoadMeshes(string path)
    {
        // init assimp
        var importer = new AssimpContext();
        importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
        
        // load model with assimp
        var model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
        var result = model != null ? $"{path} loaded successfully." : $"FAILED TO LOAD: {path}";
        Console.WriteLine(result);
        
        // parse assimp mesh data
        if (model != null)
        {
            // load each mesh in model
            foreach (var mesh in model.Meshes)
            {
                LoadSingleMesh(mesh);
            }
        }
        
        // print error if model didn't load
        else
        {
            Console.WriteLine("NuMesh failed to load model: " + path);
        }
    }

    private void LoadSingleMesh(Assimp.Mesh singleMesh)
    {
        // add this mesh's vertices to list
        if (singleMesh.HasNormals)
        {
            _vertexList.AddRange(singleMesh.Vertices);
        }
        
        // add this mesh's normals to list
        if (singleMesh.HasNormals)
        {
            _normalList.AddRange(singleMesh.Normals);
        }
        
        // add this mesh's tex-coords to list
        if (singleMesh.HasTextureCoords(0))
        {
            _texCoordList.AddRange(singleMesh.TextureCoordinateChannels[0]);
        }
        
        // add this mesh's indices to list
        _indexList.AddRange(singleMesh.GetUnsignedIndices());
    }
    
    private void InitBuffers(GL gl)
    {
        // NOTE: '3' is hardcoded because our _XLists are of type Vector3D
        
        // populate vertex buffer
        _vertBuffer = new BufferObject<float>(
            gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(3 * _vertexList.Count),
            new Span<float>(ExtractDataFromFloatVectorList(_vertexList).ToArray()) // TODO: CREATE AABB MAX/MIN FOR BOUNDING BOX
            );
        
        // populate normal buffer
        _normBuffer = new BufferObject<float>(
            gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(3 * _normalList.Count),
            new Span<float>(ExtractDataFromFloatVectorList(_normalList).ToArray())
        );
        
        // populate texCoord buffer
        _txcdBuffer = new BufferObject<float>(
            gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(3 * _texCoordList.Count),
            new Span<float>(ExtractDataFromFloatVectorList(_texCoordList).ToArray())
        );

        // populate index buffer
        _indexBuffer = new BufferObject<uint>(
            gl,
            new Span<uint>(_indexList.ToArray()),
            BufferTargetARB.ElementArrayBuffer
        );
        
        // create/configure vao
        // we're using a separate vbo for each attribute
        _vao = new VertexArrayObject<float, uint>(gl);
        
        _vertBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.VertexBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        _normBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.NormalBuffer, 3, VertexAttribPointerType.Float, 0, 0);

        _txcdBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.TexCoordBuffer, 3, VertexAttribPointerType.Float, 0, 0);

        _indexBuffer.Bind(); // NOTE: may be unnecessary?
    }
    
    /// <summary>
    /// Strips each X,Y,Z coordinate out of each <see cref="Vector3D"/> in <paramref name="vectorList"/>.
    /// </summary>
    /// <param name="vectorList"><see cref="List{T}"/> of <see cref="Vector3D"/> to be stripped.</param>
    /// <param name="map">Optional void function to run for each <see cref="Vector3D"/> in <paramref name="vectorList"/></param>
    /// <returns>A flat <see cref="List{T}"/> (T = float) of coordinates.</returns>
    private static List<float> ExtractDataFromFloatVectorList(List<Vector3D> vectorList, Action<Vector3D> map = null)
    {
        var temp = new List<float>();
        // build up list
        foreach (var vtx in vectorList)
        {
            temp.Add(vtx.X);
            temp.Add(vtx.Y);
            temp.Add(vtx.Z);
            // if map is not null, call it
            map?.Invoke(vtx);
        }
        return temp;
    }
    
    public void Render()
    {
        // TODO: implement render
        // call glDrawElementsBaseVertex for each mesh
        
        // basevertex = currMesh.VertexCount
        // baseindex = currMush.IndexCount
        // (void*)((sizeof uint) * baseIndex),
    }

    public void Dispose()
    {
        _vertBuffer.Dispose();
        _normBuffer.Dispose();
        _txcdBuffer.Dispose();
        _indexBuffer.Dispose();
        _vao.Dispose();
    }
}
