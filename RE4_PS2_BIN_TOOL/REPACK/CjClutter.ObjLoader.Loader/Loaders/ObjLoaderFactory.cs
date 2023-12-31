using System.IO;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers;

namespace ObjLoader.Loader.Loaders
{
    public class ObjLoaderFactory : IObjLoaderFactory
    {
        public IObjLoader Create()
        {
            var dataStore = new DataStore();
            
            var faceParser = new FaceParser(dataStore);
            var lineParser = new LineParser(dataStore);
        
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);
            var mtlLibParser = new MtlLibParser(dataStore);
            var groupNameParser = new GroupNameParser(dataStore);
            var materialNameParser = new MaterialNameParser(dataStore);
            var objectNameParser = new ObjectNameParser(dataStore);

            return new ObjLoader(dataStore, faceParser, lineParser, normalParser, textureParser, vertexParser, mtlLibParser, groupNameParser, materialNameParser, objectNameParser);
        }
    }
}