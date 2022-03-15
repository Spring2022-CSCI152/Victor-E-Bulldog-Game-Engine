//https://github.com/assimp/assimp-net/blob/master/AssimpNet.Sample/SimpleOpenGLSample.cs

using Assimp;
using Assimp.Configs;
using Silk.NET.OpenGL;
using Scene = Assimp.Scene;

namespace Bulldog.Utils
{
    public class ObjLoader
    {
        // mesh container
        private readonly Assimp.Scene _model;
        private readonly int _texId;
        public List<float> tempvert = new List<float>();
        public float[] verticies;
        public uint[] indicies;

        // ctor
        public ObjLoader()
        {
            const string filePath = "../../../src/Scene/suzanne.obj";
            
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            
            // load
            // ADD TRY CATCH HERE, MODEL COULD FAIL TO LOAD
            _model = importer.ImportFile(filePath, PostProcessPreset.TargetRealTimeMaximumQuality);
            string result = _model != null ? "Model Loaded." : "ERROR";
            Console.WriteLine(result);
            
            // myObj._model.Meshes[n].FaceCount
            // myObj._model.Meshes[n].Faces[n].Indicies[0..2]
            // myObj._model.Meshes[0].VertexCount
            // myObj._model.Meshes[0].Verticies[n].X|Y|Z
            // myObj._model.Meshes[0].Normals[n].X|Y|Z
            
            foreach (var vtx in _model.Meshes[0].Vertices)
            {
                tempvert.Add(vtx.X);
                tempvert.Add(vtx.Y);
                tempvert.Add(vtx.Z);
            }

            verticies = tempvert.ToArray();
            Console.WriteLine(tempvert);
            Console.WriteLine(verticies);
            indicies = _model.Meshes[0].GetUnsignedIndices();
        }
        
    }
}
