using System;
using System.Collections.Generic;
using System.Text;

namespace TexEditor.Model
{
    public class TexSection
    {
        public int SectionIndex { get; set; } // index of the offset table
        public List<string> SectionString { get; set; } = null;

        public override string ToString()
        {
            return $"Index={SectionIndex},Count={SectionString.Count}";
        }
    }
}
