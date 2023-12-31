using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_PS2_BIN_TOOL.ALL
{
    //AltTextWriter
    public class AltTextWriter
    {
        private TextWriter text;

        public AltTextWriter(string Filepatch, bool Create)
        {
            if (Create)
            {
                text = new FileInfo(Filepatch).CreateText();
            }

        }

        public void WriteLine(string text)
        {
            if (this.text != null)
            {
                this.text.WriteLine(text);
            }
        }

        public void Close()
        {
            if (this.text != null)
            {
                this.text.Close();
            }
        }
    }
}
