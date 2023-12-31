using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjLoader.Loader.Loaders
{
    public interface IMtlLoaderFactory
    {
        IMtlLoader Create();
    }
}
