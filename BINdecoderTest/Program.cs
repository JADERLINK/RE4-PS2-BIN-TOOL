using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BINdecoderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.WriteLine("BINdecoderTest Version A.1.0.0.1");

            if (args.Length >= 1 && File.Exists(args[0]) && new FileInfo(args[0]).Extension.ToUpper() == ".BIN")
            {
                bool ForceDefaultBinType = false;

                if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                {
                    ForceDefaultBinType = true;
                }

                Console.WriteLine(args[0]);
                try
                {
                    FileInfo fileInfo = new FileInfo(args[0]);
                    string baseName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

                    Stream stream = fileInfo.OpenRead();

                    var bin = BINdecoder.Decode(stream, args[0], ForceDefaultBinType);
                    BINdecoder.CreateObjMtl(bin, fileInfo.DirectoryName, baseName, baseName);
                    BINdecoder.CreateIdxbin(bin, fileInfo.DirectoryName, baseName);
                    BINdecoder.CreateDrawDistanceBoxObj(bin, fileInfo.DirectoryName, baseName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                
            }
            else 
            {
                Console.WriteLine("No arguments or invalid file");
            }

            Console.WriteLine("End");
        }
    }
}
