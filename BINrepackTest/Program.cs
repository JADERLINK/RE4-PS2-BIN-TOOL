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
            Console.WriteLine("## BINrepackTest ##");
            Console.WriteLine($"## Version {BINrepack.VERSION} ##");
            Console.WriteLine("## By JADERLINK and HardRain ##");

            if (args.Length >= 1 && File.Exists(args[0]) 
                && (new FileInfo(args[0]).Extension.ToUpper() == ".OBJ"
                || new FileInfo(args[0]).Extension.ToUpper() == ".SMD"))

            {
                var fileinfo = new FileInfo(args[0]);
                var idxbinPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".IDXBIN";

                Console.WriteLine(args[0]);
                if (File.Exists(idxbinPath))
                {
                    try
                    {
                        string binPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".BIN";
                        if (fileinfo.Extension.ToUpper().Contains("OBJ"))
                        {
                            BINrepack.RepackObj(idxbinPath, fileinfo.FullName, binPath);
                        }
                        else if (fileinfo.Extension.ToUpper().Contains("SMD"))
                        {
                            BINrepack.RepackSMD(idxbinPath, fileinfo.FullName, binPath);
                        }
                        
                      
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }

                }
                else
                {
                    Console.WriteLine(idxbinPath + " does not exist");
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
