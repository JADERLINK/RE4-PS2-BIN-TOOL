using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    public class FinalStructure
    {
        public Dictionary<string, FinalNode> Nodes { get; set; }

        public FinalStructure()
        {
            Nodes = new Dictionary<string, FinalNode>();
        }
    }

    public class FinalNode
    {
        public string MaterialName { get; set; }
        public byte[] BonesIDs { get; set; }
        public FinalSegment[] Segments { get; set; }
    }

    public class FinalSegment
    {
        public List<FinalWeightMap> WeightMapList { get; set; }    
        public List<FinalVertex> VertexList { get; set; }


        public FinalSegment()
        {
            WeightMapList = new List<FinalWeightMap>();
            VertexList = new List<FinalVertex>();
        }

    }

    public struct FinalWeightMap : IEquatable<FinalWeightMap>
    {
        public int Links { get; set; }
        
        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }


        public override bool Equals(object obj)
        {
            return obj is FinalWeightMap map 
                && map.Links == Links 
                && map.BoneID1 == BoneID1 
                && map.BoneID2 == BoneID2 
                && map.BoneID3 == BoneID3 
                && map.Weight1 == Weight1
                && map.Weight2 == Weight2
                && map.Weight3 == Weight3;
        }

        public bool Equals(FinalWeightMap other)
        {
            return other.Links == Links
                && other.BoneID1 == BoneID1
                && other.BoneID2 == BoneID2
                && other.BoneID3 == BoneID3
                && other.Weight1 == Weight1
                && other.Weight2 == Weight2
                && other.Weight3 == Weight3;
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

    public struct FinalVertex
    {
        public short PosX { get; set; }
        public short PosY { get; set; }
        public short PosZ { get; set; }

        public short NormalX { get; set; }
        public short NormalY { get; set; }
        public short NormalZ { get; set; }

        public short TextureU { get; set; }
        public short TextureV { get; set; }

        public short ColorR { get; set; }
        public short ColorG { get; set; }
        public short ColorB { get; set; }
        public short ColorA { get; set; }

        public ushort WeightMapReference { get; set; }
        public ushort IndexComplement { get; set; }
        public ushort IndexMount { get; set; }

    }


}
