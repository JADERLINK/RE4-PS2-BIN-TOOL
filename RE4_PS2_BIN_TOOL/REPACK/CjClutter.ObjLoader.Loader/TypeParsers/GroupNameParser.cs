using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class GroupNameParser : TypeParserBase, IGroupNameParser
    {
        private readonly IGroupNameDataStore _groupNameDataStore;

        public GroupNameParser(IGroupNameDataStore groupNameDataStore)
        {
            _groupNameDataStore = groupNameDataStore;
        }

        protected override string Keyword
        {
            get { return "g"; }
        }

        public override void Parse(string line)
        {
            _groupNameDataStore.PushGroup(line);
        }
    }
}