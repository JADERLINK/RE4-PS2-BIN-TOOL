using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BINrepackTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.WriteLine("BINrepackTest Version A.1.0.0.0");

            if (args.Length >= 1 && File.Exists(args[0]) && new FileInfo(args[0]).Extension.ToUpper() == ".IDXBIN")
            {
                var fileinfo = new FileInfo(args[0]);
                string objPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - 6) + "obj";

                Console.WriteLine(args[0]);
                if (File.Exists(objPath))
                {
                    try
                    {
                        string binPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - 6) + "bin";
                        BINrepack.Repack(fileinfo.FullName, objPath, binPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }

                }
                else
                {
                    Console.WriteLine(objPath + " does not exist");
                }


            }
            else
            {
                Console.WriteLine("no arguments or invalid file");
            }

            Console.WriteLine("End");
        }
    }
}
