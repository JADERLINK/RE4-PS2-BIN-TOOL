using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    public static class IdxBinLoader
    {
        public static IdxBin Loader(StreamReader idxFile)
        {
            IdxBin idx = new IdxBin();

            List<BonePairLine> BonePairLines = new List<BonePairLine>();
            List<BoneLine> BoneLines = new List<BoneLine>();

            Dictionary<string, string> pair = new Dictionary<string, string>();

            string line = "";
            while (line != null)
            {
                line = idxFile.ReadLine();
                if (line != null && line.Length != 0)
                {
                    line = line.TrimStart();

                    if (!line.StartsWith(":") && !line.StartsWith("#") && !line.StartsWith("/"))
                    {

                        var split = line.Split(new char[] { ':' });
                        if (split.Length >= 2)
                        {
                            string key = split[0].ToUpper().Trim();

                            if (!pair.ContainsKey(key))
                            {
                                pair.Add(key, split[1]);
                            }


                        }
                    }


                }
            }

            idxFile.Close();
            //----

            if (pair.ContainsKey("ISSCENARIOBIN"))
            {
                try
                {
                    idx.IsScenarioBin = bool.Parse(pair["ISSCENARIOBIN"].Trim());
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("COMPRESSVERTICES"))
            {
                try
                {
                    idx.CompressVertices = bool.Parse(pair["COMPRESSVERTICES"].Trim());
                }
                catch (Exception)
                {
                }
            }
            //----

            idx.unknown1 = new byte[2];
            idx.unknown4_B = new byte[4];
            idx.unknown4_unk009 = new byte[4];

            if (pair.ContainsKey("UNKNOWN1"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN1"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                idx.unknown1[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                idx.unknown1[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN2"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN2"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                idx.unknown2 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            //---

            if (pair.ContainsKey("UNKNOWN4_B"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_B"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    idx.unknown4_B[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }


            if (pair.ContainsKey("UNKNOWN4_UNK009"))
            {
                string value = Utils.ReturnValidHexValue(pair["UNKNOWN4_UNK009"].ToUpper());
                value = value.PadRight(4 * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < 4; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    idx.unknown4_unk009[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }
            }


            //----

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEX"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEX"]);
                    idx.DrawDistanceNegativeX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEY"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEY"]);
                    idx.DrawDistanceNegativeY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEZ"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEZ"]);
                    idx.DrawDistanceNegativeZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCENEGATIVEPADDING"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCENEGATIVEPADDING"]);
                  idx.DrawDistanceNegativePadding = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEX"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEX"]);
                    idx.DrawDistancePositiveX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEY"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEY"]);
                    idx.DrawDistancePositiveY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("DRAWDISTANCEPOSITIVEZ"))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair["DRAWDISTANCEPOSITIVEZ"]);
                    idx.DrawDistancePositiveZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            //----

            byte BonesCount = 0;
            if (pair.ContainsKey("BONESCOUNT"))
            {

                try
                {
                    string value = Utils.ReturnValidDecValue(pair["BONESCOUNT"]);
                    BonesCount = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }
            }

            for (int i = 0; i < BonesCount; i++)
            {
                byte[] boneLine = new byte[0x10];

                if (pair.ContainsKey("BONELINE_" + i))
                {

                    string value = Utils.ReturnValidHexValue(pair["BONELINE_" + i].ToUpper());
                    value = value.PadRight(0x10 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < boneLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        boneLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }

                BoneLines.Add(new BoneLine(boneLine));
            }

            // ----

            byte BonePairCount = 0;
            if (pair.ContainsKey("BONEPAIRCOUNT"))
            {

                try
                {
                    string value = Utils.ReturnValidDecValue(pair["BONEPAIRCOUNT"]);
                    BonePairCount = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }
            }

            for (int i = 0; i < BonePairCount; i++)
            {
                byte[] bonepairLine = new byte[0x8];

                if (pair.ContainsKey("BONEPAIRLINE_" + i))
                {

                    string value = Utils.ReturnValidHexValue(pair["BONEPAIRLINE_" + i].ToUpper());
                    value = value.PadRight(0x8 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < bonepairLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        bonepairLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }

                BonePairLines.Add(new BonePairLine(bonepairLine));
            }

            //---

            idx.BonePairLines = BonePairLines.ToArray();
            idx.BoneLines = BoneLines.ToArray();

            return idx;
        }


    }


    public class IdxBin 
    {
        public bool CompressVertices { get; set; }
        public bool IsScenarioBin { get; set; }

        public byte[] unknown1 { get; set; }
        public byte unknown2 { get; set; }
        public byte[] unknown4_B { get; set; }
        public byte[] unknown4_unk009 { get; set; }

        public float DrawDistanceNegativeX { get; set; }
        public float DrawDistanceNegativeY { get; set; }
        public float DrawDistanceNegativeZ { get; set; }
        public float DrawDistanceNegativePadding { get; set; }
        public float DrawDistancePositiveX { get; set; }
        public float DrawDistancePositiveY { get; set; }
        public float DrawDistancePositiveZ { get; set; }

        public BonePairLine[] BonePairLines { get; set; }

        public BoneLine[] BoneLines { get; set; }

    }

    public class BonePairLine 
    {
        public BonePairLine()
        {
        }

        public BonePairLine(byte[] line)
        {
            Line = line;
        }

        private byte[] _line = new byte[8];

        public byte[] Line
        {
            get
            {
                return _line.ToArray();
            }
            set
            {
                for (int i = 0; i < value.Length && i < _line.Length; i++)
                {
                    _line[i] = value[i];
                }
            }
        }

    }

    public class BoneLine
    {
        public BoneLine() 
        {
        }

        public BoneLine(byte[] line)
        {
            Line = line;
        }

        public BoneLine(byte boneId, byte boneParent, float posX, float posY, float posZ)
        {
            BoneId = boneId;
            BoneParent = boneParent;
            PosX = posX; 
            PosY = posY;
            PosZ = posZ;
        }

        private byte[] _line = new byte[16];

        public byte[] Line 
        { get  {
            return _line.ToArray();
            } set {
                for (int i = 0; i < value.Length && i < _line.Length; i++)
                {
                    _line[i] = value[i];
                }
            } 
        }

        public byte BoneId
        {
            get 
            {
               return _line[0];
            }
            set 
            {
                _line[0] = value;
            }
        }

        public byte BoneParent
        {
            get
            {
                return _line[1];
            }
            set
            {
                _line[1] = value;
            }
        }

        public float PosX 
        {
            get 
            {
                return BitConverter.ToSingle(_line, 0x4);
            }
            set 
            {
                var bvalue = BitConverter.GetBytes(value);
                bvalue.CopyTo(_line, 0x4);
            }
        }

        public float PosY
        {
            get
            {
                return BitConverter.ToSingle(_line, 0x8);
            }
            set
            {
                var bvalue = BitConverter.GetBytes(value);
                bvalue.CopyTo(_line, 0x8);
            }
        }

        public float PosZ
        {
            get
            {
                return BitConverter.ToSingle(_line, 0xC);
            }
            set
            {
                var bvalue = BitConverter.GetBytes(value);
                bvalue.CopyTo(_line, 0xC);
            }
        }


    }

}
