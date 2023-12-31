using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public interface IObjLoader
    {
        LoadResult Load(StreamReader lineStream);
    }
}