using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ObjLoader.Loader.Loaders;

namespace BINrepackTest
{
     /*
     Codigo feito por JADERLINK
     Pesquisas feitas por HardHain e JaderLink.
     https://www.youtube.com/@JADERLINK
     https://www.youtube.com/@HardRainModder

     Em desenvolvimento
     Para Pesquisas
     13-08-2023
     version: alfa.1.2.0.0
     */
    
    public static partial class BINrepack
    {

        public static void RepackObj(string idxbinPath, string objPath, string binpath)
        {

            StreamReader idx = File.OpenText(idxbinPath);

            Dictionary<string, string> pair = new Dictionary<string, string>();

            List<string> lines = new List<string>();

            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();
                lines.Add(endLine);
            }

            idx.Close();

            foreach (var item in lines)
            {
                if (item != null && item.Length != 0)
                {
                    if (!item.TrimStart().StartsWith(":"))
                    {
                        var split = item.Split(new char[] { ':' });
                        if (split.Length >= 2)
                        {
                            string key = split[0].ToUpper().Trim();
                            if (!pair.ContainsKey(key))
                            {
                                pair.Add(key, split[1]);
                            }
                        }
                    }

                }
            }


            // load obj
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());

            var fileStream = new System.IO.FileStream(objPath, System.IO.FileMode.Open);
            LoadResult arqObj = objLoader.Load(fileStream);
            fileStream.Close();

            // --------------

            bool IsScenarioBin = false;
            bool CompressVertices = false;
            bool AutoConversionFactor = false;

            float GlobalScale = 1.0f;
            float ManualConversionFactor = 0f;

            float AutoConversionFactorValue = 1.0f;
            float FarthestVertex = 0;


