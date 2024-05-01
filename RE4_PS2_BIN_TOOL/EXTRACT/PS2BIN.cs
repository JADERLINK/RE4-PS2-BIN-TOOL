using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    #region representação do bin

    public class PS2BIN
    {
        //binHeader
        public ushort Magic;
        public ushort Unknown1; //nTex
        public uint BonesPoint;
        public byte Unknown2; //frac
        public byte BonesCount;
        public ushort MaterialCount;
        public uint MaterialOffset;
        public uint Padding1; //CD-CD-CD-CD
        public uint Padding2; //CD-CD-CD-CD
        public uint VersionFlag; //18-08-03-20 or 01-08-01-20 //unknown4_B
        public uint BonepairPoint; // normalmente é 0x0
        public uint Unknown408; // padding, sempre 0x00
        public uint TextureFlags; //unknown4_unk009
        public uint BoundboxPoint; //fixo 0x30
        public uint Unknown410; // padding, sempre 0x00
        public float DrawDistanceNegativeX;
        public float DrawDistanceNegativeY;
        public float DrawDistanceNegativeZ;
        public float DrawDistanceNegativePadding;
        public float DrawDistancePositiveX;
        public float DrawDistancePositiveY;
        public float DrawDistancePositiveZ;
        public uint Padding3; // CD-CD-CD-CD
        //end binHeader

        //bonepair
        public uint BonepairCount;
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
        public byte[] boneLine;  // new byte[16];

        public sbyte BoneID { get { return (sbyte)boneLine[0x0]; } }
        public sbyte BoneParent { get { return (sbyte)boneLine[0x1]; } }

        public float PositionX { get { return BitConverter.ToSingle(boneLine, 0x4); } }
        public float PositionY { get { return BitConverter.ToSingle(boneLine, 0x8); } }
        public float PositionZ { get { return BitConverter.ToSingle(boneLine, 0xC); } }

    }

    public class Material
    {
        public byte[] materialLine; // new byte[16];
        public uint nodeTablePoint;
    }

    public class Node
    {
        //NodeHeader
        //tamanho total dinâmico, em blocos de 0x10
        public byte[] NodeHeaderArray;

        //partes
        public ushort TotalBytesAmount;
        public byte segmentAmountWithoutFirst;
        public byte BonesIdAmount;
        public byte[] NodeBoneList;

        //segmentos no node (valorTotalDePartes), tinha sido nomeado como subMesh
        public Segment[] Segments;
    }

    public class Segment
    {
        public bool IsScenarioColor; // extra

        //new byte[0x10];
        public byte[] WeightMapHeader;

        //[quantidade de linhas]
        public WeightMapTableLine[] WeightMapTableLines;

        //--
        //TopTagVifHeader
        public byte[] TopTagVifHeader2080; //new byte[0x10];
        public byte[] TopTagVifHeaderWithScale; //new byte[0x10];
        public byte[] TopTagVifHeader2180; //new byte[0x10];
        public float ConversionFactorValue;
        //--

        //vertexLines
        public VertexLine[] vertexLines;

        //EndTagVifCommand
        public byte[] EndTagVifCommand; //new byte[0x10];
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
