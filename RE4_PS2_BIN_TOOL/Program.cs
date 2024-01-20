using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_PS2_BIN_TOOL
{
    class Program
    {
        public const string VERSION = "B.1.4.0.1 (2024-01-20)";

        public static string headerText()
        {
            return "# RE4_PS2_BIN_TOOL" + Environment.NewLine +
                   "# By JADERLINK and HardRain" + Environment.NewLine +
                  $"# Version {VERSION}";
        }

        static void Main(string[] args)
        {
            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-PS2-BIN-TOOL");
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                Actions(args);
            }
            else
            {
                Console.WriteLine("The file does not exist;");
            }

            Console.WriteLine("End");
        }

        private static void Actions(string[] args) 
        {
            FileInfo fileInfo = new FileInfo(args[0]);
            Console.WriteLine("File: " + fileInfo.Name);

            string baseDirectory = fileInfo.DirectoryName + "\\";
            string baseName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            string Extension = fileInfo.Extension.ToUpper();

            // modo extract
            if (Extension == ".BIN")
            {
                bool createTxt2 = false;

                bool CreateDebugFiles = false;

                if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                {
                    createTxt2 = true;
                }

                if (args.Length >= 3 && args[2].ToUpper() == "TRUE")
                {
                    CreateDebugFiles = true;
                }

                try
                {
                    Stream stream = fileInfo.OpenRead();
                    ALL.AltTextWriter txt2 = new ALL.AltTextWriter(Path.Combine(baseDirectory, baseName + ".Debug.txt2"), createTxt2);
                    var bin = EXTRACT.BINdecoder.Decode(stream, 0, out _ , txt2);
                    stream.Close();
                    txt2.Close();

                    EXTRACT.OutputFiles.CreateOBJ(bin, baseDirectory, baseName);
                    EXTRACT.OutputFiles.CreateSMD(bin, baseDirectory, baseName);
                    EXTRACT.OutputFiles.CreateIdxbin(bin, baseDirectory, baseName);

                    ALL.IdxMaterial material = ALL.IdxMaterialParser.Parser(bin);
                    EXTRACT.OutputMaterial.CreateIdxMaterial(material, baseDirectory, baseName);

                    var idxMtl = ALL.IdxMtlParser.Parser(material, baseName);
                    EXTRACT.OutputMaterial.CreateMTL(idxMtl, baseDirectory, baseName);

                    if (CreateDebugFiles)
                    {
                        EXTRACT.Debug.CreateDrawDistanceBoxObj(bin, baseDirectory, baseName);
                        EXTRACT.Debug.CreateScaleLimitBoxObj(bin, baseDirectory, baseName);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }

            }

            //mode repack
            else if (Extension == ".OBJ" || Extension == ".SMD")
            {
                string idxbinPath = baseDirectory + baseName + ".IDXBIN";
                string binPath = baseDirectory + baseName + ".BIN";

                if (File.Exists(idxbinPath))
                {
                    try
                    {
                        ALL.IdxMaterial material = null;

                        // obtem o arquivo do segundo parametro, que deve ser o arquivo MTL ou idxmaterial

                        FileInfo fileInfo2 = null;
                        if (args.Length >= 2)
                        {
                            if (File.Exists(args[1]))
                            {
                                fileInfo2 = new FileInfo(args[1]);
                                Console.WriteLine("File2: " + fileInfo2.Name);
                            }
                            else
                            {
                                Console.WriteLine("The second parameter file does not exist;");
                                return;
                            }
                        }
                        else
                        {
                            string MtlFile = baseDirectory + baseName + ".MTL";
                            if (File.Exists(MtlFile))
                            {
                                fileInfo2 = new FileInfo(MtlFile);
                                Console.WriteLine("MTL File: " + fileInfo2.Name);
                            }
                            else
                            {
                                Console.WriteLine("The MTL file does not exist;");
                                return;
                            }
                          
                        }

                        string Extension2 = fileInfo2.Extension.ToUpper();

                        // verifica o arquivo do segundo parametro
                        if (Extension2 == ".MTL")
                        {
                            ALL.IdxMtl idxMtl = null;

                            Stream mtlFile = fileInfo2.OpenRead();
                            REPACK.MtlLoad.Load(mtlFile, out idxMtl);
                            REPACK.MtlConverter.Convert(idxMtl, out material);
                            mtlFile.Close();
                        }
                        else if (Extension2 == ".IDXMATERIAL")
                        {
                            Stream idxmaterialFile = fileInfo2.OpenRead();
                            material = ALL.IdxMaterialLoad.Load(idxmaterialFile);
                            idxmaterialFile.Close();
                        }
                        else 
                        {
                            Console.WriteLine("The extension of the second parameter is not valid;");
                            return;
                        }


                        if (Extension == ".OBJ")
                        {

                            REPACK.BINrepackOBJ.RepackOBJ(idxbinPath, fileInfo.FullName, binPath, material);
                        }
                        else if (Extension == ".SMD")
                        {

                            REPACK.BINrepackSMD.RepackSMD(idxbinPath, fileInfo.FullName, binPath, material);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }

                }
                else
                {
                    Console.WriteLine(idxbinPath + " does not exist;");
                }

            }

            else
            {
                Console.WriteLine("Invalid file format;");
            }
        }



    }
}
