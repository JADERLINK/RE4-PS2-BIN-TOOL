using System.IO;

namespace ObjLoader.Loader.Loaders
{
    // adaptado

    public class MaterialStreamProvider : IMaterialStreamProvider
    {
        public MaterialStreamProvider() 
        { 
        
        }

        public MaterialStreamProvider(string DiretoryBase) 
        {
            this.DiretoryBase = DiretoryBase;
            if (!this.DiretoryBase.EndsWith("\\"))
            {
                this.DiretoryBase += "\\";
            }
        }

        private string DiretoryBase = "";

        public Stream Open(string materialFilePath)
        {
            return File.Open(DiretoryBase + materialFilePath, FileMode.Open, FileAccess.Read);
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider
    {
        public Stream Open(string materialFilePath)
        {
            return null;
        }
    }
}