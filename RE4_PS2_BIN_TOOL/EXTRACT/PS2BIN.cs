using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    #region representação do bin

    public class BIN
    {
        //new byte[2];
        public byte[] unknown0 = null;

        //new byte[2];
        public byte[] unknown1 = null;

        //bones point //bone_addr
        public uint bonesPoint;

        //unknown2 //unk003
        public byte unknown2;

        //BonesCount //bone_count
        public byte BonesCount;

        //MaterialCount //table1_count
        public ushort MaterialCount;

        //MaterialsPoint //table1_addr
        public uint MaterialOffset;

        //unknown4 start

        //new byte[8];
        public byte[] Pad8Bytes; //CD-CD-CD-CD-CD-CD-CD-CD

        //byte[4];
        public byte[] unknown4_B; //18-08-03-20 or 01-08-01-20

        //bonepair_addr_
        public uint bonepairPoint; // 0x0

        //new byte[4];
        public byte[] unknown4_unk008; // padding

        //new byte[4];
        public byte[] unknown4_unk009;

        //boundbox_addr
        public uint boundboxPoint; //fixo 0x30

        //new byte[4];
        public byte[] unknown4_unk010; // padding

        public float DrawDistanceNegativeX;
        public float DrawDistanceNegativeY;
        public float DrawDistanceNegativeZ;
        public float DrawDistanceNegativePadding;

        public float DrawDistancePositiveX;
        public float DrawDistancePositiveY;
        public float DrawDistancePositiveZ;

        //new byte[4];
        public byte[] Pad4Bytes; // CD-CD-CD-CD

        // unknown4 end

        //bonepair
        public uint bonepairCount;
        public byte[][] bonepairLines;

        //bonesList
        public Bone[] bones;

        //materialsList
        public Material[] materials;

        //NodeList
        public Node[] Nodes;

        // tipo do bin
        public BinType binType = BinType.Default;
    }

    public class Bone
    {
        // new byte[16];
        public byte[] boneLine;

        public sbyte BoneID { get { return (sbyte)boneLine[0x0]; } }
        public sbyte BoneParent { get { return (sbyte)boneLine[0x1]; } }

        public float PositionX { get { return BitConverter.ToSingle(boneLine, 0x4); } }
        public float PositionY { get { return BitConverter.ToSingle(boneLine, 0x8); } }
        public float PositionZ { get { return BitConverter.ToSingle(boneLine, 0xC); } }

    }

    public class Material
    {
        // new byte[16];
        public byte[] materialLine;

        //
        public uint nodeTablePoint;
    }

    public class Node
    {
        //NodeHeader
        //tamanho total dinâmico, em blocos de 0x10
        public byte[] NodeHeaderArray;

        //segmentos no node (valorTotalDePartes), tinha sido nomeado como subMesh
        public Segment[] Segments;
    }

    public class Segment
    {
        //new byte[0x10];
        public byte[] WeightMapHeader;

        //[quantidade de linhas]
        public WeightMapTableLine[] WeightMapTableLines;

        //--
        //TopTagVifHeader
        //new byte[0x30];
        public byte[] TopTagVifHeader;
        public float ConversionFactorValue;
        //--

        //vertexLines
        public VertexLine[] vertexLines;

        //EndTagVifCommand
        //new byte[0x10];
        public byte[] EndTagVifCommand;
    }

    public class WeightMapTableLine
    {
        public byte[] weightMapTableLine;

        public uint boneId1 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x0); } }
        public uint boneId2 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x4); } }
        public uint boneId3 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x8); } }
        public int Amount { get { return BitConverter.ToInt32(weightMapTableLine, 0xC); } }
        public float weight1 { get { return BitConverter.ToSingle(weightMapTableLine, 0x10); } }
        public float weight2 { get { return BitConverter.ToSingle(weightMapTableLine, 0x14); } }
        public float weight3 { get { return BitConverter.ToSingle(weightMapTableLine, 0x18); } }
    }


    public class VertexLine
    {
        // linha total
        public byte[] line;

        public short VerticeX = 0;
        public short VerticeY = 0;
        public short VerticeZ = 0;

        public ushort UnknownB = 0;

        public short NormalX = 0;
        public short NormalY = 0;
        public short NormalZ = 0;

        public ushort IndexComplement = 0;

        public short TextureU = 0;
        public short TextureV = 0;

        public ushort UnknownA = 0;

        public ushort IndexMount = 0;
    }

    public enum BinType
    {
        Default,
        ScenarioWithColors
    }

    #endregion
}
