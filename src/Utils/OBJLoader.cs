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

        // ctor
        public ObjLoader()
        {
            const string filePath = "../../../src/Scene/suzanne.obj";
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            _model = importer.ImportFile(filePath, PostProcessPreset.TargetRealTimeMaximumQuality);
            string result = _model != null ? "Model Loaded." : "ERROR";
            Console.WriteLine(result);
        }
        
    }
}
