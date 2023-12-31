using System.Collections.Generic;
using System.IO;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.Loaders
{
    public class ObjLoader : LoaderBase, IObjLoader
    {
        private readonly IDataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = new List<ITypeParser>();

        private readonly List<string> _unrecognizedLines = new List<string>();

        public ObjLoader(
            IDataStore dataStore,
            IFaceParser faceParser, 
            ILineParser lineParser, 
            INormalParser normalParser, 
            ITextureParser textureParser, 
            IVertexParser vertexParser,
            IMtlLibParser mtlLibParser,
            IGroupNameParser groupNameParser,
            IMaterialNameParser materialNameParser,
            IObjectNameParser objectNameParser)
        {
            _dataStore = dataStore;
            SetupTypeParsers(
                vertexParser,
                faceParser,
                lineParser,
                normalParser,
                textureParser,
                groupNameParser,
                mtlLibParser,
                materialNameParser,
                objectNameParser);
        }

        private void SetupTypeParsers(params ITypeParser[] parsers)
        {
            foreach (var parser in parsers)
            {
                _typeParsers.Add(parser);
            }
        }

        protected override void ParseLine(string keyword, string data)
        {
            foreach (var typeParser in _typeParsers)
            {
                if (typeParser.CanParse(keyword))
                {
                    typeParser.Parse(data);
                    return;
                }
            }

            _unrecognizedLines.Add(keyword + " " + data);
        }

        public LoadResult Load(StreamReader lineStream)
        {
            StartLoad(lineStream);

            return CreateResult();
        }

        private LoadResult CreateResult()
        {
            var result = new LoadResult
                             {
                                 Vertices = _dataStore.Vertices,
                                 Textures = _dataStore.Textures,
                                 Normals = _dataStore.Normals,
                                 Groups = _dataStore.Groups,
                                 Mtllibs = _dataStore.MtlLibs
                             };
            return result;
        }
    }
}