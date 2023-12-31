using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ObjLoader.Loader.Loaders
{

    public class MtlLoaderFactory : IMtlLoaderFactory
    {
        public IMtlLoader Create()
        {
            var dataStore = new DataStoreMtl();
            var materialParser = new MaterialParser(dataStore);
            return new MtlLoader(dataStore, materialParser);
        }
    }
}
