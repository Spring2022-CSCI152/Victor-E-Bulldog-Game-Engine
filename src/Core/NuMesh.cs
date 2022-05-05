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
    private readonly List<Vector3D> _vertexList = new List<Vector3D>();
    private readonly List<Vector3D> _normalList = new List<Vector3D>();
    private readonly List<Vector3D> _texCoordList = new List<Vector3D>();
    private readonly List<uint> _indexList = new List<uint>();
    
    // public containers
    public float[] _vertexArr;
    public float[] _normalArr;
    public float[] _texCoordArr;
    public uint[] _indexArr;
    
    // model buffers
    private VertexArrayObject<float, uint> _vao;
    // private List<BufferObject<float>> _bufferList;
    private BufferObject<float> _vertBuffer;
    private BufferObject<float> _normBuffer;
    private BufferObject<float> _txcdBuffer;
    private BufferObject<uint> _indexBuffer;

    // defines buffer types
    enum BufferType
    {
        VertexBuffer = 0,
        TexCoordBuffer = 1,
        NormalBuffer = 2,
        NumBuffers = 3
    }
    
    // holds supplemental mesh data
    public struct MeshDataStruct
    {
        public uint IndexCount;
        public uint[] Indices;
        public int BaseVertex;
        public uint BaseIndex;
        // Texture _tex;
    }

    
    
    
    // constructor
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
        try
        {
            // assimp loads model into _model
            _model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
            var result = _model != null ? $"{path} loaded successfully." : $"FAILED TO LOAD: {path}";
            Console.WriteLine(result);

            // parse assimp mesh data
            if (_model != null)
            {
                int currVertexCount = 0;
                uint currIndexCount = 0;

                // load each mesh in model
                // foreach (var mesh in _model.Meshes)
                for (int i = 0; i < _model.MeshCount; i++)
                {
                    LoadSingleMesh(_model.Meshes[i], currVertexCount, currIndexCount);
                    
                    currVertexCount += _model.Meshes[i].VertexCount;
                    currIndexCount += _meshData[i].IndexCount;
                }

                // convert lists to public arrays
                _vertexArr = ExtractDataFromFloatVectorList(_vertexList).ToArray();
                _texCoordArr = ExtractDataFromFloatVectorList(_texCoordList).ToArray();
                _normalArr = ExtractDataFromFloatVectorList(_normalList).ToArray();
                _indexArr = _indexList.ToArray();
            }

            // print error if model didn't load
            else { Console.WriteLine("NuMesh failed to load model: " + path); }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: NuMesh failed to load model: " + e);
            throw;
        }
    }

    private void LoadSingleMesh(Assimp.Mesh singleMesh, int currVertexCount, uint currIndexCount)
    {
        // init supplemental data
        var currMeshData = new MeshDataStruct();
        
        // add this mesh's vertices to list
        if (singleMesh.HasVertices)
        {
            _vertexList.AddRange(singleMesh.Vertices);
        }
        
        // add this mesh's tex-coords to list
        if (singleMesh.HasTextureCoords(0))
        {
            _texCoordList.AddRange(singleMesh.TextureCoordinateChannels[0]);
        }
        
        // add this mesh's normals to list
        if (singleMesh.HasNormals)
        {
            _normalList.AddRange(singleMesh.Normals);
        }
        
        // add this mesh's indices to list
        var indexList = singleMesh.GetUnsignedIndices();
        _indexList.AddRange(indexList);
        
        // set supplemental data
        currMeshData.IndexCount = (uint) indexList.Length;
        currMeshData.BaseVertex = currVertexCount;
        currMeshData.BaseIndex = currIndexCount;
        currMeshData.Indices = indexList;
        _meshData.Add(currMeshData);
    }
    
    private void InitBuffers()
    {
        // populate vertex buffer
        // _bufferList[(int)BufferType.VertexBuffer] = new BufferObject<float>(
        _vertBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_vertexArr.Length,
            _vertexArr // TODO: CREATE AABB MAX/MIN FOR BOUNDING BOX
        );
        
        // populate texCoord buffer
        // _bufferList[(int)BufferType.TexCoordBuffer] = new BufferObject<float>(
        _txcdBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_texCoordArr.Length,
            _texCoordArr
        );
        
        // populate normal buffer
        // _bufferList[(int)BufferType.NormalBuffer] = new BufferObject<float>(
        _normBuffer = new BufferObject<float>(
            _gl,
            BufferTargetARB.ArrayBuffer,
            (nuint)_normalArr.Length,
            _normalArr
        );
        
        // populate index buffer
        _indexBuffer = new BufferObject<uint>(
            _gl,
            BufferTargetARB.ElementArrayBuffer,
            (nuint)_indexArr.Length,
            _indexArr
        );

        // create/configure vao
        // we're using a separate vbo for each attribute
        _vao = new VertexArrayObject<float, uint>(_gl);
        
        // vertex
        // _bufferList[(int)BufferType.VertexBuffer].Bind();
        _vertBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.VertexBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // tex-coord
        // _bufferList[(int)BufferType.TexCoordBuffer].Bind();
        _txcdBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.TexCoordBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // normal
        // _bufferList[(int)BufferType.NormalBuffer].Bind();
        _normBuffer.Bind();
        _vao.VertexAttributePointer((uint)BufferType.NormalBuffer, 3, VertexAttribPointerType.Float, 0, 0);
        
        // index
        _indexBuffer.Bind();
        
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
    
    
    // renders the model
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
        
        _vao.Unbind();
    }

    public void Dispose()
    {
        _vertBuffer.Dispose();
        _normBuffer.Dispose();
        _txcdBuffer.Dispose();
        // foreach (var vbo in _bufferList)
        // {
        //     vbo.Dispose();
        // }
        _indexBuffer.Dispose();
        _vao.Dispose();
    }
}
