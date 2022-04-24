//https://github.com/assimp/assimp-net/blob/master/AssimpNet.Sample/SimpleOpenGLSample.cs

using Assimp;
using Assimp.Configs;
//using Silk.NET.Maths;

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
                var result = model != null ? $"{path} loaded successfully." : $"ERROR: {path}";
                Console.WriteLine(result);
                
                //  get array of model's vertices, while updating _minVertex and _maxVertex
                if (model != null && model.Meshes[0].HasVertices)
                {
                    Vertices = ExtractDataFromFloatVectorList(model.Meshes[0].Vertices, UpdateMinMax).ToArray();
                }
                
                // get array of model's UVs
                if (model != null && model.Meshes[0].HasTextureCoords(0))
                {
                    TexCoords = ExtractDataFromFloatVectorList(model.Meshes[0].TextureCoordinateChannels[0]).ToArray();
                }
                
                //  get array of the model's normals
                if (model != null && model.Meshes[0].HasNormals)
                {
                    Normals = ExtractDataFromFloatVectorList(model.Meshes[0].Normals).ToArray();
                }
                
                // get array of indices
                Indices = model?.Meshes[0].GetUnsignedIndices();
            }
            
            // catch exception on model load failure
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        
        public void Dispose()
        {
            // if texture is stored, dispose of it properly
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
