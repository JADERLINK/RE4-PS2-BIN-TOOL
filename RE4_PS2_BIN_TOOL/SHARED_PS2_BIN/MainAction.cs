using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_BIN
{
    public static class MainAction
    {
        public static void MainContinue(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine(SHARED_TOOLS.Shared.HeaderText());

            bool usingBatFile = false;
            int start = 0;
            if (args.Length > 0 && args[0].ToLowerInvariant() == "-bat")
            {
                usingBatFile = true;
                start = 1;
            }

            for (int i = start; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        FileInfo fileInfo1 = new FileInfo(args[i]);
                        string file1Extension = fileInfo1.Extension.ToUpperInvariant();
                        Console.WriteLine("File: " + fileInfo1.Name);
                        ContinueActions(fileInfo1, file1Extension);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + Environment.NewLine + ex);
                    }
                }
                else
                {
                    Console.WriteLine("File specified does not exist: " + args[i]);
                }

            }

            if (args.Length == 0)
            {
                Console.WriteLine("How to use: drag the file to the executable.");
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-PS2-BIN-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Finished!!!");
                if (!usingBatFile)
                {
                    Console.WriteLine("Press any key to close the console.");
                    Console.ReadKey();
                }
            }

        }


        private static void ContinueActions(FileInfo fileInfo, string Extension)
        {
            //diretorio, e nome do arquivo
            string baseDirectory = fileInfo.DirectoryName;
            string baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            // modo extract
            if (Extension == ".BIN")
            {
                Stream stream = fileInfo.OpenRead();
                var bin = EXTRACT.BINdecoder.Decode(stream, 0, out _);
                stream.Close();

                EXTRACT.OutputFiles.CreateOBJ(bin, baseDirectory, baseName);
                EXTRACT.OutputFiles.CreateSMD(bin, baseDirectory, baseName);
                EXTRACT.OutputFiles.CreateIdxps2bin(bin, baseDirectory, baseName);

                ALL.IdxMaterial material = ALL.IdxMaterialParser.Parser(bin);
                EXTRACT.OutputMaterial.CreateIdxMaterial(material, baseDirectory, baseName);

                var idxMtl = ALL.IdxMtlParser.Parser(material, baseName);
                EXTRACT.OutputMaterial.CreateMTL(idxMtl, baseDirectory, baseName);

                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "Createtxt2.txt")))
                {
                    EXTRACT.Debug.Info(bin, baseDirectory, baseName);
                }

                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateBoundingBoxOBJ.txt")))
                {
                    ALL.BoundingBox boundingBox = new ALL.BoundingBox {
                        BoundingBoxPosX = bin.BoundingBoxPosX,
                        BoundingBoxPosY = bin.BoundingBoxPosY,
                        BoundingBoxPosZ = bin.BoundingBoxPosZ,
                        BoundingBoxPosW = bin.BoundingBoxPosW,
                        BoundingBoxWidth = bin.BoundingBoxWidth,
                        BoundingBoxHeight = bin.BoundingBoxHeight,
                        BoundingBoxDepth = bin.BoundingBoxDepth
                    };
                    string fullFilePath = Path.Combine(baseDirectory, baseName + ".Debug.BoundingBox.obj");
                    EXTRACT.Debug.CreateBoundingBoxOBJ(fullFilePath, boundingBox);
                }

            }

            //mode repack
            else if (Extension == ".OBJ" || Extension == ".SMD" || Extension == ".IDXPS2BIN")
            {
                Stream idxmaterialFile = null;
                Stream idxps2binFile = null;
                Stream objFile = null;
                Stream smdFile = null;
                Stream mtlFile = null;

                Action CloseOpenedStreams = () => {
                    idxmaterialFile?.Close();
                    idxps2binFile?.Close();
                    objFile?.Close();
                    smdFile?.Close();
                    mtlFile?.Close();
                };

                // verifica arquivos e posibiliades
                switch (Extension)
                {
                    case ".OBJ":
                        objFile = fileInfo.OpenRead();
                        break;
                    case ".SMD":
                        smdFile = fileInfo.OpenRead();
                        break;
                    case ".IDXPS2BIN":
                        idxps2binFile = fileInfo.OpenRead();
                        break;
                    default:
                        Console.WriteLine("The file format is invalid: " + fileInfo.Name);
                        return;
                }

                //carregando arquivos adicionais
                switch (Extension)
                {
                    case ".OBJ":
                    case ".SMD":
                        // modo repack, tem que carregar o .IDXPS2BIN

                        string idxbinFormat = ".idxps2bin";
                        string idxbinFilePath = Path.Combine(baseDirectory, baseName + idxbinFormat);
                        if (File.Exists(idxbinFilePath))
                        {
                            Console.WriteLine("Load File: " + baseName + idxbinFormat);
                            idxps2binFile = new FileInfo(idxbinFilePath).OpenRead();
                        }
                        else
                        {
                            Console.WriteLine($"{idxbinFormat} file does not exist, it is necessary to repack the BIN;");
                            CloseOpenedStreams();
                            return;
                        }
                        break;
                    default:
                        break;
                }

                //-----------
                //carrega os objetos arquivos.
                ALL.IdxMaterial material = null;

                REPACK.IdxPs2Bin idxbin = null;

                if (idxps2binFile != null) //.IDXPS2BIN
                {
                    idxbin = REPACK.IdxPs2BinLoader.Loader(idxps2binFile);
                    idxps2binFile.Close();
                }

                if ((Extension == ".OBJ" || Extension == ".SMD") && idxbin != null)
                {
                    if (idxbin.UseIdxMaterial)
                    {
                        // versão com idxmaterial
                        if (idxmaterialFile == null && mtlFile == null)
                        {
                            string mtlFilePath = Path.Combine(baseDirectory, baseName + ".idxmaterial");
                            if (File.Exists(mtlFilePath))
                            {
                                Console.WriteLine("Load File: " + baseName + ".idxmaterial");
                                idxmaterialFile = new FileInfo(mtlFilePath).OpenRead();
                            }
                            else
                            {
                                Console.WriteLine("IDXMATERIAL file does not exist, it is necessary to repack the BIN;");
                                CloseOpenedStreams();
                                return;
                            }
                        }
                    }
                    else
                    {
                        // versão com mtl
                        if (idxmaterialFile == null && mtlFile == null)
                        {
                            string mtlFilePath = Path.Combine(baseDirectory, baseName + ".mtl");
                            if (File.Exists(mtlFilePath))
                            {
                                Console.WriteLine("Load File: " + baseName + ".mtl");
                                mtlFile = new FileInfo(mtlFilePath).OpenRead();
                            }
                            else
                            {
                                Console.WriteLine("MTL file does not exist, it is necessary to repack the BIN;");
                                CloseOpenedStreams();
                                return;
                            }
                        }

                    }
                }

                if (idxmaterialFile != null) //.IDXMATERIAL
                {
                    Console.WriteLine("Processing IDXMATERIAL");
                    material = ALL.IdxMaterialLoad.Load(idxmaterialFile);
                    idxmaterialFile.Close();
                }

                if (mtlFile != null) //.MTL
                {
                    Console.WriteLine("Processing MTL");
                    ALL.IdxMtl idxMtl;
                    REPACK.MtlLoad.Load(mtlFile, out idxMtl);
                    REPACK.MtlConverter.Convert(idxMtl, out material);
                    // o mtlFile é fechado no metodo acima.
                }

                // cria arquivos
                float ConversionFactorValue = 0;
                REPACK.Structures.FinalStructure finalStructure = new REPACK.Structures.FinalStructure();
                REPACK.FinalBoneLine[] finalBoneLine = new REPACK.FinalBoneLine[0];
                ALL.BoundingBox finalBoundingBox = new ALL.BoundingBox();
                if (Extension == ".IDXPS2BIN")
                {
                    finalStructure = new REPACK.Structures.FinalStructure();
                    finalBoneLine = REPACK.BINrepackOBJ.GetBoneLines(idxbin.Bones);
                }
                else if (Extension == ".OBJ")
                {
                    int ObjFileUseBone = (int)idxbin.ObjFileUseBone;
                    REPACK.BINrepackOBJ.RepackOBJ(objFile, true, ObjFileUseBone, out finalStructure, out ConversionFactorValue, out finalBoundingBox);
                    finalBoneLine = REPACK.BINrepackOBJ.GetBoneLines(idxbin.Bones);
                }
                else if (Extension == ".SMD")
                {
                    REPACK.BINrepackSMD.RepackSMD(smdFile, out finalStructure, out finalBoneLine, true, out ConversionFactorValue, out finalBoundingBox);
                }

                REPACK.RepackProps props = new REPACK.RepackProps
                {
                    BonePairs = idxbin.BonePairs,
                    IsScenarioBin = idxbin.IsScenarioBin,
                    EnableBonepairTag = idxbin.EnableBonepairTag,
                    EnableAdjacentBoneTag = idxbin.EnableAdjacentBoneTag,
                    EnableUnkFlag1 = idxbin.EnableUnkFlag1,
                    EnableUnkFlag2 = idxbin.EnableUnkFlag2,
                    EnableUnkFlag4 = idxbin.EnableUnkFlag4,
                    BoundingBox = new ALL.BoundingBox
                    {
                        BoundingBoxPosX = idxbin.BoundingBoxPosX * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE,
                        BoundingBoxPosY = idxbin.BoundingBoxPosY * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE,
                        BoundingBoxPosZ = idxbin.BoundingBoxPosZ * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE,
                        BoundingBoxPosW = idxbin.BoundingBoxPosW,
                        BoundingBoxWidth = idxbin.BoundingBoxWidth * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE,
                        BoundingBoxHeight = idxbin.BoundingBoxHeight * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE,
                        BoundingBoxDepth = idxbin.BoundingBoxDepth * SHARED_TOOLS.ALL.CONSTs.GLOBAL_POSITION_SCALE
                    }
                };

                if (idxbin.AutoCalcBoundingBox && Extension != ".IDXPS2BIN")
                {
                    props.BoundingBox = finalBoundingBox;
                }

                string binFilePath = Path.Combine(baseDirectory, baseName + ".BIN");
                Stream binstream = File.Open(binFilePath, FileMode.Create);
                REPACK.BINmakeFile.MakeFinalBinFile(binstream, 0, out _, finalStructure, props, finalBoneLine, ConversionFactorValue, material);
                binstream.Close();

                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateBoundingBoxOBJ.txt")))
                {
                    string fullFilePath = Path.Combine(baseDirectory, baseName + ".Repack.BoundingBox.obj");
                    EXTRACT.Debug.CreateBoundingBoxOBJ(fullFilePath, props.BoundingBox);
                }
            }

            else if (Extension == ".MTL") // cria idxMaterial derivado do mtl
            {
                Stream mtlFile = fileInfo.OpenRead();
                ALL.IdxMtl idxMtl;
                ALL.IdxMaterial material;
                REPACK.MtlLoad.Load(mtlFile, out idxMtl);
                REPACK.MtlConverter.Convert(idxMtl, out material);
                EXTRACT.OutputMaterial.CreateIdxMaterial(material, baseDirectory, baseName + ".Repack");
            }

            else if (Extension == ".IDXMATERIAL") // cria mtl derivado do idxMaterial
            {
                Stream idxmaterialFile = fileInfo.OpenRead();
                ALL.IdxMaterial material = ALL.IdxMaterialLoad.Load(idxmaterialFile);
                idxmaterialFile.Close();
                var _idxMtl = ALL.IdxMtlParser.Parser(material, baseName);
                EXTRACT.OutputMaterial.CreateMTL(_idxMtl, baseDirectory, baseName + ".Repack");
            }

            else
            {
                Console.WriteLine("The file format is invalid: " + fileInfo.Name);
            }
        }

    }
}
