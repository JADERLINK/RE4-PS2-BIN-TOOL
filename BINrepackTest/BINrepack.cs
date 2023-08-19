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
        public const string VERSION = "A.1.2.0.0";

        static byte[] MakeBonePairArry(BonePairLine[] bonePairLines) 
        {
            if (bonePairLines.Length != 0)
            {
                int count = bonePairLines.Length;
                int calculo = (count * 8) + 4;
                int partes = calculo / 0x10;
                int div = calculo % 0x10;
                if (div != 0)
                {
                    partes += 1;
                }

                byte[] res = new byte[partes * 0x10];
                BitConverter.GetBytes(count).CopyTo(res, 0);
                int offset = 4;
                for (int i = 0; i < bonePairLines.Length; i++)
                {
                    bonePairLines[i].Line.CopyTo(res, offset);
                    offset += 8;
                }
                return res;
            }

            return new byte[0];
        }

        static byte[] MakeNodeArray(FinalNode finalNode, float ConversionFactorValue, bool IsScenarioBin) 
        {
            List<byte> res = new List<byte>();

            // node header top

            byte b2 = (byte)(finalNode.Segments.Length - 1);
            if (finalNode.Segments.Length > 256)
            {
                b2 = 255;
            }

            byte b3 = (byte)finalNode.BonesIDs.Length;
            byte[] bonesIDs = finalNode.BonesIDs;

            if (IsScenarioBin)
            {
                b3 = 0;
                bonesIDs = new byte[0];
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

            int caculo0 = finalNode.Segments[0].Vertices.Count * 24;
            int vertexBlock0length = caculo0 / 0x10;
            float div0 = caculo0 % 0x10;
            if (div0 != 0)
            {
                vertexBlock0length += 1;
            }

            if (IsScenarioBin)
            {
                lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x30 + (vertexBlock0length * 0x10));
            }
            else
            {
                int weightBlock = finalNode.Segments[0].SubBoneTable.Count * 0x20;
                //int weightBlockLength = weightBlock / 0x10;


                lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x10 + weightBlock + 0x30 + (vertexBlock0length * 0x10));
            }

            byte[] lenghtFirtSubMeshB0B1bytes = BitConverter.GetBytes(lenghtFirtSubMeshB0B1);




            byte[] headerFinal = new byte[counterNodeHeaderArrayLenght];
            headerFinal[0] = lenghtFirtSubMeshB0B1bytes[0];//b0;
            headerFinal[1] = lenghtFirtSubMeshB0B1bytes[1];//b1;
            headerFinal[2] = b2;
            headerFinal[3] = b3;
            bonesIDs.CopyTo(headerFinal, 4);
            res.AddRange(headerFinal);

            // end node header top


            // adiciona os segments

            for (int i = 0; i < finalNode.Segments.Length; i++)
            {
                if (!IsScenarioBin)
                {
                    int weightBlock = finalNode.Segments[i].SubBoneTable.Count * 0x20;
                    byte weightBlockLength = (byte)(weightBlock / 0x10);

                    res.AddRange(new byte[] {
                        weightBlockLength, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x00, 0x80, weightBlockLength, 0x6C, //SubBoneHeader
                        //0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 //subBoneTable line 1
                        });

                    for (int iw = 0; iw < finalNode.Segments[i].SubBoneTable.Count; iw++)
                    {
                        res.AddRange(MakeSubBoneTable(finalNode.Segments[i].SubBoneTable[iw], finalNode.Segments[i].SubBoneTable.Count));
                    }
                }


                // TopTagVifHeader
                res.AddRange(MakeTopTagVifHeader((byte)finalNode.Segments[i].Vertices.Count, ConversionFactorValue, i == 0));

                int caculoV = finalNode.Segments[i].Vertices.Count * 24;
                int vertexBlocklength = caculoV / 0x10;
                float div = caculoV % 0x10;
                if (div != 0)
                {
                    vertexBlocklength += 1;
                }

                byte[] submeshBlock = new byte[vertexBlocklength * 0x10];

                int count = 0;
                for (int l = 0; l < finalNode.Segments[i].Vertices.Count; l++)
                {
                    byte[] line = new byte[24];

                    var VerticeX = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].PosX);
                    var VerticeY = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].PosY);
                    var VerticeZ = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].PosZ);

                    line[0] = VerticeX[0];
                    line[1] = VerticeX[1];

                    line[2] = VerticeY[0];
                    line[3] = VerticeY[1];

                    line[4] = VerticeZ[0];
                    line[5] = VerticeZ[1];

                    var IndexComplement = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].IndexComplement);

                    line[0XE] = IndexComplement[0];
                    line[0XF] = IndexComplement[1];

                   
                    var TextureU = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].TextureU);
                    var TextureV = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].TextureV);

                    var IndexMount = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].IndexMount);
                    
                  
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

                        var ColorR = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].ColorR);
                        var ColorG = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].ColorG);
                        var ColorB = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].ColorB);
                        var ColorA = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].ColorA);

                        line[0X10] = ColorR[0];
                        line[0X11] = ColorR[1];

                        line[0X12] = ColorG[0];
                        line[0X13] = ColorG[1];

                        line[0X14] = ColorB[0];
                        line[0X15] = ColorB[1];

                        line[0X16] = ColorA[0];//0X80;
                        line[0X17] = ColorA[1];//0X00;
                    }
                    else
                    {
                        var BoneReference = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].BoneReference);

                        var NormalX = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].NormalX);
                        var NormalY = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].NormalY);
                        var NormalZ = BitConverter.GetBytes(finalNode.Segments[i].Vertices[l].NormalZ);

                        line[0X6] = BoneReference[0];
                        line[0X7] = BoneReference[1];

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

                res.AddRange(submeshBlock);


                if (i != 0)
                {
                    res.AddRange(MakeEndTagVifCommand(i == finalNode.Segments.Length - 1));
                }
            }


            return res.ToArray();
        }

        static byte[] MakeSubBoneTable(FinalWeightMap finalWeightMap, int totalAmount)
        {
            byte[] res = new byte[0x20];
            BitConverter.GetBytes(finalWeightMap.BoneID1).CopyTo(res, 0x00);
            BitConverter.GetBytes(finalWeightMap.BoneID2).CopyTo(res, 0x04);
            BitConverter.GetBytes(finalWeightMap.BoneID3).CopyTo(res, 0x08);
            BitConverter.GetBytes(finalWeightMap.links).CopyTo(res, 0x0C);
            BitConverter.GetBytes(finalWeightMap.Weight1).CopyTo(res, 0x010);
            BitConverter.GetBytes(finalWeightMap.Weight2).CopyTo(res, 0x014);
            BitConverter.GetBytes(finalWeightMap.Weight3).CopyTo(res, 0x018);
            BitConverter.GetBytes(totalAmount).CopyTo(res, 0x01C);
            return res;
        }

        static byte[] MakeTopTagVifHeader(byte vertexlength, float TopTagVif_Scale, bool isFirst)
        {
            int caculo = vertexlength * 24;
            int vertexBlocklength = caculo / 0x10;
            float div = caculo % 0x10;
            if (div != 0)
            {
                vertexBlocklength += 1;
            }

            int vertexBlocklength8 = (vertexBlocklength * 0x10) / 8;

            byte first = 0x10;
            if (isFirst)
            {
                first = 0x60;
            }

            byte[] scale = BitConverter.GetBytes(TopTagVif_Scale);

            //vertexlength in 0x10

            byte[] res = new byte[0x30] {
            0x01, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x20, 0x80, 0x01, 0x6C, vertexlength, 0x80, 0x00, 0x00, 0x00, 0x40, 0x2E, 0x30, 0x12, 0x05, 0x00, 0x00, scale[0], scale[1], scale[2], scale[3], (byte)vertexBlocklength, 0x00, 0x00, first, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x21, 0x80, (byte)vertexBlocklength8, 0x6D
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


    }

    public class MaterialNode 
    {
        public string Name;
        public byte[] MaterialHeader;
        public byte[] NodeData;
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

            // nota: do jeito que foi feito se a entrada for 1, 2, 3 por exemplo todos serão true;
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
            else if (check_op2_op1)
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
            else if (check_op3_op1)
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
