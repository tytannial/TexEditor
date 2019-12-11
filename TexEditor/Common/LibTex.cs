using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TexEditor.Model;

namespace TexEditor
{
    public static class LibTex
    {
        public static TexSection[] Deserialize(string filePath, int maxSections)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using var fs = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(fs);

            //Sections
            var offsets = new uint[maxSections];
            //Sections

            // read offsets
            for (int i = 0; i < maxSections - 1; ++i)
            {
                offsets[i] = reader.ReadUInt32();
            }

            var result = new TexSection[maxSections];

            // read strings (TODO, a bit bodged)
            for (int i = 0; i < maxSections; ++i)
            {
                uint end;
                if (i == maxSections - 1)
                {
                    end = 0;
                }
                else
                {
                    end = (offsets[i + 1] - offsets[i]) * 2;
                }

                result[i] = new TexSection();

                var sectionText = new List<string>();
                result[i].SectionString = sectionText;
                result[i].SectionIndex = i;

                // skip offsets.Length * sizeof(uint)
                //reader.BaseStream.Seek(MaxSectionIndex * sizeof(uint) + (offsets[i] * 2), SeekOrigin.Begin);
                var line = new StringBuilder();
                for (uint rp = 0; rp < end; rp += 2)
                {
                    var code = reader.ReadBytes(2);
                    if (code.Length == 0)// End of the file
                    {
                        sectionText.Add(null);
                        break;
                    }

                    if (code[0] == 0 && code[1] == 0)// End of the string
                    {
                        if (line.Length == 0)
                        {
                            continue;
                        }
                        sectionText.Add(line.ToString());
                        line.Clear();
                    }
                    else
                    {
                        line.Append(Encoding.Unicode.GetString(code));
                    }
                }
            }

            return result;
        }

        public static string ConvertToText(this TexSection[] texSections)
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(">>TEXTSTART");
            for (int i = 0; i < texSections.Length; ++i)
            {
                sb.AppendLine($">>{texSections[i].SectionIndex + 1}--------------------------------------------------------------------------------------------------");
                if (texSections.Length - 1 == i)
                {
                    sb.AppendLine("line-----------------------------------------------------------------");
                    continue;
                }
                else
                {
                    if (texSections[i].SectionString.Count == 0)
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        foreach (string line in texSections[i].SectionString)
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
