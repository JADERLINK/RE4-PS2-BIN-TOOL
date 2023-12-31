using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    public static class BINrepackIntermediary
    {
        public static IntermediaryStructure MakeIntermediaryStructure(StartStructure startStructure)
        {
            IntermediaryStructure intermediary = new IntermediaryStructure();

            foreach (var item in startStructure.FacesByMaterial)
            {
                IntermediaryGroup group = new IntermediaryGroup();
                group.MaterialName = item.Key;

                var Faces = item.Value.Faces;

                for (int i = 0; i < Faces.Count; i++)
                {
                    IntermediaryFace face = new IntermediaryFace();

                    for (int t = 0; t < Faces[i].Count; t++)
                    {
                        IntermediaryVertex vertex = new IntermediaryVertex();

                        vertex.PosX = Faces[i][t].Position.X;
                        vertex.PosY = Faces[i][t].Position.Y;
                        vertex.PosZ = Faces[i][t].Position.Z;

                        vertex.NormalX = Faces[i][t].Normal.X;
                        vertex.NormalY = Faces[i][t].Normal.Y;
                        vertex.NormalZ = Faces[i][t].Normal.Z;

                        vertex.TextureU = Faces[i][t].Texture.U;
                        vertex.TextureV = Faces[i][t].Texture.V;

                        vertex.ColorR = Faces[i][t].Color.R;
                        vertex.ColorG = Faces[i][t].Color.G;
                        vertex.ColorB = Faces[i][t].Color.B;
                        vertex.ColorA = Faces[i][t].Color.A;

                        vertex.Links = Faces[i][t].WeightMap.Links;

                        vertex.BoneID1 = Faces[i][t].WeightMap.BoneID1;
                        vertex.Weight1 = Faces[i][t].WeightMap.Weight1;
                        vertex.BoneID2 = Faces[i][t].WeightMap.BoneID2;
                        vertex.Weight2 = Faces[i][t].WeightMap.Weight2;
                        vertex.BoneID3 = Faces[i][t].WeightMap.BoneID3;
                        vertex.Weight3 = Faces[i][t].WeightMap.Weight3;

                        face.Vertexs.Add(vertex);

                        IntermediaryWeightMap weightMap = vertex.GetIntermediaryWeightMap();
                        if (!face.WeightMapOnFace.Contains(weightMap))
                        {
                            face.WeightMapOnFace.Add(weightMap);
                        }

                    }

                    group.Faces.Add(face);

                }

                intermediary.Groups.Add(item.Key, group);
            }

            return intermediary;
        }



    }
}
