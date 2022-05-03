using Bulldog.Renderer;
using Silk.NET.OpenGL;
using Texture = Bulldog.Renderer.Texture;
using Assimp;
using Assimp.Configs;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace Bulldog.Core;

public class NuMesh
{
    // GL context
    private readonly GL _gl;
    
    // model
    private Assimp.Scene _model;
    
    // supplemental mesh data container
    public List<MeshDataStruct> _meshData = new List<MeshDataStruct>();
    
    // temporary containers
    private List<Vector3D> _vertexList = new List<Vector3D>();
    private List<Vector3D> _normalList = new List<Vector3D>();
    private List<Vector3D> _texCoordList = new List<Vector3D>();
    public List<uint> _indexList = new List<uint>();
    public float[] _vertexArr;
    public float[] _normalArr;
    public float[] _texCoordArr;
    
    // model buffers
    private VertexArrayObject<float, uint> _vao;
    private List<BufferObject<float>> _bufferList;
    // private BufferObject<float> _vertBuffer;
    // private BufferObject<float> _normBuffer;
    // private BufferObject<float> _txcdBuffer;
    private BufferObject<uint> _indexBuffer;

    enum BufferType
    {
        VertexBuffer = 0,
        TexCoordBuffer = 1,
        NormalBuffer = 2,
        IndexBuffer = 3,
        NumBuffers = 4
    }
    
    public struct MeshDataStruct
    {
        public uint IndexCount;
        public uint[] Indices;
        public int BaseVertex;
        // public uint BaseIndex;
        // Texture _tex;
    }

    public NuMesh(GL gl, string path)
    {
        // TODO: make LoadMeshes its own class
        _gl = gl;
        LoadMeshes(path);
        InitBuffers();
    }

    private void LoadMeshes(string path)
    {
        // init assimp
        var importer = new AssimpContext();
        importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
        
        // load model with assimp
        _model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
        var result = _model != null ? $"{path} loaded successfully." : $"FAILED TO LOAD: {path}";
        Console.WriteLine(result);
        
        // parse assimp mesh data
        if (_model != null)
        {
            int currVertexCount = 0;
            // load each mesh in model
            foreach (var mesh in _model.Meshes)
            {
                LoadSingleMesh(mesh, currVertexCount);
                currVertexCount += mesh.VertexCount;
            }

            _vertexArr = ExtractDataFromFloatVectorList(_vertexList).ToArray();
            _texCoordArr = ExtractDataFromFloatVectorList(_texCoordList).ToArray();
            _normalArr = ExtractDataFromFloatVectorList(_normalList).ToArray();
        }
        
        // print error if model didn't load
        else
        {
            Console.WriteLine("NuMesh failed to load model: " + path);
        }
    }

    private void LoadSingleMesh(Assimp.Mesh singleMesh, int currVertexCount)
    {
        // init supplemental data
        var currMeshData = new MeshDataStruct();
        
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
        var indexList = singleMesh.GetUnsignedIndices();
        _indexList.AddRange(indexList);
        
        // set supplemental data
        currMeshData.IndexCount = (uint) indexList.Length;
        currMeshData.Indices = indexList;
        currMeshData.BaseVertex = currVertexCount;
        _meshData.Add(currMeshData);
    }
    
    private void InitBuffers()
    {
        // populate vertex buffer
        // _vertBuffer = new BufferObject<float>(
        _bufferList[(int)BufferType.VertexBuffer] = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(_vertexList.Count),
            _vertexArr // TODO: CREATE AABB MAX/MIN FOR BOUNDING BOX
        );
        
        // populate texCoord buffer
        // _txcdBuffer = new BufferObject<float>(
        _bufferList[(int)BufferType.TexCoordBuffer] = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(_texCoordList.Count),
            _texCoordArr
        );
        
        // populate normal buffer
        // _normBuffer = new BufferObject<float>(
        _bufferList[(int)BufferType.NormalBuffer] = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)(_normalList.Count),
            _normalArr
        );
        
        // populate index buffer
        _indexBuffer = new BufferObject<uint>(
            _gl,
            _indexList.ToArray(),
            BufferTargetARB.ElementArrayBuffer
        );

        // create/configure vao
        // we're using a separate vbo for each attribute
        _vao = new VertexArrayObject<float, uint>(_gl, _bufferList, _indexBuffer);
        
        // vertex
        _bufferList[(int)BufferType.VertexBuffer].Bind();
        _vao.VertexAttributePointer((uint)BufferType.VertexBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // tex-coord
        _bufferList[(int)BufferType.TexCoordBuffer].Bind();
        _vao.VertexAttributePointer((uint)BufferType.TexCoordBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // normal
        _bufferList[(int)BufferType.NormalBuffer].Bind();
        _vao.VertexAttributePointer((uint)BufferType.NormalBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // index
        _indexBuffer.Bind(); // NOTE: possibly unneeded
        
        _vao.Unbind();
    }
    
    /// <summary>
    /// Strips each X,Y,Z coordinate out of each <see cref="Vector3D"/> in <paramref name="vectorList"/>.
    /// </summary>
    /// <param name="vectorList"><see cref="List{T}"/> of <see cref="Vector3D"/> to be stripped.</param>
    /// <param name="map">Optional void function to run for each <see cref="Vector3D"/> in <paramref name="vectorList"/></param>
    /// <returns>A flat <see cref="List{T}"/> (T = float) of coordinates.</returns>
    private static Span<float> ExtractDataFromFloatVectorList(List<Vector3D> vectorList, Action<Vector3D> map = null)
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
        return temp.ToArray();
    }
    
    public unsafe void Render()
    {
        _vao.Bind();
        // render every mesh in model
        foreach (var mesh in _meshData)
        {
            fixed (void* data = mesh.Indices)
            {
                _gl.DrawElementsBaseVertex(
                    PrimitiveType.Triangles,
                    mesh.IndexCount,
                    GLEnum.UnsignedInt,
                    data,
                    mesh.BaseVertex
                );
            }
        }
        // _vao.Unbind();
    }

    public void Dispose()
    {
        // _vertBuffer.Dispose();
        // _normBuffer.Dispose();
        // _txcdBuffer.Dispose();
        foreach (var vbo in _bufferList)
        {
            vbo.Dispose();
        }
        _indexBuffer.Dispose();
        _vao.Dispose();
    }
}
