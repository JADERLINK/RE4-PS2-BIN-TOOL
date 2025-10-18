using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHARED_PS2_BIN.REPACK.Structures
{
    public static class CompressStartStructure
    {
        private static bool CompareVertexIsEquals(StartVertex toCompare, StartVertex obj) 
        {
            return toCompare.Position == obj.Position
               && toCompare.Normal == obj.Normal
               && toCompare.Texture == obj.Texture
               && toCompare.Color == obj.Color
               && toCompare.WeightMap == obj.WeightMap;

        }

        private static bool CheckOrderFirt(StartTriangle last, StartTriangle next, TriOrder lastOrder, TriOrder nextOrder,
                out (int r1, int r2, int r3) lastUseOrder, out (int r1, int r2, int r3) nextUseOrder)
        {
            // last.B == next.A && last.C == next.B

            //
            bool check_op1_op1 = CompareVertexIsEquals(last[lastOrder.op1.r2], next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3], next[nextOrder.op1.r2]);

            bool check_op1_op2 = CompareVertexIsEquals(last[lastOrder.op1.r2], next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3], next[nextOrder.op2.r2]);

            bool check_op1_op3 = CompareVertexIsEquals(last[lastOrder.op1.r2], next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op1.r3], next[nextOrder.op3.r2]);

            //
            bool check_op2_op1 = CompareVertexIsEquals(last[lastOrder.op2.r2], next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3], next[nextOrder.op1.r2]);

            bool check_op2_op2 = CompareVertexIsEquals(last[lastOrder.op2.r2], next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3], next[nextOrder.op2.r2]);

            bool check_op2_op3 = CompareVertexIsEquals(last[lastOrder.op2.r2], next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op2.r3], next[nextOrder.op3.r2]);

            //
            bool check_op3_op1 = CompareVertexIsEquals(last[lastOrder.op3.r2], next[nextOrder.op1.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3], next[nextOrder.op1.r2]);

            bool check_op3_op2 = CompareVertexIsEquals(last[lastOrder.op3.r2], next[nextOrder.op2.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3], next[nextOrder.op2.r2]);

            bool check_op3_op3 = CompareVertexIsEquals(last[lastOrder.op3.r2], next[nextOrder.op3.r1])
                              && CompareVertexIsEquals(last[lastOrder.op3.r3], next[nextOrder.op3.r2]);


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

        private static bool CheckOrder(StartTriangle last, StartTriangle next, (int r1, int r2, int r3) lastOrder, TriOrder nextOrder,
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


        private static bool CheckWeightMapCount(List<StartWeightMap> weightMapTemp, StartVertex vertex) 
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

        private static void WeightMapTempAdd(ref List<StartWeightMap> weightMapTemp, StartVertex vertex) 
        {
            if (!weightMapTemp.Contains(vertex.WeightMap))
            {
                weightMapTemp.Add(vertex.WeightMap);
            } 
        }


        private static bool Has1PositionEqualHashCode(StartTriangle toCompare, StartTriangle obj) 
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


        private static bool Has2VertexEqual(StartTriangle toCompare, StartTriangle obj)
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

        public static void CompressAllFaces(this StartStructure startStructure)
        {
            Console.WriteLine("Start of compression of vertices.");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            IDictionary<string, StartFacesGroup> newFacesByMaterial = new ConcurrentDictionary<string, StartFacesGroup>();

            Parallel.ForEach(startStructure.FacesByMaterial, item =>
            {
                newFacesByMaterial.Add(item.Key, CompressFacesOneMaterial(item.Value));
            });
            startStructure.FacesByMaterial = newFacesByMaterial;

            sw.Stop();
            Console.WriteLine("End of compression, taken time in Milliseconds: " + sw.ElapsedMilliseconds);
        }

        private static StartFacesGroup CompressFacesOneMaterial(StartFacesGroup facesGroup) 
        {
            TriOrder lastFirtOrder = TriOrder.order_1_2_3;

            var Faces = facesGroup.Faces;
   
            Dictionary<int, StartTriangle> DicTriangles = new Dictionary<int, StartTriangle>();
            for (int i = 0; i < Faces.Count; i++)
            {
                try
                {
                    StartTriangle t = new StartTriangle();
                    t.A = Faces[i][0];
                    t.B = Faces[i][1];
                    t.C = Faces[i][2];
                    DicTriangles.Add(i, t);
                }
                catch (Exception)
                {
                }
            }

            Dictionary<Vector3, List<int>> TriDic = new Dictionary<Vector3, List<int>>();
            foreach (var tri in DicTriangles)
            {
                if (!TriDic.ContainsKey(tri.Value.A.Position))
                {
                    TriDic.Add(tri.Value.A.Position, new List<int>());
                }
                if (!TriDic.ContainsKey(tri.Value.B.Position))
                {
                    TriDic.Add(tri.Value.B.Position, new List<int>());
                }
                if (!TriDic.ContainsKey(tri.Value.C.Position))
                {
                    TriDic.Add(tri.Value.C.Position, new List<int>());
                }

                TriDic[tri.Value.A.Position].Add(tri.Key);
                TriDic[tri.Value.B.Position].Add(tri.Key);
                TriDic[tri.Value.C.Position].Add(tri.Key);
            }

            //---

            List<List<StartVertex>> newFaces = new List<List<StartVertex>>();

            if (DicTriangles.Count > 0)
            {
                List<StartVertex> vtemp = new List<StartVertex>();

                List<StartWeightMap> weightMapTemp = new List<StartWeightMap>();

                int StartKey = 0;

                StartTriangle last = null;
                Action SetNewLast = () =>
                {
                    int? usedIndex = null;

                    while (true)
                    {
                        if (DicTriangles.ContainsKey(StartKey))
                        {
                            last = DicTriangles[StartKey];
                            usedIndex = StartKey;
                            break;
                        }
                        else
                        {
                            StartKey++;
                        }
                    }

                    if (usedIndex != null)
                    {
                        DicTriangles.Remove((int)usedIndex);
                    }
                };
                SetNewLast();

                (int r1, int r2, int r3) lastOrder = (1, 2, 3);

                bool isFirt = true;
                bool isAdded = false;

                int triangleAddedCount = 1;

                while (DicTriangles.Count != 0)
                {
                    isAdded = false;
                    int contLength = 0;

                    List<(StartTriangle tri, int index)> _triangles = new List<(StartTriangle tri, int index)>();
                    List<int> TriIndex = new List<int>();
                    TriIndex.AddRange(TriDic[last.A.Position]);
                    TriIndex.AddRange(TriDic[last.B.Position]);
                    TriIndex.AddRange(TriDic[last.C.Position]);
                    TriIndex = TriIndex.ToHashSet().OrderBy(x => x).ToList(); // ordena e impede de ter triangulo repetido.
                    foreach (var index in TriIndex)
                    {
                        if (DicTriangles.ContainsKey(index))
                        {
                            _triangles.Add((DicTriangles[index], index));
                        }
                    }

                    for (int i = 0; i < _triangles.Count; i++)
                    {
                        int indexCont = _triangles[i].index;
                        StartTriangle cont = _triangles[i].tri;

                        if (Has1PositionEqualHashCode(last, cont) && Has2VertexEqual(last, cont))
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
                                    WeightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r1]);
                                    WeightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r2]);
                                    WeightMapTempAdd(ref weightMapTemp, last[Order1oldlast.r3]);
                                    WeightMapTempAdd(ref weightMapTemp, cont[Order2next.r3]);

                                    last = cont;
                                    lastOrder = Order2next;
                                    _triangles.Remove((cont, indexCont));
                                    DicTriangles.Remove(indexCont);

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
                                    WeightMapTempAdd(ref weightMapTemp, cont[Order2next.r3]);


                                    last = cont;
                                    lastOrder = Order2next;
                                    _triangles.Remove((cont, indexCont));
                                    DicTriangles.Remove(indexCont);

                                    isAdded = true;
                                    isFirt = false;

                                    triangleAddedCount++;

                                    break;
                                }


                            }

                        }

                    } // fim do  for (int i = 0; i < _triangles.Count; i++)

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
                            lastOrder = (1, 2, 3);
                            SetNewLast();

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
                            lastOrder = (1, 2, 3);
                            SetNewLast();

                            isFirt = true;
                            isAdded = false;
                            triangleAddedCount = 1;
                            continue;
                        }
                    }
                    else if (vtemp.Count >= 44 || weightMapTemp.Count >= 15)
                    {
                        newFaces.Add(vtemp);
                        vtemp = new List<StartVertex>();
                        weightMapTemp.Clear();
                        lastOrder = (1, 2, 3);
                        SetNewLast();

                        isFirt = true;
                        isAdded = false;
                        triangleAddedCount = 1;
                        continue;
                    }


                } // fim do  while (DicTriangles.Count != 0)


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


            }// fim do  if (DicTriangles.Count > 0)

            return new StartFacesGroup(newFaces);
        }

    }

}
