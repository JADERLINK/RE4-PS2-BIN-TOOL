using System;
using System.Collections.Generic;
using ObjLoader.Loader.Common;
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.TypeParsers
{
    public class LineParser : TypeParserBase, ILineParser
    {
        private readonly ILineGroup _lineGroup;

        public LineParser(ILineGroup lineGroup)
        {
            _lineGroup = lineGroup;
        }

        protected override string Keyword
        {
            get { return "l"; }
        }

        public override void Parse(string line)
        {
            var s_Indexes = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> indexes = new List<int>();
            for (int i = 0; i < s_Indexes.Length; i++)
            {
                var vertexIndex = s_Indexes[i].ParseInvariantInt();
                indexes.Add(vertexIndex);
            }

            Line _line = new Line();
            _line.AddIndexes(indexes.ToArray());
            _lineGroup.AddLine(_line);
        }

     
    }
}