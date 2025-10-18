using System.Collections.Generic;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface IDataStore 
    {
        IList<Vertex> Vertices { get; }
        IList<Texture> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Group> Groups { get; }
        IList<string> MtlLibs { get; }
    }

    public interface IDataStoreMtl
    {
        IList<Material> Materials { get; }
    }

}