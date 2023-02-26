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
     26-02-2023
     version: alfa.1.0.0.1
     */
    public static class BINrepack
    {

        public static void Repack(string idxbipath, string objpath, string binpath, bool compressVertices = false) 
        {

            StreamReader idx = File.OpenText(idxbipath);

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
                if (item != null)
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

            // load obj
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());

            var fileStream = new System.IO.FileStream(objpath, System.IO.FileMode.Open);
            LoadResult arqObj = objLoader.Load(fileStream);
            fileStream.Close();

            // --------------
          
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


            #region Compactação de vertices, em desenvolvimento

            //TriOrder n = new TriOrder(2,3,1);

            // ------------ 
            // Compacta as vertices

            if (compressVertices)
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

                        //temp.Add(triangles[0].A);
                        //temp.Add(triangles[0].B);
                        //temp.Add(triangles[0].C);
                        Triangle last = triangles[0];
                        (int r1, int r2, int r3) lastOrder = (1, 2, 3);

                        triangles.RemoveAt(0);

                        bool isFirt = true;
                        bool isAdded = false;

                        Triangle? found = null;

                        while (triangles.Count != 0)
                        {
                            var cont = (from obj in triangles
                                        where obj.As2VertexEqual(last)
                                        select obj).ToArray();

                            isAdded = false;

                            for (int i = 0; i < cont.Length; i++)
                            {
                                TriOrder nextOrder = new TriOrder(cont[i][1].Value.VertexIndex, cont[i][2].Value.VertexIndex, cont[i][3].Value.VertexIndex);
                                TriOrder lastFirtOrder = new TriOrder(last[1].Value.VertexIndex, last[2].Value.VertexIndex, last[3].Value.VertexIndex);

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

                            if (cont.Length != 0)
                            {

                                if (isAdded) // ele achou valor, e colocou na variavel "temp"
                                {
                                    // não tem isso
                                    //last = triangles[0];
                                    //lastOrder = (1, 2, 3);
                                    //triangles.RemoveAt(0);

                                    //isFirt = false;
                                    //isAdded = false;
                                    continue;
                                }
                                else // ele não achou valor
                                {
                                    // o mesmo que cont.Length == 0)
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


                            }
                            //
                            else  // equivale a  cont.Length == 0
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

                        vertex.NormalX = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].X);
                        vertex.NormalY = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].Y);
                        vertex.NormalZ = (arqObj.Normals[triangles[i][ip].NormalIndex - 1].Z);

                        vertex.TextureU = (arqObj.Textures[triangles[i][ip].TextureIndex - 1].X);
                        vertex.TextureV = (arqObj.Textures[triangles[i][ip].TextureIndex - 1].Y);

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

            bool IsScenarioBin = false;
            bool ScenarioUseColors = true;


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


            if (pair.ContainsKey("SCENARIOUSECOLORS"))
            {
                try
                {
                    ScenarioUseColors = bool.Parse(pair["SCENARIOUSECOLORS"].Trim());
                }
                catch (Exception)
                {
                }
            }

            // TopTagVifHeader_Scales  ScenarioVerticeColors

            float[] TopTagVifHeader_Scales = new float[nodes.Count];
            byte[][] ScenarioVerticeColors = new byte[nodes.Count][];

            for (int t = 0; t < nodes.Count; t++)
            {
                if (pair.ContainsKey("TOPTAGVIFHEADER_SCALE_" + t))
                {
                    try
                    {
                        string value = ReturnValidFloatValue(pair["TOPTAGVIFHEADER_SCALE_" + t]);
                        TopTagVifHeader_Scales[t] = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        TopTagVifHeader_Scales[t] = 0.001f;
                    }

                }
                else
                {
                    TopTagVifHeader_Scales[t] = 0.001f;
                }


                byte[] color = new byte[3];

                if (ScenarioUseColors && pair.ContainsKey("SCENARISCENARIOVERTICECOLOR_" + t))
                {

                    string value = ReturnValidHexValue(pair["SCENARISCENARIOVERTICECOLOR_" + t].ToUpper());
                    value = value.PadRight(3 * 2, 'F');

                    int cont = 0;
                    for (int ipros = 0; ipros < 3; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        color[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }
                else 
                {
                    color[0] = 255;
                    color[1] = 255;
                    color[2] = 255;
                }
                ScenarioVerticeColors[t] = color;

            }


            // nodes header top

            List<byte[]> NodesHeaderTop = new List<byte[]>();

            for (int t = 0; t < nodes.Count; t++)
            {

                byte b2 = (byte)(nodes[t].subMeshparts.Count -1);
                byte b3 = 0;

             
                if (!IsScenarioBin && pair.ContainsKey("NODEHEADER_SPLITCOUNT_" + t))
                {
                    
                    try
                    {
                        string value = ReturnValidDecValue(pair["NODEHEADER_SPLITCOUNT_" + t]);
                        b3 = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                    }
                    catch (Exception)
                    {
                        b3 = 1;
                    }

                }
                else if (!IsScenarioBin)
                {
                    b3 = 1;
                }

                byte[] proximosBytes = new byte[b3];

                if (!IsScenarioBin && pair.ContainsKey("NODEHEADER_SPLITVALUES_" + t))
                {

                    string value = ReturnValidHexValue(pair["NODEHEADER_SPLITVALUES_" + t].ToUpper());
                    value = value.PadRight(b3 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < b3; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        proximosBytes[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }


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


            List<List<byte>> NodesBlocks = new List<List<byte>>();

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
                    nodeBytes.AddRange(MakeTopTagVifHeader((byte)nodes[t].subMeshparts[i].Count, TopTagVifHeader_Scales[t], i == 0));

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

                        var VerticeX = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].VerticeX / TopTagVifHeader_Scales[t]));
                        var VerticeY = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].VerticeY / TopTagVifHeader_Scales[t]));
                        var VerticeZ = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].VerticeZ / TopTagVifHeader_Scales[t]));

                        line[0] = VerticeX[0];
                        line[1] = VerticeX[1];

                        line[2] = VerticeY[0];
                        line[3] = VerticeY[1];

                        line[4] = VerticeZ[0];
                        line[5] = VerticeZ[1];

                        var IndexComplement = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].IndexComplement);

                        line[0XE] = IndexComplement[0];
                        line[0XF] = IndexComplement[1];

                        var NormalX = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalX * 127));
                        var NormalY = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalY * 127));
                        var NormalZ = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].NormalZ * 127));

                        var TextureU = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].TextureU * 255));
                        var TextureV = BitConverter.GetBytes(ParseFloatToShort(nodes[t].subMeshparts[i][l].TextureV * 255));

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

                            if (ScenarioUseColors)
                            {
                                line[0X10] = ScenarioVerticeColors[t][0];
                                line[0X11] = 0;

                                line[0X12] = ScenarioVerticeColors[t][1];
                                line[0X13] = 0;

                                line[0X14] = ScenarioVerticeColors[t][2];
                                line[0X15] = 0;
                            }
                            else 
                            {
                                line[0X10] = NormalX[0];
                                line[0X11] = NormalX[1];

                                line[0X12] = NormalY[0];
                                line[0X13] = NormalY[1];

                                line[0X14] = NormalZ[0];
                                line[0X15] = NormalZ[1];
                            }
                       
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
                        nodeBytes.AddRange(MakeEndTagVifCommand(i == nodes[t].subMeshparts.Count -1));
                    }
                }

                NodesBlocks.Add(nodeBytes);
            }


            // ORDEM
            // mainHeader 16 bytes
            // unknown4 bytes 64 ou variavel
            // bones
            // materials
            // nodes

            // mainheader
            byte[] Fix3000 = new byte[2] {0x30, 0x00 };
            byte[] unknown1 = new byte[2];
            int bonesPoint = 0;
            byte unknown2 = 0x0;
            byte BonesCount = 0;
            byte MaterialCount = (byte)NodesBlocks.Count;
            byte unknown3 = 0;
            int MaterialsPoint = 0;
            byte[] unknown4 = new byte[0];

            List<byte[]> Bones = new List<byte[]>();
            List<byte[]> materials = new List<byte[]>();

            if (pair.ContainsKey("FIX3000"))
            {
                string value = ReturnValidHexValue(pair["FIX3000"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                Fix3000[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                Fix3000[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN1"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN1"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                unknown1[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                unknown1[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN2"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN2"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown2 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN3"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN3"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown3 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("BONESCOUNT"))
            {

                try
                {
                    string value = ReturnValidDecValue(pair["BONESCOUNT"]);
                    BonesCount = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }

            }

            int unknown4Lenght = 0x40;

            if (pair.ContainsKey("UNKNOWN4LENGTH"))
            {

                try
                {
                    string value = ReturnValidDecValue(pair["UNKNOWN4LENGTH"]);
                    unknown4Lenght = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }

            }
            if (unknown4Lenght < 0x40)
            {
                unknown4Lenght = 0x40;
            }


            unknown4 = new byte[unknown4Lenght];

            if (pair.ContainsKey("UNKNOWN4"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN4"].ToUpper());
                value = value.PadRight(unknown4Lenght * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < unknown4Lenght; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }

            }

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
                string value = ReturnValidHexValue(pair["UNKNOWN4_B"].ToUpper());
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
                string value = ReturnValidHexValue(pair["UNKNOWN4_UNK008"].ToUpper());
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
                string value = ReturnValidHexValue(pair["UNKNOWN4_UNK009"].ToUpper());
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
                string value = ReturnValidHexValue(pair["UNKNOWN4_UNK010"].ToUpper());
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEX"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEY"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEZ"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEPADDING"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEX"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEY"]);
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
                    string value = ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEZ"]);
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

            //---

            for (int i = 0; i < BonesCount; i++)
            {
                byte[] boneLine = new byte[0x10];

                if (pair.ContainsKey("BONELINE_" + i))
                {

                    string value = ReturnValidHexValue(pair["BONELINE_" + i].ToUpper());
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
                //MATERIALLINE_0
                byte[] materialLine = new byte[0x10];

                if (pair.ContainsKey("MATERIALLINE_" + i))
                {

                    string value = ReturnValidHexValue(pair["MATERIALLINE_" + i].ToUpper());
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
                    materialLine = new byte[0x10] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                }

                materials.Add(materialLine);
            }

            // calculos de offset;

            bonesPoint = 0x10 + unknown4Lenght;
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


        static byte[] MakeTopTagVifHeader(byte vertexlength, float TopTagVif_Scale, bool isFirt)
        {
            int caculo = vertexlength * 24;
            int vertexBlocklength = caculo / 0x10;
            float div = caculo % 0x10;
            if (div != 0)
            {
                vertexBlocklength += 1;
            }

            int vertexBlocklength8 = (vertexBlocklength * 0x10) / 8;

            byte firt = 0x10;
            if (isFirt)
            {
                firt = 0x60;
            }

            byte[] scale = BitConverter.GetBytes(TopTagVif_Scale);

            //vertexlength in 0x10

            byte[] res = new byte[0x30] {
            0x01, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x20, 0x80, 0x01, 0x6C, vertexlength, 0x80, 0x00, 0x00, 0x00, 0x40, 0x2E, 0x30, 0x12, 0x05, 0x00, 0x00, scale[0], scale[1], scale[2], scale[3], (byte)vertexBlocklength, 0x00, 0x00, firt, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x21, 0x80, (byte)vertexBlocklength8, 0x6D
            };
    
            return res;
        }

        static byte[] MakeEndTagVifCommand(bool isEndEntry) 
        {
            byte EndEntry = 0x10;
            if (isEndEntry)
            {
                EndEntry = 0x60;
            }

            byte[] res = new byte[0x10]
            {
                0x00, 0x00, 0x00, EndEntry, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17
            };
            return res;
        }


        static string ReturnValidHexValue(string cont) 
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c)
                    || c == 'A'
                    || c == 'B'
                    || c == 'C'
                    || c == 'D'
                    || c == 'E'
                    || c == 'F'
                    )
                {
                    res += c;
                }
            }
            return res;
        }

        static string ReturnValidDecValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

        static string ReturnValidFloatValue(string cont)
        {
            bool Dot = false;
            bool negative = false;

            string res = "";
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res = c + res;
                    negative = true;
                }

                if (Dot == false && c == '.')
                {
                    res += c;
                    Dot = true;
                }
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }


        static short ParseFloatToShort(float value) 
        {
            string sv = value.ToString("F", System.Globalization.CultureInfo.InvariantCulture).Split('.')[0];
            int iv = 0;
            try
            {
                iv = int.Parse(sv, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception)
            {
            }
            if (iv > short.MaxValue)
            {
                iv = short.MaxValue;
            }
            else if (iv < short.MinValue)
            {
                iv = short.MinValue;
            }
            return (short)iv;
        }

    }


    public struct refFace 
    {
        public int VertexIndex;
        public int TextureIndex;
        public int NormalIndex;
        public int FaceVertexID;
        public int FaceID;

        public bool CompareVertexEquals(refFace compare) 
        {
           return NormalIndex == compare.NormalIndex && TextureIndex == compare.TextureIndex && VertexIndex == compare.VertexIndex;
        }


        //---
        public override bool Equals(object obj)
        {
            return obj is refFace o 
                && o.VertexIndex == VertexIndex 
                && o.TextureIndex == TextureIndex
                && o.NormalIndex == NormalIndex
                && o.FaceVertexID == FaceVertexID
                && o.FaceID == FaceID
                ;
        }

        public override int GetHashCode()
        {
            return (VertexIndex + TextureIndex + NormalIndex + FaceVertexID + FaceID + "").GetHashCode();
        }
    }

    public struct Triangle 
    {
        /// <summary>
        /// D  (1)
        /// </summary>
        public refFace A;
        /// <summary>
        /// E  (2)
        /// </summary>
        public refFace B;
        /// <summary>
        /// F  (3)
        /// </summary>
        public refFace C;

        /// <summary>
        /// contagem a partir de 1
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public refFace? this[int i]
        {
            get 
            {
                if (i == 1)
                {
                    return A;
                }
                else if (i == 2) 
                {
                    return B;
                }
                else if (i == 3)
                {
                    return C;
                }
                return null;//new refFace();
            }
        }

        public bool As2VertexEqual(Triangle compare) 
        {
            bool AD = A.CompareVertexEquals(compare.A);
            bool AE = A.CompareVertexEquals(compare.B);
            bool AF = A.CompareVertexEquals(compare.C);

            bool BD = B.CompareVertexEquals(compare.A);
            bool BE = B.CompareVertexEquals(compare.B);
            bool BF = B.CompareVertexEquals(compare.C);
            
            bool CD = C.CompareVertexEquals(compare.A);
            bool CE = C.CompareVertexEquals(compare.B);
            bool CF = C.CompareVertexEquals(compare.C);

            //------
            //
            bool comp1 = AD && BE;
            bool comp2 = AD && BF;
            bool comp3 = AD && CE;
            bool comp4 = AD && CF;
            //
            bool comp5 = AE && BD;
            bool comp6 = AE && BF;
            bool comp7 = AE && CD;
            bool comp8 = AE && CF;
            //
            bool comp9 = AF && BD;
            bool comp10 = AF && BE;
            bool comp11 = AF && CD;
            bool comp12 = AF && CE;
            //
            bool comp13 = BD && CE;
            bool comp14 = BD && CF;
            //
            bool comp15 = BE && CD;
            bool comp16 = BE && CF;
            //
            bool comp17 = BF && CD;
            bool comp18 = BF && CE;

            return comp1 || comp2 || comp3 || comp4 || comp5 || comp6 || comp7 || comp8 || comp9
                || comp10 || comp11 || comp12 || comp13 || comp14 || comp15 || comp16 || comp17 || comp18;
        }

        //------
        public override bool Equals(object obj)
        {
            return obj is Triangle T && T.A.Equals(A) && T.B.Equals(B) && T.C.Equals(C);
        }

        public override int GetHashCode()
        {
            return (A.GetHashCode() + B.GetHashCode() + C.GetHashCode() + "").GetHashCode();
        }
    }


    public struct TriOrder
    {
        public (int r1, int r2, int r3) op1;
        public (int r1, int r2, int r3) op2;
        public (int r1, int r2, int r3) op3;

        public TriOrder(int vertexIndex1, int vertexIndex2, int vertexIndex3)
        {
            op1 = (0,0,0);
            op2 = (0,0,0);
            op3 = (0,0,0);

            //conjntos de faces
            // faceA = 123, 231, 312
            // faceB = 132, 213, 321

            //123
            // 1 < 2 && 1 < 3 && 2 < 3

            //231
            // 2 < 3 && 2 > 1 && 3 > 1

            //312
            // 3 > 1 && 3 > 2 && 1 < 2

            //132
            // 1 < 3 && 1 < 2 && 3 > 2

            //213
            // 2 > 1 && 2 < 3 && 1 < 3

            //321
            // 3 > 2 && 3 > 1 && 2 > 1

            // nota: do jeito que foi feito se a entrada for 1, 2, 3 por exemplo todos seram true;
            //bool check123 = vertexIndex1 < vertexIndex2 && vertexIndex1 < vertexIndex3 && vertexIndex2 < vertexIndex3;
            //bool check231 = vertexIndex2 < vertexIndex3 && vertexIndex2 > vertexIndex1 && vertexIndex3 > vertexIndex1;
            //bool check312 = vertexIndex3 > vertexIndex1 && vertexIndex3 > vertexIndex2 && vertexIndex1 < vertexIndex2;
            //bool check132 = vertexIndex1 < vertexIndex3 && vertexIndex1 < vertexIndex2 && vertexIndex3 > vertexIndex2;
            //bool check213 = vertexIndex2 > vertexIndex1 && vertexIndex2 < vertexIndex3 && vertexIndex1 < vertexIndex3;
            //bool check321 = vertexIndex3 > vertexIndex2 && vertexIndex3 > vertexIndex1 && vertexIndex2 > vertexIndex1;

            // verificação correta
            bool check123 = vertexIndex1 < vertexIndex2 && vertexIndex1 < vertexIndex3 && vertexIndex2 < vertexIndex3;
            bool check231 = vertexIndex1 < vertexIndex2 && vertexIndex1 > vertexIndex3 && vertexIndex2 > vertexIndex3;
            bool check312 = vertexIndex1 > vertexIndex2 && vertexIndex1 > vertexIndex3 && vertexIndex2 < vertexIndex3;
            bool check132 = vertexIndex1 < vertexIndex2 && vertexIndex1 < vertexIndex3 && vertexIndex2 > vertexIndex3;
            bool check213 = vertexIndex1 > vertexIndex2 && vertexIndex1 < vertexIndex3 && vertexIndex2 < vertexIndex3;
            bool check321 = vertexIndex1 > vertexIndex2 && vertexIndex1 > vertexIndex3 && vertexIndex2 > vertexIndex3;

            if (check123 || check231 || check312)
            {
                op1.r1 = 1;
                op1.r2 = 2;
                op1.r3 = 3;

                op2.r1 = 2;
                op2.r2 = 3;
                op2.r3 = 1;

                op3.r1 = 3;
                op3.r2 = 1;
                op3.r3 = 2;

            }
            else if (check132 || check213 || check321)
            {
                op1.r1 = 1;
                op1.r2 = 3;
                op1.r3 = 2;

                op2.r1 = 2;
                op2.r2 = 1;
                op2.r3 = 3;

                op3.r1 = 3;
                op3.r2 = 2;
                op3.r3 = 1;
            }
        }



   
    
    }


    public static class VertexCompressUtils 
    {
        public static bool CheckOrderFirt(Triangle last, Triangle next, TriOrder lastOrder, TriOrder nextOrder,
            out (int r1, int r2, int r3) lastUseOrder, out (int r1, int r2, int r3) nextUseOrder)
        {
            // last.B == next.A && last.C == next.B

            //
            bool check_op1_op1 = last[lastOrder.op1.r2].Value.VertexIndex == next[nextOrder.op1.r1].Value.VertexIndex
                              && last[lastOrder.op1.r3].Value.VertexIndex == next[nextOrder.op1.r2].Value.VertexIndex;

            bool check_op1_op2 = last[lastOrder.op1.r2].Value.VertexIndex == next[nextOrder.op2.r1].Value.VertexIndex
                              && last[lastOrder.op1.r3].Value.VertexIndex == next[nextOrder.op2.r2].Value.VertexIndex;

            bool check_op1_op3 = last[lastOrder.op1.r2].Value.VertexIndex == next[nextOrder.op3.r1].Value.VertexIndex
                              && last[lastOrder.op1.r3].Value.VertexIndex == next[nextOrder.op3.r2].Value.VertexIndex;

            //
            bool check_op2_op1 = last[lastOrder.op2.r2].Value.VertexIndex == next[nextOrder.op1.r1].Value.VertexIndex
                              && last[lastOrder.op2.r3].Value.VertexIndex == next[nextOrder.op1.r2].Value.VertexIndex;

            bool check_op2_op2 = last[lastOrder.op2.r2].Value.VertexIndex == next[nextOrder.op2.r1].Value.VertexIndex
                              && last[lastOrder.op2.r3].Value.VertexIndex == next[nextOrder.op2.r2].Value.VertexIndex;

            bool check_op2_op3 = last[lastOrder.op2.r2].Value.VertexIndex == next[nextOrder.op3.r1].Value.VertexIndex
                              && last[lastOrder.op2.r3].Value.VertexIndex == next[nextOrder.op3.r2].Value.VertexIndex;

            //
            bool check_op3_op1 = last[lastOrder.op3.r2].Value.VertexIndex == next[nextOrder.op1.r1].Value.VertexIndex
                              && last[lastOrder.op3.r3].Value.VertexIndex == next[nextOrder.op1.r2].Value.VertexIndex;

            bool check_op3_op2 = last[lastOrder.op3.r2].Value.VertexIndex == next[nextOrder.op2.r1].Value.VertexIndex
                              && last[lastOrder.op3.r3].Value.VertexIndex == next[nextOrder.op2.r2].Value.VertexIndex;

            bool check_op3_op3 = last[lastOrder.op3.r2].Value.VertexIndex == next[nextOrder.op3.r1].Value.VertexIndex
                              && last[lastOrder.op3.r3].Value.VertexIndex == next[nextOrder.op3.r2].Value.VertexIndex;


            if (check_op1_op1)
            {
                lastUseOrder = lastOrder.op1;
                nextUseOrder = nextOrder.op1;
                return true;
            }
            else if (check_op1_op2)
            {
                lastUseOrder = lastOrder.op1;
                nextUseOrder = nextOrder.op2;
                return true;
            }
            else if (check_op1_op3)
            {
                lastUseOrder = lastOrder.op1;
                nextUseOrder = nextOrder.op3;
                return true;
            }

            //
            if (check_op2_op1)
            {
                lastUseOrder = lastOrder.op2;
                nextUseOrder = nextOrder.op1;
                return true;
            }
            else if (check_op2_op2)
            {
                lastUseOrder = lastOrder.op2;
                nextUseOrder = nextOrder.op2;
                return true;
            }
            else if (check_op2_op3)
            {
                lastUseOrder = lastOrder.op2;
                nextUseOrder = nextOrder.op3;
                return true;
            }

            //
            if (check_op3_op1)
            {
                lastUseOrder = lastOrder.op3;
                nextUseOrder = nextOrder.op1;
                return true;
            }
            else if (check_op3_op2)
            {
                lastUseOrder = lastOrder.op3;
                nextUseOrder = nextOrder.op2;
                return true;
            }
            else if (check_op3_op3)
            {
                lastUseOrder = lastOrder.op3;
                nextUseOrder = nextOrder.op3;
                return true;
            }

            lastUseOrder = (0,0,0);
            nextUseOrder = (0,0,0);
            return false;
        }


        public static bool CheckOrder(Triangle last, Triangle next, (int r1, int r2, int r3) lastOrder, TriOrder nextOrder,
            out (int r1, int r2, int r3) nextUseOrder)
        {
            // last.B == next.A && last.C == next.B

            //
            bool check_op_op1 = last[lastOrder.r2].Value.VertexIndex == next[nextOrder.op1.r1].Value.VertexIndex
                              && last[lastOrder.r3].Value.VertexIndex == next[nextOrder.op1.r2].Value.VertexIndex;

            bool check_op_op2 = last[lastOrder.r2].Value.VertexIndex == next[nextOrder.op2.r1].Value.VertexIndex
                              && last[lastOrder.r3].Value.VertexIndex == next[nextOrder.op2.r2].Value.VertexIndex;

            bool check_op_op3 = last[lastOrder.r2].Value.VertexIndex == next[nextOrder.op3.r1].Value.VertexIndex
                              && last[lastOrder.r3].Value.VertexIndex == next[nextOrder.op3.r2].Value.VertexIndex;

            if (check_op_op1)
            {
                nextUseOrder = nextOrder.op1;
                return true;
            }
            else if (check_op_op2)
            {
                nextUseOrder = nextOrder.op2;
                return true;
            }
            else if (check_op_op3)
            {
                nextUseOrder = nextOrder.op3;
                return true;
            }


            nextUseOrder = (0,0,0);
            return false;
        }

    }


    public class VertexLine
    {
        // linha total
        //public byte[] line;

        public float VerticeX = 0;
        public float VerticeY = 0;
        public float VerticeZ = 0;

        //public ushort UnknownB = 0;

        public float NormalX = 0;
        public float NormalY = 0;
        public float NormalZ = 0;

        public ushort IndexComplement = 0;

        public float TextureU = 0;
        public float TextureV = 0;

        //public ushort UnknownA = 0

        public ushort IndexMount = 0;
    }

    public class Noded 
    {
        public string materialname = null;
        public List<List<VertexLine>> subMeshparts = new List<List<VertexLine>>();

    }

}
