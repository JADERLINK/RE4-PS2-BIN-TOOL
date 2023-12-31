using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public interface IMtlLoader
    {
        LoadResultMtl Load(StreamReader lineStream);
    }

}