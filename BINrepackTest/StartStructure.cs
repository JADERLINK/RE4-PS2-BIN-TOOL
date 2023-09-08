using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BINrepackTest
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
            TriOrder lastFirtOrder = TriOrder.order_1_2_3; //new TriOrder(1, 2, 3);

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
                List<StartVertex> temp = new List<StartVertex>();

                StartTriangle last = triangles[0];
                (int r1, int r2, int r3) lastOrder = (1, 2, 3);

                triangles.RemoveAt(0);

                bool isFirt = true;
                bool isAdded = false;

                int triangleAddedCount = 1;

                while (triangles.Count != 0)
                {

                    var cont = (from obj in triangles
                                where As1PositionEqualHashCode(last, obj) && As2VertexEqual(last, obj)
                                select obj).ToArray();


                    isAdded = false;


                    for (int i = 0; i < cont.Length; i++)
                    {
                        TriOrder nextOrder = TriOrder.order_3_2_1;//new TriOrder(3, 2, 1);

                        if (triangleAddedCount % 2 == 0 && !isFirt)
                        {
                            nextOrder = TriOrder.order_1_2_3; //new TriOrder(1, 2, 3);
                        }

                        (int r1, int r2, int r3) Order1oldlast = (0, 0, 0);
                        (int r1, int r2, int r3) Order2next = (0, 0, 0);


                        if (isFirt)
                        {
                            bool checkFirt = CheckOrderFirt(last, cont[i], lastFirtOrder, nextOrder, out Order1oldlast, out Order2next);

                            if (checkFirt)
                            {
                                temp.Add(last[Order1oldlast.r1]);
                                temp.Add(last[Order1oldlast.r2]);
                                temp.Add(last[Order1oldlast.r3]);
                                temp.Add(cont[i][Order2next.r3]);

                                last = cont[i];
                                lastOrder = Order2next;
                                triangles.Remove(cont[i]);

                                isAdded = true;
                                isFirt = false;

                                triangleAddedCount++;

                                break;
                            }
                        }
                        else
                        {
                            bool checkNotFirt = CheckOrder(last, cont[i], lastOrder, nextOrder, out Order2next);

                            if (checkNotFirt)
                            {
                                temp.Add(cont[i][Order2next.r3]);

                                last = cont[i];
                                lastOrder = Order2next;
                                triangles.Remove(cont[i]);

                                isAdded = true;
                                isFirt = false;

                                triangleAddedCount++;

                                break;
                            }


                        }

                        //Console.WriteLine("i: " + i);

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

                            newFaces.Add(temp);
                            temp = new List<StartVertex>();
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
                            newFaces.Add(temp);
                            temp = new List<StartVertex>();
                            last = triangles[0];
                            lastOrder = (1, 2, 3);
                            triangles.RemoveAt(0);

                            isFirt = true;
                            isAdded = false;
                            triangleAddedCount = 1;
                            continue;
                        }
                    }
                    else if (temp.Count >= 40)
                    {
                        newFaces.Add(temp);
                        temp = new List<StartVertex>();
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
                    temp.Add(last.A);
                    temp.Add(last.B);
                    temp.Add(last.C);

                    newFaces.Add(temp);
                }
                else
                {
                    newFaces.Add(temp);
                }


            }// fim do  if (triangles.Count > 0)

            return new StartFacesGroup(newFaces);
        }


    }



    /// <summary>
    /// Representação de um Vector3, usado para Position/Normal
    /// </summary>
    public class Vector3
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private int hashCode;

        public Vector3() { }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            unchecked
            {
                hashCode = (X.GetHashCode() * 23 + Y.GetHashCode() * 23 + Z.GetHashCode() * 23).GetHashCode();
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
            //return (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()).GetHashCode();
            //return (X + "_" + Y + "_" + Z).GetHashCode();
            //return (X + Y + Z).GetHashCode();
        }
    }


    /// <summary>
    /// Representação de um Vector2, usado para TextureUV
    /// </summary>
    public class Vector2
    {
        public float U;
        public float V;

        public Vector2() { }

        public Vector2(float u, float v)
        {
            U = u;
            V = v;
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
            unchecked
            {
                return (U.GetHashCode() + V.GetHashCode()).GetHashCode();
                //return (U + "_" + V).GetHashCode();
                //return (U + V).GetHashCode();
            }
        }
    }


    /// <summary>
    /// Representação de um Vector4, usado para Colors
    /// </summary>
    public class Vector4
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Vector4() { }

        public Vector4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
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
            unchecked
            {
                return (R.GetHashCode() + G.GetHashCode() + B.GetHashCode() + A.GetHashCode()).GetHashCode();
                //return (R +"_" + G + "_" + B + "_" + A).GetHashCode();
                //return (R + G + B + A).GetHashCode();
            }
        }

    }


    /// <summary>
    /// Representa o conjunto de pesos associado a um vértice;
    /// </summary>
    public class StartWeightMap
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
                return (Links.GetHashCode() + BoneID1.GetHashCode() + Weight1.GetHashCode() + BoneID2.GetHashCode() + Weight2.GetHashCode() + BoneID3.GetHashCode() + Weight3.GetHashCode()).GetHashCode();
                //return (Links + "_" + BoneID1 + "_" + Weight1 + "_" + BoneID2 + "_" + Weight2 + "_" + BoneID3 + "_" + Weight3).GetHashCode();
                //return (Links + BoneID1 + Weight1 + BoneID2 + Weight2 + BoneID3 +Weight3).GetHashCode();
            }
        }


    }


    /// <summary>
    /// Represente um vertice, porem guarda somente os indices do valores;
    /// <para>Aviso: os indices começam por 1</para>
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

        public TriOrder(int vertexIndex1, int vertexIndex2, int vertexIndex3)
        {

            op1 = (0, 0, 0);
            op2 = (0, 0, 0);
            op3 = (0, 0, 0);

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

}
