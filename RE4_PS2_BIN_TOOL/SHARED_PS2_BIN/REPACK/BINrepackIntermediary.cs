using SHARED_TOOLS.ALL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHARED_PS2_BIN.REPACK.Structures;
using SHARED_PS2_BIN.ALL;

namespace SHARED_PS2_BIN.REPACK
{
    public static class BINrepackIntermediary
    {
        public static IntermediaryStructure MakeIntermediaryStructure(StartStructure startStructure, out BoundingBox boundingBox)
        {
            float? MinX = null;
            float? MaxX = null;
            float? MinY = null;
            float? MaxY = null;
            float? MinZ = null;
            float? MaxZ = null;

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

                        vertex.PosX = Faces[i][t].Position.X * CONSTs.GLOBAL_POSITION_SCALE;
                        vertex.PosY = Faces[i][t].Position.Y * CONSTs.GLOBAL_POSITION_SCALE;
                        vertex.PosZ = Faces[i][t].Position.Z * CONSTs.GLOBAL_POSITION_SCALE;

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

                        vertex.BoneID1 = (byte)(ushort)Faces[i][t].WeightMap.BoneID1;
                        vertex.Weight1 = Faces[i][t].WeightMap.Weight1;
                        vertex.BoneID2 = (byte)(ushort)Faces[i][t].WeightMap.BoneID2;
                        vertex.Weight2 = Faces[i][t].WeightMap.Weight2;
                        vertex.BoneID3 = (byte)(ushort)Faces[i][t].WeightMap.BoneID3;
                        vertex.Weight3 = Faces[i][t].WeightMap.Weight3;

                        face.Vertexs.Add(vertex);

                        IntermediaryWeightMap weightMap = vertex.GetIntermediaryWeightMap();
                        if (!face.WeightMapOnFace.Contains(weightMap))
                        {
                            face.WeightMapOnFace.Add(weightMap);
                        }

                        // para calculo do BoundingBox
                        if (MinX == null){ MinX = vertex.PosX; }
                        if (MinY == null){ MinY = vertex.PosY; }
                        if (MinZ == null){ MinZ = vertex.PosZ; }
                        if (MaxX == null){ MaxX = vertex.PosX; }
                        if (MaxY == null){ MaxY = vertex.PosY; }
                        if (MaxZ == null){ MaxZ = vertex.PosZ; }

                        if (vertex.PosX < MinX) { MinX = vertex.PosX; }
                        if (vertex.PosY < MinY) { MinY = vertex.PosY; }
                        if (vertex.PosZ < MinZ) { MinZ = vertex.PosZ; }
                        if (vertex.PosX > MaxX) { MaxX = vertex.PosX; }
                        if (vertex.PosY > MaxY) { MaxY = vertex.PosY; }
                        if (vertex.PosZ > MaxZ) { MaxZ = vertex.PosZ; }
                    }

                    group.Faces.Add(face);

                }

                intermediary.Groups.Add(item.Key, group);
            }

            // calculo BoundingBox
            if (MinX == null) { MinX = 0; }
            if (MinY == null) { MinY = 0; }
            if (MinZ == null) { MinZ = 0; }
            if (MaxX == null) { MaxX = 0; }
            if (MaxY == null) { MaxY = 0; }
            if (MaxZ == null) { MaxZ = 0; }

            float CenterX = ((float)MinX + (float)MaxX) / 2;
            float CenterY = ((float)MinY + (float)MaxY) / 2;
            float CenterZ = ((float)MinZ + (float)MaxZ) / 2;

            float SemiDistanceX = Math.Abs((float)MaxX - (float)MinX) / 2;
            float SemiDistanceY = Math.Abs((float)MaxY - (float)MinY) / 2;
            float SemiDistanceZ = Math.Abs((float)MaxZ - (float)MinZ) / 2;

            boundingBox = new BoundingBox();
            boundingBox.BoundingBoxPosX = CenterX;
            boundingBox.BoundingBoxPosY = CenterY;
            boundingBox.BoundingBoxPosZ = CenterZ;
            boundingBox.BoundingBoxPosW = 1f;
            boundingBox.BoundingBoxWidth = SemiDistanceX;
            boundingBox.BoundingBoxHeight = SemiDistanceY;
            boundingBox.BoundingBoxDepth = SemiDistanceZ;

            return intermediary;
        }



    }
}
