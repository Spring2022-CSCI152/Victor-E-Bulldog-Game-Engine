//https://github.com/assimp/assimp-net/blob/master/AssimpNet.Sample/SimpleOpenGLSample.cs

using Assimp;
using Assimp.Configs;
// using Silk.NET.OpenGL;
// using Scene = Assimp.Scene;

namespace Bulldog.Utils
{
    public class ObjLoader :IDisposable
    {
        // private readonly int _texId;
        public readonly float[] Vertices;
        public readonly float[] TexCoords;
        public readonly float[] Normals;
        public readonly uint[] Indices;
        //public readonly _vbo;
        private Vector3D _minVertex = new Vector3D(float.MaxValue,float.MaxValue,float.MaxValue);
        private Vector3D _maxVertex = new Vector3D(float.MinValue,float.MinValue,float.MinValue);
        
        

        /// <summary>
        ///  Loads the mesh data contained in an OBJ file.
        /// </summary>
        /// <param name="path">Path to OBJ file</param>
        public ObjLoader(/*GL gl, */string path)
        {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            
            try
            {
                // load model
                var model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
                var result = model != null ? $"{path} loaded successfully." : $"FAILED TO LOAD: {path}";
                Console.WriteLine(result);
                
                // temporary containers
                List<float> vertexList = new List<float>();
                List<float> texCoordList = new List<float>();
                List<float> normalList = new List<float>();
                List<uint> indexList = new List<uint>();
                uint maxIndex = 0;

                if (model != null)
                {
                    // load every mesh in the model file
                    foreach (var mesh in model.Meshes)
                    {
                        //  get mesh's vertices, while updating _minVertex and _maxVertex
                        if (mesh.HasVertices)
                        {
                            vertexList.AddRange(
                                ExtractDataFromFloatVectorList(mesh.Vertices, UpdateMinMax)
                                );
                        }
                
                        // get mesh's UVs
                        if (mesh.HasTextureCoords(0))
                        {
                            texCoordList.AddRange(
                                ExtractDataFromFloatVectorList(mesh.TextureCoordinateChannels[0])
                                );
                        }
                
                        // get mesh's normals
                        if (mesh.HasNormals)
                        {
                            normalList.AddRange(
                                ExtractDataFromFloatVectorList(mesh.Normals)
                                );
                        }
                        
                        // get mesh's indices
                        var indices = new List<uint>(mesh.GetUnsignedIndices());
                        // add previous mesh's maxIndex to this mesh's indices
                        if (maxIndex != 0)
                        {
                            for (int i = 0; i < indices.Count; i++)
                            {
                                indices[i] += maxIndex;
                            }
                        }
                        // update maxIndex with this mesh's max index
                        maxIndex = (uint) Math.Floor(vertexList.Count / 3.0);
                        // maxIndex = (uint) mesh.VertexCount;
                        // append data
                        indexList.AddRange(indices);
                    }
                    
                    // get arrays
                    Vertices = vertexList.ToArray();
                    TexCoords = texCoordList.ToArray();
                    Normals = normalList.ToArray();
                    Indices = indexList.ToArray();
                }
            }
            
            // catch exception on model load failure
            catch (Exception e)
            {
                Console.WriteLine("OBJLoader failed to load model: " + e.Message);
                throw;
            }
        }
        
        public void Dispose()
        {
            
        }
        
        
        
        
        // helper functions
        
        // TODO: make this function type-agnostic
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

        /// <summary>
        /// Updates _minVertex and/or _maxVertex with testVert if they are smaller or larger respectively.
        /// </summary>
        /// <param name="testVert">Vector3D to test _minVertex and _maxVertex against</param>
        private void UpdateMinMax(Vector3D testVert)
        {
            if (IsLessThan(testVert, _minVertex))
            {
                _minVertex = testVert;
            }
            if (IsGreaterThan(testVert, _maxVertex))
            {
                _maxVertex = testVert;
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
}
