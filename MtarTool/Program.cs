using System;
using System.IO;
using MtarTool.Core.Mtar;
using MtarTool.Core.Utility;
using System.Xml.Serialization;

namespace MtarTool
{
    class Program
    {
        private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(MtarFile));

        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = args[0];

                ReadArchive(path);
            } //if ends
        } //method Main ends

        static void ReadArchive(string path)
        {
            string outputPath = path.Replace(".", "_") + @"\";
            string xmlOutputPath = path + ".xml";

            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream xmlOutput = new FileStream(xmlOutputPath, FileMode.Create))
            {
                MtarFile mtarFile = new MtarFile();

                mtarFile.name = Path.GetFileName(path);
                mtarFile.Read(input);
                mtarFile.Export(input, outputPath);

                xmlSerializer.Serialize(xmlOutput, mtarFile);
            } //using ends
        } //method WriteArchive ends
    } //class Program ends
} //namespace MtarTool ends
