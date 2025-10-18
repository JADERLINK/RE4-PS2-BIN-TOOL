using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;
using ObjLoader.Loader.TypeParsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjLoader.Loader.TypeParsers
{
    public class ObjectNameParser : TypeParserBase, IObjectNameParser
    {
        private readonly IObjectNameDataStore _objectNameDataStore;

        public ObjectNameParser(IObjectNameDataStore objectNameDataStore)
        {
            _objectNameDataStore = objectNameDataStore;
        }

        protected override string Keyword
        {
            get { return "o"; }
        }

        public override void Parse(string line)
        {
            _objectNameDataStore.PushObject(line);
        }
    }
}
