//https://github.com/assimp/assimp-net/blob/master/AssimpNet.Sample/SimpleOpenGLSample.cs

using System.Diagnostics.SymbolStore;
using Assimp;
using Assimp.Configs;
using Silk.NET.OpenGL;
using Scene = Assimp.Scene;

namespace Bulldog.Utils
{
    public class ObjLoader :IDisposable
    {
        // modular variables
        private uint _handle;
        private GL _gl;
        
        // mesh container
        private readonly Assimp.Scene _model;
        private readonly int _texId;
        public float[] _verticies;
        public float[] _uvs;
        public float[] _normals;
        public uint[] _indicies;
        //public readonly _vbo;

        // ctor
        public unsafe ObjLoader(/*GL gl, */string path)
        {
            
            
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            
            try
            {
                // load model
                _model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
                string result = _model != null ? "Model Loaded." : "ERROR";
                Console.WriteLine(result);
                
                //  build a list of the model's verticies
                if (_model.Meshes[0].HasVertices)
                {
                    List<float> tempvert = new List<float>();
                    foreach (var vtx in _model.Meshes[0].Vertices)
                    {
                        tempvert.Add(vtx.X);
                        tempvert.Add(vtx.Y);
                        tempvert.Add(vtx.Z);
                    }
                    // convert list to array
                    _verticies = tempvert.ToArray();
                }
                
                // get array of UVs
                if (_model.Meshes[0].HasTextureCoords(0))
                {
                    //_model.Meshes[0].TextureCoordinateChannels[0].
                }
                
                //  build a list of the model's normals
                if (_model.Meshes[0].HasNormals)
                {
                    List<float> tempnorm = new List<float>();
                    foreach (var vtx in _model.Meshes[0].Normals)
                    {
                        tempnorm.Add(vtx.X);
                        tempnorm.Add(vtx.Y);
                        tempnorm.Add(vtx.Z);
                    }
                    // convert list to array
                    _normals = tempnorm.ToArray();
                }
                
                // get array of indicies
                _indicies = _model.Meshes[0].GetUnsignedIndices();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        
        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texture.
            //_gl.DeleteTexture(_handle);
        }
    }
}