            if (pair.ContainsKey("ISSCENARIOBIN"))
            {
                try
                {
                    IsScenarioBin = bool.Parse(pair["ISSCENARIOBIN"].Trim());
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("COMPRESSVERTICES"))
            {
                try
                {
                    CompressVertices = bool.Parse(pair["COMPRESSVERTICES"].Trim());
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("AUTOCONVERSIONFACTOR"))
            {
                try
                {
                    AutoConversionFactor = bool.Parse(pair["AUTOCONVERSIONFACTOR"].Trim());
                }
                catch (Exception)
                {
                }
            }


            if (pair.ContainsKey("GLOBALSCALE"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["GLOBALSCALE"]);
                    GlobalScale = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }


            if (pair.ContainsKey("MANUALCONVERSIONFACTOR"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["MANUALCONVERSIONFACTOR"]);
                    ManualConversionFactor = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }


            //---


            Dictionary<string, List<List<refFace>>> IndexLists = new Dictionary<string, List<List<refFace>>>();

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                List<List<refFace>> index = new List<List<refFace>>();

                for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                {
                    List<refFace> subface = new List<refFace>();

                    for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                    {
                        refFace rf = new refFace();
                        rf.VertexIndex = arqObj.Groups[iG].Faces[iF][iI].VertexIndex;
                        rf.TextureIndex = arqObj.Groups[iG].Faces[iF][iI].TextureIndex;
                        rf.NormalIndex = arqObj.Groups[iG].Faces[iF][iI].NormalIndex;
                        rf.FaceVertexID = iI;
                        rf.FaceID = iF;
                        subface.Add(rf);
                    }

                    if (subface.Count != 0)
                    {
                        index.Add(subface);
                    }

                }

                if (IndexLists.ContainsKey(arqObj.Groups[iG].Name))
                {
                    IndexLists[arqObj.Groups[iG].Name].AddRange(index);
                }
                else
                {
                    IndexLists.Add(arqObj.Groups[iG].Name, index);
                }

            }


            #region Compactação de vertices

            // ------------ 
            // Compacta as vertices

            TriOrder lastFirtOrder = new TriOrder(1, 2, 3);

            if (CompressVertices)
            {

                Dictionary<string, List<List<refFace>>> IndexListsCompressed = new Dictionary<string, List<List<refFace>>>();

                foreach (var Pair in IndexLists)
                {

                    List<Triangle> triangles = new List<Triangle>();
                    for (int i = 0; i < Pair.Value.Count; i++)
                    {
                        try
                        {
                            Triangle t = new Triangle();
                            t.A = Pair.Value[i][0];
                            t.B = Pair.Value[i][1];
                            t.C = Pair.Value[i][2];
                            triangles.Add(t);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    //---

                    List<List<refFace>> index = new List<List<refFace>>();

                    if (triangles.Count > 0)
                    {

                        List<refFace> temp = new List<refFace>();

                        Triangle last = triangles[0];
                        (int r1, int r2, int r3) lastOrder = (1, 2, 3);

                        triangles.RemoveAt(0);

                        bool isFirt = true;
                        bool isAdded = false;

                        while (triangles.Count != 0)
                        {
                            var cont = (from obj in triangles
                                        where obj.As2VertexEqual(last)
                                        select obj).ToArray();

                            isAdded = false;

                            for (int i = 0; i < cont.Length; i++)
                            {
                                TriOrder nextOrder = new TriOrder(cont[i][1].Value.VertexIndex, cont[i][2].Value.VertexIndex, cont[i][3].Value.VertexIndex);

                                (int r1, int r2, int r3) Order1oldlast = (0, 0, 0);
                                (int r1, int r2, int r3) Order2next = (0, 0, 0);


                                if (isFirt)
                                {
                                    bool checkFirt = VertexCompressUtils.CheckOrderFirt(last, cont[i], lastFirtOrder, nextOrder, out Order1oldlast, out Order2next);

                                    if (checkFirt)
                                    {
                                        temp.Add(last[Order1oldlast.r1].Value);
                                        temp.Add(last[Order1oldlast.r2].Value);
                                        temp.Add(last[Order1oldlast.r3].Value);
                                        temp.Add(cont[i][Order2next.r3].Value);

                                        last = cont[i];
                                        lastOrder = Order2next;
                                        triangles.Remove(cont[i]);

                                        isAdded = true;
                                        isFirt = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    bool checkNotFirt = VertexCompressUtils.CheckOrder(last, cont[i], lastOrder, nextOrder, out Order2next);

                                    if (checkNotFirt)
                                    {
                                        temp.Add(cont[i][Order2next.r3].Value);

                                        last = cont[i];
                                        lastOrder = Order2next;
                                        triangles.Remove(cont[i]);

                                        isAdded = true;
                                        isFirt = false;
                                        break;
                                    }


                                }

                            } // fim do  for (int i = 0; i < cont.Length; i++)

                            // considerar que não achou nada
                            // considerar achou e estava status firt
                            // considerar achou e estava status not firt
                            // considarar que cont != 0, mas não achou

                            if (cont.Length == 0 || (cont.Length != 0 && isAdded == false))
                            {
                                if (isFirt)
                                {
                                    temp.Add(last.A);
                                    temp.Add(last.B);
                                    temp.Add(last.C);

                                    index.Add(temp);
                                    temp = new List<refFace>();
                                    last = triangles[0];
                                    lastOrder = (1, 2, 3);
                                    triangles.RemoveAt(0);

                                    isFirt = true;
                                    isAdded = false;
                                    continue;
                                }
                                else
                                {
                                    index.Add(temp);
                                    temp = new List<refFace>();
                                    last = triangles[0];
                                    lastOrder = (1, 2, 3);
                                    triangles.RemoveAt(0);

                                    isFirt = true;
                                    isAdded = false;
                                    continue;
                                }
                            }
                            else if (temp.Count >= 40)
                            {
                                index.Add(temp);
                                temp = new List<refFace>();
                                last = triangles[0];
                                lastOrder = (1, 2, 3);
                                triangles.RemoveAt(0);

                                isFirt = true;
                                isAdded = false;
                                continue;
                            }

                        } // fim do  while (triangles.Count != 0)


                        // ultima seção
                        if (isFirt)
                        {
                            temp.Add(last.A);
                            temp.Add(last.B);
                            temp.Add(last.C);

                            index.Add(temp);
                        }
                        else
                        {
                            index.Add(temp);
                        }



                        if (index.Count != 0)
                        {
                            IndexListsCompressed.Add(Pair.Key, index);
                        }


                    }// fim do  if (triangles.Count > 0)




                }//fim do  foreach (var Pair in IndexLists)

                if (IndexListsCompressed.Count != 0)
                {
                    IndexLists = IndexListsCompressed;
                }

            }

            // ------------

            #endregion


            List<Noded> nodes = new List<Noded>();

            foreach (var facelists in IndexLists)
            {
                var triangles = facelists.Value;

                List<List<VertexLine>> binVerticesLineBase = new List<List<VertexLine>>();

                for (int i = 0; i < triangles.Count; i++)
                {
                    ushort oldIndexMount = 0x0000;

                    List<VertexLine> part = new List<VertexLine>();
                    for (int ip = 0; ip < triangles[i].Count; ip++)
                    {
                        VertexLine vertex = new VertexLine();
                        vertex.VerticeX = (arqObj.Vertices[triangles[i][ip].VertexIndex - 1].X);
                        vertex.VerticeY = (arqObj.Vertices[triangles[i][ip].VertexIndex - 1].Y);
                        vertex.VerticeZ = (arqObj.Vertices[triangles[i][ip].VertexIndex - 1].Z);

                        if (arqObj.Normals.Count != 0)
                        {
                            vertex.NormalX = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].X);
                            vertex.NormalY = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].Y);
                            vertex.NormalZ = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].Z);
                        }

                        if (arqObj.Textures.Count != 0)
                        {
                            vertex.TextureU = (arqObj.Textures[triangles[i][ip].TextureIndex - 1].X);
                            vertex.TextureV = (arqObj.Textures[triangles[i][ip].TextureIndex - 1].Y);
                        }

                        if (ip == 0 || ip == 1)
                        {
                            vertex.IndexMount = 0x0000;
                        }
                        else
                        {
                            if (oldIndexMount == 0x0000 || oldIndexMount == 0xFFFF)
                            {
                                vertex.IndexMount = 0x0001;
                                oldIndexMount = 0x0001;
                            }
                            else if (oldIndexMount == 0x0001)
                            {
                                vertex.IndexMount = 0xFFFF;
                                oldIndexMount = 0xFFFF;
                            }
                        }

                        part.Add(vertex);

                        // ---

                        if (AutoConversionFactor)
                        {
                            float temp = vertex.VerticeX * GlobalScale;
                            if (temp < 0)
                            {
                                temp *= -1;
                            }
                            if (temp > FarthestVertex)
                            {
                                FarthestVertex = temp;
                            }

                            temp = vertex.VerticeY * GlobalScale;
                            if (temp < 0)
                            {
                                temp *= -1;
                            }
                            if (temp > FarthestVertex)
                            {
                                FarthestVertex = temp;
                            }

                            temp = vertex.VerticeZ * GlobalScale;
                            if (temp < 0)
                            {
                                temp *= -1;
                            }
                            if (temp > FarthestVertex)
                            {
                                FarthestVertex = temp;
                            }
                        }
                    }

