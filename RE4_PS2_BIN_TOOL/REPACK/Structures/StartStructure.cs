using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.REPACK
{

    public class StartStructure
    {
        /// <summary>
        /// Dicionario, sendo string o nome do material, e StartFacesGroup, objeto que representa as faces do modelo.
        /// </summary>
        public IDictionary<string, StartFacesGroup> FacesByMaterial { get; set; }


        public StartStructure() 
        {
            FacesByMaterial = new Dictionary<string, StartFacesGroup>();
        }

        private bool CompareVertexIsEquals(StartVertex toCompare, StartVertex obj) 
        {
            return toCompare.Position == obj.Position
               && toCompare.Normal == obj.Normal
               && toCompare.Texture == obj.Texture
               && toCompare.Color == obj.Color
               && toCompare.WeightMap== obj.WeightMap;

        }

        private bool CheckOrderFirt(StartTriangle last, StartTriangle next, TriOrder lastOrder, TriOrder nextOrder,
                out (int r1, int r2, int r3) lastUseOrder, out (int r1, int r2, int r3) nextUseOrder)
        {
            // last.B == next.A && last.C == next.B

            //
            bool check_op1_op1 = CompareVertexIsEquals(last[lastOrder.op1.r2] , next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3] , next[nextOrder.op1.r2]);

            bool check_op1_op2 = CompareVertexIsEquals(last[lastOrder.op1.r2] , next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3] , next[nextOrder.op2.r2]);

            bool check_op1_op3 = CompareVertexIsEquals(last[lastOrder.op1.r2] , next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3] , next[nextOrder.op3.r2]);

            //
            bool check_op2_op1 = CompareVertexIsEquals(last[lastOrder.op2.r2] , next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3] , next[nextOrder.op1.r2]);

            bool check_op2_op2 = CompareVertexIsEquals(last[lastOrder.op2.r2] , next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3] , next[nextOrder.op2.r2]);

            bool check_op2_op3 = CompareVertexIsEquals(last[lastOrder.op2.r2] , next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3] , next[nextOrder.op3.r2]);

            //
            bool check_op3_op1 = CompareVertexIsEquals(last[lastOrder.op3.r2] , next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3] , next[nextOrder.op1.r2]);

            bool check_op3_op2 = CompareVertexIsEquals(last[lastOrder.op3.r2] , next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3] , next[nextOrder.op2.r2]);

            bool check_op3_op3 = CompareVertexIsEquals(last[lastOrder.op3.r2] , next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3] , next[nextOrder.op3.r2]);


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

            lastUseOrder = (0, 0, 0);
            nextUseOrder = (0, 0, 0);
            return false;
        }

        private bool CheckOrder(StartTriangle last, StartTriangle next, (int r1, int r2, int r3) lastOrder, TriOrder nextOrder,
                out (int r1, int r2, int r3) nextUseOrder)
        {
            // last.B == next.A && last.C == next.B

            //
            bool check_op_op1 = CompareVertexIsEquals(last[lastOrder.r2], next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.r3], next[nextOrder.op1.r2]);

            bool check_op_op2 = CompareVertexIsEquals(last[lastOrder.r2], next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.r3], next[nextOrder.op2.r2]);

            bool check_op_op3 = CompareVertexIsEquals(last[lastOrder.r2], next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.r3], next[nextOrder.op3.r2]);

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

            nextUseOrder = (0, 0, 0);
            return false;
        }

        private bool CheckWeightMapCount(List<StartWeightMap> weightMapTemp, StartVertex vertex) 
        {
            var temp = new List<StartWeightMap>();
            temp.AddRange(weightMapTemp);

            if (!temp.Contains(vertex.WeightMap))
            {
                temp.Add(vertex.WeightMap);
            }

            if (temp.Count <= 15)
            {
                return true;
            }

            return false;
        }

        private void weightMapTempAdd(ref List<StartWeightMap> weightMapTemp, StartVertex vertex) 
        {
            if (!weightMapTemp.Contains(vertex.WeightMap))
            {
                weightMapTemp.Add(vertex.WeightMap);
            } 
        }


        private bool As1PositionEqualHashCode(StartTriangle toCompare, StartTriangle obj) 
        {
            return     toCompare.A.Position.GetHashCode() == obj.A.Position.GetHashCode()
                    || toCompare.A.Position.GetHashCode() == obj.B.Position.GetHashCode()
                    || toCompare.A.Position.GetHashCode() == obj.C.Position.GetHashCode()
                    || toCompare.B.Position.GetHashCode() == obj.A.Position.GetHashCode()
                    || toCompare.B.Position.GetHashCode() == obj.B.Position.GetHashCode()
                    || toCompare.B.Position.GetHashCode() == obj.C.Position.GetHashCode()
                    || toCompare.C.Position.GetHashCode() == obj.A.Position.GetHashCode()
                    || toCompare.C.Position.GetHashCode() == obj.B.Position.GetHashCode()
                    || toCompare.C.Position.GetHashCode() == obj.C.Position.GetHashCode()
                    ;
        }


        private bool As2VertexEqual(StartTriangle toCompare, StartTriangle obj)
        {
            bool AD = CompareVertexIsEquals(toCompare.A, obj.A);
            bool AE = CompareVertexIsEquals(toCompare.B, obj.A);
            bool AF = CompareVertexIsEquals(toCompare.C, obj.A);

            bool BD = CompareVertexIsEquals(toCompare.A, obj.B);
            bool BE = CompareVertexIsEquals(toCompare.B, obj.B);
            bool BF = CompareVertexIsEquals(toCompare.C, obj.B);

            bool CD = CompareVertexIsEquals(toCompare.A, obj.C);
            bool CE = CompareVertexIsEquals(toCompare.B, obj.C);
            bool CF = CompareVertexIsEquals(toCompare.C, obj.C);

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

        public void CompressAllFaces()
        {

            Console.WriteLine("Start of compression of vertices");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            IDictionary<string, StartFacesGroup> newFacesByMaterial = new Dictionary<string, StartFacesGroup>();
            foreach (var item in FacesByMaterial)
            {
                newFacesByMaterial.Add(item.Key, CompressFacesOneMaterial(item.Value));
            }
            FacesByMaterial = newFacesByMaterial;

            sw.Stop();
            Console.WriteLine("Taken time in Milliseconds: " + sw.ElapsedMilliseconds);
        }

        private StartFacesGroup CompressFacesOneMaterial(StartFacesGroup facesGroup) 
        {
            TriOrder lastFirtOrder = TriOrder.order_1_2_3;

            var Faces = facesGroup.Faces;

            
            List<StartTriangle> triangles = new List<StartTriangle>();
            for (int i = 0; i < Faces.Count; i++)
            {
                try
                {
                    StartTriangle t = new StartTriangle();
                    t.A = Faces[i][0];
                    t.B = Faces[i][1];
                    t.C = Faces[i][2];
                    triangles.Add(t);
                }
                catch (Exception)
                {
                }
            }

            //---

            List<List<StartVertex>> newFaces = new List<List<StartVertex>>();

            if (triangles.Count > 0)
            {
                List<StartVertex> vtemp = new List<StartVertex>();

                List<StartWeightMap> weightMapTemp = new List<StartWeightMap>();

                StartTriangle last = triangles[0];
                (int r1, int r2, int r3) lastOrder = (1, 2, 3);

                triangles.RemoveAt(0);

                bool isFirt = true;
                bool isAdded = false;

                int triangleAddedCount = 1;

                while (triangles.Count != 0)
                {

                    isAdded = false;
                    int contLength = 0;

                    for (int i = 0; i < triangles.Count; i++)
                    {
                        StartTriangle cont = triangles[i];

                        if (As1PositionEqualHashCode(last, cont) && As2VertexEqual(last, cont))
                        {
                            contLength++;

                            TriOrder nextOrder = TriOrder.order_3_2_1;

                            if (triangleAddedCount % 2 == 0 && !isFirt)
                            {
                                nextOrder = TriOrder.order_1_2_3;
                            }

                            (int r1, int r2, int r3) Order1oldlast = (0, 0, 0);
                            (int r1, int r2, int r3) Order2next = (0, 0, 0);


                            if (isFirt)
                            {
                                bool checkFirt = CheckOrderFirt(last, cont, lastFirtOrder, nextOrder, out Order1oldlast, out Order2next);

                                if (checkFirt)
                                {
                                    vtemp.Add(last[Order1oldlast.r1]);
                                    vtemp.Add(last[Order1oldlast.r2]);
                                    vtemp.Add(last[Order1oldlast.r3]);
                                    vtemp.Add(cont[Order2next.r3]);

                                    //weightMapTempAdd
                                    weightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r1]);
                                    weightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r2]);
                                    weightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r3]);
                                    weightMapTempAdd(ref weightMapTemp, cont[Order2next.r3]);

                                    last = cont;
                                    lastOrder = Order2next;
                                    triangles.Remove(cont);

                                    isAdded = true;
                                    isFirt = false;

                                    triangleAddedCount++;

                                    break;
                                }
                            }
                            else
                            {
                                bool checkNotFirt = CheckOrder(last, cont, lastOrder, nextOrder, out Order2next);

                                if (checkNotFirt && CheckWeightMapCount(weightMapTemp, cont[Order2next.r3]))
                                {
                                    vtemp.Add(cont[Order2next.r3]);

                                    //weightMapTempAdd
                                    weightMapTempAdd(ref weightMapTemp, cont[Order2next.r3]);


                                    last = cont;
                                    lastOrder = Order2next;
                                    triangles.Remove(cont);

                                    isAdded = true;
                                    isFirt = false;

                                    triangleAddedCount++;

                                    break;
                                }


                            }

                        }
                        //Console.WriteLine("i: " + i);

                    } // fim do  for (int i = 0; i < cont.Length; i++)

                    // considerar que não achou nada
                    // considerar achou e estava status firt
                    // considerar achou e estava status not firt
                    // considarar que cont != 0, mas não achou


                    if (contLength == 0 || (contLength != 0 && isAdded == false))
                    {
                        if (isFirt)
                        {
                            vtemp.Add(last.A);
                            vtemp.Add(last.B);
                            vtemp.Add(last.C);

                            newFaces.Add(vtemp);
                            vtemp = new List<StartVertex>();
                            weightMapTemp.Clear();
                            last = triangles[0];
                            lastOrder = (1, 2, 3);
                            triangles.RemoveAt(0);

                            isFirt = true;
                            isAdded = false;
                            triangleAddedCount = 1;
                            continue;
                        }
                        else
                        {
                            newFaces.Add(vtemp);
                            vtemp = new List<StartVertex>();
                            weightMapTemp.Clear();
                            last = triangles[0];
                            lastOrder = (1, 2, 3);
                            triangles.RemoveAt(0);

                            isFirt = true;
                            isAdded = false;
                            triangleAddedCount = 1;
                            continue;
                        }
                    }
                    else if (vtemp.Count >= 40)
                    {
                        newFaces.Add(vtemp);
                        vtemp = new List<StartVertex>();
                        weightMapTemp.Clear();
                        last = triangles[0];
                        lastOrder = (1, 2, 3);
                        triangles.RemoveAt(0);

                        isFirt = true;
                        isAdded = false;
                        triangleAddedCount = 1;
                        continue;
                    }

                   

                } // fim do  while (triangles.Count != 0)


                // ultima seção
                if (isFirt)
                {
                    vtemp.Add(last.A);
                    vtemp.Add(last.B);
                    vtemp.Add(last.C);

                    newFaces.Add(vtemp);
                }
                else
                {
                    newFaces.Add(vtemp);
                }


            }// fim do  if (triangles.Count > 0)

            return new StartFacesGroup(newFaces);
        }


    }



    /// <summary>
    /// Representação de um Vector3, usado para Position/Normal
    /// </summary>
    public class Vector3 : IEquatable<Vector3>
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private int hashCode;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + X.GetHashCode();
                hashCode = hashCode * 23 + Y.GetHashCode();
                hashCode = hashCode * 23 + Z.GetHashCode();
            }
        }


        public static bool operator ==(Vector3 lhs, Vector3 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector3 lhs, Vector3 rhs) => !(lhs == rhs);

        public bool Equals(Vector3 obj)
        {
            return obj.X == X && obj.Y == Y && obj.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 o && o.X == X && o.Y == Y && o.Z == Z;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }


    /// <summary>
    /// Representação de um Vector2, usado para TextureUV
    /// </summary>
    public class Vector2 : IEquatable<Vector2>
    {
        public float U { get; private set; }
        public float V { get; private set; }

        private int hashCode;

        public Vector2(float u, float v)
        {
            U = u;
            V = v;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + U.GetHashCode();
                hashCode = hashCode * 23 + V.GetHashCode();
            }
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

        public bool Equals(Vector2 obj)
        {
            return obj.U == U && obj.V == V;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 o && o.U == U && o.V == V;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }


    /// <summary>
    /// Representação de um Vector4, usado para Colors
    /// </summary>
    public class Vector4 : IEquatable<Vector4>
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        private int hashCode;

        public Vector4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + R.GetHashCode();
                hashCode = hashCode * 23 + G.GetHashCode();
                hashCode = hashCode * 23 + B.GetHashCode();
                hashCode = hashCode * 23 + A.GetHashCode();
            }
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector4 lhs, Vector4 rhs) => !(lhs == rhs);

        public bool Equals(Vector4 obj)
        {
            return obj.A == A && obj.R == R && obj.G == G && obj.B == B;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 o && o.A == A && o.R == R && o.G == G && o.B == B;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

    }


    /// <summary>
    /// Representa o conjunto de pesos associado a um vértice;
    /// </summary>
    public class StartWeightMap : IEquatable<StartWeightMap>
    {
        public int Links { get; set; }

        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }

        public StartWeightMap() { }

        public StartWeightMap(int links, int boneID1, float weight1, int boneID2, float weight2, int boneID3, float weight3)
        {
            Links = links;
            BoneID1 = boneID1;
            Weight1 = weight1;
            BoneID2 = boneID2;
            Weight2 = weight2;
            BoneID3 = boneID3;
            Weight3 = weight3;
        }

        public static bool operator ==(StartWeightMap lhs, StartWeightMap rhs) => lhs.Equals(rhs);

        public static bool operator !=(StartWeightMap lhs, StartWeightMap rhs) => !(lhs == rhs);

        public bool Equals(StartWeightMap obj)
        {
            return obj.Links == Links
                && obj.BoneID1 == BoneID1
                && obj.BoneID2 == BoneID2
                && obj.BoneID3 == BoneID3
                && obj.Weight1 == Weight1
                && obj.Weight2 == Weight2
                && obj.Weight3 == Weight3;
        }

        public override bool Equals(object obj)
        {
            return obj is StartWeightMap map
                && map.Links == Links
                && map.BoneID1 == BoneID1
                && map.BoneID2 == BoneID2
                && map.BoneID3 == BoneID3
                && map.Weight1 == Weight1
                && map.Weight2 == Weight2
                && map.Weight3 == Weight3;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Links.GetHashCode();
                hash = hash * 23 + BoneID1.GetHashCode();
                hash = hash * 23 + Weight1.GetHashCode();
                hash = hash * 23 + BoneID2.GetHashCode();
                hash = hash * 23 + Weight2.GetHashCode();
                hash = hash * 23 + BoneID3.GetHashCode();
                hash = hash * 23 + Weight3.GetHashCode();
                return hash;
            }
        }


    }


    /// <summary>
    /// Represente um vertice
    /// </summary>
    public class StartVertex
    {
        public Vector3 Position;
        public Vector2 Texture;
        public Vector3 Normal;
        public Vector4 Color;
        public StartWeightMap WeightMap;

        public bool Equals(StartVertex obj)
        {
            return obj.Position == Position
                && obj.Texture == Texture
                && obj.Normal == Normal
                && obj.Color == Color
                && obj.WeightMap == WeightMap;
        }

        public override bool Equals(object obj)
        {
            return obj is StartVertex o
                && o.Position == Position
                && o.Texture == Texture
                && o.Normal == Normal
                && o.Color == Color
                && o.WeightMap == WeightMap;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() + Texture.GetHashCode() + Normal.GetHashCode() + Color.GetHashCode() + WeightMap.GetHashCode()).GetHashCode();
            }
        }

    }


    /// <summary>
    /// Reprenta um triangulo
    /// </summary>
    public class StartTriangle
    {
        /// <summary>
        /// D  (1)
        /// </summary>
        public StartVertex A;
        /// <summary>
        /// E  (2)
        /// </summary>
        public StartVertex B;
        /// <summary>
        /// F  (3)
        /// </summary>
        public StartVertex C;

        /// <summary>
        /// contagem a partir de 1
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public StartVertex this[int i]
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
                return null;
            }
        }


        public bool Equals(StartTriangle obj)
        {
            return obj.A.Equals(A) && obj.B.Equals(B) && obj.C.Equals(C);
        }

        public override bool Equals(object obj)
        {
            return obj is StartTriangle T && T.A.Equals(A) && T.B.Equals(B) && T.C.Equals(C);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A.GetHashCode() + B.GetHashCode() + C.GetHashCode()).GetHashCode();
            }
        }

    }


    public class StartFacesGroup
    {
        /// <summary>
        /// o primeiro List é o conjunto de faces, e o segundo List é o conjunto de vertices
        /// </summary>
        public List<List<StartVertex>> Faces { get; set; }

        public StartFacesGroup() 
        {
            Faces = new List<List<StartVertex>>();
        }

        public StartFacesGroup(List<List<StartVertex>> faces)
        {
            Faces = faces;
        }
    }


    public class TriOrder
    {
        public (int r1, int r2, int r3) op1;
        public (int r1, int r2, int r3) op2;
        public (int r1, int r2, int r3) op3;

        public static TriOrder order_1_2_3 = new TriOrder()
        {
            op1 = (1, 2, 3),
            op2 = (2, 3, 1),
            op3 = (3, 1, 2)
        };
        public static TriOrder order_3_2_1 = new TriOrder() 
        {
            op1 = (1, 3, 2),
            op2 = (2, 1, 3),
            op3 = (3, 2, 1),
        };

        public TriOrder() { }
    }

}
