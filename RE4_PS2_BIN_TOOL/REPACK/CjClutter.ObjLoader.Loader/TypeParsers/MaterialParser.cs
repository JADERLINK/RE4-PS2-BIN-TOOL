using ObjLoader.Loader.TypeParsers.Interfaces;
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.DataStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjLoader.Loader.TypeParsers
{
    public class MaterialParser : IMaterialParser
    {
        private readonly IMaterialDataStore _materialDataStore;

        public MaterialParser(IMaterialDataStore materialDataStore)
        {
            _materialDataStore = materialDataStore;
        }

        public void AddMaterial(Material material)
        {
            _materialDataStore.Push(material);
        }
    }
}