                    binVerticesLineBase.Add(part);

                }

                // ---------------

                List<List<VertexLine>> subMeshparts = new List<List<VertexLine>>();
                List<VertexLine> tempV = new List<VertexLine>();
                tempV.AddRange(binVerticesLineBase[0]);

                for (int i = 1; i < binVerticesLineBase.Count; i++)
                {
                    if (tempV.Count + binVerticesLineBase[i].Count <= 44)
                    {
                        tempV.AddRange(binVerticesLineBase[i]);
                    }
                    else
                    {
                        subMeshparts.Add(tempV);
                        tempV = new List<VertexLine>();
                        tempV.AddRange(binVerticesLineBase[i]);
                    }
                }

                //ultima parte
                subMeshparts.Add(tempV);


                //-- arruma campo "IndexComplement"

                for (int i = 0; i < subMeshparts.Count; i++)
                {
                    for (int l = 2; l < subMeshparts[i].Count; l++)
                    {
                        if (subMeshparts[i][l].IndexMount == 0x0000)
                        {
                            subMeshparts[i][l].IndexComplement = 0x0001;
                        }

                    }
                }


                // -- final

                Noded noded = new Noded();
                noded.materialname = facelists.Key;
                noded.subMeshparts = subMeshparts;
                nodes.Add(noded);
            }


            nodes = (from no in nodes
                     orderby no.materialname
                     select no).ToList();


            //---------


            if (AutoConversionFactor)
            {
                // regra de 3:
                // FarthestVertex == short.MaxValue
                // A              ==  B
                // FarthestVertex * B = A * short.MaxValue
                // B = (A * short.MaxValue) / FarthestVertex;
                // A = (FarthestVertex * B) / short.MaxValue;
                // scale = FarthestVertex / short.MaxValue;
                AutoConversionFactorValue = FarthestVertex / short.MaxValue;
            }


            // nodes header top

            List<byte[]> NodesHeaderTop = new List<byte[]>();

            for (int t = 0; t < nodes.Count; t++)
            {

                byte b2 = (byte)(nodes[t].subMeshparts.Count - 1);
                if (nodes[t].subMeshparts.Count > 256)
                {
                    b2 = 255;
                }

                byte b3 = 0;

                if (!IsScenarioBin)
                {
                    b3 = 1;
                }

                byte[] proximosBytes = new byte[b3];

                //---------------
                int totalBytesNoNodeHeaderArray = 4 + b3;
                int counterNodeHeaderArrayLenght = 0x10;
                while (totalBytesNoNodeHeaderArray > counterNodeHeaderArrayLenght)
                {
                    counterNodeHeaderArrayLenght += 0x10;
                }
                // ----


                ushort lenghtFirtSubMeshB0B1 = 0;
                // --- calculo tamanho

                int caculo = nodes[t].subMeshparts[0].Count * 24;
                int vertexBlocklength = caculo / 0x10;
                float div = caculo % 0x10;
                if (div != 0)
                {
                    vertexBlocklength += 1;
                }

                if (IsScenarioBin)
                {
                    lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x30 + (vertexBlocklength * 0x10));
                }
                else
                {
                    lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x10 + 0x20 + 0x30 + (vertexBlocklength * 0x10));
                }

                byte[] lenghtFirtSubMeshB0B1bytes = BitConverter.GetBytes(lenghtFirtSubMeshB0B1);




                byte[] headerFinal = new byte[counterNodeHeaderArrayLenght];
                headerFinal[0] = lenghtFirtSubMeshB0B1bytes[0];//b0;
                headerFinal[1] = lenghtFirtSubMeshB0B1bytes[1];//b1;
                headerFinal[2] = b2;
                headerFinal[3] = b3;
                proximosBytes.CopyTo(headerFinal, 4);
                NodesHeaderTop.Add(headerFinal);


            }


            float valueConversionFactor = ManualConversionFactor;
            if (AutoConversionFactor || ManualConversionFactor == 0)
            {
                valueConversionFactor = AutoConversionFactorValue;
            }

            List<List<byte>> NodesBlocks = new List<List<byte>>();
            List<string> NodesNames = new List<string>();

            for (int t = 0; t < nodes.Count; t++)
            {
                List<byte> nodeBytes = new List<byte>();
                nodeBytes.AddRange(NodesHeaderTop[t]);

                for (int i = 0; i < nodes[t].subMeshparts.Count; i++)
                {
                    if (!IsScenarioBin)
                    {
                        nodeBytes.AddRange(new byte[] {
                        0x02, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x00, 0x80, 0x02, 0x6C, //SubBoneHeader
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 //subBoneTable line 1
                        });
                    }


                    // TopTagVifHeader
                    nodeBytes.AddRange(MakeTopTagVifHeader((byte)nodes[t].subMeshparts[i].Count, valueConversionFactor, i == 0));

                    int caculo = nodes[t].subMeshparts[i].Count * 24;
                    int vertexBlocklength = caculo / 0x10;
                    float div = caculo % 0x10;
                    if (div != 0)
                    {
                        vertexBlocklength += 1;
                    }

                    byte[] submeshBlock = new byte[vertexBlocklength * 0x10];

                    int count = 0;
                    for (int l = 0; l < nodes[t].subMeshparts[i].Count; l++)
                    {
                        byte[] line = new byte[24];

                        var VerticeX = BitConverter.GetBytes(Utils.ParseFloatToShort((nodes[t].subMeshparts[i][l].VerticeX * GlobalScale) / valueConversionFactor));
                        var VerticeY = BitConverter.GetBytes(Utils.ParseFloatToShort((nodes[t].subMeshparts[i][l].VerticeY * GlobalScale) / valueConversionFactor));
                        var VerticeZ = BitConverter.GetBytes(Utils.ParseFloatToShort((nodes[t].subMeshparts[i][l].VerticeZ * GlobalScale) / valueConversionFactor));

                        line[0] = VerticeX[0];
                        line[1] = VerticeX[1];

                        line[2] = VerticeY[0];
                        line[3] = VerticeY[1];

                        line[4] = VerticeZ[0];
                        line[5] = VerticeZ[1];

                        var IndexComplement = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].IndexComplement);

                        line[0XE] = IndexComplement[0];
                        line[0XF] = IndexComplement[1];

                        var NormalX = BitConverter.GetBytes(Utils.ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalX * 127));
                        var NormalY = BitConverter.GetBytes(Utils.ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalY * 127));
                        var NormalZ = BitConverter.GetBytes(Utils.ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalZ * 127));

                        var TextureU = BitConverter.GetBytes(Utils.ParseFloatToShort(nodes[t].subMeshparts[i][l].TextureU * 255));
                        var TextureV = BitConverter.GetBytes(Utils.ParseFloatToShort(nodes[t].subMeshparts[i][l].TextureV * 255));

                        var IndexMount = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].IndexMount);

                        // -----------

                        if (IsScenarioBin)
                        {

                            line[0X6] = IndexMount[0];
                            line[0X7] = IndexMount[1];

                            line[0X8] = TextureU[0];
                            line[0X9] = TextureU[1];

                            line[0XA] = TextureV[0];
                            line[0XB] = TextureV[1];

                            line[0XC] = 0X01;
                            line[0XD] = 0X00;

                            line[0X16] = 0X80;
                            line[0X17] = 0X00;

                            line[0X10] = 255;
                            line[0X11] = 0;

                            line[0X12] = 255;
                            line[0X13] = 0;

                            line[0X14] = 255;
                            line[0X15] = 0;


                        }
                        else
                        {
                            line[0X8] = NormalX[0];
                            line[0X9] = NormalX[1];

                            line[0XA] = NormalY[0];
                            line[0XB] = NormalY[1];

                            line[0XC] = NormalZ[0];
                            line[0XD] = NormalZ[1];

                            line[0X10] = TextureU[0];
                            line[0X11] = TextureU[1];

                            line[0X12] = TextureV[0];
                            line[0X13] = TextureV[1];

                            line[0X14] = 0X01;
                            line[0X15] = 0X00;

                            line[0X16] = IndexMount[0];
                            line[0X17] = IndexMount[1];

                        }

                        line.CopyTo(submeshBlock, count);
                        count += 24;
                    }

                    nodeBytes.AddRange(submeshBlock);


                    if (i != 0)
                    {
                        nodeBytes.AddRange(MakeEndTagVifCommand(i == nodes[t].subMeshparts.Count - 1));
                    }
                }

                NodesBlocks.Add(nodeBytes);
                NodesNames.Add(nodes[t].materialname);
            }


            // ORDEM
            // mainHeader 16 bytes
            // unknown4 bytes 64 ou variavel
            // bones
            // materials
            // nodes

            // mainheader
            byte[] Fix3000 = new byte[2] { 0x30, 0x00 };
            byte[] unknown1 = new byte[2];
            int bonesPoint = 0;
            byte unknown2 = 0x0;
            byte BonesCount = 0;
            byte MaterialCount = (byte)NodesBlocks.Count;
            byte unknown3 = 0;
            int MaterialsPoint = 0;

            List<byte[]> Bones = new List<byte[]>();
            List<byte[]> materials = new List<byte[]>();

            if (pair.ContainsKey("FIX3000"))
            {
                string value = Utils.ReturnValidHexValue(pair["FIX3000"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                Fix3000[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                Fix3000[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN1"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN1"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                unknown1[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                unknown1[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN2"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN2"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown2 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN3"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN3"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown3 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("BONESCOUNT"))
            {

                try
                {
                    string value = Utils.ReturnValidDecValue(pair["BONESCOUNT"]);
                    BonesCount = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }

            }

            #region unknown4

            byte[] unknown4 = new byte[0x40];

            // new unknown4 partes

            unknown4[0x0] = 0xCD;
            unknown4[0x1] = 0xCD;
            unknown4[0x2] = 0xCD;
            unknown4[0x3] = 0xCD;
            unknown4[0x4] = 0xCD;
            unknown4[0x5] = 0xCD;
            unknown4[0x6] = 0xCD;
            unknown4[0x7] = 0xCD;

            unknown4[0x3C] = 0xCD;
            unknown4[0x3D] = 0xCD;
            unknown4[0x3E] = 0xCD;
            unknown4[0x3F] = 0xCD;

            unknown4[0x18] = 0x30;

            if (pair.ContainsKey("UNKNOWN4_B"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_B"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[0x8 + ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }

            if (pair.ContainsKey("UNKNOWN4_UNK008"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_UNK008"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[0x10 + ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }

            if (pair.ContainsKey("UNKNOWN4_UNK009"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_UNK009"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[0x14 + ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }

            if (pair.ContainsKey("UNKNOWN4_UNK010"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_UNK010"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[0x1C + ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }

            //0x20
            //0x24
            //0x28
            //0x2C
            //0x30
            //0x34
            //0x38

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEX"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEX"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x20] = b[0];
                    unknown4[0x21] = b[1];
                    unknown4[0x22] = b[2];
                    unknown4[0x23] = b[3];

                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEY"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEY"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x24] = b[0];
                    unknown4[0x25] = b[1];
                    unknown4[0x26] = b[2];
                    unknown4[0x27] = b[3];

                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEZ"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEZ"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x28] = b[0];
                    unknown4[0x29] = b[1];
                    unknown4[0x2A] = b[2];
                    unknown4[0x2B] = b[3];

                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEPADDING"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEPADDING"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x2C] = b[0];
                    unknown4[0x2D] = b[1];
                    unknown4[0x2E] = b[2];
                    unknown4[0x2F] = b[3];
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEX"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEX"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x30] = b[0];
                    unknown4[0x31] = b[1];
                    unknown4[0x32] = b[2];
                    unknown4[0x33] = b[3];
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEY"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEY"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x34] = b[0];
                    unknown4[0x35] = b[1];
                    unknown4[0x36] = b[2];
                    unknown4[0x37] = b[3];
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEZ"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEZ"]);
                    float valuef = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    byte[] b = BitConverter.GetBytes(valuef);

                    unknown4[0x38] = b[0];
                    unknown4[0x39] = b[1];
                    unknown4[0x3A] = b[2];
                    unknown4[0x3B] = b[3];
                }
                catch (Exception)
                {
                }
            }

            #endregion

            //---

            for (int i = 0; i < BonesCount; i++)
            {
                byte[] boneLine = new byte[0x10];

                if (pair.ContainsKey("BONELINE_" + i))
                {

                    string value = Utils.ReturnValidHexValue(pair["BONELINE_" + i].ToUpper());
                    value = value.PadRight(0x10 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < boneLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        boneLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }

                Bones.Add(boneLine);
            }

            for (int i = 0; i < NodesBlocks.Count; i++)
            {
                //MATERIALLINE
                byte[] materialLine = new byte[0x10];

                // NodesNames

                string keyMaterial = "MATERIALLINE?" + NodesNames[i].ToUpper();
                if (pair.ContainsKey(keyMaterial))
                {
                    Console.WriteLine($"[{i}] Used material: " + keyMaterial);

                    string value = Utils.ReturnValidHexValue(pair[keyMaterial].ToUpper());
                    value = value.PadRight(0x10 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < materialLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        materialLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }
                else
                {
                    Console.WriteLine($"[{i}] Not found material: " + keyMaterial);

                    materialLine = new byte[0x10] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                }

                materials.Add(materialLine);
            }


            // calculos de offset;

            bonesPoint = 0x10 + unknown4.Length;
            MaterialsPoint = bonesPoint + (BonesCount * 0x10);

            int firtNodePoint = MaterialsPoint + (NodesBlocks.Count * 0x10);
            byte[] firtNodePointBytes = BitConverter.GetBytes(firtNodePoint);
            firtNodePointBytes.CopyTo(materials[0], 12); //0x10-4

            int tempOffset = firtNodePoint;
            for (int i = 1; i < NodesBlocks.Count; i++)
            {
                tempOffset = tempOffset + NodesBlocks[i - 1].Count;
                byte[] anyNodePointBytes = BitConverter.GetBytes(tempOffset);
                anyNodePointBytes.CopyTo(materials[i], 12); //0x10-4
            }


            //------------------

            Stream binfile = File.Create(binpath);

            binfile.Write(Fix3000, 0, Fix3000.Length);
            binfile.Write(unknown1, 0, unknown1.Length);

            byte[] bonesPointBytes = BitConverter.GetBytes(bonesPoint);
            binfile.Write(bonesPointBytes, 0, bonesPointBytes.Length);

            binfile.WriteByte(unknown2);
            binfile.WriteByte(BonesCount);
            binfile.WriteByte(MaterialCount);
            binfile.WriteByte(unknown3);

            byte[] MaterialsPointBytes = BitConverter.GetBytes(MaterialsPoint);
            binfile.Write(MaterialsPointBytes, 0, MaterialsPointBytes.Length);

            binfile.Write(unknown4, 0, unknown4.Length);

            for (int i = 0; i < Bones.Count; i++)
            {
                binfile.Write(Bones[i], 0, Bones[i].Length);
            }

            for (int i = 0; i < materials.Count; i++)
            {
                binfile.Write(materials[i], 0, materials[i].Length);
            }

            for (int i = 0; i < NodesBlocks.Count; i++)
            {
                binfile.Write(NodesBlocks[i].ToArray(), 0, NodesBlocks[i].Count);
            }


            binfile.Close();
        }



    }
}
