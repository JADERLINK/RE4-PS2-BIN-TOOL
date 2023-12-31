using ObjLoader.Loader.Common;
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class TextureParser : TypeParserBase, ITextureParser
    {
        private readonly ITextureDataStore _textureDataStore;

        public TextureParser(ITextureDataStore textureDataStore)
        {
            _textureDataStore = textureDataStore;
        }

        protected override string Keyword
        {
            get { return "vt"; }
        }

        public override void Parse(string line)
        {
            string[] parts = line.Split(' ');

            float u = parts[0].ParseInvariantFloat();
            float v = parts[1].ParseInvariantFloat();

            var texture = new Texture(u, v);
            _textureDataStore.AddTexture(texture);
        }
    }
}