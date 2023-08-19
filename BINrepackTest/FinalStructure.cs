using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BINrepackTest
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
        public List<FinalWeightMap> SubBoneTable { get; set; }    
        public List<FinalVertex> Vertices { get; set; }


        public FinalSegment() 
        {
            SubBoneTable = new List<FinalWeightMap>();
            Vertices = new List<FinalVertex>();
        }

    }

    public class FinalWeightMap
    {
        public int links { get; set; }
        
        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }


        public override bool Equals(object obj)
        {
            return obj is FinalWeightMap map 
                && map.links == links 
                && map.BoneID1 == BoneID1 
                && map.BoneID2 == BoneID2 
                && map.BoneID3 == BoneID3 
                && map.Weight1 == Weight1
                && map.Weight2 == Weight2
                && map.Weight3 == Weight3
                ;
        }
        public override int GetHashCode()
        {
            return (links + ";" + BoneID1 + ";" + Weight1 + ";" + BoneID2 + ";" + Weight2 + ";" + BoneID3 + ";" + Weight3).GetHashCode();
        }


    }


    public class FinalVertex
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

        public ushort BoneReference { get; set; }
        public ushort IndexComplement { get; set; }
        public ushort IndexMount { get; set; }

    }


}
