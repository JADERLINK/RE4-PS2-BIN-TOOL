using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    public static class Debug
    {
        //debug
        public static void CreateDrawDistanceBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {
            TextWriter text = new FileInfo(Path.Combine(baseDiretory, baseFileName + ".Debug.DrawDistanceBox.obj")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            string DrawDistanceNegativeX = (bin.DrawDistanceNegativeX / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (bin.DrawDistanceNegativeY / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (bin.DrawDistanceNegativeZ / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = ((bin.DrawDistanceNegativeX + bin.DrawDistancePositiveX) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = ((bin.DrawDistanceNegativeY + bin.DrawDistancePositiveY) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = ((bin.DrawDistanceNegativeZ + bin.DrawDistancePositiveZ) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            // real

            //1
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);

            //2
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);

            //inverso Y

            //3
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistancePositiveY + " " + DrawDistanceNegativeZ);

            //4
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistanceNegativeY + " " + DrawDistancePositiveZ);

            // inveso Z

            //5
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistancePositiveZ);

            //6
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistanceNegativeZ);

            // inverso X

            //7
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);

            //8
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);


            text.WriteLine("g Original");
            text.WriteLine("l 1 2");

            text.WriteLine("g Box");
            //text.WriteLine("g l13");
            text.WriteLine("l 1 3"); //ok
            //text.WriteLine("g l24");
            text.WriteLine("l 2 4"); //ok
            //text.WriteLine("g l15");
            text.WriteLine("l 1 5"); //ok
            //text.WriteLine("g l26");
            text.WriteLine("l 2 6"); //ok
            //text.WriteLine("g l36");
            text.WriteLine("l 3 6"); //ok
            //text.WriteLine("g l45");
            text.WriteLine("l 4 5"); //ok
            //text.WriteLine("g l17");
            text.WriteLine("l 1 7"); //ok
            //text.WriteLine("g l28");
            text.WriteLine("l 2 8"); //ok
            //text.WriteLine("g l58");
            text.WriteLine("l 5 8"); //ok
            //text.WriteLine("g l67");
            text.WriteLine("l 6 7"); //ok
            //text.WriteLine("g l47");
            text.WriteLine("l 4 7"); //ok
            //text.WriteLine("g l38");
            text.WriteLine("l 3 8"); //ok

            text.Close();
        }

        //debug
        public static void CreateScaleLimitBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDiretory, baseFileName + ".Debug.ScaleLimitBox.obj")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            float scalenodes = 0;
            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                float allScales = 0;
                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    allScales += bin.Nodes[t].Segments[i].ConversionFactorValue;
                }

                scalenodes += allScales / bin.Nodes[t].Segments.Length;
            }
            float scale = scalenodes / bin.Nodes.Length;

            float maxPos = scale * short.MaxValue / CONSTs.GLOBAL_SCALE;
            float minPos = scale * short.MinValue / CONSTs.GLOBAL_SCALE;

            text.WriteLine("# scale: " + scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            text.WriteLine("");


            //1
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //2
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //inverso Y

            //3
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //4
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            // inveso Z

            //5
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //6
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            // inverso X

            //7
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //8
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //9
            text.WriteLine("v " + 0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //10
            text.WriteLine("v " + scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );



            //11
            text.WriteLine("v " + 0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );


            text.WriteLine("g shorterDistance");
            text.WriteLine("f 9 10 11");

            text.WriteLine("g Box");
            text.WriteLine("l 1 3");
            text.WriteLine("l 2 4");
            text.WriteLine("l 1 5");
            text.WriteLine("l 2 6");
            text.WriteLine("l 3 6");
            text.WriteLine("l 4 5");
            text.WriteLine("l 1 7");
            text.WriteLine("l 2 8");
            text.WriteLine("l 5 8");
            text.WriteLine("l 6 7");
            text.WriteLine("l 4 7");
            text.WriteLine("l 3 8");

            text.Close();

        }

    }
}
