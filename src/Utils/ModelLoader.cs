using Assimp;
using Assimp.Configs;

namespace Bulldog.Utils;

public class ModelLoader
{
    // holds supplemental mesh data
    public struct MeshDataStruct
    {
        public uint IndexCount;
        public uint BaseIndex;
        public uint BaseVertex;
    }

    // temporary containers
    private readonly List<Vector3D> _vertexList = new List<Vector3D>();
    private readonly List<Vector3D> _normalList = new List<Vector3D>();
    private readonly List<Vector3D> _texCoordList = new List<Vector3D>();
    private readonly List<uint> _indexList = new List<uint>();
    
    // supplemental mesh data container
    public readonly List<MeshDataStruct> MeshDataList = new List<MeshDataStruct>();
    
    // public containers
    public readonly float[] Vertices;
    public readonly float[] Normals;
    public readonly float[] TexCoords;
    public readonly uint[] Indices;
    
    // AABB bounding box
    public Vector3D MinVertex = new Vector3D(float.MaxValue,float.MaxValue,float.MaxValue);
    public Vector3D MaxVertex = new Vector3D(float.MinValue,float.MinValue,float.MinValue);
    
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="path">Path to 3D model file.</param>
    public ModelLoader(string path)
    {
        // init assimp
        var importer = new AssimpContext();
        importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));

        // load model with assimp
        try
        {
            // assimp loads model into var model
            var model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
            // print debug string
            var result = model != null ? $"{path} loaded successfully." : $"FAILED TO LOAD: {path}";
            Console.WriteLine(result);

            // parse assimp mesh data
            if (model != null)
            {
                uint currVertexCount = 0;
                uint currIndexCount = 0;

                // load each mesh in model
                foreach (var currMesh in model.Meshes)
                {
                    // ASSERT: _model.MeshCount == MeshDataList.Count
                    // MeshDataList's number of elements MUST EQUAL _model.MeshCount from assimp
                    var currMeshData = new MeshDataStruct();

                    // accumulate vertices
                    if (currMesh.HasVertices)
                    {
                        _vertexList.AddRange(currMesh.Vertices);
                    }
                    
                    // accumulate tex-coords
                    if (currMesh.HasTextureCoords(0))
                    {
                        _texCoordList.AddRange(currMesh.TextureCoordinateChannels[0]);
                    }
                    
                    // accumulate normals
                    if (currMesh.HasNormals)
                    {
                        _normalList.AddRange(currMesh.Normals);
                    }

                    // accumulate indices
                    var currMeshIndices = currMesh.GetUnsignedIndices();
                    _indexList.AddRange(currMeshIndices);

                    // set supplemental data
                    currMeshData.IndexCount = (uint)currMeshIndices.Length;
                    currMeshData.BaseIndex = currIndexCount;
                    currMeshData.BaseVertex = currVertexCount;
                    // add to supplemental data list
                    MeshDataList.Add(currMeshData);

                    // set up next iteration's counts
                    currVertexCount += (uint)currMesh.VertexCount;
                    currIndexCount += currMeshData.IndexCount;
                }

                // convert lists to public arrays
                Vertices = ExtractDataFromFloatVectorList(_vertexList).ToArray();
                TexCoords = ExtractDataFromFloatVectorList(_texCoordList).ToArray();
                Normals = ExtractDataFromFloatVectorList(_normalList).ToArray();
                Indices = _indexList.ToArray();
                
                // destroy temporary lists
                _vertexList.Clear();
                _texCoordList.Clear();
                _normalList.Clear();
                _indexList.Clear();
            }

            // print error if model didn't load
            else { Console.WriteLine("Model failed to load model: " + path); }
        }
        // throw if assimp failed to load
        catch (Exception e)
        {
            Console.WriteLine("Exception: Model failed to load model: " + e);
            throw;
        }
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
    
    /// <summary>
    /// Updates _minVertex and/or _maxVertex with testVert if they are smaller or larger respectively.
    /// </summary>
    /// <param name="testVert">Vector3D to test _minVertex and _maxVertex against</param>
    private void UpdateMinMax(Vector3D testVert)
    {
        if (IsLessThan(testVert, MinVertex))
        {
            MinVertex = testVert;
        }
        if (IsGreaterThan(testVert, MaxVertex))
        {
            MaxVertex = testVert;
        }
    }
    
    /// Compares 2 Vector3D's
    private static bool IsGreaterThan(Vector3D lhs, Vector3D rhs)
    {
        return
        (
            lhs.X > rhs.X &&
            lhs.Y > rhs.Y &&
            lhs.Z > rhs.Z
        );
    }
    
    /// Compares 2 Vector3D's
    private static bool IsLessThan(Vector3D lhs, Vector3D rhs)
    {
        return
        (
            lhs.X < rhs.X &&
            lhs.Y < rhs.Y &&
            lhs.Z < rhs.Z
        );
    }
}
