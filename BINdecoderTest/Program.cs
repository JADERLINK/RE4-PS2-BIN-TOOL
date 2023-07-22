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
            Console.WriteLine("##BINdecoderTest##");
            Console.WriteLine($"##Version {BINdecoder.VERSION}##");

            if (args.Length >= 1 && File.Exists(args[0]) && new FileInfo(args[0]).Extension.ToUpper() == ".BIN")
            {
                bool ForceDefaultBinType = false;

                bool CreateDebugFiles = false;

                if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                {
                    ForceDefaultBinType = true;
                }

                if (args.Length >= 3 && args[2].ToUpper() == "TRUE")
                {
                    CreateDebugFiles = true;
                }

                Console.WriteLine(args[0]);
                try
                {
                    FileInfo fileInfo = new FileInfo(args[0]);
                    string baseName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

                    Stream stream = fileInfo.OpenRead();

                    var bin = BINdecoder.Decode(stream, args[0], ForceDefaultBinType, CreateDebugFiles);
                    BINdecoder.CreateObjMtl(bin, fileInfo.DirectoryName, baseName, baseName);
                    BINdecoder.CreateSMD(bin, fileInfo.DirectoryName, baseName, baseName);
                    BINdecoder.CreateIdxbin(bin, fileInfo.DirectoryName, baseName);

                    if (CreateDebugFiles)
                    {
                        BINdecoder.CreateDrawDistanceBoxObj(bin, fileInfo.DirectoryName, baseName);
                        BINdecoder.CreateScaleLimitBoxObj(bin, fileInfo.DirectoryName, baseName);
                    }
                 
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
