using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class MtlLibParser : TypeParserBase, IMtlLibParser
    {
        private readonly IMtlLibDataStore _materialLibraryDataStore;

        public MtlLibParser(IMtlLibDataStore materialLibraryDataStore)
        {
            _materialLibraryDataStore = materialLibraryDataStore;
        }

        protected override string Keyword
        {
            get { return "mtllib"; }
        }

        public override void Parse(string line)
        {
            _materialLibraryDataStore.AddMtlLib(line);
        }
    }
}