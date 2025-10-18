using System;
using System.Collections.Generic;
using System.Text;

namespace ObjLoader.Loader.Data.Elements
{
    public class Line
    {
        private readonly List<int> _indexes = new List<int>();

        public void AddIndexes(int[] Indexes)
        {
            _indexes.AddRange(Indexes);
        }

        public int this[int i]
        {
            get { return _indexes[i]; }
        }

        public int Count
        {
            get { return _indexes.Count; }
        }





    }
}
