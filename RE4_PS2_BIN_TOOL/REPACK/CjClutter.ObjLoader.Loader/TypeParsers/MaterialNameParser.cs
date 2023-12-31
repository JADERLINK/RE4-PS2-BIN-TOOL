using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class MaterialNameParser : TypeParserBase, IMaterialNameParser
    {
        private readonly IMaterialNameDataStore _materialNameDataStore;

        public MaterialNameParser(IMaterialNameDataStore materialNameDataStore)
        {
            _materialNameDataStore = materialNameDataStore;
        }

        protected override string Keyword
        {
            get { return "usemtl"; }
        }

        public override void Parse(string line)
        {
            _materialNameDataStore.PushMaterial(line);
        }
    }
}