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
            Console.WriteLine("BINdecoderTest Version A.1.0.0.0");

            if (args.Length >= 1 && File.Exists(args[0]) && new FileInfo(args[0]).Extension.ToUpper() == ".BIN")
            {
                bool itIsScenario = false;

                if (args.Length >= 2 && args[1].ToUpper() =="TRUE")
                {
                    itIsScenario = true;
                }

                Console.WriteLine(args[0]);
                try
                {
                    BINdecoder.Decode(args[0], itIsScenario);
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
